// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsDataTypeArrayInfo
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.ComponentModel;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Array definition for a single dimension.</summary>
  public class AdsDataTypeArrayInfo : IAdsCustomMarshal<AdsDataTypeArrayInfo>, IAdsDataTypeArrayInfo
  {
    private int lBound;
    private int elements;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsDataTypeArrayInfo" /> class.
    /// </summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AdsDataTypeArrayInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsDataTypeArrayInfo" /> class.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    internal AdsDataTypeArrayInfo(StringMarshaler marshaler, ReadOnlySpan<byte> span) => this.Unmarshal((IStringMarshaler) marshaler, span);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsDataTypeArrayInfo" /> class.
    /// </summary>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="elements">The elements.</param>
    internal AdsDataTypeArrayInfo(int lowerBound, int elements)
    {
      this.lBound = lowerBound;
      this.elements = elements;
    }

    /// <summary>
    /// Unmarshals data from the span into this <see cref="T:TwinCAT.Ads.AdsDataTypeArrayInfo" />,
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The read buffer.</param>
    /// <returns>A tuple that contains the data validity plus the read bytes.</returns>
    /// <remarks>This method should try to resync the readBytes result to the next valid readable object. Usually
    /// read structures contain their size as first element. If the Read is not valid, than the Unmarshalled object should be ignored.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public (bool valid, int readBytes) Unmarshal(
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span)
    {
      this.lBound = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(0, 4));
      this.elements = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(4, 4));
      return (true, 8);
    }

    /// <summary>
    /// Marshals the content of the <see cref="T:TwinCAT.Ads.AdsDataTypeArrayInfo" />.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    public int Marshal(IStringMarshaler marshaler, Span<byte> buffer)
    {
      int start1 = 0;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start1), (uint) this.lBound);
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start2), (uint) this.elements);
      return start2 + 4;
    }

    /// <summary>
    /// Gets the marshal size of  the <see cref="T:TwinCAT.Ads.AdsDataTypeArrayInfo" />.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>System.Int32.</returns>
    public int MarshalSize(IStringMarshaler marshaler) => 8;

    /// <summary>Gets the lower bound.</summary>
    public int LowerBound => this.lBound;

    /// <summary>Gets the number of elements.</summary>
    public int Elements => this.elements;
  }
}
