// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.Symbol
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Symbol class</summary>
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.Instance" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IValueSymbol" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IValueAnySymbol" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IValueAccessorProvider" />
  /// <seealso cref="T:TwinCAT.TypeSystem.ISymbolFactoryServicesProvider" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IHierarchicalSymbol" />
  /// <seealso cref="T:TwinCAT.TypeSystem.ISymbolValueChangeNotify" />
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.IContextMaskProvider" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IInstanceInternal" />
  /// <seealso cref="T:TwinCAT.TypeSystem.ISymbolInternal" />
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.IAdsSymbol" />
  /// <remarks>A Symbol is a (named) memory object within the Process Image with a fixed address indicated by Index Group and Index Offset.
  /// Symbols can optionally be addressed by instance path and are bound to a specific <see cref="T:TwinCAT.Ads.TypeSystem.DataType" />.</remarks>
  [DebuggerDisplay("Path = { instancePath }, Type = {typeName}, Size = {size}, IG = {indexGroup}, IO = {indexOffset}, Category = {category}, Static = {staticAddress}")]
  public class Symbol : 
    Instance,
    IValueSymbol,
    IValueRawSymbol,
    IHierarchicalSymbol,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IValueAnySymbol,
    IValueAccessorProvider,
    ISymbolFactoryServicesProvider,
    ISymbolValueChangeNotify,
    IInstanceInternal,
    ISymbolInternal,
    IAdsSymbol,
    IProcessImageAddress,
    IContextMaskProvider,
    IAdsSymbolInternal
  {
    private AdsDataTypeFlags _memberFlags;
    /// <summary>The Symbol Factory Services</summary>
    private ISymbolFactoryServices factoryServices;
    private const uint TCOMOBJ_MIN_OID = 1048576;
    /// <summary>Notification Settings</summary>
    /// <value>The notification settings.</value>
    private INotificationSettings? _notificationSettings;
    /// <summary>The parent Symbol</summary>
    /// <exclude />
    private ISymbol? parent;
    /// <summary>The index group</summary>
    /// <exclude />
    private uint indexGroup;
    /// <summary>The index offset</summary>
    /// <exclude />
    private uint indexOffset;
    /// <summary>The instance path</summary>
    /// <exclude />
    private string instancePath = string.Empty;
    /// <summary>The image base address</summary>
    /// <exclude />
    private AmsAddress? imageBaseAddress;
    /// <summary>Weak reference to SubSymbols</summary>
    internal WeakReference<ISymbolCollection<ISymbol>>? subSymbols;
    /// <summary>RawValueChanged delegate</summary>
    private EventHandler<RawValueChangedEventArgs>? _rawValueChanged;
    /// <summary>Synchronization object</summary>
    protected object syncObject = new object();
    /// <summary>ValueChanged delegate.</summary>
    private EventHandler<ValueChangedEventArgs>? _valueChanged;
    /// <summary>The access rights</summary>
    /// <exclude />
    private SymbolAccessRights accessRights = (SymbolAccessRights) 7;

    internal Symbol(AdsSymbolEntry entry, ISymbol? parent, ISymbolFactoryServices factoryServices)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      this.factoryServices = factoryServices != null ? factoryServices : throw new ArgumentNullException(nameof (factoryServices));
      this.indexGroup = entry.IndexGroup;
      this.indexOffset = entry.IndexOffset;
      this.SetComment(entry.Comment);
      this.DataTypeId = entry.DataTypeId;
      this.Category = (DataTypeCategory) 0;
      this.parent = parent;
      this.InstanceName = this.getInstanceName(entry.Name);
      this.instancePath = this.getInstancePath(entry.Name);
      this.Size = (int) entry.Size;
      this.TypeName = entry.TypeName;
      this.DataType = (IDataType) null;
      this.Flags = entry.Flags;
      if (entry.AttributeCount <= (ushort) 0)
        return;
      this.SetAttributes(new TypeAttributeCollection((IEnumerable<ITypeAttribute>) entry.Attributes));
    }

    /// <summary>
    /// Called when the <see cref="T:TwinCAT.Ads.TypeSystem.Symbol" /> is bound.
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void OnBound(IBinder binder)
    {
      if (!(binder is IAdsBinder adsBinder))
        return;
      this.imageBaseAddress = adsBinder.ImageBaseAddress;
    }

    /// <summary>Sets a new InstanceName InstancePath</summary>
    /// <param name="instanceName">Instance name.</param>
    protected override void OnSetInstanceName(string instanceName)
    {
      string instanceName1 = this.InstanceName;
      this.InstanceName = instanceName;
      this.instancePath = this.instancePath.Substring(0, this.instancePath.LastIndexOf('.') + 1) + instanceName;
    }

    private string getInstanceName(string entryName)
    {
      int num = entryName.LastIndexOf('.');
      return num < 0 ? entryName : entryName.Substring(num + 1, entryName.Length - (num + 1));
    }

    private string getInstancePath(string entryName) => entryName;

    internal Symbol(
      AdsSymbolEntry entry,
      IDataType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : this(entry, parent, factoryServices)
    {
      this.Category = type.Category;
      this.DataType = type;
      if (this.Attributes == null || ((ICollection<ITypeAttribute>) this.Attributes).Count < 2)
        return;
      ISubRangeType subRange = (ISubRangeType) null;
      if (!Symbol.TryParseSubRange(type, this.Attributes, factoryServices.Binder, out subRange))
        return;
      this.Category = ((IDataType) subRange).Category;
      this.DataType = (IDataType) subRange;
    }

    private static bool TryParseSubRange(
      IDataType baseType,
      ITypeAttributeCollection attributes,
      IBinder binder,
      [NotNullWhen(true)] out ISubRangeType? subRange)
    {
      string str1;
      string str2;
      if (attributes != null && ((ICollection<ITypeAttribute>) attributes).Count >= 2 && baseType.Category == 1 && attributes.TryGetValue("LowerBorder", ref str1) & attributes.TryGetValue("UpperBorder", ref str2))
      {
        Type managedType = ((IManagedMappableType) baseType).ManagedType;
        if (managedType != (Type) null)
        {
          Type type = managedType;
          if (managedType == typeof (byte))
            type = typeof (sbyte);
          else if (managedType == typeof (ushort))
            type = typeof (short);
          else if (managedType == typeof (uint))
            type = typeof (int);
          else if (managedType == typeof (ulong))
            type = typeof (long);
          object val1;
          object val2;
          if (DataTypeStringParser.TryParse(str2, type, out val1) & DataTypeStringParser.TryParse(str1, type, out val2))
          {
            object val3;
            object val4;
            if (managedType != type)
            {
              PrimitiveTypeMarshaler primitiveTypeMarshaler = PrimitiveTypeMarshaler.Default;
              byte[] numArray1 = new byte[((IBitSize) baseType).Size];
              byte[] numArray2 = new byte[((IBitSize) baseType).Size];
              primitiveTypeMarshaler.Marshal(val2, (Span<byte>) numArray1);
              primitiveTypeMarshaler.Marshal(val1, (Span<byte>) numArray2);
              primitiveTypeMarshaler.Unmarshal(managedType, (ReadOnlySpan<byte>) numArray1, primitiveTypeMarshaler.DefaultValueEncoding, out val3);
              primitiveTypeMarshaler.Unmarshal(managedType, (ReadOnlySpan<byte>) numArray2, primitiveTypeMarshaler.DefaultValueEncoding, out val4);
            }
            else
            {
              val3 = val2;
              val4 = val1;
            }
            string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1}..{2})", (object) baseType.Name, val3, val4);
            IDataType idataType = (IDataType) null;
            if (((IDataTypeResolver) binder).TryResolveType(str3, ref idataType))
            {
              subRange = (ISubRangeType) idataType;
              return subRange != null;
            }
            string str4 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot resolve RangeTypeString '{0}'!", (object) str3);
            AdsModule.Trace.TraceWarning(str4);
          }
        }
      }
      subRange = (ISubRangeType) null;
      return false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Symbol" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    /// <param name="factoryServices">The factory services.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Symbol(
      string instanceName,
      IDataType type,
      ISymbol parent,
      int fieldOffset,
      ISymbolFactoryServices factoryServices)
      : this(instanceName, (string) null, 0U, 0U, type, parent, factoryServices)
    {
      DataType dataType = (DataType) type;
      this.calcAccess(parent, (DataType) type, fieldOffset, DataTypeFlagConverter.Convert(dataType.Flags), dataType.Flags, out this.indexGroup, out this.indexOffset);
      DataTypeCategory category = this.Category;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Symbol(
      string instanceName,
      string? instancePath,
      uint indexGroup,
      uint indexOffset,
      IDataType? type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
    {
      if (factoryServices == null)
        throw new ArgumentNullException(nameof (factoryServices));
      if (string.IsNullOrEmpty(instanceName))
        throw new ArgumentOutOfRangeException(nameof (instanceName));
      this.factoryServices = factoryServices;
      this.parent = parent;
      this.Bind(factoryServices.Binder);
      this.indexGroup = indexGroup;
      this.indexOffset = indexOffset;
      if (type != null)
      {
        this.DataTypeId = ((DataType) type).DataTypeId;
        this.Category = type.Category;
      }
      this.InstanceName = instanceName;
      this.instancePath = instancePath ?? string.Empty;
      if (string.IsNullOrEmpty(this.instancePath))
      {
        if (parent != null)
        {
          ((DataType) type)?.ResolveType((DataTypeResolveStrategy) 1);
          IDataType idataType = ((DataType) ((IInstance) parent).DataType)?.ResolveType((DataTypeResolveStrategy) 1);
          if (idataType != null && idataType.IsPointer)
          {
            this.InstanceName = ((IInstance) parent).InstanceName + "^";
            this.instancePath = ((IInstance) parent).InstancePath + "^";
          }
          else
            this.instancePath = idataType == null || !idataType.IsReference ? ((IInstance) parent).InstancePath + "." + instanceName : ((IInstance) parent).InstancePath;
        }
        else
          this.instancePath = instanceName;
      }
      this.SetContextMask((byte) 0);
      if (parent != null)
        this.SetContextMask(((IContextMaskProvider) parent).ContextMask);
      if (type != null)
      {
        this.Flags = DataTypeFlagConverter.Convert(((DataType) type).Flags);
        this.Size = ((IBitSize) type).Size;
        this.TypeName = type.Name;
      }
      this.DataType = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Symbol" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="factoryServices">The factory services.</param>
    internal Symbol(string instanceName, ISymbol parent, ISymbolFactoryServices factoryServices)
      : this(instanceName, (string) null, 0U, 0U, (IDataType) null, parent, factoryServices)
    {
      DataTypeCategory category = this.Category;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Symbol" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="factoryServices">The factory services.</param>
    internal Symbol(
      string instanceName,
      string instancePath,
      ISymbolFactoryServices factoryServices)
      : this(instanceName, instancePath, 0U, 0U, (IDataType) null, (ISymbol) null, factoryServices)
    {
      DataTypeCategory category = this.Category;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Symbol" /> class which represents an instance of <see cref="T:TwinCAT.Ads.TypeSystem.Member" />
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal Symbol(Member member, ISymbol parent)
    {
      if (member == null)
        throw new ArgumentNullException(nameof (member));
      this.factoryServices = parent != null ? ((ISymbolFactoryServicesProvider) parent).FactoryServices : throw new ArgumentNullException(nameof (parent));
      this.parent = parent;
      this.Bind(((IBinderProvider) parent).Binder);
      string instanceName = member.InstanceName;
      this.InstanceName = instanceName;
      this.instancePath = ((IInstance) parent).InstancePath + "." + instanceName;
      this.DataType = member.DataType;
      this.Flags = this.getSymbolFlags(parent, (Field) member);
      this._memberFlags = member.MemberFlags;
      this.IsStatic = ((IInstance) parent).IsStatic || ((Enum) (object) this._memberFlags).HasFlag((Enum) (object) (AdsDataTypeFlags) 131072);
      if (((ICollection<ITypeAttribute>) member.Attributes).Count > 0)
        this.SetAttributes(new TypeAttributeCollection((IEnumerable<ITypeAttribute>) member.Attributes));
      else
        this.SetAttributes((TypeAttributeCollection) null);
      this.SetComment(member.Comment);
      if (this.DataType != null)
      {
        this.Size = ((IBitSize) this.DataType).Size;
        this.DataTypeId = ((DataType) this.DataType).DataTypeId;
        this.Category = this.DataType.Category;
        this.TypeName = this.DataType.Name;
      }
      else
      {
        this.Size = member.Size;
        this.DataTypeId = member.DataTypeId;
        this.Category = member.Category;
        this.TypeName = member.TypeName;
      }
      this.calcAccess(parent, member, out this.indexGroup, out this.indexOffset);
      this.SetContextMask(((IContextMaskProvider) parent).ContextMask);
      DataTypeCategory category = this.Category;
    }

    /// <summary>Gets the symbol flags.</summary>
    /// <param name="structParent">The structure parent.</param>
    /// <param name="subSymbol">The sub symbol.</param>
    /// <returns>AdsSymbolFlags.</returns>
    private AdsSymbolFlags getSymbolFlags(ISymbol structParent, Field subSymbol) => (AdsSymbolFlags) (subSymbol.Flags | ((ISymbolFlagProvider) Symbol.Unwrap(structParent)).Flags & 1);

    /// <summary>Gets the symbol flags.</summary>
    /// <param name="arrayParent">The array parent.</param>
    /// <param name="elementType">Type of the element.</param>
    /// <returns>AdsSymbolFlags.</returns>
    private AdsSymbolFlags getSymbolFlags(ISymbol arrayParent, DataType? elementType)
    {
      AdsSymbolFlags adsSymbolFlags = (AdsSymbolFlags) 0;
      ArrayType arrayType = (ArrayType) ((DataType) ((IInstance) arrayParent).DataType)?.ResolveType((DataTypeResolveStrategy) 1);
      if (elementType != null)
        adsSymbolFlags = DataTypeFlagConverter.Convert(elementType.Flags);
      else if (arrayType != null && arrayType.ElementTypeId == 33)
        adsSymbolFlags = (AdsSymbolFlags) (adsSymbolFlags | 2);
      ISymbolFlagProvider symbolFlagProvider = (ISymbolFlagProvider) Symbol.Unwrap(arrayParent);
      return (AdsSymbolFlags) (adsSymbolFlags | symbolFlagProvider.Flags & 1);
    }

    internal AdsDataTypeFlags MemberFlags => this._memberFlags;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Symbol" /> class.
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="oversampleElement">Indicates, that is Symbol is the virtual oversampling element.</param>
    /// <param name="parent">The parent (Can be IArrayInstance or IAliasInstance)</param>
    internal Symbol(int[] indices, bool oversampleElement, ISymbol parent)
    {
      this.factoryServices = parent != null ? ((ISymbolFactoryServicesProvider) parent).FactoryServices : throw new ArgumentNullException(nameof (parent));
      this.parent = parent;
      this.Bind(((IBinderProvider) parent).Binder);
      string empty = string.Empty;
      ArrayType type = (ArrayType) ((DataType) ((IInstance) parent).DataType)?.ResolveType((DataTypeResolveStrategy) 1);
      if (type == null)
        throw new CannotResolveDataTypeException(((IInstance) parent).TypeName);
      type.CheckIndices(indices, oversampleElement);
      string str = !oversampleElement ? ArrayIndexConverter.IndicesToString(indices) : ArrayIndexConverter.OversamplingSubElementToString(type.Dimensions.ElementCount);
      this.InstanceName = ((IInstance) parent).InstanceName + str;
      this.instancePath = ((IInstance) parent).InstancePath + str;
      DataType elementType = (DataType) type.ElementType;
      this.DataType = (IDataType) elementType;
      this.Flags = this.getSymbolFlags(parent, elementType);
      if (this.DataType != null)
      {
        this.Size = ((IBitSize) this.DataType).Size;
        this.DataTypeId = ((DataType) this.DataType).DataTypeId;
        this.Category = this.DataType.Category;
        this.TypeName = this.DataType.Name;
      }
      else
      {
        if (this.IsBitType)
          this.Size = 1;
        else
          this.Size = type.ElementSize;
        this.DataTypeId = type.ElementTypeId;
        this.Category = (DataTypeCategory) 0;
        this.TypeName = type.ElementTypeName;
      }
      if (oversampleElement)
      {
        this.indexGroup = 61465U;
        this.indexOffset = 0U;
      }
      else
      {
        int elementOffset = ArrayType.GetElementOffset(indices, (IArrayType) type);
        if (elementType != null)
          this.calcAccess(parent, elementType, elementOffset, this.Flags, elementType.Flags, out this.indexGroup, out this.indexOffset);
        else
          this.calcAccess(parent, elementType, elementOffset, this.Flags, (AdsDataTypeFlags) 0, out this.indexGroup, out this.indexOffset);
      }
      this.SetContextMask(((IContextMaskProvider) parent).ContextMask);
      DataTypeCategory category = this.Category;
    }

    /// <summary>Equals</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj != null && !(this.GetType() != obj.GetType()) && string.Equals(this.instancePath, ((Instance) obj).InstancePath, StringComparison.OrdinalIgnoreCase);

    /// <summary>Operator==</summary>
    /// <param name="o1">The o1.</param>
    /// <param name="o2">The o2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(Symbol? o1, Symbol? o2)
    {
      if (!object.Equals((object) o1, (object) null))
        return o1.Equals((object) o2);
      return object.Equals((object) o2, (object) null);
    }

    /// <summary>Implements the != operator.</summary>
    /// <param name="o1">The o1.</param>
    /// <param name="o2">The o2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(Symbol? o1, Symbol? o2) => !(o1 == o2);

    /// <summary>Gets the HashCode of the Address</summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => 91 * 10 + this.instancePath.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>Gets the factory services.</summary>
    /// <value>The factory services.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ISymbolFactoryServices FactoryServices => this.factoryServices;

    /// <summary>Gets the value accessor.</summary>
    /// <value>The value accessor.</value>
    /// <exclude />
    public IAccessorRawValue? ValueAccessor => this.factoryServices is ISymbolFactoryValueServices ? ((ISymbolFactoryValueServices) this.factoryServices).ValueAccessor : (IAccessorRawValue) null;

    /// <summary>
    /// Gets the connection that produces values for this <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <value>The connection object.</value>
    public IConnection? Connection => this.ValueAccessor is IAccessorConnection valueAccessor ? valueAccessor.Connection : (IConnection) null;

    /// <summary>Calculates the access.</summary>
    /// <param name="parent">The parent.</param>
    /// <param name="member">The member.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    private void calcAccess(
      ISymbol parent,
      Member member,
      out uint indexGroup,
      out uint indexOffset)
    {
      DataType dataType = (DataType) member.DataType;
      int offset = member.Offset;
      AdsSymbolFlags flags = member.Flags;
      AdsDataTypeFlags memberFlags = member.MemberFlags;
      this.calcAccess(parent, dataType, offset, flags, memberFlags, out indexGroup, out indexOffset);
    }

    private void calcAccess(
      ISymbol parent,
      DataType? memberType,
      int offset,
      AdsSymbolFlags symbolFlags,
      AdsDataTypeFlags flags,
      out uint indexGroup,
      out uint indexOffset)
    {
      ISymbol parent1 = Symbol.Unwrap(parent);
      bool flag = false;
      if (memberType != null)
        flag = memberType.IsBitType;
      if (((Enum) (object) flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 131072))
      {
        indexGroup = 61465U;
        indexOffset = 0U;
      }
      else if (((IBitSize) parent1).IsBitType | flag)
        this.calcBitAccess(parent1, offset, symbolFlags, flags, out indexGroup, out indexOffset);
      else if (this.IsDereferencedPointer)
      {
        indexGroup = !flag ? 61460U : 61466U;
        indexOffset = 0U;
      }
      else if (this.IsReference || this.IsDereferencedReference)
      {
        indexGroup = !flag ? 61462U : 61467U;
        indexOffset = 0U;
      }
      else
      {
        indexGroup = ((IProcessImageAddress) parent1).IndexGroup;
        indexOffset = ((IProcessImageAddress) parent1).IndexOffset + (uint) offset;
      }
    }

    /// <summary>
    /// (Re)Calculates the IndexGroup and Index Offset from Byte/Access To BitAccess or for TCom Pid masking
    /// </summary>
    /// <param name="parent">The parent Symbol</param>
    /// <param name="member">The member.</param>
    /// <param name="indexGroup">The index group result</param>
    /// <param name="indexOffset">The index offset result</param>
    private void calcBitAccess(
      ISymbol parent,
      Member member,
      out uint indexGroup,
      out uint indexOffset)
    {
      DataType dataType = (DataType) member.DataType;
      int offset = member.Offset;
      AdsSymbolFlags flags1 = member.Flags;
      AdsDataTypeFlags flags2 = (AdsDataTypeFlags) 0;
      if (dataType != null)
        flags2 = dataType.Flags;
      this.calcBitAccess(parent, offset, flags1, flags2, out indexGroup, out indexOffset);
    }

    private void calcBitAccess(
      ISymbol parent,
      int bitOffset,
      AdsSymbolFlags symbolFlags,
      AdsDataTypeFlags flags,
      out uint indexGroup,
      out uint indexOffset)
    {
      ISymbol isymbol = Symbol.Unwrap(parent);
      uint num1 = ((IProcessImageAddress) isymbol).IndexGroup;
      uint indexOffset1 = ((IProcessImageAddress) isymbol).IndexOffset;
      uint num2;
      if (this.IsDereferencedPointer)
      {
        num1 = 61466U;
        num2 = 0U;
      }
      else if (this.IsReference || this.IsDereferencedReference)
      {
        num1 = 61467U;
        num2 = 0U;
      }
      else
      {
        if (num1 <= 16416U)
        {
          if (num1 != 16384U && num1 != 16400U && num1 != 16416U)
            goto label_10;
        }
        else if (num1 <= 16448U)
        {
          if (num1 != 16432U && num1 != 16448U)
            goto label_10;
        }
        else if (num1 != 61472U && num1 != 61488U)
          goto label_10;
        ++num1;
        num2 = (uint) ((ulong) (indexOffset1 * 8U) + (ulong) bitOffset);
        goto label_11;
label_10:
        num2 = num1 <= 1048576U ? indexOffset1 + (uint) bitOffset : Symbol.calcPidBitAddressing(indexOffset1, bitOffset);
      }
label_11:
      indexGroup = num1;
      indexOffset = num2;
    }

    private static uint calcPidBitAddressing(uint indexOffset, int offset)
    {
      if (indexOffset <= 1048576U)
        throw new ArgumentOutOfRangeException(nameof (indexOffset));
      indexOffset = (uint) (-1073741824 | 1056964608 & (int) ((indexOffset & 1056964608U) >> 24) << 24 | 16777215 & ((int) indexOffset & 16777215) * 8 + offset);
      return indexOffset;
    }

    /// <summary>Gets or sets the notification settings.</summary>
    /// <value>The notification settings.</value>
    /// <remarks>The <see cref="P:TwinCAT.Ads.TypeSystem.Symbol.NotificationSettings" /> will be inherited from <see cref="P:TwinCAT.Ads.TypeSystem.Symbol.Parent" /> if the setting is not overwritten.
    /// If the Root Symbol also doesn't contain the settings, then the <see cref="P:TwinCAT.Ads.TypeSystem.IAdsSymbolLoader.DefaultNotificationSettings" /> will be returned.</remarks>
    public INotificationSettings? NotificationSettings
    {
      get
      {
        if (this._notificationSettings != null)
          return this._notificationSettings;
        INotificationSettings notificationSettings = (INotificationSettings) null;
        if (this.Parent is IValueSymbol parent)
          notificationSettings = parent.NotificationSettings;
        if (notificationSettings != null)
          return notificationSettings;
        return this.ValueAccessor is IAccessorNotification valueAccessor ? valueAccessor.DefaultNotificationSettings : (INotificationSettings) TwinCAT.Ads.NotificationSettings.Default;
      }
      set => this._notificationSettings = value;
    }

    /// <summary>Gets the parent Symbol</summary>
    /// <value>The parent.</value>
    public ISymbol? Parent => this.parent;

    public void SetParent(ISymbol? parent) => this.parent = parent;

    /// <summary>Gets the index group of the Symbol</summary>
    /// <value>The index group.</value>
    public uint IndexGroup => this.indexGroup;

    /// <summary>Gets the index offset of the Symbol</summary>
    /// <value>The index offset.</value>
    public uint IndexOffset => this.indexOffset;

    /// <summary>
    /// Gets the relative / absolute access path to the instance (with periods (.))
    /// </summary>
    /// <value>The instance path.</value>
    /// <remarks>
    /// If this path is relative or absolute depends on the context. <see cref="T:TwinCAT.TypeSystem.IMember" /> are using relative paths, <see cref="T:TwinCAT.TypeSystem.ISymbol" />s are using absolute ones.
    /// </remarks>
    public override string InstancePath => this.instancePath;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      string str;
      if (!this.IsBitType)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} (IG: 0x{1}, IO: 0x{2}, Size: {3} bytes)'", (object) this.InstancePath, (object) this.IndexGroup.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.IndexOffset.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.ByteSize);
      else
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} (IG: 0x{1}, IO: 0x{2}, Size: {3} bits)", (object) this.InstancePath, (object) this.IndexGroup.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.IndexOffset.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.BitSize);
      return str;
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.Ads.AmsAddress" /> of the Process Image
    /// </summary>
    /// <value>The address.</value>
    public AmsAddress? ImageBaseAddress => this.imageBaseAddress;

    /// <summary>Creates the sub symbols collection.</summary>
    internal virtual ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentSymbol)
    {
      return (ISymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 0);
    }

    internal virtual int OnGetSubSymbolCount(ISymbol parentSymbol) => !this.IsContainerType ? 0 : ((ICollection<ISymbol>) this.SubSymbols).Count;

    /// <summary>
    /// Gets the SubSymbols of the <see cref="T:TwinCAT.TypeSystem.ISymbol" />
    /// </summary>
    /// <remarks>
    /// Used for Array, Struct, Pointer and Reference instances. Otherwise empty
    /// </remarks>
    public ISymbolCollection<ISymbol> SubSymbols => (ISymbolCollection<ISymbol>) new ReadOnlySymbolCollection((IInstanceCollection<ISymbol>) ((ISymbolInternal) this).SubSymbolsInternal);

    /// <summary>Gets the SubSymbols Collection (internal variant)</summary>
    /// <value>The sub symbols internal.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ISymbolCollection<ISymbol> SubSymbolsInternal => this.OnGetSubSymbols();

    /// <summary>Handler function getting the SubSymbols</summary>
    /// <returns>ISymbolCollection.</returns>
    /// <remarks>
    /// The default case is, that the SubSymbols are cached via WeakReferences
    /// </remarks>
    /// <exclude />
    protected virtual ISymbolCollection<ISymbol> OnGetSubSymbols()
    {
      ISymbolCollection<ISymbol> target = (ISymbolCollection<ISymbol>) null;
      if (this.subSymbols == null)
        this.subSymbols = new WeakReference<ISymbolCollection<ISymbol>>(this.OnCreateSubSymbols((ISymbol) this));
      this.subSymbols.TryGetTarget(out target);
      if (target == null)
      {
        target = this.OnCreateSubSymbols((ISymbol) this);
        this.subSymbols.SetTarget(target);
      }
      return target;
    }

    /// <summary>Gets the number of SubSymbols</summary>
    /// <value>The Number of SubSymbols.</value>
    /// <remarks>If the <see cref="P:TwinCAT.Ads.TypeSystem.Symbol.SubSymbols" /> collection is not generated yet (WeakReference),
    /// then this method is less memory and cpu consuming to use for just determining the
    /// the number of child symbols (instead of using SubSymbols.Count)/&gt;</remarks>
    public int SubSymbolCount => this.OnGetSubSymbolCount();

    /// <summary>
    /// Handler function determining the SubSymbolCode (optimized)
    /// </summary>
    /// <returns>System.Int32.</returns>
    /// <remarks>If the SubSymbols WeakReference is avaliable, we take that one for determining the Count as optimization.
    /// </remarks>
    /// <exclude />
    protected virtual int OnGetSubSymbolCount()
    {
      ISymbolCollection<ISymbol> target = (ISymbolCollection<ISymbol>) null;
      if (this.subSymbols != null)
        this.subSymbols.TryGetTarget(out target);
      return target != null ? ((ICollection<ISymbol>) target).Count : this.OnGetSubSymbolCount((ISymbol) this);
    }

    /// <summary>
    /// Gets a value indicating whether [sub symbols created].
    /// </summary>
    /// <value><c>true</c> if [sub symbols created]; otherwise, <c>false</c>.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool SubSymbolsCreated
    {
      get
      {
        ISymbolCollection<ISymbol> target = (ISymbolCollection<ISymbol>) null;
        return this.subSymbols != null && this.subSymbols.TryGetTarget(out target);
      }
    }

    /// <summary>Creates the sub symbols.</summary>
    /// <param name="parent">The parent.</param>
    /// <returns>SymbolCollection.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ISymbolCollection<ISymbol> CreateSubSymbols(ISymbol parent)
    {
      ISymbolCollection<ISymbol> target = (ISymbolCollection<ISymbol>) null;
      if (this.subSymbols == null)
        this.subSymbols = new WeakReference<ISymbolCollection<ISymbol>>(this.OnCreateSubSymbols(parent));
      this.subSymbols.TryGetTarget(out target);
      if (target == null)
      {
        target = this.OnCreateSubSymbols(parent);
        this.subSymbols.SetTarget(target);
      }
      return target;
    }

    /// <summary>
    /// Gets or sets a value indicating whether an ancestor is a dereferenced Reference
    /// </summary>
    /// <value><c>true</c> if this instance is ancestor is reference; otherwise, <c>false</c>.</value>
    public bool IsDereferencedReference
    {
      get
      {
        for (ISymbol isymbol = (ISymbol) this; isymbol.Parent != null; isymbol = isymbol.Parent)
        {
          if (((IInstance) isymbol.Parent).IsReference)
            return true;
        }
        return false;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether an ancestor is a dereferenced Pointer
    /// </summary>
    /// <value><c>true</c> if this instance is ancestor is pointer; otherwise, <c>false</c>.</value>
    public bool IsDereferencedPointer
    {
      get
      {
        for (ISymbol isymbol = (ISymbol) this; isymbol.Parent != null; isymbol = isymbol.Parent)
        {
          if (((IInstance) isymbol.Parent).IsPointer)
            return true;
        }
        return false;
      }
    }

    /// <summary>
    /// Reads the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <returns>System.Byte[].</returns>
    /// <value>The raw value.</value>
    public byte[] ReadRawValue() => this.OnReadRawValue(-1);

    /// <summary>
    /// Reads the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <returns>System.Byte[].</returns>
    /// <value>The raw value.</value>
    public byte[] ReadRawValue(int timeout) => this.OnReadRawValue(-1);

    /// <summary>
    /// Reads the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write) asynchronously.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>System.Byte[].</returns>
    /// <value>The raw value.</value>
    public Task<ResultReadRawAccess> ReadRawValueAsync(
      CancellationToken cancel)
    {
      return this.OnReadRawValueAsync(cancel);
    }

    /// <summary>
    /// Writes the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <param name="rawValue">The value as byte array.</param>
    /// <value>The value.</value>
    public void WriteRawValue(byte[] rawValue) => this.OnWriteRawValue((ReadOnlyMemory<byte>) rawValue, -1);

    /// <summary>
    /// Writes the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <param name="rawValue">The value as byte array.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultRead" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="P:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution..
    /// </returns>
    public Task<ResultWriteAccess> WriteRawValueAsync(
      byte[] rawValue,
      CancellationToken cancel)
    {
      return this.OnWriteRawValueAsync((ReadOnlyMemory<byte>) rawValue, cancel);
    }

    /// <summary>
    /// Writes the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <param name="rawValue">The raw value.</param>
    /// <param name="timeout">The timeout.</param>
    /// <value>The raw value.</value>
    /// <remarks>A negative timeout indicates that the Default Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public void WriteRawValue(byte[] rawValue, int timeout) => this.OnWriteRawValue((ReadOnlyMemory<byte>) rawValue, timeout);

    /// <summary>Handler function for writing the RawValue</summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    protected virtual void OnWriteRawValue(ReadOnlyMemory<byte> value, int timeout)
    {
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException();
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      IAccessorConnection valueAccessor = this.ValueAccessor as IAccessorConnection;
      int num = 0;
      DateTimeOffset? nullable;
      if (timeout >= 0 && valueAccessor != null)
      {
        if (valueAccessor.Connection == null)
          throw new CannotAccessValueException();
        using (new AdsTimeoutSetter(valueAccessor.Connection, timeout))
          num = this.ValueAccessor.TryWriteRaw((ISymbol) this, value, ref nullable);
      }
      else
        num = this.ValueAccessor.TryWriteRaw((ISymbol) this, value, ref nullable);
      if (num != 0)
        throw new SymbolException(string.Format((IFormatProvider) null, "Cannot write RawValue of Symbol '{0}'! Error: {1}", (object) this.InstancePath, (object) num), (ISymbol) this);
    }

    /// <summary>
    /// Handler function Writing the raw value asynchronously.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultWriteAccess&gt;.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual Task<ResultWriteAccess> OnWriteRawValueAsync(
      ReadOnlyMemory<byte> value,
      CancellationToken cancel)
    {
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException();
      return !this.HasValue ? Task.FromResult<ResultWriteAccess>(new ResultWriteAccess(1796, 0U)) : this.ValueAccessor.WriteRawAsync((ISymbol) this, value, cancel);
    }

    /// <summary>Handler function for reading the raw value</summary>
    /// <returns>System.Byte[].</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException"></exception>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException"></exception>
    protected virtual byte[] OnReadRawValue(int timeout)
    {
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!this.HasValue)
        return Array.Empty<byte>();
      byte[] array = new byte[this.ByteSize];
      int num = 0;
      IAccessorConnection iaccessorConnection = valueAccessor as IAccessorConnection;
      DateTimeOffset? nullable;
      if (timeout >= 0 && iaccessorConnection != null)
      {
        if (iaccessorConnection.Connection == null)
          throw new CannotAccessValueException((ISymbol) this);
        using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
          num = ((IAccessorRawValue) valueAccessor).TryReadRaw((ISymbol) this, array.AsMemory<byte>(), ref nullable);
      }
      else
        num = ((IAccessorRawValue) valueAccessor).TryReadRaw((ISymbol) this, array.AsMemory<byte>(), ref nullable);
      if (num != 0)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot read RawValue of Symbol '{0}'! Error: {1}", (object) this.InstancePath, (object) num), (ISymbol) this);
      return array;
    }

    /// <summary>Handler function for reading the raw value</summary>
    /// <returns>System.Byte[].</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException"></exception>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException"></exception>
    protected virtual Task<ResultReadRawAccess> OnReadRawValueAsync(
      CancellationToken cancel)
    {
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException();
      return !this.HasValue ? Task.FromResult<ResultReadRawAccess>(ResultReadRawAccess.Empty) : this.ValueAccessor.ReadRawAsync((ISymbol) this, new byte[this.ByteSize].AsMemory<byte>(), cancel);
    }

    /// <summary>
    /// Occurs when the RawValue of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> has changed.
    /// </summary>
    public event EventHandler<RawValueChangedEventArgs>? RawValueChanged
    {
      add
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          bool flag = false;
          if (this._rawValueChanged == null || this._rawValueChanged.GetInvocationList().Length == 0)
            flag = true;
          this._rawValueChanged += value;
          if (!flag || !(this.ValueAccessor is IAccessorNotification valueAccessor))
            return;
          valueAccessor.OnRegisterNotification((ISymbol) this, (SymbolNotificationTypes) 2, this.NotificationSettings);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
      remove
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          this._rawValueChanged -= value;
          if (this._rawValueChanged != null && this._rawValueChanged.GetInvocationList().Length != 0 || !(this.ValueAccessor is IAccessorNotification valueAccessor))
            return;
          valueAccessor.OnUnregisterNotification((ISymbol) this, (SymbolNotificationTypes) 2);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
    }

    /// <summary>
    /// Occurs when the (Primitive) value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> has changed.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs>? ValueChanged
    {
      add
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          bool flag = false;
          if (this._valueChanged == null || this._valueChanged.GetInvocationList().Length == 0)
            flag = true;
          this._valueChanged += value;
          if (!flag || (this.accessRights & 1) <= 0 || !(this.ValueAccessor is IAccessorNotification valueAccessor))
            return;
          valueAccessor.OnRegisterNotification((ISymbol) this, (SymbolNotificationTypes) 1, this.NotificationSettings);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
      remove
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          this._valueChanged -= value;
          if (this._valueChanged != null && this._valueChanged.GetInvocationList().Length != 0 || !(this.ValueAccessor is IAccessorNotification valueAccessor))
            return;
          valueAccessor.OnUnregisterNotification((ISymbol) this, (SymbolNotificationTypes) 1);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
    }

    /// <summary>Gets the access rights.</summary>
    /// <value>The access rights.</value>
    public SymbolAccessRights AccessRights => this.accessRights;

    /// <summary>
    /// Reads the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <returns>System.Object.</returns>
    /// <value>The value.</value>
    /// <remarks>Calling on primitive types, a call of this method will return the primitive value.
    /// On complex types (structures and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: the raw byte Array will be returned,
    /// in dynamic mode: A Value will be created on the fly.</remarks>
    public object ReadValue() => this.OnReadValue(-1);

    /// <summary>
    /// Reads the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <param name="timeout">The timeout in ms.</param>
    /// <returns>System.Object.</returns>
    /// <value>The value.</value>
    /// <remarks>Calling on primitive types, a call of this method will return the primitive value.
    /// On complex types (structures and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: the raw byte Array will be returned,
    /// in dynamic mode: A Value will be created on the fly.
    /// A negative timeout indicates that the Default Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public object ReadValue(int timeout) => this.OnReadValue(timeout);

    /// <summary>
    /// Reads the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <param name="timeout">The timeout in ms.</param>
    /// <param name="value">The read value.</param>
    /// <returns>The error Code..</returns>
    /// <remarks>Calling on primitive types, a call of this method will return the primitive value.
    /// On complex types (structures and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: the raw byte Array will be returned,
    /// in dynamic mode: A Value will be created on the fly.
    /// A negative timeout indicates that the Default Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public int TryReadValue(int timeout, out object? value) => this.OnTryReadValue(timeout, out value);

    /// <summary>
    /// Reads the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> asynchronously.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A tasks that represents the asynchronous 'ReadValue' operation. The read result is stored in the <see cref="T:TwinCAT.ValueAccess.ResultReadValueAccess" /> return value and contains
    /// the <see cref="P:TwinCAT.ValueAccess.ResultReadValueAccess`1.Value" /> and the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" />.</returns>
    /// <remarks>Calling on primitive types, a call of this method will return the primitive value.
    /// On complex types (structures and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: the raw byte Array will be returned,
    /// in dynamic mode: A Value will be created on the fly.</remarks>
    public Task<ResultReadValueAccess> ReadValueAsync(
      CancellationToken cancel)
    {
      return this.OnReadValueAsync(cancel);
    }

    /// <summary>
    /// Writes the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>Calling on primitive types, a call of this method will directly write this Value.
    /// On complex types (structs and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: Only byte Arrays (of correct size) can be written)
    /// in dynamic mode: A Value that represents the value will be accepted also.</remarks>
    public void WriteValue(object value) => this.OnWriteValue(value, -1);

    /// <summary>
    /// Writes the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout in ms.</param>
    /// <returns>The error code.</returns>
    /// <remarks>Calling on primitive types, a call of this method will directly write this Value.
    /// On complex types (structs and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: Only byte Arrays (of correct size) can be written)
    /// in dynamic mode: A Value that represents the value will be accepted also. A negative timeout indicates that the Default
    /// Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public int TryWriteValue(object value, int timeout) => this.OnTryWriteValue(value, timeout);

    /// <summary>
    /// Writes the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout in ms.</param>
    /// <remarks>Calling on primitive types, a call of this method will directly write this Value.
    /// On complex types (structs and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: Only byte Arrays (of correct size) can be written)
    /// in dynamic mode: A Value that represents the value will be accepted also. A negative timeout indicates that the Default
    /// Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public void WriteValue(object value, int timeout) => this.OnWriteValue(value, timeout);

    /// <summary>
    /// Writes the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A tasks that represents the asynchronous 'ReadValue' operation. The read result is stored in the <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" /> return value and contains
    /// the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" />.</returns>
    /// <remarks>Calling on primitive types, a call of this method will directly write this Value.
    /// On complex types (structs and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: Only byte Arrays (of correct size) can be written)
    /// in dynamic mode: A Value that represents the value will be accepted also.</remarks>
    public Task<ResultWriteAccess> WriteValueAsync(
      object value,
      CancellationToken cancel)
    {
      return this.OnWriteValueAsync(value, cancel);
    }

    /// <summary>Handler function for writing the dynamic value</summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual void OnWriteValue(object value, int timeout)
    {
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException();
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      this.EnsureRights((SymbolAccessRights) 2);
      IAccessorConnection iaccessorConnection = valueAccessor as IAccessorConnection;
      DateTimeOffset? nullable;
      if (timeout >= 0 && iaccessorConnection != null)
      {
        if (iaccessorConnection.Connection == null)
          throw new CannotAccessValueException();
        using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
          valueAccessor.WriteValue((ISymbol) this, value, ref nullable);
      }
      else
        valueAccessor.WriteValue((ISymbol) this, value, ref nullable);
    }

    /// <summary>Handler function for writing the dynamic value</summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected internal virtual int OnTryWriteValue(object value, int timeout)
    {
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!this.HasValue)
        return 1796;
      this.EnsureRights((SymbolAccessRights) 2);
      IAccessorConnection iaccessorConnection = valueAccessor as IAccessorConnection;
      DateTimeOffset? nullable;
      if (timeout < 0 || iaccessorConnection == null)
        return valueAccessor.TryWriteValue((ISymbol) this, value, ref nullable);
      if (iaccessorConnection.Connection == null)
        throw new CannotAccessValueException((ISymbol) this);
      using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
        return valueAccessor.TryWriteValue((ISymbol) this, value, ref nullable);
    }

    /// <summary>Handler function for writing the dynamic value</summary>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token..</param>
    /// <returns>Task&lt;ResultWriteAccess&gt;.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual Task<ResultWriteAccess> OnWriteValueAsync(
      object value,
      CancellationToken cancel)
    {
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException();
      if (!this.HasValue)
        return Task.FromResult<ResultWriteAccess>(new ResultWriteAccess(1796, 0U));
      this.EnsureRights((SymbolAccessRights) 2);
      return valueAccessor.WriteValueAsync((ISymbol) this, value, cancel);
    }

    /// <summary>Ensures that the AccessRights are matched.</summary>
    /// <param name="requested">The requested rights.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.InsufficientAccessRightsException"></exception>
    protected void EnsureRights(SymbolAccessRights requested)
    {
      if ((this.accessRights & requested) != requested)
        throw new InsufficientAccessRightsException((IValueSymbol) this, requested);
    }

    /// <summary>Handler function for reading the dynamic value.</summary>
    /// <returns>The Value</returns>
    protected virtual object OnReadValue(int timeout)
    {
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      this.EnsureRights((SymbolAccessRights) 1);
      IAccessorConnection iaccessorConnection = valueAccessor as IAccessorConnection;
      DateTimeOffset? nullable;
      if (timeout < 0 || iaccessorConnection == null)
        return valueAccessor.ReadValue((ISymbol) this, ref nullable);
      if (iaccessorConnection.Connection == null)
        throw new CannotAccessValueException((ISymbol) this);
      using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
        return valueAccessor.ReadValue((ISymbol) this, ref nullable);
    }

    /// <summary>Handler function for reading the dynamic value.</summary>
    /// <returns>The Value</returns>
    protected virtual Task<ResultReadValueAccess> OnReadValueAsync(
      CancellationToken cancel)
    {
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!this.HasValue)
        return Task.FromResult<ResultReadValueAccess>(new ResultReadValueAccess(1796, 0U));
      this.EnsureRights((SymbolAccessRights) 1);
      return valueAccessor.ReadValueAsync((ISymbol) this, cancel);
    }

    /// <summary>Handler function for reading the dynamic value.</summary>
    /// <returns>The Value</returns>
    protected internal virtual int OnTryReadValue(int timeout, out object? value)
    {
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!this.HasValue)
      {
        value = (object) null;
        return 1796;
      }
      this.EnsureRights((SymbolAccessRights) 1);
      IAccessorConnection iaccessorConnection = valueAccessor as IAccessorConnection;
      DateTimeOffset? nullable;
      if (timeout < 0 || iaccessorConnection == null)
        return valueAccessor.TryReadValue((ISymbol) this, ref value, ref nullable);
      if (iaccessorConnection.Connection == null)
        throw new CannotAccessValueException((ISymbol) this);
      using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
        return valueAccessor.TryReadValue((ISymbol) this, ref value, ref nullable);
    }

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see> into a new created instance of the managed type
    /// </summary>
    /// <param name="managedType">The tp.</param>
    /// <returns>Read value (System.Object).</returns>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.WriteAnyValue(System.Object)" />
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.UpdateAnyValue(System.Object@)" />
    public object ReadAnyValue(Type managedType) => this.OnReadAnyValue(managedType, -1);

    /// <summary>
    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.IValueAnySymbol">Value</see> into a new created instance of the managed type
    /// </summary>
    /// </summary>
    /// <typeparam name="T">Type of the Value to be read.</typeparam>
    /// <returns>T.</returns>
    public T ReadAnyValue<T>() => (T) this.OnReadAnyValue(typeof (T), -1);

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see> into a new created instance of the managed type
    /// </summary>
    /// <param name="managedType">The tp.</param>
    /// <param name="timeout">The timeout in ms.</param>
    /// <returns>Read value (System.Object).</returns>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.WriteAnyValue(System.Object)" />
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.UpdateAnyValue(System.Object@)" />
    public object ReadAnyValue(Type managedType, int timeout) => this.OnReadAnyValue(managedType, timeout);

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see> into a new created instance of the managed type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="timeout">The timeout in ms.</param>
    /// <returns>Read value (System.Object).</returns>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.WriteAnyValue(System.Object)" />
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.UpdateAnyValue(System.Object@)" />
    public T ReadAnyValue<T>(int timeout) => (T) this.OnReadAnyValue(typeof (T), timeout);

    /// <summary>Reads the (AnyType) value asynchronously.</summary>
    /// <param name="managedType">Managed type of the value to read.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task object that is representing the asynchronous 'ReadAnyValue' operation. The result will be returned in a <see cref="T:TwinCAT.ValueAccess.ResultReadValueAccess" />, which
    /// contains the <see cref="P:TwinCAT.ValueAccess.ResultReadValueAccess`1.Value" /> and the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" />.</returns>
    public Task<ResultReadValueAccess> ReadAnyValueAsync(
      Type managedType,
      CancellationToken cancel)
    {
      return this.OnReadAnyValueAsync(managedType, cancel);
    }

    /// <summary>Read any value as an asynchronous operation.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A Task&lt;ResultReadValueAccess`1&gt; representing the asynchronous operation.</returns>
    public async Task<ResultReadValueAccess<T>> ReadAnyValueAsync<T>(
      CancellationToken cancel)
    {
      ResultReadValueAccess resultReadValueAccess = await this.OnReadAnyValueAsync(typeof (T), cancel).ConfigureAwait(false);
      return new ResultReadValueAccess<T>((T) ((ResultReadValueAccess<object>) resultReadValueAccess).Value, ((ResultAccess) resultReadValueAccess).ErrorCode, ((ResultAccess) resultReadValueAccess).DateTime, ((ResultAccess) resultReadValueAccess).InvokeId);
    }

    /// <summary>Handler function 'ReadAnyValue'</summary>
    /// <param name="managedType">Type of the managed.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.ValueAccess.ValueAccessorException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    private object OnReadAnyValue(Type managedType, int timeout)
    {
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      if (!(valueAccessor is IAccessorValueAny iaccessorValueAny))
        throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support IValueAnyAccessor", (object) this.ValueAccessor), this.ValueAccessor);
      int num = 0;
      IAccessorConnection iaccessorConnection = iaccessorValueAny as IAccessorConnection;
      object obj;
      DateTimeOffset? nullable;
      if (timeout >= 0 && iaccessorConnection != null)
      {
        if (iaccessorConnection.Connection == null)
          throw new CannotAccessValueException();
        using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
          num = iaccessorValueAny.TryReadAnyValue((ISymbol) this, managedType, ref obj, ref nullable);
      }
      else
        num = iaccessorValueAny.TryReadAnyValue((ISymbol) this, managedType, ref obj, ref nullable);
      if (num != 0)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot read (any) of Symbol '{0}'! Error: {1}", (object) this.InstancePath, (object) num), (ISymbol) this);
      return obj;
    }

    /// <summary>Asynchronous handler function for 'ReadAnyValue'.</summary>
    /// <param name="managedType">Type of the managed.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultReadValueAccess&gt;.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.ValueAccess.ValueAccessorException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    private Task<ResultReadValueAccess> OnReadAnyValueAsync(
      Type managedType,
      CancellationToken cancel)
    {
      IAccessorValueAny valueAccessor = this.ValueAccessor as IAccessorValueAny;
      if (!this.HasValue)
        return Task.FromResult<ResultReadValueAccess>(new ResultReadValueAccess(1796, 0U));
      IAccessorConnection iaccessorConnection = valueAccessor != null ? valueAccessor as IAccessorConnection : throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support IValueAnyAccessor", (object) this.ValueAccessor), this.ValueAccessor);
      return valueAccessor.ReadAnyValueAsync((ISymbol) this, managedType, cancel);
    }

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see> into the specified managed value.
    /// </summary>
    /// <param name="managedObject">The managed object.</param>
    /// <returns>Read value (System.Object).</returns>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.ReadAnyValue(System.Type)" />
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.WriteAnyValue(System.Object)" />
    public void UpdateAnyValue(ref object managedObject) => managedObject = this.OnUpdateAnyValue(managedObject, -1);

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see> into the specified managed value.
    /// </summary>
    /// <param name="managedObject">The managed object.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>Read value (System.Object).</returns>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.ReadAnyValue(System.Type)" />
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.WriteAnyValue(System.Object)" />
    public void UpdateAnyValue(ref object managedObject, int timeout) => managedObject = this.OnUpdateAnyValue(managedObject, timeout);

    /// <summary>Handler function for 'UpdateAnyValue'</summary>
    /// <param name="managedObject">The managed object.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.ValueAccess.ValueAccessorException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    private object OnUpdateAnyValue(object managedObject, int timeout)
    {
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      if (!(this.ValueAccessor is IAccessorValueAny valueAccessor))
        throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support IValueAnyAccessor", (object) this.ValueAccessor), this.ValueAccessor);
      int num = 0;
      IAccessorConnection iaccessorConnection = valueAccessor as IAccessorConnection;
      DateTimeOffset? nullable;
      if (timeout >= 0 && iaccessorConnection != null)
      {
        if (iaccessorConnection.Connection == null)
          throw new CannotAccessValueException();
        using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
          num = valueAccessor.TryUpdateAnyValue((ISymbol) this, ref managedObject, ref nullable);
      }
      else
        num = valueAccessor.TryUpdateAnyValue((ISymbol) this, ref managedObject, ref nullable);
      if (num != 0)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot read (any) of Symbol '{0}'! Error: {1}", (object) this.InstancePath, (object) num), (ISymbol) this);
      return managedObject;
    }

    /// <summary>
    /// Writes the value represented by the managed value to this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see>
    /// </summary>
    /// <param name="managedValue">The managed value.</param>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.ReadAnyValue(System.Type)" />
    /// .
    ///             <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.UpdateAnyValue(System.Object@)" />
    /// .
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void WriteAnyValue(object managedValue) => this.OnWriteAnyValue(managedValue, -1);

    /// <summary>
    /// Writes the value represented by the managed value to this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see>
    /// </summary>
    /// <param name="managedValue">The managed value.</param>
    /// <param name="timeout">The timeout in ms.</param>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.ReadAnyValue(System.Type)" />
    /// .
    ///             <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.UpdateAnyValue(System.Object@)" />
    /// .
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void WriteAnyValue(object managedValue, int timeout) => this.OnWriteAnyValue(managedValue, timeout);

    /// <summary>Handler function for 'WriteAnyValue'</summary>
    /// <param name="managedValue">The managed value.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.ValueAccess.ValueAccessorException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    private void OnWriteAnyValue(object managedValue, int timeout)
    {
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      if (!(this.ValueAccessor is IAccessorValueAny valueAccessor))
        throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support IValueAnyAccessor", (object) this.ValueAccessor), this.ValueAccessor);
      int num = 0;
      IAccessorConnection iaccessorConnection = valueAccessor as IAccessorConnection;
      DateTimeOffset? nullable;
      if (timeout >= 0 && iaccessorConnection != null)
      {
        if (iaccessorConnection.Connection == null)
          throw new CannotAccessValueException();
        using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
          num = valueAccessor.TryWriteAnyValue((ISymbol) this, managedValue, ref nullable);
      }
      else
        num = valueAccessor.TryWriteAnyValue((ISymbol) this, managedValue, ref nullable);
      if (num != 0)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot write (any) of Symbol '{0}'! Error: {1}", (object) this.InstancePath, (object) num), (ISymbol) this);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is primitive.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is primitive; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsPrimitiveType => this.DataType != null ? this.DataType.IsPrimitive : PrimitiveTypeMarshaler.IsPrimitiveType(this.Category);

    /// <summary>
    /// Gets a value indicating whether the Symbols datatype is a Container type.
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    /// <seealso cref="P:TwinCAT.TypeSystem.IDataType.Category" />
    /// <remarks>Container Types are all types that contain SubElements like
    /// <list type="bullet"><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Array" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Pointer" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Union" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Struct" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Function" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.FunctionBlock" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Program" /></description></item></list>
    /// and the <see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Alias" /> and <see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Reference" /> types, if they have a container type as base type.</remarks>
    public virtual bool IsContainerType => this.DataType != null ? this.DataType.IsContainer : PrimitiveTypeMarshaler.IsContainerType(this.Category);

    /// <summary>
    /// Gets a value indicating whether this instance is recursive.
    /// </summary>
    /// <value><c>true</c> if this instance is recursive; otherwise, <c>false</c>.</value>
    public bool IsRecursive
    {
      get
      {
        List<ISymbol> parentList = this.getParentList();
        parentList.Insert(0, (ISymbol) this);
        for (int index1 = 0; index1 < parentList.Count - 1; ++index1)
        {
          ISymbol isymbol1 = parentList[index1];
          for (int index2 = index1 + 1; index2 < parentList.Count; ++index2)
          {
            ISymbol isymbol2 = parentList[index2];
            if (((IInstance) isymbol1).DataType == ((IInstance) isymbol2).DataType)
              return true;
          }
        }
        return false;
      }
    }

    /// <summary>Gets the value encoding.</summary>
    /// <value>The value encoding.</value>
    public virtual Encoding ValueEncoding
    {
      get
      {
        Encoding valueEncoding = ((IAttributedInstance) this).GetValueEncoding();
        if (valueEncoding != null)
          return valueEncoding;
        IBinder binder = this.Binder;
        return binder != null ? ((ISymbolServer) binder.Provider).DefaultValueEncoding : StringMarshaler.DefaultEncoding;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is virtual.
    /// </summary>
    /// <value><c>true</c> if this instance is virtual; otherwise, <c>false</c>.</value>
    /// <remarks>Virtual symbols are are only organizational elements within the Symbols Hierarchy and cannot
    /// be accessed seperately by IndexGroup/IndexOffset, Value Read/Writes, notifications or handles.</remarks>
    public bool IsVirtual => this is IVirtualStructInstance;

    /// <summary>Gets the parent list.</summary>
    /// <returns>List&lt;ISymbol&gt;.</returns>
    private List<ISymbol> getParentList()
    {
      List<ISymbol> parentList = new List<ISymbol>();
      for (ISymbol parent = this.Parent; parent != null; parent = parent.Parent)
        parentList.Add(parent);
      return parentList;
    }

    /// <summary>
    /// Called when the RawValue has been changed (firing RawValueChanged event).
    /// </summary>
    /// <param name="args">The arguments.</param>
    void ISymbolValueChangeNotify.OnRawValueChanged(
      RawValueChangedEventArgs args)
    {
      if (this._rawValueChanged == null)
        return;
      this._rawValueChanged((object) this, args);
    }

    /// <summary>
    /// Called when the Value has been changed (firing ValueChanged event).
    /// </summary>
    /// <param name="args">The arguments.</param>
    void ISymbolValueChangeNotify.OnValueChanged(ValueChangedEventArgs args)
    {
      if (this._valueChanged == null)
        return;
      this._valueChanged((object) this, args);
    }

    /// <summary>Gets the Unwrapped Symbol</summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The unwrapped symbol if dynamically wrapped, the original symbol otherwise.</returns>
    internal static ISymbol Unwrap(ISymbol symbol) => !(symbol is IDynamicSymbol idynamicSymbol) ? symbol : (ISymbol) idynamicSymbol.Unwrap();

    void IAdsSymbolInternal.SetAddress(uint indexGroup, uint indexOffset)
    {
      this.indexGroup = indexGroup;
      this.indexOffset = indexOffset;
    }
  }
}
