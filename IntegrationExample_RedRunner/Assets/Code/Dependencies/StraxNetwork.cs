using NBitcoin;
using NBitcoin.DataEncoders;

namespace Stratis.Bitcoin.Networks
{
    public static class StraxNetwork
    {
        /// <summary> Stratis maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int StratisMaxTimeOffsetSeconds = 25 * 60;

        /// <summary> Stratis default value for the maximum tip age in seconds to consider the node in initial block download (2 hours). </summary>
        public const int StratisDefaultMaxTipAgeInSeconds = 2 * 60 * 60;

        /// <summary> The name of the root folder containing the different Stratis blockchains (StratisMain, StratisTest, StratisRegTest). </summary>
        public const string StraxRootFolderName = "strax";

        /// <summary> The default name used for the Strax configuration file. </summary>
        public const string StraxDefaultConfigFilename = "strax.conf";
        
        public static Block CreateGenesisBlock(ConsensusFactory consensusFactory, uint time, uint nonce, uint bits, int version, Money genesisReward, string genesisText)
        {
            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)42 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(genesisText)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
            });

            Block genesis = consensusFactory.CreateBlock();
            genesis.Header.BlockTime = Utils.UnixTimeToDateTime(time);
            genesis.Header.Bits = bits;
            genesis.Header.Nonce = nonce;
            genesis.Header.Version = version;
            genesis.Transactions.Add(txNew);
            genesis.Header.HashPrevBlock = uint256.Zero;
            genesis.UpdateMerkleRoot();

            /*
            Procedure for creating a new genesis block:
            1. Create the template block as above in the CreateStraxGenesisBlock method

            3. Iterate over the nonce until the proof-of-work is valid
            */

            //while (!genesis.CheckProofOfWork())
            //{
            //   genesis.Header.Nonce++;
            //   if (genesis.Header.Nonce == 0)
            //       genesis.Header.Time++;
            //}

            /*
            4. This will mean the block header hash is under the target
            5. Retrieve the Nonce and Time values from the resulting block header and insert them into the network definition
            */

            return genesis;

        }
    }
}
