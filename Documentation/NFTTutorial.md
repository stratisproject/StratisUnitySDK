# NFT with StratisUnity3dSDK

In this tutorial I'll explain how to start using NFTs with StratisUnity3dSDK.



### Prerequisites

1. First you need to get a full node and sync it. Full node repository can be found here: https://github.com/stratisproject/StratisBitcoinFullNode. To run it you can just go to `\src\Stratis.CirrusD` and run `dotnet run -testnet` using cmd or powershell. If you want to run on Cirrus Main use same command line arguments but without `-testnet`.

2. Create a new unity project and import StratisUnitySDK. Latest version can be found here: https://github.com/stratisproject/Unity3dIntegration/tree/main/Resources

3. Generate new address and fund it with some TCRS. You will need coins in order to deploy and interact with NFTs. 

```c#
Network network = new CirrusTest();

StratisNodeClient client = new StratisNodeClient("http://localhost:38223/");

Mnemonic mnemonic = new Mnemonic("legal door leopard fire attract stove similar response photo prize seminar frown", Wordlist.English);

StratisUnityManager stratisUnityManager = new StratisUnityManager(client, new BlockCoreApi("https://tcrs.indexer.blockcore.net/api/"), network, mnemonic);

Debug.Log("Your address: " + stratisUnityManager.GetAddress());

```

    

### Deploying new NFT

Choose name and symbol for your NFT and call `NFTWrapper.DeployNFTContractAsync`, return value is transaction id. Once transaction is mined it's executed and your contract is deployed. After that you can use txId to get a receipt which will contain new contract's address. For example:  

```
string nftName = "gameSword";
string nftSymbol = "GS";

string txId = await NFTWrapper.DeployNFTContractAsync(this.stratisUnityManager, nftName, nftSymbol, false, this.stratisUnityManager.GetAddress().ToString(), 0);

ReceiptResponse res = await this.stratisUnityManager.WaitTillReceiptAvailable(txId);

Debug.Log("NFT deployed, it's address: " + res.NewContractAddress);

```



### Minting NFT

Calling `MintAsync` with specified target owner address will result in minting a new NFT that will belong to that address. For example: 

```
string firstAddress = "t8ehx5Nm4QXeRhzt92ATTgCRc1zDkFXAdw";
string uri = "https://rijsat.com/wp-content/nftcollction/demonft.png";
string nftContractAddress = "tRxYDrnKGAKcrSrc1VQMoKa28RSGUXywP5";

NFTWrapper nft = new NFTWrapper(stratisUnityManager, nftContractAddress);        

await nft.MintAsync(firstAddress, uri).ConfigureAwait(false);
Debug.Log("NFT minted successfully!");

```



### Getting NFT balance

NFT balance of address is the amount of NFTs that this address controls. You can get it like this:

```

NFTWrapper nft = new NFTWrapper(stratisUnityManager, "t8snCz4kQgovGTAGReAryt863NwEYqjJqy");

ulong balance = await nft.BalanceOfAsync(this.firstAddress).ConfigureAwait(false);
Debug.Log("NFT balance: " + balance);

```



### Transferring NFT to another address

To transfer an nft you need to use `TransferFromAsync` and specify address from which transfer should occur, receiver address and id of a token you want to transfer. 



```
ulong tokenId = 12345;
        string txId = await nft.TransferFromAsync("tD5aDZSu4Go4A23R7VsjuJTL51YMyeoLyS", "tP2r8anKBWczcBR89yv7rQ1rsSZA2BANhd", tokenId);
```



Discussion on where to get token Id from can be found here: https://github.com/stratisproject/CirrusSmartContracts/pull/44
