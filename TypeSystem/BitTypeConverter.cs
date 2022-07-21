// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.BitTypeConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Globalization;
using System.Text;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class BitTypeConverter.</summary>
  /// <exclude />
  internal static class BitTypeConverter
  {
    internal static int MarshalSize(IDataType type, object value)
    {
      if (!((IBitSize) type).IsBitType)
        throw new ArgumentOutOfRangeException(nameof (type));
      int num = 0;
      num = !(value.GetType() != typeof (BitArray)) ? ((IBitSize) type).ByteSize : PrimitiveTypeMarshaler.Default.MarshalSize(value, StringMarshaler.DefaultEncoding);
      return ((IBitSize) type).ByteSize;
    }

    /// <summary>Converts a Bit type value to bytes.</summary>
    /// <param name="type">The type.</param>
    /// <param name="value">The value.</param>
    /// <param name="marshal">The marshal.</param>
    /// <returns>System.Byte[].</returns>
    /// <exception cref="T:System.ArgumentException">type</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">bitSize</exception>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    internal static int Marshal(IDataType type, object value, Span<byte> marshal)
    {
      if (!((IBitSize) type).IsBitType)
        throw new ArgumentException("Type is no bit type!", nameof (type));
      return BitTypeConverter.Marshal(((IBitSize) type).BitSize, value, marshal);
    }

    /// <summary>Converts a Bit type value to bytes.</summary>
    /// <param name="bitSize">Bit size of the underlying data type.</param>
    /// <param name="value">The value.</param>
    /// <param name="destination">The destination memory.</param>
    /// <returns>System.Byte[].</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">bitSize</exception>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">bitSize</exception>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    internal static int Marshal(int bitSize, object value, Span<byte> destination)
    {
      if (bitSize < 1)
        throw new ArgumentOutOfRangeException(nameof (bitSize));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      int num = 0;
      if (value.GetType() != typeof (BitArray))
      {
        num = PrimitiveTypeMarshaler.Default.MarshalSize(value, StringMarshaler.DefaultEncoding);
        return PrimitiveTypeMarshaler.Default.Marshal(value, StringMarshaler.DefaultEncoding, destination);
      }
      int length = bitSize / 8 + 1;
      BitArray bitArray = (BitArray) value;
      byte[] source = new byte[length];
      bitArray.CopyTo((Array) source, 0);
      source.CopyTo<byte>(destination);
      return length;
    }

    /// <summary>
    /// Converts a BitSet represented in Byte[] to DotNetObject.
    /// </summary>
    /// <param name="bitSize">Size of the Bitset in bits.</param>
    /// <param name="data">The data to convert.</param>
    /// <param name="bitOffset">The bit offset where the Bitset data is in data bytes.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value.</param>
    /// <returns>bool, ushort, uint, ulong, BitArray dependent on bitSize.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">bitSize</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">data</exception>
    /// <exception cref="T:System.ArgumentException">Data array to small! - data</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">bitSize
    /// or
    /// data</exception>
    /// <exception cref="T:System.ArgumentNullException">data</exception>
    /// <exception cref="T:System.ArgumentException">Data not large enough!;data</exception>
    internal static int Unmarshal(
      int bitSize,
      ReadOnlySpan<byte> data,
      int bitOffset,
      Encoding encoding,
      out object val)
    {
      if (bitSize < 1)
        throw new ArgumentOutOfRangeException(nameof (bitSize));
      if (data.Length == 0)
        throw new ArgumentOutOfRangeException(nameof (data));
      int num = 0;
      if (data.Length * 8 < bitSize + bitOffset)
        throw new ArgumentException("Data array to small!", nameof (data));
      BitArray bitArray1 = new BitArray(data.ToArray());
      BitArray bitArray2 = new BitArray(bitSize);
      for (int index = 0; index < bitSize; ++index)
        bitArray2[index] = bitArray1[index + bitOffset];
      if (bitSize == 1)
        val = (object) bitArray2[0];
      else if (bitSize % 8 == 0)
      {
        byte[] source = new byte[(bitArray2.Length - 1) / 8 + 1];
        bitArray2.CopyTo((Array) source, 0);
        PrimitiveTypeMarshaler primitiveTypeMarshaler = PrimitiveTypeMarshaler.Default;
        switch (bitSize)
        {
          case 8:
            num = primitiveTypeMarshaler.Unmarshal(typeof (byte), (ReadOnlySpan<byte>) source, encoding, out val);
            break;
          case 16:
            num = primitiveTypeMarshaler.Unmarshal(typeof (ushort), (ReadOnlySpan<byte>) source, encoding, out val);
            break;
          case 32:
            num = primitiveTypeMarshaler.Unmarshal(typeof (uint), (ReadOnlySpan<byte>) source, encoding, out val);
            break;
          case 64:
            num = primitiveTypeMarshaler.Unmarshal(typeof (ulong), (ReadOnlySpan<byte>) source, encoding, out val);
            break;
          default:
            val = (object) source;
            break;
        }
      }
      else
        val = (object) bitArray2;
      return num;
    }

    /// <summary>
    ///  Converts a numeric value to <see cref="T:System.Collections.BitArray" />
    /// </summary>
    /// <typeparam name="T">Type of Numeric value (msut support <see cref="T:System.IConvertible" /></typeparam>
    /// <param name="numeric">The numeric value.</param>
    /// <returns>BitArray.</returns>
    /// <exception cref="T:System.ArgumentException">Type '{0}' not supported!</exception>
    internal static BitArray ToBinary<T>(T numeric) where T : IConvertible
    {
      Type type = typeof (T);
      byte[] bytes;
      if ((object) numeric is int)
        bytes = BitConverter.GetBytes((int) (object) numeric);
      else if (type == typeof (long))
        bytes = BitConverter.GetBytes((long) (object) numeric);
      else if (type == typeof (uint))
        bytes = BitConverter.GetBytes((uint) (object) numeric);
      else if (type == typeof (ulong))
        bytes = BitConverter.GetBytes((ulong) (object) numeric);
      else if (type == typeof (short))
        bytes = BitConverter.GetBytes((short) (object) numeric);
      else if (type == typeof (ushort))
        bytes = BitConverter.GetBytes((ushort) (object) numeric);
      else if (type == typeof (byte))
      {
        bytes = BitConverter.GetBytes((short) (byte) (object) numeric);
      }
      else
      {
        if (!(type == typeof (sbyte)))
          throw new ArgumentException("Type '{0}' not supported!", type.Name);
        bytes = BitConverter.GetBytes((short) (sbyte) (object) numeric);
      }
      return new BitArray(bytes);
    }

    /// <summary>
    /// Converts the <see cref="T:System.Collections.BitArray" /> to a numeric value.
    /// </summary>
    /// <typeparam name="T">Type of Numeric value (must support <see cref="T:System.IConvertible" /></typeparam>
    /// <param name="binary">The binary.</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.ArgumentNullException">binary</exception>
    /// <exception cref="T:System.ArgumentException">binary</exception>
    internal static T ToNumeric<T>(BitArray binary) where T : IConvertible
    {
      if (binary == null)
        throw new ArgumentNullException(nameof (binary));
      Type type = typeof (T);
      int length = PrimitiveTypeMarshaler.Default.MarshalSize((object) type, StringMarshaler.DefaultEncoding);
      int num = binary.Length / 8 + (binary.Length % 8 > 0 ? 1 : 0);
      byte[] source = length >= num ? new byte[length] : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Target type must be at least '{0}' bytes long!", (object) num), nameof (binary));
      binary.CopyTo((Array) source, 0);
      object val;
      PrimitiveTypeMarshaler.Default.Unmarshal(type, (ReadOnlySpan<byte>) source, StringMarshaler.DefaultEncoding, out val);
      return (T) val;
    }

    /// <summary>
    /// Converts the specified <see cref="T:System.Collections.BitArray" /> to the specified numeric value type.
    /// </summary>
    /// <param name="tp">Type of the numeric value.</param>
    /// <param name="binary">Bit array.</param>
    /// <returns>Numeric value boxed as System.Object</returns>
    /// <exception cref="T:System.ArgumentException">Type '{0}' not supported!</exception>
    internal static object ToNumeric(Type tp, BitArray binary)
    {
      if (tp == typeof (int))
        return (object) BitTypeConverter.ToNumeric<int>(binary);
      if (tp == typeof (long))
        return (object) BitTypeConverter.ToNumeric<long>(binary);
      if (tp == typeof (uint))
        return (object) BitTypeConverter.ToNumeric<uint>(binary);
      if (tp == typeof (ulong))
        return (object) BitTypeConverter.ToNumeric<int>(binary);
      if (tp == typeof (short))
        return (object) BitTypeConverter.ToNumeric<short>(binary);
      if (tp == typeof (ushort))
        return (object) BitTypeConverter.ToNumeric<ushort>(binary);
      if (tp == typeof (byte))
        return (object) BitTypeConverter.ToNumeric<byte>(binary);
      if (tp == typeof (sbyte))
        return (object) BitTypeConverter.ToNumeric<sbyte>(binary);
      throw new ArgumentException("Type '{0}' not supported!", tp.Name);
    }
  }
}
