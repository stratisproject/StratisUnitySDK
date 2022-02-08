using System.Threading.Tasks;
using NBitcoin;
using Stratis.Sidechains.Networks;
using Stratis.SmartContracts;
using Unity3dApi;
using UnityEngine;
using UnityEngine.Assertions;
using Network = NBitcoin.Network;

/// <summary>
/// This example tests wrappers for StandardToken and NFT contracts.
/// </summary>
public class SCInteractTest : MonoBehaviour
{
    public string ApiUrl = "http://localhost:44336/";

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

        // StandardTokenWrapper test.
        //await this.StandardTokenWrapperTestAsync();

        // NFT wrapper test.
        await this.NFTWrapperTestAsync();
    }

    private async Task StandardTokenWrapperTestAsync()
    {
        // If you want to deploy new instance of standard token contract use: StandardTokenWrapper.DeployStandardTokenAsync
        // For this example we will be using already deployed contract.
        Debug.Log("Testing StandardTokenWrapper.");

        string standardTokenAddr = "tLG1Eap1f7H5tnRwhs58Jn7NVDrP3YTgrg";
        StandardTokenWrapper stw = new StandardTokenWrapper(stratisUnityManager, standardTokenAddr);

        Debug.Log("Symbol: " + await stw.GetSymbolAsync());
        Debug.Log("Name: " + await stw.GetNameAsync());
        Debug.Log("TotalSupply: " + await stw.GetTotalSupplyAsync());
        Debug.Log("Balance: " + await stw.GetBalanceAsync(firstAddress));
        Debug.Log("Decimals: " + await stw.GetDecimalsAsync());
        Debug.Log("Allowance: " + await stw.GetAllowanceAsync(firstAddress, secondAddress));

        UInt256 firstAddrBalance = await stw.GetBalanceAsync(firstAddress);
        UInt256 secondAddrBalance = await stw.GetBalanceAsync(secondAddress);

        // Transfer 1 to 2nd address.
        var txId = await stw.TransferToAsync(secondAddress, 1);

        var receipt = await this.stratisUnityManager.WaitTillReceiptAvailable(txId);

        Assert.IsTrue(bool.Parse(receipt.ReturnValue));

        UInt256 firstAddrBalance2 = await stw.GetBalanceAsync(firstAddress);
        UInt256 secondAddrBalance2 = await stw.GetBalanceAsync(secondAddress);

        Assert.IsTrue(firstAddrBalance - firstAddrBalance2 == 1);
        Assert.IsTrue(secondAddrBalance2 - secondAddrBalance == 1);

        Debug.Log("StandardTokenWrapper test successful.");
    }

    private async Task NFTWrapperTestAsync()
    {
        // If you want to deploy new instance of standard token contract use: NFTWrapper.DeployNFTContractAsync
        // For this example we will be using already deployed contract.

        // Contract deployment:
        //string deplId = await NFTWrapper.DeployNFTContractAsync(this.stratisUnityManager, "TestNFT", "TNFT", false);
        //ReceiptResponse res = await this.stratisUnityManager.WaitTillReceiptAvailable(deplId);
        //Debug.Log(res.NewContractAddress);

        Debug.Log("Testing NFT.");
        string nftAddr = "tHK8Qf7WrUaKqk9nF9JsQfPqwpVvJNKNKn";
        NFTWrapper nft = new NFTWrapper(stratisUnityManager, nftAddr);

        UInt256 balanceBefore = await nft.BalanceOfAsync(this.firstAddress);
        Debug.Log("NFT balance: " + balanceBefore);

        string mintId = await nft.MintAsync(firstAddress, "uri");

        await this.stratisUnityManager.WaitTillReceiptAvailable(mintId);

        UInt256 balanceAfter = await nft.BalanceOfAsync(this.firstAddress);

        Assert.IsTrue(balanceAfter == balanceBefore + 1);

        Assert.AreEqual("TestNFT", await nft.NameAsync().ConfigureAwait(false));
        Assert.AreEqual("TNFT", await nft.SymbolAsync().ConfigureAwait(false));

        Debug.Log("NFT test successful.");
    }
}
