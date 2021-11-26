// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.UIntBase
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\noescape0\.nuget\packages\stratis.smartcontracts\2.0.0\lib\netcoreapp2.1\Stratis.SmartContracts.dll

using System;
using System.Globalization;
using System.Numerics;

namespace Stratis.SmartContracts
{
    internal struct UIntBase : IComparable
    {
        private int width;
        private BigInteger value;

        public UIntBase(int width) => this.width = (width & 3) == 0 ? width : throw new ArgumentException("The 'width' must be a multiple of 4.");

        public UIntBase(int width, BigInteger value)
          : this(width)
          => this.SetValue(value);

        public UIntBase(int width, UIntBase value)
          : this(width)
          => this.SetValue(value.value);

        public UIntBase(int width, ulong b)
          : this(width)
          => this.SetValue(new BigInteger(b));

        public UIntBase(int width, byte[] vch, bool lendian = true)
          : this(width)
        {
            if (vch.Length > this.width)
                throw new FormatException(string.Format("The byte array should be {0} bytes or less.", (object)this.width));
            this.SetValue(new BigInteger(vch));
        }

        public UIntBase(int width, string str)
          : this(width)
        {
            if (str.StartsWith("0x"))
                this.SetValue(BigInteger.Parse("0" + str.Substring(2), NumberStyles.HexNumber));
            else
                this.SetValue(BigInteger.Parse(str));
        }

        public UIntBase(int width, uint[] array)
          : this(width)
        {
            int num = this.width / 4;
            if (array.Length != num)
                throw new FormatException(string.Format("The array length should be {0}.", (object)num));
            byte[] numArray = new byte[this.width];
            for (int index = 0; index < num; ++index)
                BitConverter.GetBytes(array[index]).CopyTo((Array)numArray, index * 4);
            this.SetValue(new BigInteger(numArray));
        }

        private bool TooBig(byte[] bytes) => bytes.Length > this.width && (bytes.Length != this.width + 1 || bytes[this.width] != (byte)0);

        private void SetValue(BigInteger value)
        {
            if (value.Sign < 0)
                throw new OverflowException("Only positive or zero values are allowed.");
            this.value = !this.TooBig(value.ToByteArray()) ? value : throw new OverflowException();
        }

        public BigInteger GetValue() => this.value;

        private uint[] ToUIntArray()
        {
            byte[] bytes = this.ToBytes();
            int length = this.width / 4;
            uint[] numArray = new uint[length];
            for (int index = 0; index < length; ++index)
                numArray[index] = BitConverter.ToUInt32(bytes, index * 4);
            return numArray;
        }

        public byte[] ToBytes(bool lendian = true)
        {
            byte[] byteArray = this.value.ToByteArray();
            byte[] array = new byte[this.width];
            Array.Copy((Array)byteArray, (Array)array, Math.Min(byteArray.Length, array.Length));
            
            if (!lendian)
                Array.Reverse(array);

            return array;
        }

        internal BigInteger ShiftRight(int shift) => this.value >> shift;

        internal BigInteger ShiftLeft(int shift) => this.value << shift;

        internal BigInteger Add(BigInteger value2) => this.value + value2;

        internal BigInteger Subtract(BigInteger value2)
        {
            if (this.value.CompareTo(value2) < 0)
                throw new OverflowException("Result cannot be negative.");
            return this.value - value2;
        }

        internal BigInteger Multiply(BigInteger value2) => this.value * value2;

        internal BigInteger Divide(BigInteger value2) => this.value / value2;

        internal BigInteger Mod(BigInteger value2) => this.value % value2;

        public int CompareTo(object b) => this.value.CompareTo(((UIntBase)b).value);

        public static int Comparison(UIntBase a, UIntBase b) => a.CompareTo((object)b);

        public override int GetHashCode()
        {
            uint[] uintArray = this.ToUIntArray();
            uint num = 0;
            for (int index = 0; index < uintArray.Length; ++index)
                num ^= uintArray[index];
            return (int)num;
        }

        public override bool Equals(object obj) => this.CompareTo(obj) == 0;

        private static string ByteArrayToString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "");

        public string ToHex() => UIntBase.ByteArrayToString(this.ToBytes(false)).ToLower();

        public override string ToString() => this.value.ToString();
    }
}
