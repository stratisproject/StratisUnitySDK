using System;
using NBitcoin;

/// <summary>
/// Represent a coin which need a redeem script to be spent (P2SH or P2WSH)
/// </summary>
public class ScriptCoinSC : Coin
{
    public ScriptCoinSC()
    {

    }

    public ScriptCoinSC(OutPoint fromOutpoint, TxOut fromTxOut, Script redeem)
        : base(fromOutpoint, fromTxOut)
    {
        this.Redeem = redeem;
    }

    internal ScriptCoinSC(Transaction fromTx, uint fromOutputIndex, Script redeem)
        : base(fromTx, fromOutputIndex)
    {
        this.Redeem = redeem;
    }

    internal ScriptCoinSC(Transaction fromTx, TxOut fromOutput, Script redeem)
        : base(fromTx, fromOutput)
    {
        this.Redeem = redeem;
    }

    internal ScriptCoinSC(ICoin coin, Script redeem)
        : base(coin.Outpoint, coin.TxOut)
    {
        this.Redeem = redeem;
    }

    internal ScriptCoinSC(IndexedTxOut txOut, Script redeem)
        : base(txOut)
    {
        this.Redeem = redeem;
    }

    internal ScriptCoinSC(uint256 txHash, uint outputIndex, Money amount, Script scriptPubKey, Script redeem)
        : base(txHash, outputIndex, amount, scriptPubKey)
    {
        this.Redeem = redeem;
    }

    public static ScriptCoinSC Create(Network network, OutPoint fromOutpoint, TxOut fromTxOut, Script redeem)
    {
        return new ScriptCoinSC(fromOutpoint, fromTxOut, redeem).AssertCoherent(network);
    }

    public static ScriptCoinSC Create(Network network, Transaction fromTx, uint fromOutputIndex, Script redeem)
    {
        return new ScriptCoinSC(fromTx, fromOutputIndex, redeem).AssertCoherent(network);
    }

    public static ScriptCoinSC Create(Network network, Transaction fromTx, TxOut fromOutput, Script redeem)
    {
        return new ScriptCoinSC(fromTx, fromOutput, redeem).AssertCoherent(network);
    }

    public static ScriptCoinSC Create(Network network, ICoin coin, Script redeem)
    {
        return new ScriptCoinSC(coin, redeem).AssertCoherent(network);
    }

    public static ScriptCoinSC Create(Network network, IndexedTxOut txOut, Script redeem)
    {
        return new ScriptCoinSC(txOut, redeem).AssertCoherent(network);
    }

    public static ScriptCoinSC Create(Network network, uint256 txHash, uint outputIndex, Money amount, Script scriptPubKey, Script redeem)
    {
        return new ScriptCoinSC(txHash, outputIndex, amount, scriptPubKey, redeem).AssertCoherent(network);
    }

    public bool IsP2SH
    {
        get
        {
            return this.ScriptPubKey.ToBytes(true)[0] == (byte)OpcodeType.OP_HASH160;
        }
    }

    public Script GetP2SHRedeem()
    {
        if (!this.IsP2SH)
            return null;
        Script p2shRedeem = this.RedeemType == RedeemType.P2SH ? this.Redeem :
            this.RedeemType == RedeemType.WitnessV0 ? this.Redeem.WitHash.ScriptPubKey :
                        null;
        if (p2shRedeem == null)
            throw new NotSupportedException("RedeemType not supported for getting the P2SH script, contact the library author");
        return p2shRedeem;
    }

    public RedeemType RedeemType
    {
        get
        {
            return this.Redeem.Hash.ScriptPubKey == this.TxOut.ScriptPubKey ?
                RedeemType.P2SH :
                RedeemType.WitnessV0;
        }
    }

    public ScriptCoinSC AssertCoherent(Network network)
    {
        if (this.Redeem == null)
            throw new ArgumentException("redeem cannot be null", "redeem");

        TxDestination expectedDestination = GetRedeemHash(network, this.TxOut.ScriptPubKey);
        if (expectedDestination == null)
        {
            throw new ArgumentException("the provided scriptPubKey is not P2SH or P2WSH");
        }
        if (expectedDestination is ScriptId)
        {
            if (PayToWitScriptHashTemplate.Instance.CheckScriptPubKey(this.Redeem))
            {
                throw new ArgumentException("The redeem script provided must be the witness one, not the P2SH one");
            }

            if (expectedDestination.ScriptPubKey != this.Redeem.Hash.ScriptPubKey)
            {
                if (this.Redeem.WitHash.ScriptPubKey.Hash.ScriptPubKey != expectedDestination.ScriptPubKey)
                    throw new ArgumentException("The redeem provided does not match the scriptPubKey of the coin");
            }
        }
        else if (expectedDestination is WitScriptId)
        {
            if (expectedDestination.ScriptPubKey != this.Redeem.WitHash.ScriptPubKey)
                throw new ArgumentException("The redeem provided does not match the scriptPubKey of the coin");
        }
        else
            throw new NotSupportedException("Not supported redeemed scriptPubkey");

        return this;
    }


    public Script Redeem
    {
        get;
        set;
    }

    public override Script GetScriptCode(Network network)
    {
        if (!CanGetScriptCode(network))
            throw new InvalidOperationException("You need to provide the P2WSH redeem script with ScriptCoinSC.ToScriptCoin()");
        if (this._OverrideScriptCode != null)
            return this._OverrideScriptCode;
        WitKeyId key = PayToWitPubKeyHashTemplate.Instance.ExtractScriptPubKeyParameters(network, this.Redeem);
        if (key != null)
            return key.AsKeyId().ScriptPubKey;
        return this.Redeem;
    }

    public override bool CanGetScriptCode(Network network)
    {
        return this._OverrideScriptCode != null || !this.IsP2SH || !PayToWitScriptHashTemplate.Instance.CheckScriptPubKey(this.Redeem);
    }

    public override HashVersion GetHashVersion(Network network)
    {
        bool isWitness = PayToWitTemplate.Instance.CheckScriptPubKey(this.ScriptPubKey) ||
                        PayToWitTemplate.Instance.CheckScriptPubKey(this.Redeem) || this.RedeemType == RedeemType.WitnessV0;
        return isWitness ? HashVersion.Witness : HashVersion.Original;
    }

    /// <summary>
    /// Returns the hash contained in the scriptPubKey (P2SH or P2WSH)
    /// </summary>
    /// <param name="scriptPubKey">The scriptPubKey</param>
    /// <returns>The hash of the scriptPubkey</returns>
    public static TxDestination GetRedeemHash(Network network, Script scriptPubKey)
    {
        if (scriptPubKey == null)
            throw new ArgumentNullException("scriptPubKey");
        return PayToScriptHashTemplate.Instance.ExtractScriptPubKeyParameters(scriptPubKey) as TxDestination
                ??
                PayToWitScriptHashTemplate.Instance.ExtractScriptPubKeyParameters(network, scriptPubKey);
    }
}
