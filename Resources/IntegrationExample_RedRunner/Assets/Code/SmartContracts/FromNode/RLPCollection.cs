// Decompiled with JetBrains decompiler
// Type: Nethereum.RLP.RLPCollection
// Assembly: Nethereum.RLP, Version=3.8.0.0, Culture=neutral, PublicKeyToken=8768a594786aba4e
// MVID: 25BCB5E5-890F-431D-B8DB-9F8F5E3AEA2E
// Assembly location: C:\Users\noescape0\.nuget\packages\nethereum.rlp\3.8.0\lib\netcoreapp3.1\Nethereum.RLP.dll

using System.Collections.Generic;

namespace Nethereum.RLP
{
    public class RLPCollection : List<IRLPElement>, IRLPElement
    {
        public byte[] RLPData { get; set; }
    }

    public interface IRLPElement
    {
        byte[] RLPData { get; }
    }
}