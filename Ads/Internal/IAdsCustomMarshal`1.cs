// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.IAdsCustomMarshal`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Interface IAdsCustomMarshal</summary>
  /// <typeparam name="T"></typeparam>
  internal interface IAdsCustomMarshal<T>
  {
    /// <summary>
    /// Unmarshals data into the <typeparamref name="T" /> type object.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The read buffer.</param>
    /// <returns>A tuple that contains the data validity plus the read bytes.</returns>
    /// <remarks>This method should try to resync the readBytes result to the next valid readable object. Usually
    /// read structures contain their size as first element. If the Read is not valid, than the Unmarshalled object should be ignored.
    /// </remarks>
    (bool valid, int readBytes) Unmarshal(IStringMarshaler marshaler, ReadOnlySpan<byte> span);

    /// <summary>
    /// Marshals the content of <typeparamref name="T" />
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    int Marshal(IStringMarshaler marshaler, Span<byte> buffer);

    /// <summary>
    /// Gets the marshal size of  <typeparamref name="T" />.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>System.Int32.</returns>
    int MarshalSize(IStringMarshaler marshaler);
  }
}
