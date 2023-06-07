using System.Collections.Generic;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace Stratis.Sidechains.Networks
{
    public class CirrusMain : PoANetwork
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

            // The height at which the following list of members apply.
            this.MultisigMinersApplicabilityHeight = 1413998;

            // Set the list of Strax Era mining keys.
            this.StraxMiningMultisigMembers = new List<PubKey>()
            {
                new PubKey("02ace4fbe6a622cdfc922a447c3253e8635f3fecb69241f73629e6f0596a567907"),
                new PubKey("028e1d9fd64b84a2ec85fac7185deb2c87cc0dd97270cf2d8adc3aa766dde975a7"),
                new PubKey("025cb67811d0922ca77fa33f19c3e5c37961f9639a1f0a116011b9075f6796abcb"),
                new PubKey("027e793fbf4f6d07de15b0aa8355f88759b8bdf92a9ffb8a65a87fa8ee03baeccd"),
                new PubKey("03eb5db0b1703ea7418f0ad20582bf8de0b4105887d232c7724f43f19f14862488"),
                new PubKey("03e8809be396745434ee8c875089e518a3eef40e31ade81869ce9cbef63484996d"),
                new PubKey("0317abe6a28cc7af44a46de97e7c6120c1ccec78afb83efe18030f5c36e3016b32"),
                new PubKey("038e1a76f0e33474144b61e0796404821a5150c00b05aad8a1cd502c865d8b5b92"),
                new PubKey("036437789fac0ab74cda93d98b519c28608a48ef86c3bd5e8227af606c1e025f61"),
                new PubKey("03d8b5580b7ec709c006ef497327db27ea323bd358ca45412171c644214483b74f"),
                new PubKey("02f40bd4f662ba20629a104115f0ac9ee5eab695716edfe01b240abf56e05797e2"),
                new PubKey("0323033679aa439a0388f09f2883bf1ca6f50283b41bfeb6be6ddcc4e420144c16"),
                new PubKey("03535a285d0919a9bd71df3b274cecb46e16b78bf50d3bf8b0a3b41028cf8a842d"),
                new PubKey("03a37019d2e010b046ef9d0459e4844a015758007602ddfbdc9702534924a23695"),
                new PubKey("03f5de5176e29e1e7d518ae76c1e020b1da18b57a3713ac81b16015026e232748e"),
            };
            
            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new NoBIP9Deployments();
            
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

            this.Checkpoints = new Dictionary<int, CheckpointInfo>()
            {
                { 50000, new CheckpointInfo(new uint256("0xf3ed37db1c56751fdf9f45902696dd034444a697cd8c106a08f4c60cd2de9d77")) },
                { 100000, new CheckpointInfo(new uint256("0x1400cb20800d54cd7fff5fea90133a1a8ca44e7043268cd0c7efdd7f8186b2d0")) },
                { 150000, new CheckpointInfo(new uint256("0x505d22805f0fc4ea057edad778e7334412526a7c1b017b179be5d274c8d42914")) },
                { 200000, new CheckpointInfo(new uint256("0x5569221c600e42b0467c92bd932046c12198eee5c50ac98eadff7d3159f55b75")) },
                { 250000, new CheckpointInfo(new uint256("0x1a0d5f43335eff00e8a3b5dc09e4f6849b571b6870eb58364cf86623222922d7")) },
                { 300000, new CheckpointInfo(new uint256("0x3b1c3704e0cb79e7fff46ab7e9feacbfa9e2e95ab90b273d99520dbd42cc34b6")) },
                { 350000, new CheckpointInfo(new uint256("0xcb420b8ef20e1da9eb63b6847005b17928b4bad6c2920eebc964ecf21c50ce5a")) },
                { 400000, new CheckpointInfo(new uint256("0xa501a5c69dfce78e39bf0c25d2c1eafa9fd7a9f32ee06b419d3a3c0a6ac29d8b")) },
                { 450000, new CheckpointInfo(new uint256("0xc3ae6119d23294ac51c05f9c761da5271711b1945592cb83cc1bcc1b908780c7")) },
                { 500000, new CheckpointInfo(new uint256("0x810cc011d6d5158aaefcc38550a31b4118fae1bb18ea7894f81a2edc81126d5f")) },
                { 550000, new CheckpointInfo(new uint256("0x3a6b0a58deb1997879d35fc6e017123594c00eafb3ac45d8c31a5dbf68c2bccc")) },
                { 600000, new CheckpointInfo(new uint256("0xc79bf7066ec9243a335fcd2a43380a47a5b9dccdeaee3f67ab5503cef0cd1626")) },
                { 700000, new CheckpointInfo(new uint256("0xe777ae5e283564a994cbcf88315a594854c12d626e6908fb27e3d0cd7d04fcc7")) },
                { 800000, new CheckpointInfo(new uint256("0xe8b2b9b4e342b0ff9a0b1b967b0f2b7481fe420c5922322d1b77cfae66471fa1")) },
                { 900000, new CheckpointInfo(new uint256("0x30599fbbce4404ebaff9f8d0ea7071c684f124439f1f4e9fabec0debad6c7a06")) },
                { 1_000_000, new CheckpointInfo(new uint256("0x547faf99acb45e2195ea5fbb6873562c44a7696f6571e8a309d6c9f509be064a")) },
                { 1_100_000, new CheckpointInfo(new uint256("0x7abc2882bcb5e9723ba71ff4155ed3c4006ee655e9f52f8787bcae31b4c796a8")) },
                { 1_200_000, new CheckpointInfo(new uint256("0x8411b830270cc9d6c2e28de1c2e8025c57a5673835f63e30708967adfee5a92c")) },
                { 1_300_000, new CheckpointInfo(new uint256("0x512c19a8245316b4d3b13513c7901f41842846f539f668ca4ac349daaab6dc20")) },
                { 1_400_000, new CheckpointInfo(new uint256("0xbfd4a96a6c5250f18bf7c586761256fa5f8753ffa10b24160f0648a452823a95")) },
                { 1_500_000, new CheckpointInfo(new uint256("0x2a1602877a5231997654bae975223762ee636be2f371cb444b2d3fb564e6989e")) },
                { 1_750_000, new CheckpointInfo(new uint256("0x58c96a878efeeffea1b1924b61eed627687900e01588ffaa2f4a161973f01abf")) },
                { 1_850_000, new CheckpointInfo(new uint256("0x6e2590bd9a8eaab25b236c0c9ac314abec70b18aa053b96c9257f2356dec8314")) },
                { 2_150_000, new CheckpointInfo(new uint256("0x4c65f29b5098479cab275afd77d302ebe5ed8d8ef33e02ae54bf185865763f18")) },
                { 2_500_000, new CheckpointInfo(new uint256("0x2853be7b7224840d3d4b60427ea832e9bd67d8fc6bfcd4956b8c6b2414cf8fc2")) },
            };
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
