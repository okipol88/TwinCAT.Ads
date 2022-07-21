// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.IAdsEnumCustomMarshal`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Interface IAdsEnumCustomMarshal</summary>
  /// <typeparam name="T"></typeparam>
  internal interface IAdsEnumCustomMarshal<T>
  {
    /// <summary>
    /// Unmarshals data for the type <typeparamref name="T" />.
    /// </summary>
    /// <param name="enumValueSize">The size of the Enum data type</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The read buffer..</param>
    /// <returns>A tuple that contains the data validity plus the read bytes.</returns>
    (bool valid, int readBytes) Unmarshal(
      uint enumValueSize,
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span);

    /// <summary>
    /// Marshals the content of <typeparamref name="T" />
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>The amount of marshalled bytes.</returns>
    int Marshal(IStringMarshaler marshaler, Span<byte> buffer);

    /// <summary>
    /// Gets the marshal size of  <typeparamref name="T" />.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>The marshal size of the type <paramref name="marshaler" />.</returns>
    int MarshalSize(IStringMarshaler marshaler);
  }
}
