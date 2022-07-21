// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsMethodParaInfo
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class AdsMethodParaInfo.</summary>
  internal class AdsMethodParaInfo : IAdsCustomMarshal<AdsMethodParaInfo>
  {
    /// <summary>length of complete datatype entry</summary>
    internal uint entryLength;
    /// <summary>size of datatype ( in bytes )</summary>
    internal uint size;
    /// <summary>
    /// size of biggest element for alignment (biggest element used for marshalling ???)
    /// </summary>
    internal uint alignSize;
    /// <summary>adsDataType of symbol (if alias)</summary>
    internal AdsDataTypeId dataType;
    /// <summary>The flags</summary>
    internal MethodParamFlags flags;
    /// <summary>The reserved</summary>
    internal uint reserved;
    /// <summary>The type unique identifier</summary>
    internal Guid typeGuid;
    /// <summary>
    /// index-1 of corresponding parameter with length info - 0 = no para, 1 = first para...
    /// </summary>
    /// <remarks>This field references to the Parameter that defines the length for this
    /// generic one. Equally to the marshalling attributes of COM (sizeof, lenght)
    /// this enables to transport parameter of type (PVOID)
    /// </remarks>
    internal ushort lengthIsPara;
    /// <summary>length of datatype name (excl. \0)</summary>
    internal ushort nameLength;
    /// <summary>length of dataitem type name (excl. \0)</summary>
    internal ushort typeLength;
    /// <summary>length of comment (excl. \0)</summary>
    internal ushort commentLength;
    /// <summary>name of datatype with terminating \0</summary>
    internal string name = string.Empty;
    /// <summary>type name of dataitem with terminating \0</summary>
    internal string type = string.Empty;
    /// <summary>comment of datatype with terminating \0</summary>
    internal string comment = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsMethodParaInfo" /> class.
    /// </summary>
    /// <exclude />
    public AdsMethodParaInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsMethodParaInfo" /> class.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    internal AdsMethodParaInfo(IStringMarshaler marshaler, ReadOnlySpan<byte> span) => this.Unmarshal(marshaler, span);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsMethodParaInfo" /> class.
    /// </summary>
    /// <param name="para">The para.</param>
    /// <param name="marshaler">The marshaler.</param>
    internal AdsMethodParaInfo(IRpcMethodParameter para, IStringMarshaler marshaler)
    {
      this.name = para.Name;
      this.nameLength = (ushort) marshaler.MarshalSize(para.Name);
      this.size = (uint) para.Size;
      this.alignSize = (uint) para.Size;
      this.comment = "";
      this.commentLength = (ushort) marshaler.MarshalSize(this.comment);
      this.flags = para.ParameterFlags;
      this.lengthIsPara = (ushort) para.LengthIsParameterIndex;
      this.reserved = 0U;
      this.type = para.TypeName;
      this.typeGuid = Guid.Empty;
      this.typeLength = (ushort) marshaler.MarshalSize(para.TypeName);
    }

    /// <summary>
    /// Gets the marshal size of this <see cref="T:TwinCAT.Ads.Internal.AdsMethodParaInfo" /> object.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>System.Int32.</returns>
    public int MarshalSize(IStringMarshaler marshaler) => 48 + marshaler.MarshalSize(this.name) + marshaler.MarshalSize(this.type) + marshaler.MarshalSize(this.comment);

    /// <summary>
    /// Marshals the content of this <see cref="T:TwinCAT.Ads.Internal.AdsMethodParaInfo" /> into the buffer.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    public int Marshal(IStringMarshaler marshaler, Span<byte> buffer)
    {
      int num = this.MarshalSize(marshaler);
      int start1 = 0;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start1), (uint) num);
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start2), this.size);
      int start3 = start2 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start3), this.alignSize);
      int start4 = start3 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start4), (uint) this.dataType);
      int start5 = start4 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start5), (uint) this.flags);
      int start6 = start5 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start6), this.reserved);
      int start7 = start6 + 4;
      this.typeGuid.ToByteArray().CopyTo<byte>(buffer.Slice(start7));
      int start8 = start7 + 16;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start8), this.lengthIsPara);
      int start9 = start8 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start9), (ushort) (marshaler.MarshalSize(this.name) - 1));
      int start10 = start9 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start10), (ushort) (marshaler.MarshalSize(this.type) - 1));
      int start11 = start10 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start11), (ushort) (marshaler.MarshalSize(this.comment) - 1));
      int start12 = start11 + 2;
      int start13 = start12 + marshaler.Marshal(this.name, buffer.Slice(start12));
      int start14 = start13 + marshaler.Marshal(this.type, buffer.Slice(start13));
      return start14 + marshaler.Marshal(this.comment, buffer.Slice(start14));
    }

    /// <summary>
    /// Unmarshals data into the <see cref="T:TwinCAT.Ads.Internal.AdsMethodParaInfo" /> object.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The read buffer.</param>
    /// <returns>A tuple that contains the data validity plus the read bytes.</returns>
    /// <remarks>This method should try to resync the readBytes result to the next valid readable object. Usually
    /// read structures contain their size as first element. If the Read is not valid, than the Unmarshalled object should be ignored.</remarks>
    public (bool valid, int readBytes) Unmarshal(
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span)
    {
      bool flag = true;
      int start1 = 0;
      this.entryLength = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start1));
      int start2 = start1 + 4;
      int entryLength = (int) this.entryLength;
      this.size = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start2));
      int start3 = start2 + 4;
      this.alignSize = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start3));
      int start4 = start3 + 4;
      this.dataType = (AdsDataTypeId) (int) BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start4));
      int start5 = start4 + 4;
      this.flags = (MethodParamFlags) (int) BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start5));
      int start6 = start5 + 4;
      this.reserved = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start6));
      int start7 = start6 + 4;
      this.typeGuid = new Guid(span.Slice(start7, 16));
      int start8 = start7 + 16;
      this.lengthIsPara = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start8));
      int start9 = start8 + 2;
      this.nameLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start9));
      int start10 = start9 + 2;
      this.typeLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start10));
      int start11 = start10 + 2;
      this.commentLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start11));
      int start12 = start11 + 2;
      int start13 = start12 + marshaler.Unmarshal(span.Slice(start12, (int) this.nameLength + 1), ref this.name);
      int start14 = start13 + marshaler.Unmarshal(span.Slice(start13, (int) this.typeLength + 1), ref this.type);
      int start15 = start14 + marshaler.Unmarshal(span.Slice(start14, (int) this.commentLength + 1), ref this.comment);
      if (start15 <= entryLength)
      {
        byte[] destination = new byte[entryLength - start15];
        span.Slice(start15, entryLength - start15).CopyTo((Span<byte>) destination);
      }
      else if (start15 > entryLength)
      {
        AdsModule.Trace.TraceError("Reading MethodPara entry for '{0}' failed!", new object[1]
        {
          (object) this.name
        });
        start15 = entryLength;
        flag = false;
      }
      return (flag, start15);
    }
  }
}
