using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Sidechains.Networks;
using Stratis.SmartContracts;
using StratisNodeApi;
using UnityEngine;
using Network = NBitcoin.Network;

public class NFTExample : MonoBehaviour
{
    public string NodeApiUrl = "https://cirrus-api-ha.stratisplatform.com";

    public string BlockCoreApiUrl = "https://crs.indexer.thedude.pro/api/";

    private readonly string firstAddrMnemonic = "legal door leopard fire attract stove similar response photo prize seminar frown";

    private readonly string secondAddrMnemonic = "want honey rifle wisdom verb sunny enter virtual phone kite aunt surround";

    private string firstAddress, secondAddress;

    private StratisUnityManager stratisUnityManager;

    private Network network = new CirrusTest();

    private StratisNodeClient client;

    async void Start()
    {
        client = new StratisNodeClient(NodeApiUrl);

        Mnemonic mnemonic = new Mnemonic(firstAddrMnemonic, Wordlist.English);
        BlockCoreApi blockcoreAPI = new BlockCoreApi(BlockCoreApiUrl);
        stratisUnityManager = new StratisUnityManager(client, blockcoreAPI, network, mnemonic);
        firstAddress = stratisUnityManager.GetAddress().ToString();
        secondAddress = new Mnemonic(secondAddrMnemonic).DeriveExtKey().PrivateKey.PubKey.GetAddress(network).ToString();
        decimal balance = await stratisUnityManager.GetBalanceAsync();

        Debug.Log("FirstAddr: " + firstAddress + "    Balance: " + balance);
        Debug.Log("SecondAddr: " + secondAddress);

        string nftAddress = "tG1vSp7Fd8S6UKH54sZ6sfi9frCXpxUrSz";
        NFTWrapper nft = new NFTWrapper(stratisUnityManager, nftAddress);

         
        List<long> ids = (await blockcoreAPI.GetOwnedNFTIds(firstAddress)).Select(x => x.id).ToList();

        string idsJoined = String.Join(",", ids);
        Debug.Log("IDs: " + idsJoined);
        
        UInt256 balanceFirstAddr = await nft.BalanceOfAsync(this.firstAddress);
        UInt256 balanceSecondAddr = await nft.BalanceOfAsync(this.secondAddress);

        // Mint NFT
        string mintId = await nft.MintAsync(firstAddress, "uri");
        await this.stratisUnityManager.WaitTillReceiptAvailable(mintId);
        Debug.Log("NFT balance first addr: " + balanceFirstAddr + "    second addr: " + balanceSecondAddr);

        string transferId = await nft.TransferFromAsync(firstAddress, secondAddress, 1);
        ReceiptResponse transferReceipt = await this.stratisUnityManager.WaitTillReceiptAvailable(transferId);
        Debug.Log("NFT transfer: Success: " + transferReceipt.Success + "   returned: " + transferReceipt.ReturnValue + "  transferID: " + transferId);

        balanceFirstAddr = await nft.BalanceOfAsync(this.firstAddress);
        balanceSecondAddr = await nft.BalanceOfAsync(this.secondAddress);
        Debug.Log("NFT balance first addr: " + balanceFirstAddr + "    second addr: " + balanceSecondAddr);
    }

    private async Task DeployNFTAsync()
    {
        string nftName = "gameSword";
        string nftSymbol = "GS";

        string txId = await NFTWrapper.DeployNFTContractAsync(this.stratisUnityManager, nftName, nftSymbol, false,
            this.stratisUnityManager.GetAddress().ToString(), 0);

        ReceiptResponse res = await this.stratisUnityManager.WaitTillReceiptAvailable(txId);

        Debug.Log("NFT deployed, it's address: " + res.NewContractAddress);
    }
}
