// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.BinaryWriterExtension
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.IO;
using System.Text;
using TwinCAT.PlcOpen;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class BinaryWriterExtension.</summary>
  /// <exclude />
  internal static class BinaryWriterExtension
  {
    /// <summary>Writes the PLC string.</summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length.</param>
    /// <param name="encoding">The encoding.</param>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    internal static void WritePlcString(
      this BinaryWriter writer,
      string value,
      int length,
      Encoding encoding)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (encoding == StringMarshaler.DefaultEncoding)
        writer.WritePlcAnsiString(value, length);
      else if (encoding == StringMarshaler.UnicodeEncoding)
      {
        writer.WritePlcUnicodeString(value, length);
      }
      else
      {
        if (encoding != Encoding.Unicode && encoding != Encoding.ASCII)
        {
          Encoding encoding1 = Encoding.Default;
        }
        if (value.Length >= length)
        {
          string s = value.Substring(0, length);
          StringMarshaler.UnicodeEncoding.GetByteCount("a");
          byte[] numArray = new byte[encoding.GetByteCount(s)];
          encoding.GetBytes(value, 0, length, numArray, 0);
          writer.Write(numArray);
        }
        else
        {
          if (value.Length >= length)
            return;
          byte[] numArray = new byte[encoding.GetByteCount(value)];
          encoding.GetBytes(value, 0, value.Length, numArray, 0);
          writer.Write(numArray);
        }
      }
    }

    /// <summary>
    /// Writes a string as a PLC string to the current stream.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The string to write to the stream.</param>
    /// <param name="length">The length of the string without '\0' terminator!</param>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcAnsiStringFixedLength(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcUnicodeString(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcUnicodeStringFixedLength(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <remarks>This method is meant for writing single string variables defined in the PlcControl format.
    /// E.g. to write a 'STRING(80)' (byte size is 81) a length of '80' must be given to the 'length' parameter.
    /// If the string length is larger or equal than the length parameter, then only length characters are written to the
    /// <see cref="T:TwinCAT.Ads.AdsStream" /> (without terminating character).
    /// If the string value character count is shorter than the specified length parameter, the string + a terminating
    /// \0 will be added to the <see cref="T:TwinCAT.Ads.AdsStream" />.
    /// This method cannot be used for marshalling purposes, for example several fields of a struct, because no filling
    /// bytes will be written to the stream. In that case use the <see cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcAnsiStringFixedLength(System.IO.BinaryWriter,System.String,System.Int32)" /> method.</remarks>
    public static void WritePlcAnsiString(this BinaryWriter writer, string value, int length)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      Encoding defaultEncoding = StringMarshaler.DefaultEncoding;
      if (value.Length >= length)
      {
        defaultEncoding.GetByteCount("a");
        byte[] numArray = new byte[length * defaultEncoding.GetByteCount("a")];
        defaultEncoding.GetBytes(value, 0, length, numArray, 0);
        writer.Write(numArray);
      }
      else
      {
        if (value.Length >= length)
          return;
        byte[] numArray = new byte[length * defaultEncoding.GetByteCount("a")];
        defaultEncoding.GetBytes(value, 0, value.Length, numArray, 0);
        writer.Write(numArray);
      }
    }

    /// <summary>
    /// Writes a (unicode) string as a PLC string to the current stream.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The string to write to the stream.</param>
    /// <param name="length">The length of the string without '\0' terminator!</param>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcAnsiString(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcAnsiStringFixedLength(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcUnicodeStringFixedLength(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <remarks>This method is meant for writing single string variables defined in the PlcControl format.
    /// E.g. to write a 'WSTRING(80)' (byte size is 162) a length of '80' must be given to the 'length' parameter.
    /// If the string length is larger or equal than the length parameter, then only length characters are written to the
    /// <see cref="T:TwinCAT.Ads.AdsStream" /> (without terminating character).
    /// If the string value character count is shorter than the specified length parameter, the string + a terminating
    /// \0 will be added to the <see cref="T:TwinCAT.Ads.AdsStream" />.
    /// This method cannot be used for marshalling purposes, for example several fields of a struct, because no filling
    /// bytes will be written to the stream. In that case use the <see cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcUnicodeStringFixedLength(System.IO.BinaryWriter,System.String,System.Int32)" /> method.</remarks>
    public static void WritePlcUnicodeString(this BinaryWriter writer, string value, int length)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      Encoding unicode = Encoding.Unicode;
      if (value.Length >= length)
      {
        unicode.GetByteCount("a");
        byte[] numArray = new byte[length * Encoding.Unicode.GetByteCount("a")];
        Encoding.Unicode.GetBytes(value, 0, length, numArray, 0);
        writer.Write(numArray);
      }
      else
      {
        byte[] numArray = new byte[(value.Length + 1) * Encoding.Unicode.GetByteCount("a")];
        Encoding.Unicode.GetBytes(value, 0, value.Length, numArray, 0);
        writer.Write(numArray);
      }
    }

    /// <summary>
    /// Writes the PLC ANSI string in a data block of the specified size.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="byteSize">Size of the String including the '\0' terminator.</param>
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcAnsiString(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcUnicodeString(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcUnicodeStringFixedLength(System.IO.BinaryWriter,System.String,System.Int32)" />
    internal static void WritePlcAnsiStringFixedLength(
      this BinaryWriter writer,
      string value,
      int byteSize)
    {
      StringMarshaler stringMarshaler = StringMarshaler.Default;
      byte[] numArray = new byte[stringMarshaler.MarshalSize(value)];
      stringMarshaler.Marshal(value, byteSize, numArray.AsSpan<byte>());
      writer.Write(numArray);
    }

    /// <summary>
    /// Writes the PLC UNICODE string into a data block of the specified size.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="byteSize">Size of the String including the '\0' terminator.</param>
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcAnsiStringFixedLength(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcUnicodeString(System.IO.BinaryWriter,System.String,System.Int32)" />
    /// <seealso cref="M:TwinCAT.Ads.Internal.BinaryWriterExtension.WritePlcAnsiString(System.IO.BinaryWriter,System.String,System.Int32)" />
    internal static void WritePlcUnicodeStringFixedLength(
      this BinaryWriter writer,
      string value,
      int byteSize)
    {
      StringMarshaler stringMarshaler = new StringMarshaler(Encoding.Unicode, (StringConvertMode) 1);
      byte[] numArray = new byte[stringMarshaler.MarshalSize(value)];
      stringMarshaler.Marshal(value, byteSize, numArray.AsSpan<byte>());
      writer.Write(numArray);
    }

    /// <summary>Writes a date as PLC date type to the current stream.</summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The date to write to the stream.</param>
    internal static void WritePlcType(this BinaryWriter writer, DateTime value) => writer.Write(((DateBase) new DATE(value)).Ticks);

    /// <summary>Writes a date as PLC date type to the current stream.</summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The date to write to the stream.</param>
    internal static void WritePlcType(this BinaryWriter writer, DateTimeOffset value) => writer.Write(((DateBase) new DATE(value)).Ticks);

    /// <summary>
    /// Writes a time span as PLC time type to the current stream.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The time span to write to the stream.</param>
    internal static void WritePlcType(this BinaryWriter writer, TimeSpan value) => writer.Write(((TimeBase) new TIME(value)).Ticks);

    /// <summary>Writes the Guid (16 Bytes) to the current stream.</summary>
    /// <param name="writer">The writer.</param>
    /// <param name="guid">The unique identifier.</param>
    internal static void WriteGuid(this BinaryWriter writer, Guid guid) => writer.Write(guid.ToByteArray());
  }
}
