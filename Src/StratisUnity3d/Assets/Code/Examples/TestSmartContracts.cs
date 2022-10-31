using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Sidechains.Networks;
using Stratis.SmartContracts.CLR.Serialization;
using StratisNodeApi;
using UnityEngine;
using Network = NBitcoin.Network;

public class TestSmartContracts : MonoBehaviour
{
    async void Start()
    {
        // First we create network on which we want to interact with smart contracts. 
        // Cirrus main and test networks support smart contracts.
        Network network = new CirrusMain();

        // API Client used to interact with a node. Note that you should run node with '-txindex=1 -addressindex=1 -unityapi_enable=true' arguments.
        StratisNodeClient client = new StratisNodeClient("https://cirrus-api-ha.stratisplatform.com");
        
        Mnemonic mnemonic = new Mnemonic("legal door leopard fire attract stove similar response photo prize seminar frown", Wordlist.English);
        StratisUnityManager stratisUnityManager = new StratisUnityManager(client, new BlockCoreApi("https://crs.indexer.thedude.pro/api/"), network, mnemonic);

        Debug.Log("Your address: " + stratisUnityManager.GetAddress());

        decimal balance = await stratisUnityManager.GetBalanceAsync();
        Debug.Log("Your balance: " + balance);

        // Deploy DAO contract
        //await this.DeployDaoContractAsync(stratisUnityManager);
        //return;

        ReceiptResponse receipt = await client.ReceiptAsync("95b9c1e8ab28071b750ab61a3647954b0476d75173d91d0c8db0267c4894d1f6");
        
        bool isSuccess = receipt.Success;
        string contractAddr = receipt.NewContractAddress;
        Debug.Log("Checking contract deployment receipt. Success: " + isSuccess + " ContractAddress: " + contractAddr);

        Debug.Log("Making local contract call.");

        var localCallData = new LocalCallContractRequest()
        {
            GasPrice = 10000,
            Amount = "0",
            GasLimit = 250000,
            ContractAddress = contractAddr,
            MethodName = "MaxVotingDuration",
            Sender = stratisUnityManager.GetAddress().ToString(),
            Parameters = new List<string>()
        };
        LocalExecutionResult localCallResult = await client.LocalCallAsync(localCallData);
        Debug.Log("MaxVotingDuration: " + localCallResult.Return.ToString());

        // Make an on-chain smart contract call.
        string callId = await stratisUnityManager.SendCallContractTransactionAsync("CNiJEPppjvBf1zAAyjcLD81QbVd8NQ59Bv",
            "WhitelistAddress", new string[] {"9#CPokn4GjJHtM7t2b99pdsbLuGd4RbM7pGL"});

        // Call response can be taken from receipt once tx is mined.

        Debug.Log("Api test done");
    }

    private async Task DeployDaoContractAsync(StratisUnityManager stratisUnityManager)
    {
        // Deploy DAO contract
        string constructorParameter = ((int)MethodParameterDataType.UInt) + "#" + "18900";
        string txId = await stratisUnityManager.SendCreateContractTransactionAsync(WhitelistedContracts.DaoContract.ByteCode, new string[] { constructorParameter }, 0);
        Debug.Log("Contract deployment tx sent. TxId: " + txId);
    }
}
