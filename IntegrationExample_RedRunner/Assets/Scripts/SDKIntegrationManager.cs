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

    public Button Button_SetMnmemonic, Button_NewMnmemonic, RefreshButton, SendStraxButton, SendRRTButton;

    public string ApiUrl = "http://localhost:44336/";

    public string Mnemonic = "legal door leopard fire attract stove similar response photo prize seminar frown";

    public string RedRunnerTokenContractAddress = "t778saxw6Xdgs77Z5ePpaFPCZ9bk4oNrPT";

    public static SDKIntegrationManager Instance { get; private set; }

    private Network network = new CirrusTest();

    private StratisUnityManager stratisUnityManager;

    private string address;

    private Unity3dClient client;

    async void Start()
    {
        Instance = this;

        client = new Unity3dClient(ApiUrl);

        Mnemonic mnemonic = new Mnemonic(Mnemonic, Wordlist.English);
        stratisUnityManager = new StratisUnityManager(client, network, mnemonic);
        address = stratisUnityManager.GetAddress().ToString();
        Debug.Log("Your address: " + address);

        decimal balance = await stratisUnityManager.GetBalanceAsync();
        Debug.Log("Your balance: " + balance);

        StandartTokenWrapper token = new StandartTokenWrapper(stratisUnityManager, this.RedRunnerTokenContractAddress);

        var balanceToken = await token.GetBalanceAsync(address);
        var ownerBalanceToken = await token.GetBalanceAsync("t7SjCNX2yJUHNTVHk6M6N6usDG6uT7XfzB");

        Debug.Log("TokenBalance_ ME: " + balanceToken + "       Game Owner: " + ownerBalanceToken);

        this.InitializeUI();
    }

    private void InitializeUI()
    {
        // TODO
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
}
