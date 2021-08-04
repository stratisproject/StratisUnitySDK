using UnityEngine;
using NBitcoin;
using Stratis.Bitcoin.Networks;
using Unity3dApi;
using Network = NBitcoin.Network;
using ValidatedAddress = Unity3dApi.ValidatedAddress;

public class TestApiMethods : MonoBehaviour
{
    async void Start()
    {
        // Create network instance. Strax test and main & cirrus test and main are supported. 
        Network network = new StraxMain();

        // API Client used to interact with strax node. Note that you should run node with '-txindex=1 -addressindex=1 -unityapi_enable=true' arguments.
        Unity3dClient client = new Unity3dClient("http://localhost:44336/");

        //Mnemonic newMnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
        Mnemonic mnemonic = new Mnemonic("leopard fire legal door attract stove similar response photo prize seminar frown", Wordlist.English);
        StratisUnityManager stratisUnityManager = new StratisUnityManager(client, network, mnemonic);

        Debug.Log("Your address: " + stratisUnityManager.GetAddress());

        decimal balance = await stratisUnityManager.GetBalanceAsync();
        Debug.Log("Your balance: " + balance);

        //await stratisUnityManager.SendTransactionAsync("XRfFRF7M3xnAu6FutrdqmgBqJFQ16ZKRW2", (long)(0.1 * Money.COIN));
        //await stratisUnityManager.SendOpReturnTransactionAsync("WAZAAAA");

        // Provides list of UTXOs for target address.
        GetUTXOsResponseModel utxos = await client.GetUTXOsForAddressAsync("XUQUbLJ9TMFXsvKbfpitvbBnMPzJwM2ftQ");

        // Provides block header for given block hash.
        BlockHeaderModel blockHeader = await client.GetBlockHeaderAsync("d4a1678831917514bbae45354ff9cf4638a495558bffc66c9fe07313cce810d7");

        // Gets tx hex by tx id.
        // This method also can be used to check if tx that was sent was actually included in a block. 
        RawTxModel rawTx = await client.GetRawTransactionAsync("b3de5b8d0bff178a0aeff695a7959fbc0362adf82b2aec8ad02c61b41ffa5cab");
        Transaction decodedTx = network.CreateTransaction(rawTx.Hex);
        
        //Validates given address.
        bool isValid = (await client.ValidateAddressAsync("XPn7rte7zHuH2PnrK3WRZqa3Stpb8YLvq6")).Isvalid;

        // Provides block by given block hash.
        BlockModel block = await client.BlockAsync("1726b2a83a92dfa932fca44f1a8f8ac430346aa3b9dc1a4bef2988676efe7b31", true, true);

        // Provides tip (hash and height of the last synced block).
        TipModel tip = await client.TipAsync();

        Debug.Log("Api test done");
    }
}
