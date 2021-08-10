# Stratis smart contracts with Unity3d

Stratis smart contracts are enabled on Cirrus Main and Cirrus Test networks. Only whitelisted smart contracts can be deployed. List of whitelisted smart contracts can be found here: https://github.com/stratisproject/CirrusSmartContracts

You can also check if smart contract is whitelisted using `/api/Voting/whitelistedhashes`. For example if you run node on cirrus test this link will provide you with list of hashes of whitelisted contracts: http://localhost:38223/api/Voting/whitelistedhashes`



### What is required to work with stratis smart contracts

First you need to get a full node and sync it. Full node repository can be found here: https://github.com/stratisproject/StratisBitcoinFullNode

To run it you can just go to `\src\Stratis.CirrusD` and run `dotnet run -txindex=1 -addressindex=1 -testnet -unityapi_enable=true` using cmd or powershell. If you want to run on Cirrus Main use same command line arguments but without `-testnet`.



Secondly you need to import StratisUnitySDK. Latest version can be found here: https://github.com/stratisproject/Unity3dIntegration/tree/main/Resources



### Deploying smart contracts

When you deploy smart contracts you are creating a transaction which requires fee. So before you proceed make sure you have some STRAX or TSTRAX (STRAX on testnet) deposited to your address.



Following code generates your address and then displays it in the debug console.

```c#
Unity3dClient Client = new Unity3dClient("http://localhost:44336/");

Mnemonic mnemonic = new Mnemonic("legal door leopard fire attract stove similar response photo prize seminar frown", Wordlist.English);

StratisUnityManager stratisUnityManager = new StratisUnityManager(client, network, mnemonic);

Debug.Log("Your address: " + stratisUnityManager.GetAddress());
```

 

To deploy a smart contract you need to use `stratisUnityManager.SendCreateContractTransactionAsync` which returns txId after execution. That txId can be used to get a receipt once transaction is executed. Receipt will contain smart contract address. 



For example here is how to deploy DAO contract: 

```
string constructorParameter = ((int)MethodParameterDataType.UInt) + "#" + "18900";
        string txId = await stratisUnityManager.SendCreateContractTransactionAsync(WhitelistedContracts.DaoContract.ByteCode, new string[] { constructorParameter }, 0).ConfigureAwait(false);
```



And once transaction is confirmed you can use 

```
ReceiptResponse receipt = await client.ReceiptAsync("95b9c1e8ab28071b750ab61a3647954b0476d75173d91d0c8db0267c4894d1f6").ConfigureAwait(false);
        
string contractAddr = receipt.NewContractAddress;
```

to get smart contract address. 



Also there are wrappers for smart contracts that do constructor parameters encoding for you. You can check `StandartTokenWrapper` and `NFTWrapper`.  Here is StandartToken deployment example using a wrapper: 

```
await NFTWrapper.DeployNFTContractAsync(this.stratisUnityManager, "TestNFT", "TNFT", "TestNFT_{0}", false);
```



### Using smart contracts

TODO:

general usage

NFT and ST wrappers





### Examples

You can find full listings for smart contract examples in the Examples folder. 

`\Assets\Code\Examples\TestSmartContracts.cs` - general example that covers contract deployment and interaction. 

`\Assets\Code\Examples\SCInteractTest.cs` - example that covers NFT and StandartToken contracts deployment and interaction. 



To run those examples just add their scripts to any object in your scene or use prebuilt scenes from `\Assets\Scenes`.
