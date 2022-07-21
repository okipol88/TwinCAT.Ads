// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsSymbolEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>For internal use only.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Browsable(false)]
  internal sealed class AdsSymbolEntry : ISymbolInfo, IAdsCustomMarshal<AdsSymbolEntry>
  {
    /// <summary>ADS Entry length (the marshal size, 0)</summary>
    private uint _entryLength;
    /// <summary>4 indexGroup of symbol: input, output etc.</summary>
    private uint _indexGroup;
    /// <summary>8 indexOffset of symbol</summary>
    private uint _indexOffset;
    /// <summary>12 size of symbol ( in bytes, 0 = bit )</summary>
    private uint _size;
    /// <summary>16 adsDataType of symbol</summary>
    private uint _dataTypeId;
    /// <summary>20 see above</summary>
    private AdsSymbolFlags _flags;
    /// <summary>ExtendedFlags</summary>
    private uint _extendedFlags;
    /// <summary>Reserved must be 0, legacy Array Dimension count (22)</summary>
    private ushort _reservedUShort;
    /// <summary>24 length of symbol name (excl. \0)</summary>
    private ushort _nameLength;
    /// <summary>26 length of type name (excl. \0)</summary>
    private ushort _typeNameLength;
    /// <summary>//28 length of comment (excl. \0)</summary>
    private ushort _commentLength;
    /// <summary>Symbol name string (30)</summary>
    private string _name = string.Empty;
    /// <summary>Symbol Type Name (30 +namelength +1)</summary>
    private string _typeName = string.Empty;
    /// <summary>The comment (30 +namelength+1 +typelenght + 1)</summary>
    private string _comment = string.Empty;
    /// <summary>The type unique identifier (16 Bytes)</summary>
    private Guid _typeGuid = Guid.Empty;
    /// <summary>The attribute count (2 Bytes)</summary>
    private ushort _attributeCount;
    /// <summary>The attributes</summary>
    private AdsAttributeEntry[] _attributeEntries = Array.Empty<AdsAttributeEntry>();
    /// <summary>The reserved</summary>
    private byte[] _reserved = Array.Empty<byte>();

    /// <summary>
    /// Prevents a default instance of the <see cref="T:TwinCAT.Ads.Internal.AdsSymbolEntry" /> class from being created.
    /// </summary>
    private AdsSymbolEntry()
    {
    }

    internal AdsSymbolEntry(IAdsSymbol symbol, IStringMarshaler symbolNameMarshaler)
    {
      this._indexGroup = ((IProcessImageAddress) symbol).IndexGroup;
      this._indexOffset = ((IProcessImageAddress) symbol).IndexOffset;
      this._size = (uint) ((IBitSize) symbol).Size;
      this._dataTypeId = (uint) symbol.DataTypeId;
      this._flags = ((Instance) symbol).Flags;
      this._name = ((IInstance) symbol).InstancePath;
      this._nameLength = (ushort) (symbolNameMarshaler.MarshalSize(((IInstance) symbol).InstanceName) - 1);
      this._typeName = ((IInstance) symbol).TypeName;
      this._typeNameLength = (ushort) (symbolNameMarshaler.MarshalSize(((IInstance) symbol).TypeName) - 1);
      this._comment = ((IInstance) symbol).Comment;
      this._commentLength = (ushort) (symbolNameMarshaler.MarshalSize(((IInstance) symbol).Comment) - 1);
      this._reservedUShort = (ushort) 0;
      this._typeGuid = Guid.Empty;
      this._attributeCount = (ushort) ((ICollection<ITypeAttribute>) ((IAttributedInstance) symbol).Attributes).Count;
      this._attributeEntries = ((ReadOnlyTypeAttributeCollection) ((IAttributedInstance) symbol).Attributes).ToEntries();
      this._extendedFlags = 0U;
      this._reserved = new byte[0];
      this._entryLength = (uint) this.MarshalSize(symbolNameMarshaler);
    }

    /// <summary>
    /// Parses the <see cref="T:TwinCAT.Ads.Internal.AdsSymbolEntry" />.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    /// <param name="symbol">The symbol.</param>
    /// <returns>System.ValueTuple&lt;System.Boolean, System.Int32&gt;.</returns>
    internal static (bool valid, int readBytes) Parse(
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span,
      out AdsSymbolEntry? symbol)
    {
      AdsSymbolEntry adsSymbolEntry = new AdsSymbolEntry();
      (bool valid, int readBytes) tuple = adsSymbolEntry.Unmarshal(marshaler, span);
      symbol = !tuple.valid ? (AdsSymbolEntry) null : adsSymbolEntry;
      return tuple;
    }

    /// <summary>Gets the minimum MarshalSize</summary>
    /// <value>The minimum size of the marshal.</value>
    public static int MinMarshalSize => 30;

    /// <summary>
    /// Gets the marshal size of this <see cref="T:TwinCAT.Ads.Internal.AdsSymbolEntry" />.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>System.Int32.</returns>
    public int MarshalSize(IStringMarshaler marshaler)
    {
      int num = 30 + (marshaler.MarshalSize(this._name) + marshaler.MarshalSize(this._typeName) + marshaler.MarshalSize(this._comment));
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 8))
        num += 16;
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 4096) && this._attributeEntries != null && this._attributeEntries.Length >= 0)
        num = num + 2 + SubStructureReader<AdsAttributeEntry>.MarshalSize(this._attributeEntries, marshaler);
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 32768))
        num += 4;
      if (this._reserved != null)
        num += this._reserved.Length;
      return num;
    }

    /// <summary>
    /// Marshals the content of this <see cref="T:TwinCAT.Ads.Internal.AdsSymbolEntry" />.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    public int Marshal(IStringMarshaler marshaler, Span<byte> buffer)
    {
      int start1 = 0;
      uint num = (uint) this.MarshalSize(marshaler);
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start1), num);
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start2), this._indexGroup);
      int start3 = start2 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start3), this._indexOffset);
      int start4 = start3 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start4), this._size);
      int start5 = start4 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start5), this._dataTypeId);
      int start6 = start5 + 4;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start6), (ushort) this._flags);
      int start7 = start6 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start7), (ushort) 0);
      int start8 = start7 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start8), (ushort) (marshaler.MarshalSize(this._name) - 1));
      int start9 = start8 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start9), (ushort) (marshaler.MarshalSize(this._typeName) - 1));
      int start10 = start9 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start10), (ushort) (marshaler.MarshalSize(this._comment) - 1));
      int start11 = start10 + 2;
      int start12 = start11 + marshaler.Marshal(this._name, buffer.Slice(start11));
      int start13 = start12 + marshaler.Marshal(this._typeName, buffer.Slice(start12));
      int start14 = start13 + marshaler.Marshal(this._comment, buffer.Slice(start13));
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 8))
      {
        this._typeGuid.ToByteArray().CopyTo<byte>(buffer.Slice(start14));
        start14 += 16;
      }
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 4096) && this._attributeEntries != null && this._attributeEntries.Length >= 0)
      {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start14), this._attributeCount);
        int start15 = start14 + 2;
        start14 = start15 + SubStructureReader<AdsAttributeEntry>.Marshal(this._attributeEntries, marshaler, buffer.Slice(start15));
      }
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 32768))
      {
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start14), this._extendedFlags);
        start14 += 4;
      }
      if (this._reserved != null)
        this._reserved.CopyTo<byte>(buffer.Slice(start14));
      return start14;
    }

    /// <summary>
    /// Unmarshals data into this <see cref="T:TwinCAT.Ads.Internal.AdsSymbolEntry" /> objecct.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The read buffer.</param>
    /// <returns>A tuple that contains the data validity plus the read bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">marshaler</exception>
    /// <remarks>This method should try to resync the readBytes result to the next valid readable object. Usually
    /// read structures contain their size as first element. If the Read is not valid, than the Unmarshalled object should be ignored.</remarks>
    public (bool valid, int readBytes) Unmarshal(
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span)
    {
      if (marshaler == null)
        throw new ArgumentNullException(nameof (marshaler));
      bool flag = true;
      int start1 = 0;
      this._entryLength = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start1, 4));
      int start2 = start1 + 4;
      int entryLength = (int) this._entryLength;
      int start3;
      DefaultInterpolatedStringHandler interpolatedStringHandler;
      try
      {
        this._indexGroup = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start2, 4));
        int start4 = start2 + 4;
        this._indexOffset = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start4, 4));
        int start5 = start4 + 4;
        this._size = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start5, 4));
        int start6 = start5 + 4;
        this._dataTypeId = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start6, 4));
        int start7 = start6 + 4;
        ushort num = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start7, 2));
        int start8 = start7 + 2;
        this._flags = (AdsSymbolFlags) (int) num;
        this._reservedUShort = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start8, 2));
        int start9 = start8 + 2;
        this._nameLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start9, 2));
        int start10 = start9 + 2;
        this._typeNameLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start10, 2));
        int start11 = start10 + 2;
        this._commentLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start11, 2));
        int start12 = start11 + 2;
        int start13 = start12 + marshaler.Unmarshal(span.Slice(start12, (int) this._nameLength + 1), ref this._name);
        int start14 = start13 + marshaler.Unmarshal(span.Slice(start13, (int) this._typeNameLength + 1), ref this._typeName);
        start3 = start14 + marshaler.Unmarshal(span.Slice(start14, (int) this._commentLength + 1), ref this._comment);
        if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 8))
        {
          this._typeGuid = new Guid(span.Slice(start3, 16));
          start3 += 16;
        }
        if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 4096))
        {
          this._attributeCount = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start3, 2));
          int start15 = start3 + 2;
          start3 = start15 + SubStructureReader<AdsAttributeEntry>.Unmarshal((uint) this._attributeCount, marshaler, span.Slice(start15, entryLength - start15), out this._attributeEntries);
        }
        if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsSymbolFlags) 32768))
        {
          this._extendedFlags = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start3, 4));
          start3 += 4;
        }
        if (start3 != entryLength)
        {
          if (start3 <= entryLength)
          {
            this._reserved = new byte[entryLength - start3];
            span.Slice(start3, entryLength - start3).CopyTo(this._reserved.AsSpan<byte>());
            start3 = entryLength;
          }
          else
          {
            interpolatedStringHandler = new DefaultInterpolatedStringHandler(79, 2);
            interpolatedStringHandler.AppendLiteral("Symbol entry for '");
            interpolatedStringHandler.AppendFormatted(this._name);
            interpolatedStringHandler.AppendLiteral(":");
            interpolatedStringHandler.AppendFormatted(this._typeName);
            interpolatedStringHandler.AppendLiteral("' has a mismatching entry size. This symbol will be ignored!");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            AdsModule.Trace.TraceError(stringAndClear);
            start3 = entryLength;
            flag = false;
          }
        }
      }
      catch (Exception ex)
      {
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(82, 3);
        interpolatedStringHandler.AppendLiteral("Symbol entry for '");
        interpolatedStringHandler.AppendFormatted(this._name);
        interpolatedStringHandler.AppendLiteral(":");
        interpolatedStringHandler.AppendFormatted(this._typeName);
        interpolatedStringHandler.AppendLiteral("' could not be read (Exception: ");
        interpolatedStringHandler.AppendFormatted(ex.Message);
        interpolatedStringHandler.AppendLiteral("). This symbol will be ignored!");
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        AdsModule.Trace.TraceError(stringAndClear);
        start3 = entryLength;
        flag = false;
      }
      return (flag, start3);
    }

    /// <summary>Gets the index group of the symbol</summary>
    /// <value>The index group.</value>
    public uint IndexGroup => this._indexGroup;

    /// <summary>Gets the index offset of the symbol</summary>
    /// <value>The index offset.</value>
    public uint IndexOffset => this._indexOffset;

    /// <summary>12 size of symbol ( in bytes, 0 = bit )</summary>
    public uint Size => this._size;

    /// <summary>Gets the AdsDatatype</summary>
    /// <value>The type of the ads data.</value>
    public AdsDataTypeId DataTypeId => (AdsDataTypeId) (int) this._dataTypeId;

    public AdsSymbolFlags Flags => this._flags;

    /// <summary>Gets the symbol name.</summary>
    /// <value>The name.</value>
    public string Name => this._name;

    public string TypeName => this._typeName;

    /// <summary>Gets the symbol comment.</summary>
    /// <value>The comment.</value>
    public string Comment => this._comment;

    /// <summary>Gets the attribute count.</summary>
    /// <value>The attribute count.</value>
    public ushort AttributeCount => this._attributeCount;

    /// <summary>Gets the attributes.</summary>
    /// <value>The attributes.</value>
    public AdsAttributeEntry[] Attributes => this._attributeEntries;

    /// <summary>Gets the context mask of this instance.</summary>
    /// <value>The context mask.</value>
    /// <remarks>The Size of the internal data is 4-Bit</remarks>
    public byte ContextMask
    {
      get
      {
        // ISSUE: unable to decompile the method.
      }
    }
  }
}
