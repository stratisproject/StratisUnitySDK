using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Sidechains.Networks;
using Unity3dApi;
using UnityEngine;
using UnityEngine.Assertions;
using Network = NBitcoin.Network;

public class NFTExample : MonoBehaviour
{
    public const string ApiUrl = "http://localhost:44336/";

    private readonly string firstAddrMnemonic = "legal door leopard fire attract stove similar response photo prize seminar frown";

    private readonly string secondAddrMnemonic = "want honey rifle wisdom verb sunny enter virtual phone kite aunt surround";

    private string firstAddress, secondAddress;

    private StratisUnityManager stratisUnityManager;

    private Network network = new CirrusTest();

    async void Start()
    {
        Unity3dClient client = new Unity3dClient(ApiUrl);

        Mnemonic mnemonic = new Mnemonic(firstAddrMnemonic, Wordlist.English);
        stratisUnityManager = new StratisUnityManager(client, network, mnemonic);
        firstAddress = stratisUnityManager.GetAddress().ToString();
        secondAddress = new Mnemonic(secondAddrMnemonic).DeriveExtKey().PrivateKey.PubKey.GetAddress(network).ToString();

        Debug.Log("FirstAddr: " + firstAddress);
        Debug.Log("SecondAddr: " + secondAddress);

        string nftAddress = "t8snCz4kQgovGTAGReAryt863NwEYqjJqy";
        NFTWrapper nft = new NFTWrapper(stratisUnityManager, nftAddress);

        ulong balanceFirstAddr = await nft.BalanceOfAsync(this.firstAddress).ConfigureAwait(false);
        ulong balanceSecondAddr = await nft.BalanceOfAsync(this.secondAddress).ConfigureAwait(false);

        Debug.Log("NFT balance first addr: " + balanceFirstAddr + "    second addr: " + balanceSecondAddr);
        
        // Now let's get token ID
        List<ReceiptResponse> receipts = (await client.ReceiptSearchAsync(nftAddress, null, null, 2300000, null).ConfigureAwait(false)).ToList();

        foreach (ReceiptResponse receiptRes in receipts)
        {
            var log = receiptRes.Logs.First().Log;
            Debug.Log(log.ToString());
        }

        return;

        ulong tokenId = 12345;
        string txId = await nft.TransferFromAsync("tD5aDZSu4Go4A23R7VsjuJTL51YMyeoLyS", "tP2r8anKBWczcBR89yv7rQ1rsSZA2BANhd", tokenId);

        ReceiptResponse receipt = await stratisUnityManager.WaitTillReceiptAvailable(txId);

        Debug.Log("Success: " + receipt.Success + "   returned: " + receipt.ReturnValue);
    }

    private async void DeployNFT()
    {
        string nftName = "gameSword";
        string nftSymbol = "GS";

        string txId = await NFTWrapper.DeployNFTContractAsync(this.stratisUnityManager, nftName, nftSymbol, nftName + "_{0}", false);

        ReceiptResponse res = await this.stratisUnityManager.WaitTillReceiptAvailable(txId).ConfigureAwait(false);

        Debug.Log("NFT deployed, it's address: " + res.NewContractAddress);
    }
}
