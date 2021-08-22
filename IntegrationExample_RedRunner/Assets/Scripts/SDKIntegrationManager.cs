using NBitcoin;
using Stratis.Sidechains.Networks;
using Unity3dApi;
using UnityEngine;
using Network = NBitcoin.Network;

public class SDKIntegrationManager : MonoBehaviour
{
    public string ApiUrl = "http://localhost:44336/";

    public string Mnemonic = "legal door leopard fire attract stove similar response photo prize seminar frown";

    public static SDKIntegrationManager Instance { get; private set; }

    private Network network = new CirrusTest();

    private StratisUnityManager stratisUnityManager;

    private string address;

    async void Start()
    {
        Instance = this;

        Unity3dClient client = new Unity3dClient(ApiUrl);

        Mnemonic mnemonic = new Mnemonic(Mnemonic, Wordlist.English);
        stratisUnityManager = new StratisUnityManager(client, network, mnemonic);
        address = stratisUnityManager.GetAddress().ToString();
        Debug.Log("Your address: " + address);

        decimal balance = await stratisUnityManager.GetBalanceAsync();
        Debug.Log("Your balance: " + balance);
    }

    public void CoinCollected(int newCoinValue)
    {
        //Get CRS20 token rewards after
        //Getting 20\50\150\500 coins or
        //after setting highest score of
        //20 / 40 / 70 / 100 / 200m!

        // TODO check if newCoinValue equals to 20\50\150\500 and send info to the server
        Debug.Log("New coin value" + newCoinValue);
    }

    public void HighScoreSet(int newHighScore)
    {
        Debug.Log("New high score" + newHighScore);
    }
}
