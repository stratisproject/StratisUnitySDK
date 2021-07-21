using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using Stratis.Sidechains.Networks;

public class PoANetwork : Network
{
    /// <summary> The name of the root folder containing the different PoA blockchains.</summary>
    private const string NetworkRootFolderName = "poa";

    /// <summary> The default name used for the Stratis configuration file. </summary>
    private const string NetworkDefaultConfigFilename = "poa.conf";
    
    /// <summary>
    /// This is the height at which collateral commitment height data was committed to blocks.
    /// </summary>
    public int CollateralCommitmentActivationHeight { get; set; }

    /// <summary>
    /// The height at which the StraxMiningMultisigMembers become applicable.
    /// </summary>
    public int? MultisigMinersApplicabilityHeight { get; set; }

    /// <summary> The mining keys of the new multisig members to become active with the first Strax-era Cirrus collateral block mined.</summary>
    public IList<PubKey> StraxMiningMultisigMembers { get; protected set; }

    public IList<Key> FederationKeys { get; set; }

    public PoANetwork()
    {
        // The message start string is designed to be unlikely to occur in normal data.
        // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
        // a large 4-byte int at any alignment.
        var messageStart = new byte[4];
        messageStart[0] = 0x76;
        messageStart[1] = 0x36;
        messageStart[2] = 0x23;
        messageStart[3] = 0x06;
        uint magic = BitConverter.ToUInt32(messageStart, 0);

        this.Name = "PoAMain";
        this.NetworkType = NetworkType.Mainnet;
        this.Magic = magic;
        this.DefaultPort = 16438;
        this.DefaultMaxOutboundConnections = 16;
        this.DefaultMaxInboundConnections = 109;
        this.DefaultRPCPort = 16474;
        this.DefaultAPIPort = 37221; // TODO: Confirm
        this.MaxTipAge = 2 * 60 * 60;
        this.MinTxFee = 10000;
        this.FallbackFee = 10000;
        this.MinRelayTxFee = 10000;
        this.RootFolderName = NetworkRootFolderName;
        this.DefaultConfigFilename = NetworkDefaultConfigFilename;
        this.MaxTimeOffsetSeconds = 25 * 60;
        this.CoinTicker = "POA";
        
        // Create the genesis block.
        this.GenesisTime = 1513622125;
        this.GenesisNonce = 1560058197;
        this.GenesisBits = 402691653;
        this.GenesisVersion = 1;
        this.GenesisReward = Money.Zero;

        Block genesisBlock = CreatePoAGenesisBlock(new ConsensusFactory(), this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);

        this.Genesis = genesisBlock;
        
        var buriedDeployments = new BuriedDeploymentsArray
        {
            [BuriedDeployments.BIP34] = 0,
            [BuriedDeployments.BIP65] = 0,
            [BuriedDeployments.BIP66] = 0
        };

        var bip9Deployments = new NoBIP9Deployments();

        var consensusOptions = new ConsensusOptions(
            maxBlockBaseSize: 1_000_000,
            maxStandardVersion: 2,
            maxStandardTxWeight: 150_000,
            maxBlockSigopsCost: 20_000,
            maxStandardTxSigopsCost: 20_000 / 5, 1
        )
        {
            EnforceMinProtocolVersionAtBlockHeight = 384675, // setting the value to zero makes the functionality inactive
            EnforcedMinProtocolVersion = ProtocolVersion.CIRRUS_VERSION, // minimum protocol version which will be enforced at block height defined in EnforceMinProtocolVersionAtBlockHeight
        };

        this.Consensus = new Consensus(
            consensusFactory: new ConsensusFactory(),
            consensusOptions: consensusOptions,
            coinType: 401,
            hashGenesisBlock: genesisBlock.GetHash(),
            subsidyHalvingInterval: 210000,
            majorityEnforceBlockUpgrade: 750,
            majorityRejectBlockOutdated: 950,
            majorityWindow: 1000,
            buriedDeployments: buriedDeployments,
            bip9Deployments: bip9Deployments,
            bip34Hash: new uint256("0x000000000000024b89b42a942fe0d9fea3bb44ab7bd1b19115dd6a759c0808b8"),
            minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
            maxReorgLength: 240, // Heuristic. Roughly 2 * mining members
            defaultAssumeValid: new uint256("0xbfd4a96a6c5250f18bf7c586761256fa5f8753ffa10b24160f0648a452823a95"), // 1400000
            maxMoney: Money.Coins(100_000_000),
            coinbaseMaturity: 1,
            premineHeight: 2,
            premineReward: Money.Coins(100_000_000),
            proofOfWorkReward: Money.Coins(0),
            powTargetTimespan: TimeSpan.FromDays(14), // two weeks
            targetSpacing: TimeSpan.FromSeconds(16),
            powAllowMinDifficultyBlocks: false,
            posNoRetargeting: false,
            powNoRetargeting: true,
            powLimit: null,
            minimumChainWork: null,
            isProofOfStake: false,
            lastPowBlock: 0,
            proofOfStakeLimit: null,
            proofOfStakeLimitV2: null,
            proofOfStakeReward: Money.Zero
        );

        // https://en.bitcoin.it/wiki/List_of_address_prefixes
        this.Base58Prefixes = new byte[12][];
        this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (55) }; // 'P' prefix
        this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (117) }; // 'p' prefix
        this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (63 + 128) };
        this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
        this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
        this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
        this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
        this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
        this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
        this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2a };
        this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 23 };
        this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

        this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                { 0, new CheckpointInfo(new uint256("0x0621b88fb7a99c985d695be42e606cb913259bace2babe92970547fa033e4076")) },
            };

        var encoder = new Bech32Encoder("bc");
        this.Bech32Encoders = new Bech32Encoder[2];
        this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
        this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;
        
        this.StandardScriptsRegistry = new PoAStandardScriptsRegistry();

        Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x0621b88fb7a99c985d695be42e606cb913259bace2babe92970547fa033e4076"));
        Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("0x9928b372fd9e4cf62a31638607344c03c48731ba06d24576342db9c8591e1432"));
    }
    
    protected static Block CreatePoAGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
    {
        string data = "506f41202d204345485450414a6c75334f424148484139205845504839";

        Transaction txNew = consensusFactory.CreateTransaction();
        txNew.Version = 1;
        // TODO: Removing the time field will affect the genesis block hash of the Cirrus networks. Need to make a call about only developing Cirrus via the SBFN project that still has nTime
        txNew.AddInput(new TxIn()
        {
            ScriptSig = new Script(Op.GetPushOp(0), new Op()
            {
                Code = (OpcodeType)0x1,
                PushData = new[] { (byte)42 }
            }, Op.GetPushOp(Encoders.ASCII.DecodeData(data)))
        });
        txNew.AddOutput(new TxOut()
        {
            Value = genesisReward,
        });

        Block genesis = consensusFactory.CreateBlock();
        genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
        genesis.Header.Bits = nBits;
        genesis.Header.Nonce = nNonce;
        genesis.Header.Version = nVersion;
        genesis.Transactions.Add(txNew);
        genesis.Header.HashPrevBlock = uint256.Zero;
        genesis.UpdateMerkleRoot();
        return genesis;
    }
}