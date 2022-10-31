using UnityEngine;
using NBitcoin;
using Stratis.Bitcoin.Networks;
using StratisNodeApi;
using Network = NBitcoin.Network;

public class TestApiMethods : MonoBehaviour
{
    async void Start()
    {
        // Create network instance. Strax test and main & cirrus test and main are supported. 
        Network network = new StraxMain();

        // API Client used to interact with strax node. Note that you should run node with '-txindex=1 -addressindex=1 -unityapi_enable=true' arguments.
        StratisNodeClient client = new StratisNodeClient("http://localhost:44336/");

        //Mnemonic newMnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
        Mnemonic mnemonic = new Mnemonic("leopard fire legal door attract stove similar response photo prize seminar frown", Wordlist.English);
        StratisUnityManager stratisUnityManager = new StratisUnityManager(client, new BlockCoreApi("https://crs.indexer.thedude.pro/api/"), network, mnemonic);

        Debug.Log("Your address: " + stratisUnityManager.GetAddress());

        decimal balance = await stratisUnityManager.GetBalanceAsync();
        Debug.Log("Your balance: " + balance);
        
        Debug.Log("Api test done");
    }
}
