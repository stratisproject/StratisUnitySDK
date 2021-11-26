// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.UInt128
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\noescape0\.nuget\packages\stratis.smartcontracts\2.0.0\lib\netcoreapp2.1\Stratis.SmartContracts.dll

using System;
using System.Numerics;

namespace Stratis.SmartContracts
{
    public struct UInt128 : IComparable
    {
        private const int WIDTH = 16;
        internal UIntBase value;

        public static UInt128 Zero => (UInt128)0;

        public static UInt128 MinValue => (UInt128)0;

        public static UInt128 MaxValue => new UInt128((BigInteger.One << 128) - (BigInteger)1);

        public UInt128(string hex) => this.value = new UIntBase(16, hex);

        public static UInt128 Parse(string str) => new UInt128(str);

        internal UInt128(BigInteger value) => this.value = new UIntBase(16, value);

        public UInt128(byte[] vch, bool lendian = true) => this.value = new UIntBase(16, vch, lendian);

        public static UInt128 operator >>(UInt128 a, int shift) => new UInt128(a.value.ShiftRight(shift));

        public static UInt128 operator <<(UInt128 a, int shift) => new UInt128(a.value.ShiftLeft(shift));

        public static UInt128 operator -(UInt128 a, UInt128 b) => new UInt128(a.value.Subtract(b.value.GetValue()));

        public static UInt128 operator +(UInt128 a, UInt128 b) => new UInt128(a.value.Add(b.value.GetValue()));

        public static UInt128 operator *(UInt128 a, UInt128 b) => new UInt128(a.value.Multiply(b.value.GetValue()));

        public static UInt128 operator /(UInt128 a, UInt128 b) => new UInt128(a.value.Divide(b.value.GetValue()));

        public static UInt128 operator %(UInt128 a, UInt128 b) => new UInt128(a.value.Mod(b.value.GetValue()));

        public UInt128(byte[] vch)
          : this(vch, true)
        {
        }

        public static bool operator <(UInt128 a, UInt128 b) => UIntBase.Comparison(a.value, b.value) < 0;

        public static bool operator >(UInt128 a, UInt128 b) => UIntBase.Comparison(a.value, b.value) > 0;

        public static bool operator <=(UInt128 a, UInt128 b) => UIntBase.Comparison(a.value, b.value) <= 0;

        public static bool operator >=(UInt128 a, UInt128 b) => UIntBase.Comparison(a.value, b.value) >= 0;

        public static bool operator ==(UInt128 a, UInt128 b) => UIntBase.Comparison(a.value, b.value) == 0;

        public static bool operator !=(UInt128 a, UInt128 b) => !(a == b);

        public static implicit operator UInt128(ulong value) => new UInt128((BigInteger)value);

        public static implicit operator UInt128(long value) => new UInt128((BigInteger)value);

        public static implicit operator UInt128(int value) => new UInt128((BigInteger)value);

        public static implicit operator UInt128(uint value) => new UInt128((BigInteger)value);

        public static explicit operator int(UInt128 value) => (int)value.value.GetValue();

        public static explicit operator uint(UInt128 value) => (uint)value.value.GetValue();

        public static explicit operator long(UInt128 value) => (long)value.value.GetValue();

        public static explicit operator ulong(UInt128 value) => (ulong)value.value.GetValue();

        public byte[] ToBytes() => this.value.ToBytes();

        public int CompareTo(object b) => this.value.CompareTo((object)((UInt128)b).value);

        public override int GetHashCode() => this.value.GetHashCode();

        public override bool Equals(object obj) => this.CompareTo(obj) == 0;

        public override string ToString() => this.value.ToString();
    }
}
