// Decompiled with JetBrains decompiler
// Type: Nethereum.RLP.RLP
// Assembly: Nethereum.RLP, Version=3.8.0.0, Culture=neutral, PublicKeyToken=8768a594786aba4e
// MVID: 25BCB5E5-890F-431D-B8DB-9F8F5E3AEA2E
// Assembly location: C:\Users\noescape0\.nuget\packages\nethereum.rlp\3.8.0\lib\netcoreapp3.1\Nethereum.RLP.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.RLP
{
    public class RLP
    {
        private const int SIZE_THRESHOLD = 56;
        private const byte OFFSET_SHORT_ITEM = 128;
        private const byte OFFSET_LONG_ITEM = 183;
        private const byte OFFSET_SHORT_LIST = 192;
        private const byte OFFSET_LONG_LIST = 247;
        public static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];
        public static readonly byte[] ZERO_BYTE_ARRAY = new byte[1];

        public static int ByteArrayToInt(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static IRLPElement Decode(byte[] msgData)
        {
            RLPCollection rlpCollection = new RLPCollection();
            Nethereum.RLP.RLP.Decode(msgData, 0, 0, msgData.Length, 1, rlpCollection);
            return rlpCollection[0];
        }

        public static void Decode(
          byte[] msgData,
          int level,
          int startPosition,
          int endPosition,
          int levelToIndex,
          RLPCollection rlpCollection)
        {
            if (msgData == null || msgData.Length == 0)
                return;
            byte[] numArray = new byte[endPosition - startPosition];
            Array.Copy((Array)msgData, startPosition, (Array)numArray, 0, numArray.Length);
            try
            {
                int currentPosition = startPosition;
                while (currentPosition < endPosition)
                {
                    if (Nethereum.RLP.RLP.IsListBiggerThan55Bytes(msgData, currentPosition))
                        currentPosition = Nethereum.RLP.RLP.ProcessListBiggerThan55Bytes(msgData, level, levelToIndex, rlpCollection, currentPosition);
                    else if (Nethereum.RLP.RLP.IsListLessThan55Bytes(msgData, currentPosition))
                        currentPosition = Nethereum.RLP.RLP.ProcessListLessThan55Bytes(msgData, level, levelToIndex, rlpCollection, currentPosition);
                    else if (Nethereum.RLP.RLP.IsItemBiggerThan55Bytes(msgData, currentPosition))
                        currentPosition = Nethereum.RLP.RLP.ProcessItemBiggerThan55Bytes(msgData, rlpCollection, currentPosition);
                    else if (Nethereum.RLP.RLP.IsItemLessThan55Bytes(msgData, currentPosition))
                        currentPosition = Nethereum.RLP.RLP.ProcessItemLessThan55Bytes(msgData, rlpCollection, currentPosition);
                    else if (Nethereum.RLP.RLP.IsNullItem(msgData, currentPosition))
                        currentPosition = Nethereum.RLP.RLP.ProcessNullItem(rlpCollection, currentPosition);
                    else if (Nethereum.RLP.RLP.IsSigleByteItem(msgData, currentPosition))
                        currentPosition = Nethereum.RLP.RLP.ProcessSingleByteItem(msgData, rlpCollection, currentPosition);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid RLP ", ex);
            }
        }

        private static bool IsListBiggerThan55Bytes(byte[] msgData, int currentPosition) => msgData[currentPosition] > (byte)247;

        private static bool IsListLessThan55Bytes(byte[] msgData, int currentPosition) => msgData[currentPosition] >= (byte)192 && msgData[currentPosition] <= (byte)247;

        private static bool IsItemBiggerThan55Bytes(byte[] msgData, int currentPosition) => msgData[currentPosition] > (byte)183 && msgData[currentPosition] < (byte)192;

        private static bool IsItemLessThan55Bytes(byte[] msgData, int currentPosition) => msgData[currentPosition] > (byte)128 && msgData[currentPosition] <= (byte)183;

        private static bool IsNullItem(byte[] msgData, int currentPosition) => msgData[currentPosition] == (byte)128;

        private static bool IsSigleByteItem(byte[] msgData, int currentPosition) => msgData[currentPosition] < (byte)128;

        private static int ProcessSingleByteItem(
          byte[] msgData,
          RLPCollection rlpCollection,
          int currentPosition)
        {
            RLPItem rlpItem = new RLPItem(new byte[1]
            {
        msgData[currentPosition]
            });
            rlpCollection.Add((IRLPElement)rlpItem);
            ++currentPosition;
            return currentPosition;
        }

        private static int ProcessNullItem(RLPCollection rlpCollection, int currentPosition)
        {
            RLPItem rlpItem = new RLPItem(Nethereum.RLP.RLP.EMPTY_BYTE_ARRAY);
            rlpCollection.Add((IRLPElement)rlpItem);
            ++currentPosition;
            return currentPosition;
        }

        private static int ProcessItemLessThan55Bytes(
          byte[] msgData,
          RLPCollection rlpCollection,
          int currentPosition)
        {
            byte num = (byte)((uint)msgData[currentPosition] - 128U);
            byte[] rlpData = new byte[(int)num];
            Array.Copy((Array)msgData, currentPosition + 1, (Array)rlpData, 0, (int)num);
            byte[] numArray = new byte[2];
            Array.Copy((Array)msgData, currentPosition, (Array)numArray, 0, 2);
            RLPItem rlpItem = new RLPItem(rlpData);
            rlpCollection.Add((IRLPElement)rlpItem);
            currentPosition += 1 + (int)num;
            return currentPosition;
        }

        private static int ProcessItemBiggerThan55Bytes(
          byte[] msgData,
          RLPCollection rlpCollection,
          int currentPosition)
        {
            byte num = (byte)((uint)msgData[currentPosition] - 183U);
            int length = Nethereum.RLP.RLP.CalculateLength((int)num, msgData, currentPosition);
            byte[] rlpData = new byte[length];
            Array.Copy((Array)msgData, currentPosition + (int)num + 1, (Array)rlpData, 0, length);
            byte[] numArray = new byte[(int)num + 1];
            Array.Copy((Array)msgData, currentPosition, (Array)numArray, 0, (int)num + 1);
            RLPItem rlpItem = new RLPItem(rlpData);
            rlpCollection.Add((IRLPElement)rlpItem);
            currentPosition += (int)num + length + 1;
            return currentPosition;
        }

        private static int ProcessListLessThan55Bytes(
          byte[] msgData,
          int level,
          int levelToIndex,
          RLPCollection rlpCollection,
          int currentPosition)
        {
            int num = (int)msgData[currentPosition] - 192;
            int length = num + 1;
            byte[] numArray = new byte[num + 1];
            Array.Copy((Array)msgData, currentPosition, (Array)numArray, 0, length);
            RLPCollection rlpCollection1 = new RLPCollection()
            {
                RLPData = numArray
            };
            if (num > 0)
                Nethereum.RLP.RLP.Decode(msgData, level + 1, currentPosition + 1, currentPosition + length, levelToIndex, rlpCollection1);
            rlpCollection.Add((IRLPElement)rlpCollection1);
            currentPosition += length;
            return currentPosition;
        }

        private static int ProcessListBiggerThan55Bytes(
          byte[] msgData,
          int level,
          int levelToIndex,
          RLPCollection rlpCollection,
          int currentPosition)
        {
            byte num = (byte)((uint)msgData[currentPosition] - 247U);
            int length1 = Nethereum.RLP.RLP.CalculateLength((int)num, msgData, currentPosition);
            int length2 = (int)num + length1 + 1;
            byte[] numArray = new byte[length2];
            Array.Copy((Array)msgData, currentPosition, (Array)numArray, 0, length2);
            RLPCollection rlpCollection1 = new RLPCollection()
            {
                RLPData = numArray
            };
            Nethereum.RLP.RLP.Decode(msgData, level + 1, currentPosition + (int)num + 1, currentPosition + length2, levelToIndex, rlpCollection1);
            rlpCollection.Add((IRLPElement)rlpCollection1);
            currentPosition += length2;
            return currentPosition;
        }

        public static IRLPElement DecodeFirstElement(byte[] msgData, int startPos)
        {
            RLPCollection rlpCollection = new RLPCollection();
            Nethereum.RLP.RLP.Decode(msgData, 0, startPos, startPos + 1, 1, rlpCollection);
            return rlpCollection[0];
        }

        public static byte[] EncodeByte(byte singleByte) => singleByte == (byte)0 ? new byte[1]
        {
      (byte) 128
        } : (singleByte <= (byte)127 ? new byte[1]
        {
      singleByte
        } : new byte[2] { (byte)129, singleByte });

        public static byte[] EncodeElement(byte[] srcData)
        {
            if (Nethereum.RLP.RLP.IsNullOrZeroArray(srcData))
                return new byte[1] { (byte)128 };
            if (Nethereum.RLP.RLP.IsSingleZero(srcData) || srcData.Length == 1 && srcData[0] < (byte)128)
                return srcData;
            if (srcData.Length < 56)
            {
                byte num = (byte)(128 + srcData.Length);
                byte[] numArray = new byte[srcData.Length + 1];
                Array.Copy((Array)srcData, 0, (Array)numArray, 1, srcData.Length);
                numArray[0] = num;
                return numArray;
            }
            int length = srcData.Length;
            byte num1 = 0;
            for (; length != 0; length >>= 8)
                ++num1;
            byte[] numArray1 = new byte[(int)num1];
            for (int index = 0; index < (int)num1; ++index)
                numArray1[(int)num1 - 1 - index] = (byte)(srcData.Length >> 8 * index);
            byte[] numArray2 = new byte[srcData.Length + 1 + (int)num1];
            Array.Copy((Array)srcData, 0, (Array)numArray2, 1 + (int)num1, srcData.Length);
            numArray2[0] = (byte)(183U + (uint)num1);
            Array.Copy((Array)numArray1, 0, (Array)numArray2, 1, numArray1.Length);
            return numArray2;
        }

        public static byte[] EncodeElementsAndList(params byte[][] dataItems) => Nethereum.RLP.RLP.EncodeList(((IEnumerable<byte[]>)dataItems).Select<byte[], byte[]>(new Func<byte[], byte[]>(Nethereum.RLP.RLP.EncodeElement)).ToArray<byte[]>());

        public static byte[] EncodeList(params byte[][] items)
        {
            if (items == null)
                return new byte[1] { (byte)192 };
            int num1 = 0;
            for (int index = 0; index < items.Length; ++index)
                num1 += items[index].Length;
            byte[] numArray1;
            int destinationIndex;
            if (num1 < 56)
            {
                numArray1 = new byte[1 + num1];
                numArray1[0] = (byte)(192 + num1);
                destinationIndex = 1;
            }
            else
            {
                int num2 = num1;
                byte num3 = 0;
                for (; num2 != 0; num2 >>= 8)
                    ++num3;
                int num4 = num1;
                byte[] numArray2 = new byte[(int)num3];
                for (int index = 0; index < (int)num3; ++index)
                    numArray2[(int)num3 - 1 - index] = (byte)(num4 >> 8 * index);
                numArray1 = new byte[1 + numArray2.Length + num1];
                numArray1[0] = (byte)(247U + (uint)num3);
                Array.Copy((Array)numArray2, 0, (Array)numArray1, 1, numArray2.Length);
                destinationIndex = numArray2.Length + 1;
            }
            foreach (byte[] numArray2 in items)
            {
                Array.Copy((Array)numArray2, 0, (Array)numArray1, destinationIndex, numArray2.Length);
                destinationIndex += numArray2.Length;
            }
            return numArray1;
        }

        public static bool IsNullOrZeroArray(byte[] array) => array == null || array.Length == 0;

        public static bool IsSingleZero(byte[] array) => array.Length == 1 && array[0] == (byte)0;

        private static int CalculateLength(int lengthOfLength, byte[] msgData, int pos)
        {
            byte num1 = (byte)(lengthOfLength - 1);
            int num2 = 0;
            for (int index = 1; index <= lengthOfLength; ++index)
            {
                num2 += (int)msgData[pos + index] << 8 * (int)num1;
                --num1;
            }
            return num2;
        }
    }

    public class RLPItem : IRLPElement
    {
        private readonly byte[] rlpData;

        public RLPItem(byte[] rlpData) => this.rlpData = rlpData;

        public byte[] RLPData => this.GetRLPData();

        private byte[] GetRLPData() => this.rlpData.Length != 0 ? this.rlpData : (byte[])null;
    }
}
