using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json;
using Stratis.Sidechains.Networks;
using Stratis.SmartContracts;
using Unity3dApi;
using UnityEngine;
using Network = NBitcoin.Network;

public class NFTExample : MonoBehaviour
{
    public string ApiUrl = "http://localhost:44336/";

    private readonly string firstAddrMnemonic = "legal door leopard fire attract stove similar response photo prize seminar frown";

    private readonly string secondAddrMnemonic = "want honey rifle wisdom verb sunny enter virtual phone kite aunt surround";

    private string firstAddress, secondAddress;

    private StratisUnityManager stratisUnityManager;

    private Network network = new CirrusTest();

    private Unity3dClient client;

    async void Start()
    {
        client = new Unity3dClient(ApiUrl);

        Mnemonic mnemonic = new Mnemonic(firstAddrMnemonic, Wordlist.English);
        stratisUnityManager = new StratisUnityManager(client, network, mnemonic);
        firstAddress = stratisUnityManager.GetAddress().ToString();
        secondAddress = new Mnemonic(secondAddrMnemonic).DeriveExtKey().PrivateKey.PubKey.GetAddress(network).ToString();
        decimal balance = await stratisUnityManager.GetBalanceAsync();

        Debug.Log("FirstAddr: " + firstAddress + "    Balance: " + balance);
        Debug.Log("SecondAddr: " + secondAddress);

        string nftAddress = "tG1vSp7Fd8S6UKH54sZ6sfi9frCXpxUrSz";
        NFTWrapper nft = new NFTWrapper(stratisUnityManager, nftAddress);

        await client.WatchNftContractAsync(nftAddress);
        List<string> ids = (await client.GetOwnedNftsAsync(firstAddress)).OwnedIDsByContractAddress.ToList().First().Value.Select(x => x.ToString()).ToList();
        string idsJoined = String.Join(",", ids);
        Debug.Log("IDs: " + idsJoined);

        ICollection<string> watchedContracts = await client.GetWatchedNFTContractsAsync();
        string watchedContractsString = String.Join(",", watchedContracts);
        Debug.Log("Watched contracts: " + watchedContractsString);
        
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

        // Display all transfers
        await this.DisplayTransfersAsync(nftAddress);
    }

    private async Task DeployNFTAsync()
    {
        string nftName = "gameSword";
        string nftSymbol = "GS";

        string txId = await NFTWrapper.DeployNFTContractAsync(this.stratisUnityManager, nftName, nftSymbol, false);

        ReceiptResponse res = await this.stratisUnityManager.WaitTillReceiptAvailable(txId);

        Debug.Log("NFT deployed, it's address: " + res.NewContractAddress);
    }

    private async Task DisplayTransfersAsync(string nftAddress)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[NFT transfers]");

        List<ReceiptResponse> transferReceipts = (await client.ReceiptSearchAsync(nftAddress, "TransferLog", null, 2300000, null).ConfigureAwait(false)).ToList();
        
        foreach (ReceiptResponse receiptRes in transferReceipts)
        {
            string log = receiptRes.Logs.First().Log.ToString();
            TransferLogEvent transferEvent = JsonConvert.DeserializeObject<TransferLogEvent>(log);

            sb.AppendLine($"From: {transferEvent.From}, To: {transferEvent.To}, TokenId: {transferEvent.TokenId}");
        }

        Debug.Log(sb.ToString());
    }
}
