using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using RedRunner;
using Stratis.Sidechains.Networks;
using Unity3dApi;
using UnityEngine;
using UnityEngine.UI;
using Network = NBitcoin.Network;

public class SDKIntegrationManager : MonoBehaviour
{
    public InputField Mnemonic_InputField, DestAddrInputField, AmountInputField;

    public Button Button_SetMnmemonic, Button_NewMnmemonic, RefreshButton, SendStraxButton, SendRRTButton, Button_CopyAddr;

    public Text AddressText, BalanceStraxText, BalanceRRTText;

    public string ApiUrl = "http://localhost:44336/";

    public string Mnemonic = "legal door leopard fire attract stove similar response photo prize seminar frown";

    public string RedRunnerTokenContractAddress = "t778saxw6Xdgs77Z5ePpaFPCZ9bk4oNrPT";

    public static SDKIntegrationManager Instance { get; private set; }

    private Network network = new CirrusTest();

    private StratisUnityManager stratisUnityManager;

    private string address;

    private Unity3dClient client;

    private StandartTokenWrapper tokenRRT;

    void Awake()
    {
        Instance = this;
        client = new Unity3dClient(ApiUrl);
        this.InitializeUI();

        InitMnemonic(Mnemonic);
    }

    private void InitMnemonic(string mnemonic)
    {
        Mnemonic m = new Mnemonic(mnemonic, Wordlist.English);
        stratisUnityManager = new StratisUnityManager(client, network, m);
        this.tokenRRT = new StandartTokenWrapper(stratisUnityManager, this.RedRunnerTokenContractAddress);
        address = stratisUnityManager.GetAddress().ToString();
        Debug.Log("Your address: " + address);
        
        this.Mnemonic_InputField.text = mnemonic;
        this.AddressText.text = address;

        this.StartCoroutine(RefreshBalance());
    }

    private IEnumerator RefreshBalance()
    {
        decimal straxBalance = -1;
        ulong rrtBalance = 0;

        Task task = Task.Run(async () =>
        {
            try
            {
                straxBalance = await stratisUnityManager.GetBalanceAsync();
                rrtBalance = await tokenRRT.GetBalanceAsync(address);

                Debug.Log("STRAXBalance: " + straxBalance + " | TokenBalance: " + rrtBalance);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });

        while (!task.IsCompleted)
            yield return null;


        SynchronizationContext.Current.Post(state =>
        {
            this.BalanceRRTText.text = "RRT: " + rrtBalance;
            this.BalanceStraxText.text = "tSTRAX: " + straxBalance;
        }, null);
    }

    #region UI
    private void InitializeUI()
    {
        this.Button_NewMnmemonic.onClick.AddListener(NewMnemonic_bnCall);
        this.Button_SetMnmemonic.onClick.AddListener(SetMnemonic_bnCall);

        Button_CopyAddr.onClick.AddListener(CopyAddress_ButtonCall);
        RefreshButton.onClick.AddListener(() => this.StartCoroutine(RefreshBalance()));

        // TODO send trax and RRT UI left
    }

    private void CopyAddress_ButtonCall()
    {
        GUIUtility.systemCopyBuffer = this.address;
    }

    private void SetMnemonic_bnCall()
    {
        InitMnemonic(this.Mnemonic_InputField.text);
    }

    private void NewMnemonic_bnCall()
    {
        Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
        this.Mnemonic_InputField.text = mnemonic.ToString();

        SetMnemonic_bnCall();
    }
    #endregion

    public void PlayerDied(int distance, bool isHighestScore)
    {
        Debug.Log("PLAYER DEATH. coins collected = " + GameManager.Singleton.m_Coin.Value + "    distanceRan = " + distance);

        Task.Run(async () =>
        {
            var coinsCollected = GameManager.Singleton.m_Coin.Value;

            if (isHighestScore)
                Debug.Log("New high distance" + distance);


            PlayerNotificationResult result =
                await client.PlayerAchievementsNotificationAsync(address, "NO_NAME", coinsCollected, distance);

            Debug.Log("SEND TOKENS ON DEATH: " + result.SendTokens);
        });
    }
}
