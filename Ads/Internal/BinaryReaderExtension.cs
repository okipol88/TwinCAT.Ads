// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.BinaryReaderExtension
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
  internal static class BinaryReaderExtension
  {
    /// <summary>Reads the PLC string.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="byteLength">Length of the byte.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>System.String.</returns>
    internal static string ReadPlcString(
      this BinaryReader reader,
      int byteLength,
      Encoding encoding)
    {
      string str;
      new StringMarshaler(encoding, (StringConvertMode) 1).Unmarshal((ReadOnlySpan<byte>) reader.ReadBytes(byteLength).AsSpan<byte>(), ref str);
      return str;
    }

    /// <summary>
    /// Reads a <see cref="T:System.Guid" /> from the current stream.
    /// </summary>
    /// <returns>Guid.</returns>
    internal static Guid ReadGuid(this BinaryReader reader) => new Guid(reader.ReadBytes(16));

    /// <summary>
    /// Reads a PLC string from the current stream (ANSI Encoding)
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="byteLength">The length of the string in the PLC (byte length equals character count on PLC + '\0')</param>
    /// <returns>The string being read (until the first '\0' character)</returns>
    /// <remarks>The byte length of a String(80) in the PLC is 81.
    /// The byte length of a WSTRING(80) in the PLC is 162.
    /// Because of ANSI Encoding the number of Chars could differ with the number of Bytes (e.g on Double Byte Codepages DBCS, Codpage 932, Japan)</remarks>
    internal static string ReadPlcAnsiString(this BinaryReader reader, int byteLength)
    {
      string str;
      StringMarshaler.Default.Unmarshal((ReadOnlySpan<byte>) reader.ReadBytes(byteLength), ref str);
      return str;
    }

    /// <summary>
    /// Reads a PLC string from the current stream (Unicode Encoding)
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="byteLength">The length of the string in the PLC (byte length equals character count on PLC + '\0')</param>
    /// <returns>The string being read (until the first '\0' character)</returns>
    /// <remarks>The byte length of a String(80) in the PLC is 81.
    /// The byte length of a WSTRING(80) in the PLC is 162.
    /// Because of ANSI Encoding the number of Chars could differ with the number of Bytes (e.g on Double Byte Codepages DBCS, Codpage 932, Japan)</remarks>
    internal static string ReadPlcUnicodeString(this BinaryReader reader, int byteLength)
    {
      string str;
      StringMarshaler.Unicode.Unmarshal((ReadOnlySpan<byte>) reader.ReadBytes(byteLength), ref str);
      return str;
    }

    /// <summary>Reads a PLC Date type from the current stream.</summary>
    /// <returns>The date being read.</returns>
    internal static DateTimeOffset ReadPlcDATE(this BinaryReader reader) => (DateTimeOffset) new DATE(reader.ReadUInt32()).Date;

    /// <summary>Reads a PLC Time type from the current stream.</summary>
    /// <returns>The time being read.</returns>
    internal static TimeSpan ReadPlcTIME(this BinaryReader reader) => ((TimeBase) new TIME(reader.ReadUInt32())).Time;
  }
}
