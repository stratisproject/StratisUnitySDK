using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Unity3dApi;
using UnityEngine;
using Network = NBitcoin.Network;

public class StratisUnityManager
{
    private Unity3dClient client;

    private Network network;

    private Mnemonic mnemonic;

    private Key privateKey;

    private PubKey publicKey;

    private BitcoinPubKeyAddress address;

    public StratisUnityManager(Unity3dClient client, Network network, Mnemonic mnemonic)
    {
        this.client = client;
        this.network = network;
        this.mnemonic = mnemonic;

        this.privateKey = this.mnemonic.DeriveExtKey().PrivateKey;
        this.publicKey = this.privateKey.PubKey;
        this.address = this.publicKey.GetAddress(network);
    }

    public async Task<decimal> GetBalanceAsync()
    {
        long balanceSat = await client.GetAddressBalanceAsync(this.address.ToString()).ConfigureAwait(false);

        decimal balance = new Money(balanceSat).ToUnit(MoneyUnit.BTC);

        return balance;
    }

    public BitcoinPubKeyAddress GetAddress()
    {
        return this.address;
    }

    public async Task<string> SendTransactionAsync(string destinationAddress, Money sendAmount)
    {
        Coin[] coins = await this.GetCoinsAsync().ConfigureAwait(false);

        BitcoinPubKeyAddress addrTo = new BitcoinPubKeyAddress(destinationAddress, this.network);
    
        var txBuilder = new TransactionBuilder(this.network);
        Transaction tx = txBuilder
            .AddCoins(coins)
            .AddKeys(this.privateKey)
            .Send(addrTo, sendAmount)
            .SendFees("0.0001")
            .SetChange(this.address)
            .BuildTransaction(true);
    
        if (!txBuilder.Verify(tx))
            Debug.LogError("Tx wasn't fully signed!");

        Debug.Log(string.Format("Created tx {0} to {1}, amount: {2}.", tx.GetHash(), destinationAddress, sendAmount));

        await client.SendTransactionAsync(new SendTransactionRequest() {Hex = tx.ToHex()}).ConfigureAwait(false);

        Debug.Log("Transaction sent.");
        return tx.GetHash().ToString();
    }

    public async Task<string> SendOpReturnTransactionAsync(string opReturnData)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(opReturnData);

        return await this.SendOpReturnTransactionAsync(bytes).ConfigureAwait(false);
    }

    public async Task<string> SendOpReturnTransactionAsync(byte[] bytes)
    {
        Coin[] coins = await this.GetCoinsAsync().ConfigureAwait(false);

        Script opReturnScript = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes);

        var txBuilder = new TransactionBuilder(this.network);
        Transaction tx = txBuilder
            .AddCoins(coins)
            .AddKeys(this.privateKey)
            .Send(opReturnScript, Money.Zero)
            .SendFees("0.0001")
            .SetChange(this.address)
            .BuildTransaction(true);

        if (!txBuilder.Verify(tx))
            Debug.LogError("Tx wasn't fully signed!");

        Debug.Log(string.Format("Created OP_RETURN tx {0}, data: {1}.", tx.GetHash(), Encoding.UTF8.GetString(bytes)));

        await client.SendTransactionAsync(new SendTransactionRequest() { Hex = tx.ToHex() }).ConfigureAwait(false);

        Debug.Log("Transaction sent.");
        return tx.GetHash().ToString();
    }

    private async Task<Coin[]> GetCoinsAsync()
    {
        GetUTXOsResponseModel utxos = await client.GetUTXOsForAddressAsync(this.address.ToString()).ConfigureAwait(false);

        Coin[] coins = utxos.Utxos.Select(x => new Coin(new OutPoint(uint256.Parse(x.Hash), x.N), new TxOut(new Money(x.Satoshis), address))).ToArray();

        return coins;
    }
}
