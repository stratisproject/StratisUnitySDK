# Stratis smart contracts with Unity3d

Stratis smart contracts are enabled on Cirrus Main and Cirrus Test networks. Only whitelisted smart contracts can be deployed. List of whitelisted smart contracts can be found here: https://github.com/stratisproject/CirrusSmartContracts

You can also check if smart contract is whitelisted using `/api/Voting/whitelistedhashes`. For example if you run node on cirrus test this link will provide you with list of hashes of whitelisted contracts: http://localhost:38223/api/Voting/whitelistedhashes`





### What is required to work with stratis smart contracts

First you need to get a full node and sync it. Full node repository can be found here: https://github.com/stratisproject/StratisBitcoinFullNode

To run it you can just go to `\src\Stratis.CirrusD` and run `dotnet run -testnet` using cmd or powershell. If you want to run on Cirrus Main use same command line arguments but without `-testnet`.



Secondly you need to import StratisUnitySDK. Latest version can be found here: https://github.com/stratisproject/Unity3dIntegration/tree/main/Resources





### Deploying smart contracts

When you deploy smart contracts you are creating a transaction which requires fee. So before you proceed make sure you have some CRS or TCRS (CRS on testnet) deposited to your address.



Following code generates your address and then displays it in the debug console.

```c#
Network network = new CirrusTest();

StratisNodeClient client = new StratisNodeClient("http://localhost:38223/");

Mnemonic mnemonic = new Mnemonic("legal door leopard fire attract stove similar response photo prize seminar frown", Wordlist.English);

StratisUnityManager stratisUnityManager = new StratisUnityManager(client, new BlockCoreApi("https://tcrs.indexer.blockcore.net/api/"), network, mnemonic);

Debug.Log("Your address: " + stratisUnityManager.GetAddress());
```

 

To deploy a smart contract you need to use `stratisUnityManager.SendCreateContractTransactionAsync` which returns txId after execution. That txId can be used to get a receipt once transaction is executed. Receipt will contain smart contract address. 



For example here is how to deploy StandardToken contract: 

```
        List<string> constructorParameter = new List<string>()
        {
            $"{(int)MethodParameterDataType.ULong}#1000000",
            $"{(int)MethodParameterDataType.String}#TestToken",
            $"{(int)MethodParameterDataType.String}#TT",
            $"{(int)MethodParameterDataType.UInt}#8"
        };

        string txId = await stratisUnityManager.SendCreateContractTransactionAsync(WhitelistedContracts.StandardTokenContract.ByteCode, constructorParameter.ToArray(), 0).ConfigureAwait(false);
        Debug.Log("Contract deployment tx sent. TxId: " + txId);
```



And once transaction is confirmed, you can use below line of sample codes to get smart contract address. 

```
ReceiptResponse receipt = await client.ReceiptAsync("95b9c1e8ab28071b750ab61a3647954b0476d75173d91d0c8db0267c4894d1f6").ConfigureAwait(false);
        
string contractAddr = receipt.NewContractAddress;
```




Also there are wrappers for smart contracts that do constructor parameters encoding for you. You can check `StandardTokenWrapper` and `NFTWrapper`.  Here is StandardToken deployment example using a wrapper: 

```
await NFTWrapper.DeployNFTContractAsync(this.stratisUnityManager, "TestNFT", "TNFT", "TestNFT_{0}", false);
```





### Using smart contracts

There are 2 ways to interact with a smart contract: call and local call. 

Calls should be used when you want to change smart contract's state. Local calls are used to get data from smart contract and using them doesn't result in creating an on-chain transaction. 



Here is an example of making local call: 

```
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

LocalExecutionResult localCallResult = await client.LocalCallAsync(localCallData).ConfigureAwait(false);
Debug.Log("MaxVotingDuration: " + localCallResult.Return.ToString());
```



Call example: 

```
// Make an on-chain smart contract call.
string callId = await stratisUnityManager.SendCallContractTransactionAsync("CNiJEPppjvBf1zAAyjcLD81QbVd8NQ59Bv","WhitelistAddress", new string[] {"9#CPokn4GjJHtM7t2b99pdsbLuGd4RbM7pGL"}).ConfigureAwait(false);
```



For more you can check examples in `TestSmartContracts.cs`





### Using smart contracts via wrappers

NFT and StandardToken contracts have wrappers to make it easier to interact with them. Wrapper is a class that constructs call parameters and makes a call. 

Here is an example for standard token wrapper that displays information about target standard token: 

```
string standardTokenAddr = "tLG1Eap1f7H5tnRwhs58Jn7NVDrP3YTgrg";
StandardTokenWrapper stw = new StandardTokenWrapper(stratisUnityManager, standardTokenAddr);

Debug.Log("Symbol: " + await stw.GetSymbolAsync());
Debug.Log("Name: " + await stw.GetNameAsync());
Debug.Log("TotalSupply: " + await stw.GetTotalSupplyAsync());
Debug.Log("Balance: " + await stw.GetBalanceAsync(firstAddress));
Debug.Log("Decimals: " + await stw.GetDecimalsAsync());
```



Here is an example for NFT that mints new NFT: 

```
string nftAddr = "t8snCz4kQgovGTAGReAryt863NwEYqjJqy";
NFTWrapper nft = new NFTWrapper(stratisUnityManager, nftAddr);

ulong balanceBefore = await nft.BalanceOfAsync(this.firstAddress).ConfigureAwait(false);
Debug.Log("NFT balance: " + balanceBefore);

string mintId = await nft.MintAsync(firstAddress).ConfigureAwait(false);

await this.WaitTillReceiptAvailable(mintId).ConfigureAwait(false);

ulong balanceAfter = await nft.BalanceOfAsync(this.firstAddress).ConfigureAwait(false);

Assert.IsTrue(balanceAfter == balanceBefore + 1);
```



For more you can check examples in  `SCInteractTest.cs`





### Examples

You can find full listings for smart contract examples in the Examples folder. 

`\Assets\Code\Examples\TestSmartContracts.cs` - general example that covers contract deployment and interaction. 

`\Assets\Code\Examples\SCInteractTest.cs` - example that covers NFT and StandardToken contracts deployment and interaction. 



To run those examples just add their scripts to any object in your scene or use prebuilt scenes from `\Assets\Scenes`.
