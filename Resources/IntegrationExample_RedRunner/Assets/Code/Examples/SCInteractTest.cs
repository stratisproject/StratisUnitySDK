using System;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Sidechains.Networks;
using Unity3dApi;
using UnityEngine;
using UnityEngine.Assertions;
using Network = NBitcoin.Network;

/// <summary>
/// This example tests wrappers for StandartToken and NFT contracts.
/// </summary>
public class SCInteractTest : MonoBehaviour
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
        Debug.Log("Your address: " + firstAddress);

        decimal balance = await stratisUnityManager.GetBalanceAsync();
        Debug.Log("Your balance: " + balance);

        secondAddress = new Mnemonic(secondAddrMnemonic).DeriveExtKey().PrivateKey.PubKey.GetAddress(network).ToString();

        // StandartTokenWrapper test.
        //await this.StandartTokenWrapperTestAsync().ConfigureAwait(false);

        // NFT wrapper test.
        await this.NFTWrapperTestAsync().ConfigureAwait(false);
    }

    private async Task StandartTokenWrapperTestAsync()
    {
        // If you want to deploy new instance of standart token contract use: StandartTokenWrapper.DeployStandartTokenAsync
        // For this example we will be using already deployed contract.
        Debug.Log("Testing StandartTokenWrapper.");

        string standartTokenAddr = "tLG1Eap1f7H5tnRwhs58Jn7NVDrP3YTgrg";
        StandartTokenWrapper stw = new StandartTokenWrapper(stratisUnityManager, standartTokenAddr);

        Debug.Log("Symbol: " + await stw.GetSymbolAsync());
        Debug.Log("Name: " + await stw.GetNameAsync());
        Debug.Log("TotalSupply: " + await stw.GetTotalSupplyAsync());
        Debug.Log("Balance: " + await stw.GetBalanceAsync(firstAddress));
        Debug.Log("Decimals: " + await stw.GetDecimalsAsync());
        Debug.Log("Allowance: " + await stw.GetAllowanceAsync(firstAddress, secondAddress));

        ulong firstAddrBalance = await stw.GetBalanceAsync(firstAddress);
        ulong secondAddrBalance = await stw.GetBalanceAsync(secondAddress);

        // Transfer 1 to 2nd address.
        var txId = await stw.TransferToAsync(secondAddress, 1);

        var receipt = await this.stratisUnityManager.WaitTillReceiptAvailable(txId).ConfigureAwait(false);

        Assert.IsTrue(bool.Parse(receipt.ReturnValue));

        ulong firstAddrBalance2 = await stw.GetBalanceAsync(firstAddress);
        ulong secondAddrBalance2 = await stw.GetBalanceAsync(secondAddress);

        Assert.IsTrue(firstAddrBalance - firstAddrBalance2 == 1);
        Assert.IsTrue(secondAddrBalance2 - secondAddrBalance == 1);

        Debug.Log("StandartTokenWrapper test successful.");
    }

    private async Task NFTWrapperTestAsync()
    {
        // If you want to deploy new instance of standart token contract use: NFTWrapper.DeployNFTContractAsync
        // For this example we will be using already deployed contract.

        // Contract deployment:
        //string deplId = await NFTWrapper.DeployNFTContractAsync(this.stratisUnityManager, "TestNFT", "TNFT", "TestNFT_{0}", false).ConfigureAwait(false);
        //ReceiptResponse res = await this.stratisUnityManager.WaitTillReceiptAvailable(deplId).ConfigureAwait(false);
        //Debug.Log(res.NewContractAddress);

        Debug.Log("Testing NFT.");
        string nftAddr = "t8snCz4kQgovGTAGReAryt863NwEYqjJqy";
        NFTWrapper nft = new NFTWrapper(stratisUnityManager, nftAddr);

        ulong balanceBefore = await nft.BalanceOfAsync(this.firstAddress).ConfigureAwait(false);
        Debug.Log("NFT balance: " + balanceBefore);

        string mintId = await nft.MintAsync(firstAddress).ConfigureAwait(false);

        await this.stratisUnityManager.WaitTillReceiptAvailable(mintId).ConfigureAwait(false);
        
        ulong balanceAfter = await nft.BalanceOfAsync(this.firstAddress).ConfigureAwait(false);

        Assert.IsTrue(balanceAfter == balanceBefore + 1);

        Assert.AreEqual("TestNFT", await nft.NameAsync().ConfigureAwait(false));
        Assert.AreEqual("TNFT", await nft.SymbolAsync().ConfigureAwait(false));
        
        Debug.Log("NFT test successful.");
    }
}
