using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Sidechains.Networks;
using Unity3dApi;
using UnityEngine;
using Network = NBitcoin.Network;

public class SCInteractTest : MonoBehaviour
{
    async void Start()
    {
        Network network = new CirrusTest();
        
        Unity3dClient client = new Unity3dClient("http://localhost:44336/");

        Mnemonic mnemonic = new Mnemonic("legal door leopard fire attract stove similar response photo prize seminar frown", Wordlist.English);
        StratisUnityManager stratisUnityManager = new StratisUnityManager(client, network, mnemonic);

        Debug.Log("Your address: " + stratisUnityManager.GetAddress());

        decimal balance = await stratisUnityManager.GetBalanceAsync();
        Debug.Log("Your balance: " + balance);

        // TODO
    }
}
