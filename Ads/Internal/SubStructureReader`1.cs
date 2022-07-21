// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.SubStructureReader`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Helper class Marshalling ADS Substructures.</summary>
  /// <typeparam name="T">Substructure to read elements of</typeparam>
  internal class SubStructureReader<T> where T : IAdsCustomMarshal<T>, new()
  {
    /// <summary>
    /// Unmarshals the specified amount of substructures from the reader.
    /// </summary>
    /// <param name="elementCount">The element count.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    /// <param name="subStructures">The sub structures.</param>
    /// <returns>T[].</returns>
    internal static int Unmarshal(
      uint elementCount,
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span,
      out T[] subStructures)
    {
      int start = 0;
      subStructures = new T[(int) elementCount];
      for (int length = 0; (long) length < (long) elementCount; ++length)
      {
        if (start < span.Length)
        {
          subStructures[length] = new T();
          (bool valid, int readBytes) tuple = subStructures[length].Unmarshal(marshaler, span.Slice(start));
          start += tuple.readBytes;
        }
        else
        {
          T[] destinationArray = new T[length];
          Array.Copy((Array) subStructures, 0, (Array) destinationArray, 0, length);
          subStructures = destinationArray;
          start = span.Length;
          break;
        }
      }
      return start;
    }

    /// <summary>Marshals the specified sub structures.</summary>
    /// <param name="subStructures">The sub structures.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    internal static int Marshal(T[] subStructures, IStringMarshaler marshaler, Span<byte> buffer)
    {
      int start = 0;
      foreach (T subStructure in subStructures)
        start += subStructure.Marshal(marshaler, buffer.Slice(start));
      return start;
    }

    /// <summary>Gets the marshal size of the substructures.</summary>
    /// <param name="subStructures">The sub structures.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <returns>System.Int32.</returns>
    internal static int MarshalSize(T[] subStructures, IStringMarshaler marshaler)
    {
      if (subStructures == null)
        throw new ArgumentNullException(nameof (subStructures));
      if (marshaler == null)
        throw new ArgumentNullException(nameof (marshaler));
      int num = 0;
      foreach (T subStructure in subStructures)
        num += subStructure.MarshalSize(marshaler);
      return num;
    }
  }
}
