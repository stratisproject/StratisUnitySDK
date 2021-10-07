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

    public Button Button_SetMnmemonic, Button_NewMnmemonic, RefreshButton, SendStraxButton, SendRRTButton, Button_CopyAddr, Button_SendNFT, Button_MintNFT;

    public Text AddressText, BalanceStraxText, BalanceRRTText, NFTBalanceText;

    public GameObject PopupPanel;

    public Text PopupPanel_Text;

    public Button PopupPanelOk_Button;

    public string ApiUrl = "http://localhost:44336/";

    public string Mnemonic = "legal door leopard fire attract stove similar response photo prize seminar frown";

    public string RedRunnerTokenContractAddress = "t778saxw6Xdgs77Z5ePpaFPCZ9bk4oNrPT";

    public string RedRunnerNFTContractAddress = "tA9k1WMEtanRrY3k4oudiswZr4Z3bXkNNn";

    public static SDKIntegrationManager Instance { get; private set; }

    private Network network = new CirrusTest();

    private StratisUnityManager stratisUnityManager;

    private string address;

    private Unity3dClient client;

    private StandartTokenWrapper tokenRRT;

    private NFTWrapper nft;

    private const int NFTContractLogsStartHeight = 2641320;

    private decimal straxBalance = -1;
    private ulong rrtBalance = 0;
    private ulong NFTBalance = 0;

    async void Awake()
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
        this.nft = new NFTWrapper(stratisUnityManager, RedRunnerNFTContractAddress);

        address = stratisUnityManager.GetAddress().ToString();
        Debug.Log("Your address: " + address);
        
        this.Mnemonic_InputField.text = mnemonic;
        this.AddressText.text = address;

        this.StartCoroutine(RefreshBalance());
    }

    private IEnumerator RefreshBalance()
    {
        Task task = Task.Run(async () =>
        {
            try
            {
                this.straxBalance = await stratisUnityManager.GetBalanceAsync();
                this.rrtBalance = await tokenRRT.GetBalanceAsync(address);
                this.NFTBalance = await this.nft.BalanceOfAsync(this.address);

                Debug.Log("STRAXBalance: " + straxBalance + " | TokenBalance: " + rrtBalance + " | NFTBalance: " + NFTBalance);
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
            this.BalanceStraxText.text = "STRAX: " + Math.Round(straxBalance, 2);
            this.NFTBalanceText.text = "NFT: " + NFTBalance;
        }, null);
    }
    
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

    #region UI
    private void InitializeUI()
    {
        this.PopupPanel.SetActive(false);
        this.Button_NewMnmemonic.onClick.AddListener(NewMnemonic_bnCall);
        this.Button_SetMnmemonic.onClick.AddListener(SetMnemonic_bnCall);

        Button_CopyAddr.onClick.AddListener(CopyAddress_ButtonCall);
        RefreshButton.onClick.AddListener(() => this.StartCoroutine(RefreshBalance()));

        SendStraxButton.onClick.AddListener(() => this.StartCoroutine(SendStrax_ButtonCall()));
        SendRRTButton.onClick.AddListener(() => this.StartCoroutine(SendRRT_ButtonCall()));
        Button_MintNFT.onClick.AddListener(() => this.StartCoroutine(MintNFT_ButtonCall()));

        PopupPanelOk_Button.onClick.AddListener(() => this.PopupPanel.SetActive(false));
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

    private void DisplayPopup(string text)
    {
        SynchronizationContext.Current.Post(state =>
        {
            this.PopupPanel.SetActive(true);
            this.PopupPanel_Text.text = text;
        }, null);
    }

    private IEnumerator SendStrax_ButtonCall()
    {
        string destAddress = this.DestAddrInputField.text;
        Money amount = new Money(Decimal.Parse(this.AmountInputField.text), MoneyUnit.BTC);

        if (straxBalance < amount.ToUnit(MoneyUnit.BTC))
        {
            this.DisplayPopup("Error sending tx: not enough STRAX");
        }
        else
        {
            string txHash = null;
            string error = null;

            Task task = Task.Run(async () =>
            {
                try
                {
                    txHash = await this.stratisUnityManager.SendTransactionAsync(destAddress, amount);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    error = e.ToString();
                    return;
                }
            });

            while (!task.IsCompleted)
                yield return null;

            if (error != null)
            {
                this.DisplayPopup("Error sending tx: " + error);
            }
            else
            {
                this.DisplayPopup(string.Format("Transaction {0} to {1} with amount: {2} STRAX was sent.", txHash, destAddress, amount));
            }

            this.DestAddrInputField.text = "";
            this.AmountInputField.text = "";
        }
    }

    private IEnumerator SendRRT_ButtonCall()
    {
        string destAddress = this.DestAddrInputField.text;
        ulong amount = ulong.Parse(this.AmountInputField.text);

        if (rrtBalance < amount)
        {
            this.DisplayPopup("Error sending tx: not enough RRT");
        }
        else
        {
            string txHash = null;
            string error = null;

            Task task = Task.Run(async () =>
            {
                try
                {
                    txHash = await this.tokenRRT.TransferToAsync(destAddress, amount);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    error = e.ToString();
                    return;
                }
            });

            while (!task.IsCompleted)
                yield return null;

            if (error != null)
            {
                this.DisplayPopup("Error sending tx: " + error);
            }
            else
            {
                this.DisplayPopup(string.Format("Transaction {0} to {1} with amount: {2} RRT was sent.", txHash, destAddress, amount));
            }

            this.DestAddrInputField.text = "";
            this.AmountInputField.text = "";
        }
    }

    private IEnumerator MintNFT_ButtonCall()
    {
        if (straxBalance <= 0)
        {
            this.DisplayPopup("You need to have some STRAX to mint NFT.");
        }
        else
        {
            string txHash = null;
            string error = null;

            Task task = Task.Run(async () =>
            {
                try
                {
                    string txId = await this.nft.MintAsync(this.address);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    error = e.ToString();
                    return;
                }
            });

            while (!task.IsCompleted)
                yield return null;

            if (error != null)
            {
                this.DisplayPopup("Error minting NFT: " + error);
            }
            else
            {
                this.DisplayPopup(string.Format("Transaction {0} to mint NFT was sent.", txHash));
            }
        }
    }

    #endregion
}
