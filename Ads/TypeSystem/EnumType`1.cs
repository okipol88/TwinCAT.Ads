// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.EnumType`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Enum <see cref="T:TwinCAT.Ads.TypeSystem.DataType" />.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed class EnumType<T> : 
    DataType,
    IEnumType<T>,
    IAliasType,
    IDataType,
    IBitSize,
    IEnumType
    where T : struct, IConvertible
  {
    /// <summary>The _base type identifier</summary>
    private AdsDataTypeId _baseTypeId;
    /// <summary>The _base type name</summary>
    private string _baseTypeName = string.Empty;
    /// <summary>The _base type</summary>
    private IDataType? _baseType;
    /// <summary>The _fields</summary>
    private EnumValueCollection<T> _fields = EnumValueCollection<T>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal EnumType(AdsDataTypeEntry entry)
      : base((DataTypeCategory) 3, entry)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      Type type = typeof (T);
      this.ManagedType = type == typeof (byte) || type == typeof (sbyte) || type == typeof (short) || type == typeof (ushort) || type == typeof (int) || type == typeof (uint) || type == typeof (long) || type == typeof (ulong) ? type : throw new AdsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' is not a valid base type for an enumeration!", (object) type));
      this._baseTypeId = entry.BaseTypeId;
      this._baseTypeName = entry.TypeName;
      this._fields = new EnumValueCollection<T>(entry.Enums);
      this.flags = (AdsDataTypeFlags) (this.flags | 8192);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.EnumType`1" /> class.
    /// </summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnumType(string name, string baseTypeName, int byteSize, EnumValueCollection<T> values)
      : base(name, (AdsDataTypeId) 65, (DataTypeCategory) 3, byteSize, typeof (T))
    {
      if (!PrimitiveTypeMarshaler.TryGetDataTypeId(typeof (T), out this._baseTypeId))
        throw new NotSupportedException();
      this._baseTypeName = baseTypeName;
      this._fields = values;
      this.flags = (AdsDataTypeFlags) (this.flags | 8192);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.EnumType`1" /> class.
    /// </summary>
    /// <param name="name">The name of the Enum</param>
    /// <param name="baseType">Enum Base/Alias type.</param>
    /// <param name="values">The Enum values.</param>
    /// <exception cref="T:System.NotSupportedException"></exception>
    public EnumType(string name, IDataType baseType, EnumValueCollection<T> values)
      : base(name, (AdsDataTypeId) 65, (DataTypeCategory) 3, ((IBitSize) baseType).ByteSize, typeof (T))
    {
      if (!PrimitiveTypeMarshaler.TryGetDataTypeId(typeof (T), out this._baseTypeId))
        throw new NotSupportedException();
      this._baseTypeName = baseType.Name;
      if (values.Count > 0 && values[0].Size != this.ByteSize)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 1);
        interpolatedStringHandler.AppendLiteral("Values are not matching enum base type '");
        interpolatedStringHandler.AppendFormatted<IDataType>(baseType);
        interpolatedStringHandler.AppendLiteral("'!");
        throw new ArgumentException(interpolatedStringHandler.ToStringAndClear(), nameof (values));
      }
      this._fields = values;
      this.flags = (AdsDataTypeFlags) (this.flags | 8192);
    }

    /// <summary>Gets the BaseType name</summary>
    /// <value>The name of the base type.</value>
    public string BaseTypeName => this._baseTypeName;

    /// <summary>Gets the Base Type</summary>
    /// <value>The type of the base.</value>
    public IDataType? BaseType
    {
      get
      {
        if (!this.IsBindingResolved(false) && this.resolver != null)
          ((IDataTypeResolver) this.resolver).TryResolveType(this._baseTypeName, ref this._baseType);
        return this._baseType;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is resolved.
    /// </summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <value><c>true</c> if this instance is resolved; otherwise, <c>false</c>.</value>
    /// <exclude />
    public override bool IsBindingResolved(bool recurse)
    {
      bool flag = false;
      if (this._baseType != null)
        flag = !recurse || ((IBindable2) this._baseType).IsBindingResolved(true);
      return flag;
    }

    /// <summary>Called when [resolve with binder].</summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The binder.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">binder</exception>
    protected override bool OnResolveWithBinder(bool recurse, IBinder binder)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool flag = true;
      if (!this.IsBindingResolved(recurse))
      {
        if (this._baseType == null)
          flag = ((IDataTypeResolver) binder).TryResolveType(this._baseTypeName, ref this._baseType);
        if (this._baseType != null & recurse)
          flag = ((IBindable2) this._baseType).ResolveWithBinder(recurse, binder);
      }
      return flag;
    }

    /// <summary>
    /// Handler function resolving the DataType binding asynchronously.
    /// </summary>
    /// <param name="recurse">if set to this method resolves all subtypes recursivly.</param>
    /// <param name="binder">The binder.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exclude />
    protected override async Task<bool> OnResolveWithBinderAsync(
      bool recurse,
      IBinder binder,
      CancellationToken cancel)
    {
      EnumType<T> enumType = this;
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool ret = true;
      if (!enumType.IsBindingResolved(recurse))
      {
        if (enumType._baseType == null)
        {
          ResultValue<IDataType> resultValue = await ((IDataTypeResolver) binder).ResolveTypeAsync(enumType._baseTypeName, cancel).ConfigureAwait(false);
          if (((ResultAds) resultValue).Succeeded)
            enumType._baseType = resultValue.Value;
        }
        if (enumType._baseType != null & recurse)
          ret = await ((IBindable2) enumType._baseType).ResolveWithBinderAsync(recurse, binder, cancel).ConfigureAwait(false);
      }
      return ret;
    }

    /// <summary>Enumeration specification (if enum)</summary>
    /// <value>The enum specification.</value>
    IEnumValueCollection IEnumType.EnumValues => (IEnumValueCollection) ((EnumValueCollection) this._fields).AsReadOnly();

    /// <summary>
    /// Gets the values of the <see cref="T:TwinCAT.TypeSystem.IEnumType`1" />
    /// </summary>
    /// <returns>T[].</returns>
    public T[] GetValues() => this._fields.GetValues();

    /// <summary>
    /// Gets the filed names of the <see cref="T:TwinCAT.TypeSystem.IEnumType`1" />
    /// </summary>
    /// <returns>System.String[].</returns>
    public string[] GetNames() => this._fields.GetNames();

    /// <summary>Tries to parse the Enum Value</summary>
    /// <param name="strValue">Enum value (in string representation).</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string strValue, out T value) => this._fields.TryParse(strValue, out value);

    public bool TryParse(string strValue, [NotNullWhen(true)] out IEnumValue? value)
    {
      EnumValue<T> enumValue = (EnumValue<T>) null;
      value = (IEnumValue) null;
      if (this._fields.TryParse(strValue, out enumValue))
        value = (IEnumValue) enumValue;
      return value != null;
    }

    /// <summary>
    /// Parses a name of the <see cref="T:TwinCAT.TypeSystem.IEnumType`1" /> and returns the value (as base type)
    /// </summary>
    /// <param name="strValue">Enum Value as string.</param>
    /// <returns>T.</returns>
    public T Parse(string strValue) => this._fields.Parse(strValue);

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <param name="val">The value.</param>
    /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
    /// <exception cref="T:System.ArgumentException">val</exception>
    public string ToString(T val)
    {
      EnumValue<T> ei;
      return this._fields.TryGetInfo(val, out ei) ? ei.Name : val.ToString();
    }

    /// <summary>
    /// Gets the values of the <see cref="T:TwinCAT.TypeSystem.IEnumType`1" />
    /// </summary>
    /// <returns>T[].</returns>
    IConvertible[] IEnumType.GetValues()
    {
      T[] values1 = this._fields.GetValues();
      object[] values2 = new object[this._fields.Count];
      values1.CopyTo((Array) values2, 0);
      return (IConvertible[]) values2;
    }

    /// <summary>
    /// Parses a name of the <see cref="T:TwinCAT.TypeSystem.IEnumType`1" /> and returns the value (as base type)
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    IConvertible IEnumType.Parse(string name) => (IConvertible) this.Parse(name);

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <param name="val">The value.</param>
    /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
    public string ToString(IConvertible val) => this.ToString((T) Convert.ChangeType((object) val, typeof (T), (IFormatProvider) null));

    /// <summary>Parses the value from value  name.</summary>
    /// <param name="name">The value name.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if value name was found, <c>false</c> otherwise.</returns>
    bool IEnumType.TryParse(string name, [NotNullWhen(true)] out IConvertible? value)
    {
      T obj = default (T);
      value = (IConvertible) null;
      if (!this.TryParse(name, out obj))
        return false;
      value = (IConvertible) obj;
      return true;
    }

    /// <summary>
    /// Determines whether the enum values contains the specified name
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if contains the value, otherwise, <c>false</c>.</returns>
    public bool Contains(string name)
    {
      T obj = default (T);
      return this.TryParse(name, out obj);
    }

    /// <summary>Enumeration specification (if enum)</summary>
    /// <value>The enum specification.</value>
    public IEnumValueCollection EnumValues => (IEnumValueCollection) this._fields.AsReadOnly();
  }
}
