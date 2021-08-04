using NBitcoin;
using Stratis.Sidechains.Networks;
using Unity3dApi;
using UnityEngine;
using Stratis.SmartContracts.CLR.Serialization;
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
        
        string standartTokenAddr = "tLG1Eap1f7H5tnRwhs58Jn7NVDrP3YTgrg";
        StandartTokenWrapper stw = new StandartTokenWrapper(stratisUnityManager, standartTokenAddr );

        string randomAddress = new Mnemonic(Wordlist.English, WordCount.TwentyFour).DeriveExtKey().PrivateKey.PubKey.GetAddress(network).ToString();

        Debug.Log("Symbol: " + await stw.GetSymbolAsync());
        Debug.Log("Name: " + await stw.GetNameAsync());
        Debug.Log("TotalSupply: " + await stw.GetTotalSupplyAsync());
        Debug.Log("Balance: " + await stw.GetBalanceAsync(stratisUnityManager.GetAddress().ToString()));
        Debug.Log("Decimals: " + await stw.GetDecimalsAsync());
        Debug.Log("Allowance: " + await stw.GetAllowanceAsync(stratisUnityManager.GetAddress().ToString(), randomAddress));

        // TODO 
    }
}
