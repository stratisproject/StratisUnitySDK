// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.UInt256
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\noescape0\.nuget\packages\stratis.smartcontracts\2.0.0\lib\netcoreapp2.1\Stratis.SmartContracts.dll

using System;
using System.Numerics;

namespace Stratis.SmartContracts
{
    public struct UInt256 : IComparable
    {
        private const int WIDTH = 32;
        internal UIntBase value;

        public static UInt256 Zero => (UInt256)0;

        public static UInt256 MinValue => (UInt256)0;

        public static UInt256 MaxValue => new UInt256((BigInteger.One << 256) - (BigInteger)1);

        public UInt256(string hex) => this.value = new UIntBase(32, hex);

        public static UInt256 Parse(string str) => new UInt256(str);

        internal UInt256(BigInteger value) => this.value = new UIntBase(32, value);

        public UInt256(byte[] vch, bool lendian = true) => this.value = new UIntBase(32, vch, lendian);

        public static UInt256 operator >>(UInt256 a, int shift) => new UInt256(a.value.ShiftRight(shift));

        public static UInt256 operator <<(UInt256 a, int shift) => new UInt256(a.value.ShiftLeft(shift));

        public static UInt256 operator -(UInt256 a, UInt256 b) => new UInt256(a.value.Subtract(b.value.GetValue()));

        public static UInt256 operator +(UInt256 a, UInt256 b) => new UInt256(a.value.Add(b.value.GetValue()));

        public static UInt256 operator *(UInt256 a, UInt256 b) => new UInt256(a.value.Multiply(b.value.GetValue()));

        public static UInt256 operator /(UInt256 a, UInt256 b) => new UInt256(a.value.Divide(b.value.GetValue()));

        public static UInt256 operator %(UInt256 a, UInt256 b) => new UInt256(a.value.Mod(b.value.GetValue()));

        public UInt256(byte[] vch)
          : this(vch, true)
        {
        }

        public static bool operator <(UInt256 a, UInt256 b) => UIntBase.Comparison(a.value, b.value) < 0;

        public static bool operator >(UInt256 a, UInt256 b) => UIntBase.Comparison(a.value, b.value) > 0;

        public static bool operator <=(UInt256 a, UInt256 b) => UIntBase.Comparison(a.value, b.value) <= 0;

        public static bool operator >=(UInt256 a, UInt256 b) => UIntBase.Comparison(a.value, b.value) >= 0;

        public static bool operator ==(UInt256 a, UInt256 b) => UIntBase.Comparison(a.value, b.value) == 0;

        public static bool operator !=(UInt256 a, UInt256 b) => !(a == b);

        public static implicit operator UInt256(ulong value) => new UInt256((BigInteger)value);

        public static implicit operator UInt256(long value) => new UInt256((BigInteger)value);

        public static implicit operator UInt256(int value) => new UInt256((BigInteger)value);

        public static implicit operator UInt256(uint value) => new UInt256((BigInteger)value);

        public static implicit operator UInt256(UInt128 value)
        {
            byte[] vch = new byte[32];
            value.ToBytes().CopyTo((Array)vch, 0);
            return new UInt256(vch);
        }

        public static explicit operator int(UInt256 value) => (int)value.value.GetValue();

        public static explicit operator uint(UInt256 value) => (uint)value.value.GetValue();

        public static explicit operator long(UInt256 value) => (long)value.value.GetValue();

        public static explicit operator ulong(UInt256 value) => (ulong)value.value.GetValue();

        public static explicit operator UInt128(UInt256 value) => new UInt128(value.value.GetValue());

        public byte[] ToBytes() => this.value.ToBytes();

        public int CompareTo(object b) => this.value.CompareTo((object)((UInt256)b).value);

        public override int GetHashCode() => this.value.GetHashCode();

        public override bool Equals(object obj) => this.CompareTo(obj) == 0;

        public override string ToString() => this.value.ToString();
    }
}
