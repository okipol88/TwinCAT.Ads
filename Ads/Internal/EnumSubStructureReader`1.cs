// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.EnumSubStructureReader`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Helper class Marshalling Enum ADS Substructures</summary>
  /// <typeparam name="T">Enum substructure to read.</typeparam>
  internal class EnumSubStructureReader<T> where T : IAdsEnumCustomMarshal<T>, new()
  {
    /// <summary>Unmarshals the specified element count.</summary>
    /// <param name="elementCount">The element count.</param>
    /// <param name="valueSize">Size of the value.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    /// <param name="subStructures">The sub structures.</param>
    /// <returns>System.Int32.</returns>
    internal static int Unmarshal(
      uint elementCount,
      uint valueSize,
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span,
      out T[] subStructures)
    {
      int start = 0;
      subStructures = new T[(int) elementCount];
      for (int index = 0; (long) index < (long) elementCount && start < span.Length; ++index)
      {
        subStructures[index] = new T();
        (bool valid, int readBytes) tuple = subStructures[index].Unmarshal(valueSize, marshaler, span.Slice(start));
        start += tuple.readBytes;
      }
      return start;
    }

    /// <summary>Marshals the specified enum values.</summary>
    /// <param name="enumValues">The enum values.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    internal static int Marshal(T[] enumValues, IStringMarshaler marshaler, Span<byte> buffer)
    {
      int start = 0;
      foreach (T enumValue in enumValues)
        start += enumValue.Marshal(marshaler, buffer.Slice(start));
      return start;
    }

    /// <summary>Gets the marshal size of the enum values.</summary>
    /// <param name="enumValues">The enum values.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <returns>System.Int32.</returns>
    internal static int MarshalSize(T[] enumValues, IStringMarshaler marshaler)
    {
      if (enumValues == null)
        throw new ArgumentNullException(nameof (enumValues));
      if (marshaler == null)
        throw new ArgumentNullException(nameof (marshaler));
      int num = 0;
      foreach (T enumValue in enumValues)
        num += enumValue.MarshalSize(marshaler);
      return num;
    }
  }
}
