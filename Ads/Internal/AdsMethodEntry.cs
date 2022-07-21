// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsMethodEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class AdsMethodEntry.</summary>
  internal class AdsMethodEntry : IAdsCustomMarshal<AdsMethodEntry>
  {
    /// <summary>length of complete datatype entry</summary>
    internal uint entryLength;
    /// <summary>version of datatype structure</summary>
    internal uint version;
    /// <summary>vTable index of this method</summary>
    internal uint vTableIndex;
    /// <summary>size of datatype ( in bytes )</summary>
    internal uint returnSize;
    /// <summary>size of biggest element for alignment</summary>
    internal uint returnAlignSize;
    /// <summary>The reserved</summary>
    internal uint reserved;
    /// <summary>The return type unique identifier</summary>
    internal Guid returnTypeGuid;
    /// <summary>adsDataType of symbol (if alias)</summary>
    internal uint returnDataType;
    /// <summary>Internal option flags</summary>
    internal uint flags;
    /// <summary>length of datatype name (excl. \0)</summary>
    internal ushort nameLength;
    /// <summary>length of dataitem type name (excl. \0)</summary>
    internal ushort returnTypeLength;
    /// <summary>length of comment (excl. \0)</summary>
    internal ushort commentLength;
    /// <summary>The parameter count</summary>
    internal ushort parameterCount;
    /// <summary>name of datatype with terminating \0</summary>
    internal string name = string.Empty;
    /// <summary>type name of dataitem with terminating \0</summary>
    internal string returnType = string.Empty;
    /// <summary>comment of datatype with terminating \0</summary>
    internal string comment = string.Empty;
    /// <summary>Parameters Collection</summary>
    internal AdsMethodParaInfo[] parameters = Array.Empty<AdsMethodParaInfo>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsMethodEntry" /> class.
    /// </summary>
    public AdsMethodEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsMethodEntry" /> class.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    internal AdsMethodEntry(StringMarshaler marshaler, ReadOnlySpan<byte> span) => this.Unmarshal((IStringMarshaler) marshaler, span);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsMethodEntry" /> class.
    /// </summary>
    /// <param name="method">The method.</param>
    internal AdsMethodEntry(IRpcMethod method)
    {
      StringMarshaler unicode = StringMarshaler.Unicode;
      RpcMethod rpcMethod = (RpcMethod) method;
      this.version = 1U;
      this.vTableIndex = (uint) rpcMethod.VTableIndex;
      this.returnSize = (uint) rpcMethod.ReturnTypeSize;
      this.returnAlignSize = (uint) rpcMethod.ReturnAlignSize;
      this.reserved = 0U;
      this.returnTypeGuid = Guid.Empty;
      this.returnType = method.ReturnType;
      this.returnTypeLength = (ushort) unicode.MarshalSize(method.ReturnType);
      this.name = method.Name;
      this.nameLength = (ushort) unicode.MarshalSize(method.Comment);
      this.comment = method.Comment;
      this.commentLength = (ushort) unicode.MarshalSize(method.Comment);
      this.parameterCount = (ushort) ((ICollection<IRpcMethodParameter>) method.Parameters).Count;
      this.parameters = RpcMethodParaConverter.ToMethodParameterInfoArray(method.Parameters, (IStringMarshaler) unicode);
      this.entryLength = (uint) this.MarshalSize((IStringMarshaler) unicode);
    }

    /// <summary>
    /// Marshals the content this <see cref="T:TwinCAT.Ads.Internal.AdsMethodEntry" />.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    public int Marshal(IStringMarshaler marshaler, Span<byte> buffer)
    {
      uint num = (uint) this.MarshalSize(marshaler);
      int start1 = 0;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start1), num);
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start2), this.version);
      int start3 = start2 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start3), this.vTableIndex);
      int start4 = start3 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start4), this.returnSize);
      int start5 = start4 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start5), this.returnAlignSize);
      int start6 = start5 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start6), this.reserved);
      int start7 = start6 + 4;
      this.returnTypeGuid.ToByteArray().CopyTo<byte>(buffer.Slice(start7));
      int start8 = start7 + 16;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start8), this.returnDataType);
      int start9 = start8 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start9), this.flags);
      int start10 = start9 + 4;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start10), (ushort) (marshaler.MarshalSize(this.name) - marshaler.StringTerminatorSize));
      int start11 = start10 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start11), (ushort) (marshaler.MarshalSize(this.returnType) - marshaler.StringTerminatorSize));
      int start12 = start11 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start12), (ushort) (marshaler.MarshalSize(this.comment) - marshaler.StringTerminatorSize));
      int start13 = start12 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start13), this.parameterCount);
      int start14 = start13 + 2;
      int start15 = start14 + marshaler.Marshal(this.name, buffer.Slice(start14));
      int start16 = start15 + marshaler.Marshal(this.returnType, buffer.Slice(start15));
      int start17 = start16 + marshaler.Marshal(this.comment, buffer.Slice(start16));
      return start17 + SubStructureReader<AdsMethodParaInfo>.Marshal(this.parameters, marshaler, buffer.Slice(start17));
    }

    /// <summary>
    /// Unmarshals data into the <see cref="T:TwinCAT.Ads.Internal.AdsMethodEntry" /> object.
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
      int start1 = 0;
      this.entryLength = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start1));
      int start2 = start1 + 4;
      int entryLength = (int) this.entryLength;
      this.version = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start2));
      int start3 = start2 + 4;
      this.vTableIndex = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start3));
      int start4 = start3 + 4;
      this.returnSize = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start4));
      int start5 = start4 + 4;
      this.returnAlignSize = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start5));
      int start6 = start5 + 4;
      this.reserved = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start6));
      int start7 = start6 + 4;
      this.returnTypeGuid = new Guid(span.Slice(start7, 16));
      int start8 = start7 + 16;
      this.returnDataType = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start8));
      int start9 = start8 + 4;
      this.flags = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start9));
      int start10 = start9 + 4;
      this.nameLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start10));
      int start11 = start10 + 2;
      this.returnTypeLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start11));
      int start12 = start11 + 2;
      this.commentLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start12));
      int start13 = start12 + 2;
      this.parameterCount = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start13));
      int start14 = start13 + 2;
      marshaler.Encoding.GetByteCount(new char[1]);
      int start15 = start14 + marshaler.Unmarshal(span.Slice(start14, (int) this.nameLength + marshaler.StringTerminatorSize), ref this.name);
      int start16 = start15 + marshaler.Unmarshal(span.Slice(start15, (int) this.returnTypeLength + marshaler.StringTerminatorSize), ref this.returnType);
      int start17 = start16 + marshaler.Unmarshal(span.Slice(start16, (int) this.commentLength + marshaler.StringTerminatorSize), ref this.comment);
      int num = start17 + SubStructureReader<AdsMethodParaInfo>.Unmarshal((uint) this.parameterCount, marshaler, span.Slice(start17, entryLength - start17), out this.parameters);
      if (num != entryLength)
        num = entryLength;
      return (true, num);
    }

    /// <summary>
    /// Gets the marshal size of this <see cref="T:TwinCAT.Ads.Internal.AdsMethodEntry" /> object.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>System.Int32.</returns>
    public int MarshalSize(IStringMarshaler marshaler) => 56 + marshaler.MarshalSize(this.name) + marshaler.MarshalSize(this.returnType) + marshaler.MarshalSize(this.comment) + SubStructureReader<AdsMethodParaInfo>.MarshalSize(this.parameters, marshaler);
  }
}
