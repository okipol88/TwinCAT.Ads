// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsDataTypeEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class AdsDatatypeEntry (for internal use only)</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class AdsDataTypeEntry : IAdsCustomMarshal<AdsDataTypeEntry>
  {
    /// <summary>0 length of complete datatype entry</summary>
    private uint _entryLength;
    /// <summary>4 version of datatype structure</summary>
    private uint _version;
    /// <summary>8 hashValue of datatype to compare datatypes</summary>
    private uint _hashValue;
    /// <summary>
    /// 12 hashValue of base type / Code Offset to setter Method (typeHashValue or offsSetCode)
    /// </summary>
    private uint _typeHashValue;
    /// <summary>
    /// 16 size of datatype ( in bytes or bits depending on the BitValues flag )
    /// </summary>
    private uint _size;
    /// <summary>
    /// 20 offs of dataitem in parent datatype ( in bytes or bits depending on the BitValues flag )
    /// </summary>
    private uint _offset;
    /// <summary>24 adsDataType of symbol (if alias)</summary>
    private AdsDataTypeId _baseTypeId;
    /// <summary>The flags (28)</summary>
    private AdsDataTypeFlags _flags;
    /// <summary>length 32 length of datatype name (excl. \0)</summary>
    private ushort _entryNameLength;
    /// <summary>34 length of dataitem type name (excl. \0)</summary>
    private ushort _typeNameLength;
    /// <summary>36 length of comment (excl. \0)</summary>
    private ushort _commentLength;
    /// <summary>38, count of array dimensions</summary>
    private ushort _arrayDim;
    /// <summary>40, count of SubItems</summary>
    private ushort _subItems;
    /// <summary>
    /// 42, Name of the Entry (Name of the DataType on DataType, Fieldname on Field / SubItem)
    /// </summary>
    private string _entryName = string.Empty;
    /// <summary>
    /// Datatype name (42 + nameLength + 1 ) (only for fields)
    /// </summary>
    private string _typeName = string.Empty;
    /// <summary>Comment (42 + typeLength +1 + typeLength+1)</summary>
    private string _comment = string.Empty;
    /// <summary>The array infos</summary>
    private AdsDataTypeArrayInfo[]? _arrayInfos;
    /// <summary>The sub entries</summary>
    private AdsFieldEntry[]? _subEntries;
    /// <summary>The type unique identifier</summary>
    private Guid _typeGuid = Guid.Empty;
    /// <summary>The copy mask</summary>
    private byte[] _copyMask = Array.Empty<byte>();
    /// <summary>The method count</summary>
    private ushort _methodCount;
    /// <summary>The methods</summary>
    private AdsMethodEntry[]? _methods;
    /// <summary>The attribute count</summary>
    private ushort _attributeCount;
    /// <summary>The attributes</summary>
    private AdsAttributeEntry[]? _attributes;
    /// <summary>The enum information count</summary>
    private ushort _enumInfoCount;
    /// <summary>The enums</summary>
    private AdsEnumInfoEntry[]? _enums;
    /// <summary>The reserved</summary>
    private byte[] _reserved = Array.Empty<byte>();
    /// <summary>The s_id count</summary>
    private static int s_idCount;
    /// <summary>The _root entry</summary>
    private bool _rootEntry;
    /// <summary>The _id</summary>
    private int _id;

    /// <summary>
    /// ashValue of base type / Code Offset to setter Method (typeHashValue or offsSetCode)
    /// </summary>
    /// <value>The type hash value.</value>
    public uint TypeHashValue => this._typeHashValue;

    /// <summary>Gets the offset of the SubItem</summary>
    /// <value>The offset.</value>
    public int Offset => (int) this._offset;

    internal AdsDataTypeId BaseTypeId => this._baseTypeId;

    /// <summary>Gets the DataType Flags</summary>
    /// <value>The flags.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal AdsDataTypeFlags Flags => this._flags;

    internal int ArrayDimCount => (int) this._arrayDim;

    /// <summary>Gets the number of SubItems.</summary>
    /// <value>The sub item count.</value>
    public int SubItemCount => (int) this._subItems;

    internal string EntryName => this._entryName;

    internal string TypeName => this._typeName;

    internal AdsDataTypeArrayInfo[]? ArrayInfos => this._arrayInfos;

    internal AdsFieldEntry[]? SubEntries => this._subEntries;

    internal int MethodCount => (int) this._methodCount;

    internal AdsMethodEntry[]? Methods => this._methods;

    internal int EnumInfoCount => (int) this._enumInfoCount;

    internal AdsEnumInfoEntry[]? Enums => this._enums;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" /> class.
    /// </summary>
    internal AdsDataTypeEntry()
    {
      this._id = ++AdsDataTypeEntry.s_idCount;
      this._rootEntry = false;
    }

    protected AdsDataTypeEntry(IMember member)
    {
      StringMarshaler stringMarshaler = new StringMarshaler(Encoding.Unicode, (StringConvertMode) 1);
      this._rootEntry = false;
      DataType dataType = (DataType) ((IInstance) member).DataType;
      this._entryNameLength = (ushort) stringMarshaler.MarshalSize(((IInstance) member).InstanceName);
      this._entryName = ((IInstance) member).InstanceName;
      this._baseTypeId = dataType.DataTypeId;
      this._typeNameLength = (ushort) stringMarshaler.MarshalSize(dataType.Name);
      this._typeName = dataType.Name;
      this._comment = ((IInstance) member).Comment;
      this._commentLength = (ushort) stringMarshaler.MarshalSize(((IInstance) member).Comment);
      this._size = (uint) dataType.Size;
      this._typeGuid = Guid.Empty;
      this._hashValue = 0U;
      this._version = 1U;
      this._flags = (AdsDataTypeFlags) 0;
      this._offset = (uint) member.Offset;
      this._reserved = new byte[0];
      this._attributeCount = (ushort) ((ICollection<ITypeAttribute>) ((IAttributedInstance) member).Attributes).Count;
      this._attributes = ((IEnumerable<ITypeAttribute>) ((IAttributedInstance) member).Attributes).Select<ITypeAttribute, AdsAttributeEntry>((Func<ITypeAttribute, AdsAttributeEntry>) (a => new AdsAttributeEntry(a))).ToArray<AdsAttributeEntry>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="symbolNameMarshaler">The symbol name marshaler.</param>
    internal AdsDataTypeEntry(IDataType type, IStringMarshaler symbolNameMarshaler)
      : this()
    {
      this._rootEntry = true;
      this._baseTypeId = ((DataType) type).DataTypeId;
      this._entryNameLength = (ushort) symbolNameMarshaler.MarshalSize(type.Name);
      this._entryName = type.Name;
      this._commentLength = (ushort) symbolNameMarshaler.MarshalSize(type.Comment);
      this._comment = type.Comment;
      this._size = (uint) ((IBitSize) type).Size;
      this._typeGuid = Guid.Empty;
      this._hashValue = 0U;
      this._version = 1U;
      this._flags = ((DataType) type).Flags;
      this._offset = 0U;
      this._reserved = new byte[0];
      this._attributeCount = (ushort) ((ICollection<ITypeAttribute>) type.Attributes).Count;
      this._attributes = ((IEnumerable<ITypeAttribute>) type.Attributes).Select<ITypeAttribute, AdsAttributeEntry>((Func<ITypeAttribute, AdsAttributeEntry>) (a => new AdsAttributeEntry(a))).ToArray<AdsAttributeEntry>();
      switch ((int) type.Category)
      {
        case 2:
          DataType baseType1 = (DataType) ((AliasType) type).BaseType;
          this._baseTypeId = baseType1.DataTypeId;
          this._typeNameLength = (ushort) symbolNameMarshaler.MarshalSize(baseType1.Name);
          this._typeName = baseType1.Name;
          break;
        case 3:
          IEnumType ienumType = (IEnumType) type;
          DataType baseType2 = (DataType) ((IAliasType) ienumType).BaseType;
          this._baseTypeId = baseType2.DataTypeId;
          this._typeNameLength = (ushort) symbolNameMarshaler.MarshalSize(baseType2.Name);
          this._typeName = baseType2.Name;
          this._enumInfoCount = (ushort) ((ICollection<IEnumValue>) ienumType.EnumValues).Count;
          this._enums = ((IEnumerable<IEnumValue>) ienumType.EnumValues).Select<IEnumValue, AdsEnumInfoEntry>((Func<IEnumValue, AdsEnumInfoEntry>) (e => new AdsEnumInfoEntry(e.Name, e.RawValue, symbolNameMarshaler))).ToArray<AdsEnumInfoEntry>();
          break;
        case 4:
          IArrayType iarrayType = (IArrayType) type;
          DataType elementType = (DataType) iarrayType.ElementType;
          this._baseTypeId = elementType.DataTypeId;
          this._typeNameLength = (ushort) symbolNameMarshaler.MarshalSize(elementType.Name);
          this._typeName = elementType.Name;
          this._arrayDim = (ushort) ((ICollection<IDimension>) iarrayType.Dimensions).Count;
          this._arrayInfos = DimensionConverter.ToAdsDataTypeArrayInfo((IList<IDimension>) iarrayType.Dimensions);
          break;
        case 5:
          IStructType istructType = (IStructType) type;
          DataType baseType3 = (DataType) ((IInterfaceType) istructType).BaseType;
          if (baseType3 != null)
          {
            this._baseTypeId = baseType3.DataTypeId;
            this._typeNameLength = (ushort) symbolNameMarshaler.MarshalSize(baseType3.Name);
            this._typeName = baseType3.Name;
          }
          this._subItems = (ushort) ((ICollection<IMember>) ((IInterfaceType) istructType).Members).Count;
          this._subEntries = TypeMembersConverter.ToFieldEntryArray(((IInterfaceType) istructType).Members);
          IRpcCallableType irpcCallableType = (IRpcCallableType) istructType;
          if (irpcCallableType == null || ((ICollection<IRpcMethod>) irpcCallableType.RpcMethods).Count <= 0)
            break;
          this._methods = RpcMethodConverter.ToMethodEntryArray(irpcCallableType.RpcMethods);
          this._methodCount = (ushort) ((ICollection<IRpcMethod>) irpcCallableType.RpcMethods).Count;
          this._flags = (AdsDataTypeFlags) (this._flags | 2048);
          break;
        case 13:
          DataType referencedType1 = (DataType) ((IPointerType) type).ReferencedType;
          if (referencedType1 == null)
            break;
          this._baseTypeId = referencedType1.DataTypeId;
          this._typeNameLength = (ushort) symbolNameMarshaler.MarshalSize(referencedType1.Name);
          this._typeName = referencedType1.Name;
          break;
        case 15:
          DataType referencedType2 = (DataType) ((IReferenceType) type).ReferencedType;
          if (referencedType2 == null)
            break;
          this._baseTypeId = referencedType2.DataTypeId;
          this._typeNameLength = (ushort) symbolNameMarshaler.MarshalSize(referencedType2.Name);
          this._typeName = referencedType2.Name;
          break;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" /> class.
    /// </summary>
    /// <param name="rootEntry">if set to <c>true</c> [root entry].</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The read buffer.</param>
    internal AdsDataTypeEntry(bool rootEntry, IStringMarshaler marshaler, ReadOnlySpan<byte> span)
    {
      this._id = ++AdsDataTypeEntry.s_idCount;
      this._rootEntry = rootEntry;
      this.Unmarshal(marshaler, span);
    }

    /// <summary>Gets the minimum MarshalSize</summary>
    /// <value>The minimum size of the marshal.</value>
    public static int MinMarshalSize => 42;

    /// <summary>
    /// Gets the marshal size of this <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" />.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>System.Int32.</returns>
    public virtual int MarshalSize(IStringMarshaler marshaler)
    {
      int num = 42 + marshaler.MarshalSize(this._entryName) + marshaler.MarshalSize(this._typeName) + marshaler.MarshalSize(this._comment);
      if (this._arrayInfos != null && this._arrayInfos.Length != 0)
        num += SubStructureReader<AdsDataTypeArrayInfo>.MarshalSize(this._arrayInfos, marshaler);
      if (this._subEntries != null && this._subEntries.Length != 0)
        num += SubStructureReader<AdsFieldEntry>.MarshalSize(this._subEntries, marshaler);
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 128))
        num += 16;
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 512))
        num += this._copyMask.Length;
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 2048))
        num = num + 2 + SubStructureReader<AdsMethodEntry>.MarshalSize(this._methods, marshaler);
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 4096))
        num = num + 2 + SubStructureReader<AdsAttributeEntry>.MarshalSize(this._attributes, marshaler);
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 8192))
        num = num + 2 + EnumSubStructureReader<AdsEnumInfoEntry>.MarshalSize(this._enums, marshaler);
      return num;
    }

    /// <summary>
    /// Marshals the content of this <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" />.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    public virtual int Marshal(IStringMarshaler marshaler, Span<byte> buffer)
    {
      int start1 = 0;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start1), (uint) this.MarshalSize(marshaler));
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start2), this._version);
      int start3 = start2 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start3), this._hashValue);
      int start4 = start3 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start4), this._typeHashValue);
      int start5 = start4 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start5), this._size);
      int start6 = start5 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start6), this._offset);
      int start7 = start6 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start7), (uint) this._baseTypeId);
      int start8 = start7 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start8), (uint) this._flags);
      int start9 = start8 + 4;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start9), (ushort) (marshaler.MarshalSize(this._entryName) - 1));
      int start10 = start9 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start10), (ushort) (marshaler.MarshalSize(this._typeName) - 1));
      int start11 = start10 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start11), (ushort) (marshaler.MarshalSize(this._comment) - 1));
      int start12 = start11 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start12), this._arrayDim);
      int start13 = start12 + 2;
      BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start13), this._subItems);
      int start14 = start13 + 2;
      int start15 = start14 + marshaler.Marshal(this._entryName, buffer.Slice(start14));
      int start16 = start15 + marshaler.Marshal(this._typeName, buffer.Slice(start15));
      int start17 = start16 + marshaler.Marshal(this._comment, buffer.Slice(start16));
      if (this._arrayInfos != null && this._arrayInfos.Length != 0)
        start17 += SubStructureReader<AdsDataTypeArrayInfo>.Marshal(this._arrayInfos, marshaler, buffer.Slice(start17));
      if (this._subEntries != null && this._subEntries.Length != 0)
        start17 += SubStructureReader<AdsFieldEntry>.Marshal(this._subEntries, marshaler, buffer.Slice(start17));
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 128))
      {
        this._typeGuid.ToByteArray().CopyTo<byte>(buffer.Slice(start17));
        start17 += 16;
      }
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 512))
      {
        this._copyMask.CopyTo<byte>(buffer.Slice(start17));
        start17 += this._copyMask.Length;
      }
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 2048))
      {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start17), this._methodCount);
        int start18 = start17 + 2;
        start17 = start18 + SubStructureReader<AdsMethodEntry>.Marshal(this._methods, marshaler, buffer.Slice(start18));
      }
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 4096))
      {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start17), this._attributeCount);
        int start19 = start17 + 2;
        start17 = start19 + SubStructureReader<AdsAttributeEntry>.Marshal(this._attributes, marshaler, buffer.Slice(start19));
      }
      if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 8192))
      {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(start17), this._enumInfoCount);
        int start20 = start17 + 2;
        start17 = start20 + EnumSubStructureReader<AdsEnumInfoEntry>.Marshal(this._enums, marshaler, buffer.Slice(start20));
      }
      return start17;
    }

    /// <summary>
    /// Unmarshals data into this <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" /> object.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The read buffer.</param>
    /// <returns>A tuple that contains the data validity plus the read bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">marshaler</exception>
    /// <remarks>This method should try to resync the readBytes result to the next valid readable object. Usually
    /// read structures contain their size as first element. If the Read is not valid, than the Unmarshalled object should be ignored.</remarks>
    public virtual (bool valid, int readBytes) Unmarshal(
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span)
    {
      if (marshaler == null)
        throw new ArgumentNullException(nameof (marshaler));
      bool flag = true;
      int start1 = 0;
      this._entryLength = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start1));
      int start2 = start1 + 4;
      int entryLength = (int) this._entryLength;
      int start3;
      try
      {
        this._version = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start2));
        int start4 = start2 + 4;
        this._hashValue = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start4));
        int start5 = start4 + 4;
        this._typeHashValue = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start5));
        int start6 = start5 + 4;
        this._size = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start6));
        int start7 = start6 + 4;
        this._offset = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start7));
        int start8 = start7 + 4;
        this._baseTypeId = (AdsDataTypeId) (int) BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start8));
        int start9 = start8 + 4;
        this._flags = (AdsDataTypeFlags) (int) BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start9));
        int start10 = start9 + 4;
        this._entryNameLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start10));
        int start11 = start10 + 2;
        this._typeNameLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start11));
        int start12 = start11 + 2;
        this._commentLength = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start12));
        int start13 = start12 + 2;
        this._arrayDim = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start13));
        int start14 = start13 + 2;
        this._subItems = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start14));
        int start15 = start14 + 2;
        int start16 = start15 + marshaler.Unmarshal(span.Slice(start15, (int) this._entryNameLength + 1), ref this._entryName);
        int start17 = start16 + marshaler.Unmarshal(span.Slice(start16, (int) this._typeNameLength + 1), ref this._typeName);
        int start18 = start17 + marshaler.Unmarshal(span.Slice(start17, (int) this._commentLength + 1), ref this._comment);
        int start19 = start18 + SubStructureReader<AdsDataTypeArrayInfo>.Unmarshal((uint) this._arrayDim, marshaler, span.Slice(start18, entryLength - start18), out this._arrayInfos);
        start3 = start19 + SubStructureReader<AdsFieldEntry>.Unmarshal((uint) this._subItems, marshaler, span.Slice(start19, entryLength - start19), out this._subEntries);
        if (this._subEntries != null && this._subEntries.Length != (int) this._subItems)
        {
          AdsModule.Trace.TraceWarning("Entry name '{0}', type '{1}' indicates {2} subentries but only {3} subentries found!", new object[4]
          {
            (object) this._entryName,
            (object) this._typeName,
            (object) this._subItems,
            (object) this._subEntries.Length
          });
          this._subItems = (ushort) this._subEntries.Length;
        }
        if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 128))
        {
          this._typeGuid = new Guid(span.Slice(start3, 16));
          start3 += 16;
        }
        if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 512))
        {
          this._copyMask = new byte[(int) this._size];
          span.Slice(start3, (int) this._size).CopyTo((Span<byte>) this._copyMask);
          start3 += (int) this._size;
        }
        if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 2048))
        {
          this._methodCount = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start3));
          int start20 = start3 + 2;
          start3 = start20 + SubStructureReader<AdsMethodEntry>.Unmarshal((uint) this._methodCount, marshaler, span.Slice(start20, entryLength - start20), out this._methods);
        }
        if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 4096))
        {
          this._attributeCount = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start3));
          int start21 = start3 + 2;
          start3 = start21 + SubStructureReader<AdsAttributeEntry>.Unmarshal((uint) this._attributeCount, marshaler, span.Slice(start21, entryLength - start21), out this._attributes);
        }
        if (((Enum) (object) this._flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 8192))
        {
          this._enumInfoCount = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(start3));
          int start22 = start3 + 2;
          start3 = start22 + EnumSubStructureReader<AdsEnumInfoEntry>.Unmarshal((uint) this._enumInfoCount, this._size, marshaler, span.Slice(start22, entryLength - start22), out this._enums);
        }
        if (start3 <= entryLength)
        {
          this._reserved = new byte[entryLength - start3];
          span.Slice(start3, entryLength - start3).CopyTo((Span<byte>) this._reserved);
          start3 = entryLength;
        }
        else if (start3 > entryLength)
        {
          string str = "Reading DataType entry for '" + this._entryName + "' failed because of mismatching entry size. Ignoring datatype!";
          AdsModule.Trace.TraceError(str);
          start3 = entryLength;
          flag = false;
        }
      }
      catch (Exception ex)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(69, 2);
        interpolatedStringHandler.AppendLiteral("Reading DataType entry for '");
        interpolatedStringHandler.AppendFormatted(this._entryName);
        interpolatedStringHandler.AppendLiteral("' failed (Exception: ");
        interpolatedStringHandler.AppendFormatted(ex.Message);
        interpolatedStringHandler.AppendLiteral(". Ignoring datatype!");
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        AdsModule.Trace.TraceError(stringAndClear);
        start3 = entryLength;
        flag = false;
      }
      return (flag, start3);
    }

    /// <summary>
    /// Determines whether this object is an SubItem (DataType Member, true) or a native DataType (false)
    /// </summary>
    /// <value><c>true</c> if this instance is sub item; otherwise, <c>false</c>.</value>
    public virtual bool IsSubItem => false;

    /// <summary>
    /// Gets a value indicating whether this instance has attributes.
    /// </summary>
    /// <value><c>true</c> if this instance has attributes; otherwise, <c>false</c>.</value>
    public bool HasAttributes => this._attributeCount > (ushort) 0;

    /// <summary>
    /// Gets a value indicating whether this instance has RPC methods.
    /// </summary>
    /// <value><c>true</c> if this instance has RPC methods; otherwise, <c>false</c>.</value>
    public bool HasRpcMethods => this._methodCount > (ushort) 0;

    /// <summary>
    /// Gets the size of the <see cref="T:TwinCAT.TypeSystem.IDataType" />
    /// </summary>
    /// <value>The size.</value>
    /// <remarks>If <see cref="P:TwinCAT.TypeSystem.IBitSize.IsBitType" /> indicates 'BitSize' then this value will be in Bits, otherwise Bytes.</remarks>
    public int Size => (int) this._size;

    /// <summary>
    /// Indicates, that the <see cref="P:TwinCAT.Ads.Internal.AdsDataTypeEntry.Size" /> and Offset values of the SubItems are in Bit size
    /// </summary>
    /// <value><c>true</c> if this instance is bit size; otherwise, <c>false</c>.</value>
    public bool IsBitType => (this._flags & 32) == 32;

    /// <summary>
    /// Indicates, that the <see cref="P:TwinCAT.Ads.Internal.AdsDataTypeEntry.Size" /> and Offset values of the SubItems are in Bit size
    /// </summary>
    /// <value><c>true</c> if this instance is bit size; otherwise, <c>false</c>.</value>
    internal bool IsAnySizeArray => (this._flags & 1048576) == 32;

    /// <summary>Gets the data type identifier.</summary>
    /// <value>The data type identifier.</value>
    public AdsDataTypeId DataTypeId => this._baseTypeId;

    /// <summary>Gets the name of the symbol.</summary>
    /// <value>Name of the symbol.</value>
    public string Name => this._typeName;

    /// <summary>Gets the comment behind the variable declaration.</summary>
    /// <value>Comment behind the variable declaration.</value>
    public string Comment => this._comment;

    /// <summary>
    /// Gets the attributes of the <see cref="T:TwinCAT.Ads.TypeSystem.IAdsSymbol" />
    /// </summary>
    /// <value>The attributes.</value>
    public ITypeAttributeCollection Attributes => this._attributes == null ? (ITypeAttributeCollection) new TypeAttributeCollection().AsReadOnly() : (ITypeAttributeCollection) new TypeAttributeCollection((IEnumerable<ITypeAttribute>) this._attributes).AsReadOnly();

    /// <summary>Gets the RPC method descriptions</summary>
    /// <value>The RPC methods.</value>
    public IRpcMethodCollection RpcMethods => (IRpcMethodCollection) new RpcMethodCollection(this._methods).AsReadOnly();

    /// <summary>
    /// Gets the name of the base type (if enum, alias, array)
    /// </summary>
    /// <value>The name of the base type.</value>
    public string BaseTypeName => this._typeName;

    /// <summary>Gets the dimensions of an array type</summary>
    /// <value>The array infos.</value>
    public IDimensionCollection? Dimensions => this._arrayInfos != null ? (IDimensionCollection) DimensionConverter.ToDimensionCollection(this._arrayInfos).AsReadOnly() : (IDimensionCollection) null;

    /// <summary>Gets the enum infos.</summary>
    /// <value>The enum infos.</value>
    public IEnumValueCollection? EnumInfos => this._enums != null ? (IEnumValueCollection) new EnumValueCollection(this._baseTypeId, this._enums).AsReadOnly() : (IEnumValueCollection) null;

    /// <summary>
    /// Gets the size of this <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" /> in bits.
    /// </summary>
    /// <value>The size of the bit.</value>
    public int BitSize => this.IsBitType ? (int) this._size : (int) this._size * 8;

    /// <summary>
    /// Gets the (aligned) size of of the Type/Instance in Bytes
    /// </summary>
    /// <value>The size of the byte.</value>
    public int ByteSize
    {
      get
      {
        int byteSize;
        if (this.IsBitType)
        {
          byteSize = this.BitSize / 8;
          if (this.BitSize % 8 > 0)
            ++byteSize;
        }
        else
          byteSize = (int) this._size;
        return byteSize;
      }
    }
  }
}
