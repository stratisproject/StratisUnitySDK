using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace Stratis.Sidechains.Networks
{
    public class CirrusMain : Network
    {
        public CirrusMain()
        {
            this.Name = "CirrusMain";
            this.NetworkType = NetworkType.Mainnet;
            this.CoinTicker = "CRS";
            this.Magic = 0x522357AC;
            this.DefaultPort = 16179;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.DefaultRPCPort = 16175;
            this.DefaultAPIPort = 37223;
            this.DefaultSignalRPort = 38823;
            this.MaxTipAge = 768; // 20% of the fastest time it takes for one MaxReorgLength of blocks to be mined.
            this.MinTxFee = 10000;
            this.FallbackFee = 10000;
            this.MinRelayTxFee = 10000;
            this.RootFolderName = CirrusNetwork.NetworkRootFolderName;
            this.DefaultConfigFilename = CirrusNetwork.NetworkDefaultConfigFilename;
            this.MaxTimeOffsetSeconds = 25 * 60;
            this.DefaultBanTimeSeconds = 1920; // 240 (MaxReorg) * 16 (TargetSpacing) / 2 = 32 Minutes

            this.CirrusRewardDummyAddress = "CPqxvnzfXngDi75xBJKqi4e6YrFsinrJka";

            var consensusFactory = new ConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1561982325;
            this.GenesisNonce = 3038481;
            this.GenesisBits = new Target(new uint256("00000fffff000000000000000000000000000000000000000000000000000000"));
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            string coinbaseText = "https://github.com/stratisproject/StratisBitcoinFullNode";
            Block genesisBlock = CirrusNetwork.CreateGenesis(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward, coinbaseText);

            this.Genesis = genesisBlock;
            
            this.Federations = new Federations();
            var straxFederationTransactionSigningKeys = new List<PubKey>()
            {
                new PubKey("03797a2047f84ba7dcdd2816d4feba45ae70a59b3aa97f46f7877df61aa9f06a21"),
                new PubKey("0209cfca2490dec022f097114090c919e85047de0790c1c97451e0f50c2199a957"),
                new PubKey("032e4088451c5a7952fb6a862cdad27ea18b2e12bccb718f13c9fdcc1caf0535b4"),
                new PubKey("035bf78614171397b080c5b375dbb7a5ed2a4e6fb43a69083267c880f66de5a4f9"),
                new PubKey("02387a219b1de54d4dc73a710a2315d957fc37ab04052a6e225c89205b90a881cd"),
                new PubKey("028078c0613033e5b4d4745300ede15d87ed339e379daadc6481d87abcb78732fa"),
                new PubKey("02b3e16d2e4bbad6dba1e699934a52d58d9b60b6e7eed303e400e95f2dbc2ef3fd"),
                new PubKey("02ba8b842997ce50c8e29c24a5452de5482f1584ae79778950b7bae24d4cc68dad"),
                new PubKey("02cbd907b0bf4d757dee7ea4c28e63e46af19dc8df0c924ee5570d9457be2f4c73"),
                new PubKey("02d371f3a0cffffcf5636e6d4b79d9f018a1a18fbf64c39542b382c622b19af9de"),
                new PubKey("02f891910d28fc26f272da8d7f548fdc18c286704907673e839dc07e8df416c15e"),
                new PubKey("0337e816a3433c71c4bbc095a54a0715a6da7a70526d2afb8dba3d8d78d33053bf"),
                new PubKey("035569e42835e25c854daa7de77c20f1009119a5667494664a46b5154db7ee768a"),
                new PubKey("03cda7ea577e8fbe5d45b851910ec4a795e5cc12d498cf80d39ba1d9a455942188"),
                new PubKey("02680321118bce869933b07ea42cc04d2a2804134b06db582427d6b9688b3536a4")
            };

            // Register the new set of federation members.
            this.Federations.RegisterFederation(new Federation(straxFederationTransactionSigningKeys));
            
            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new NoBIP9Deployments();

            this.Consensus = new Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: null,
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

            // Same as current smart contracts test networks to keep tests working
            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { 28 }; // C
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { 88 }; // c
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (239) };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x35), (0x87), (0xCF) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x35), (0x83), (0x94) };
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
            this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2b };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 115 };
            this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

            Bech32Encoder encoder = Encoders.Bech32("tb");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            this.StandardScriptsRegistry = new PoAStandardScriptsRegistry();

            Assert(this.DefaultBanTimeSeconds <= this.Consensus.MaxReorgLength * this.Consensus.TargetSpacing.TotalSeconds / 2);
            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("000005769503496300ec879afd7543dc9f86d3b3d679950b2b83e2f49f525856"));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("1669a55d45b642af0ce82c5884cf5b8d8efd5bdcb9a450c95f442b9bd1ff65ea"));
        }
    }

    /// <summary>
    /// PoA-specific standard transaction definitions.
    /// </summary>
    public class PoAStandardScriptsRegistry : StandardScriptsRegistry
    {
        public const int MaxOpReturnRelay = 153;

        private static readonly List<ScriptTemplate> scriptTemplates = new List<ScriptTemplate>
        {
            { new PayToPubkeyHashTemplate() },
            { new PayToPubkeyTemplate() },
            { new PayToScriptHashTemplate() },
            { new PayToMultiSigTemplate() },
            { new PayToFederationTemplate() },
            { new TxNullDataTemplate(MaxOpReturnRelay) },
            { new PayToWitTemplate() }
        };

        public override List<ScriptTemplate> GetScriptTemplates => scriptTemplates;
    }
}
