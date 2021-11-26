using System;
using System.Collections.Generic;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;

namespace Stratis.Sidechains.Networks
{
    public class CirrusTest : Network
    {
        public CirrusTest()
        {
            this.Name = "CirrusTest";
            this.NetworkType = NetworkType.Testnet;
            this.CoinTicker = "TCRS";
            this.Magic = 0x522357B;
            this.DefaultPort = 26179;
            this.DefaultMaxOutboundConnections = 16;
            this.DefaultMaxInboundConnections = 109;
            this.DefaultRPCPort = 26175;
            this.DefaultAPIPort = 38223;
            this.DefaultSignalRPort = 39823;
            this.MaxTipAge = 768; // 20% of the fastest time it takes for one MaxReorgLength of blocks to be mined.
            this.MinTxFee = 10000;
            this.FallbackFee = 10000;
            this.MinRelayTxFee = 10000;
            this.MaxTimeOffsetSeconds = 25 * 60;
            this.DefaultBanTimeSeconds = 1920; // 240 (MaxReorg) * 16 (TargetSpacing) / 2 = 32 Minutes

            this.CirrusRewardDummyAddress = "tGXZrZiU44fx3SQj8tAQ3Zexy2VuELZtoh";

            var consensusFactory = new ConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1556631753;
            this.GenesisNonce = 146421;
            this.GenesisBits = new Target(new uint256("0000ffff00000000000000000000000000000000000000000000000000000000"));
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

            string coinbaseText = "https://github.com/stratisproject/StratisBitcoinFullNode/tree/master/src/Stratis.CirrusD";
            Block genesisBlock = CirrusNetwork.CreateGenesis(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward, coinbaseText);

            this.Genesis = genesisBlock;
            
            this.Federations = new Federations();
            var straxFederationTransactionSigningKeys = new List<PubKey>()
            {
               new PubKey("021040ef28c82fcffb63028e69081605ed4712910c8384d5115c9ffeacd9dbcae4"),//Node1
               new PubKey("0244290a31824ba7d53e59c7a29d13dbeca15a9b0d36fdd4d28fce426753107bfc"),//Node2
               new PubKey("032df4a2d62c0db12cd1d66201819a10788637c9b90a1cd2a5a3f5196fdab7a621"),//Node3
               new PubKey("028ed190eb4ed6e46440ac6af21d8a67a537bd1bd7edb9cc5177d36d5a0972244d"),//Node4
               new PubKey("02ff9923324399a188daf4310825a85dd3b89e2301d0ad073295b6f33ae1c72f7a"),//Node5
               new PubKey("030e03b808ddb51701d4d3dbc0a74a6f9aedfecf23d5f874914641fc81197b239a"),//Node7
               new PubKey("02270d6c20d3393fad7f74c59d2d26b0824ed016ccbc15e698e7354314459a60a5"),//Node8
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
                coinType: 400,
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
                defaultAssumeValid: new uint256("0x57a3119de52cf43b66d6e805a644c20fdee63557038cd68c429d47b21d111084"), // 1800000
                maxMoney: Money.Coins(20_000_000),
                coinbaseMaturity: 1,
                premineHeight: 2,
                premineReward: Money.Coins(20_000_000),
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
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { 127 }; // t
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { 137 }; // x
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

            this.Checkpoints = new Dictionary<int, CheckpointInfo>()
            {
                { 50000, new CheckpointInfo(new uint256("0x2b2a85fcad21c4e5c91a7afef04dce2eb72426b0c6275d87669a561f9f6db1f3")) },
                { 100000, new CheckpointInfo(new uint256("0x364be98c01780accfea63c52703b7dc4731fdead1b6769cf9a893b4e6c736f10")) },
                { 150000, new CheckpointInfo(new uint256("0xaf862418d54d95221dac077cdbd0d49d68304d67721df7b44775739f093985f1")) },
                { 200000, new CheckpointInfo(new uint256("0x40f99ccbd290c2c66c16eac602b4a8b4dc7d87bfceb31c64ae5942d5899e86b2")) },
                { 250000, new CheckpointInfo(new uint256("0x33deee954579b8b3ffde1d9246a3e9e548dc7f4f8c0c9cbf206eb14ac04ab500")) },
                { 300000, new CheckpointInfo(new uint256("0x1c1670f9ea4d211abe255a516be95ec6329d03e0ebfc81890cae0900e9f07964")) },
                { 350000, new CheckpointInfo(new uint256("0x5b3493243a9f8c8997acad7cb13058e11a9d2d91c9494ebe5c88446540640472")) },
                { 400000, new CheckpointInfo(new uint256("0x33d57af0bc04916eb43f6d5c3f0b97b0f281662feac3d03b987bb9ab4978fe0a")) },
                { 450000, new CheckpointInfo(new uint256("0x7c85cc3aa0694c7573b1455e555c9f6a919dfa916381d6c094cdc2da46a0c7bc")) },
                { 500000, new CheckpointInfo(new uint256("0x3f00eb415856128976e786cb094e88d4dfaabfedea462498386e201c1ac2a1fa")) },
                { 550000, new CheckpointInfo(new uint256("0xaaf247bd66568db8945fc8947525539160073bcfb4a60a09d23fdcbf4d775a15")) },
                { 600000, new CheckpointInfo(new uint256("0x610a60579898e9160509ea4453cb946e1fdb9ebc18eedffd77513f42a61c0d77")) },
                { 700000, new CheckpointInfo(new uint256("0x6d5addc975a93eb323933bcdf2c3b7e098e324e8b205232a490cd585aceb1518")) },
                { 800000, new CheckpointInfo(new uint256("0x6ff2a00696e1601efba88b98ef63e691e8da7acffd5703614e971c932d93af80")) },
                { 900000, new CheckpointInfo(new uint256("0x84b550eafbfe777d28321eabed9a118a3175bcd607481bbfe24dc5fa2a9de0cf")) },
                { 1_000_000, new CheckpointInfo(new uint256("0xc3da5b782bdf6b9d0606147996479b0ea621322d9df1d239cbbd814175f4ed61")) },
                { 1_100_000, new CheckpointInfo(new uint256("0xbb2d946fa7101c14c6374b0e40993ef73401a360e74652e1677d8d6b3b4be01c")) },
                { 1_200_000, new CheckpointInfo(new uint256("0x8b7c48e0e814afbedb0d6e67dc71aaa395886db58e17fd622e571e1d140fbbb3")) },
                { 1_300_000, new CheckpointInfo(new uint256("0xe4aecd9ecdbf4e55b08255ed6d8a98e811fbd3e7c72ef267c26ebfae4e315990")) },
                { 1_400_000, new CheckpointInfo(new uint256("0x7165b03c170869b318253d470aa904f9c674c0d0f4ca2e9a64416b1d42beecc5")) },
                { 1_500_000, new CheckpointInfo(new uint256("0xb458117f195f936d7767f7299d0976ad90700e321870c18ec1e3481924f2afc3")) },
                { 1_600_000, new CheckpointInfo(new uint256("0x696cd64ec08b67ed3a3ec1e3add77c0e8203d8d6c0bb7df96dd9508dda4ba67e")) },
                { 1_700_000, new CheckpointInfo(new uint256("0xf42564107701d81e847e5dc6bd95da6bf32cb54e762d84118a7764349b414e68")) },
                { 1_800_000, new CheckpointInfo(new uint256("0x57a3119de52cf43b66d6e805a644c20fdee63557038cd68c429d47b21d111084")) },
                { 1_900_000, new CheckpointInfo(new uint256("0xd413f3aed50f4a1a4580e7c506223a605e222849da9649ca6d43ad7aac5c5af5")) },
                { 2_050_000, new CheckpointInfo(new uint256("0x543511cdefc38ee4fc272872543427cf08c6406ab602799b47138e418aa195fc")) },
            };

            this.DNSSeeds = new List<DNSSeedData>
            {
                new DNSSeedData("cirrustest1.stratisnetwork.com", "cirrustest1.stratisnetwork.com")
            };

            this.SeedNodes = new List<NetworkAddress>();
            this.StandardScriptsRegistry = new PoAStandardScriptsRegistry();
        }
    }
}
