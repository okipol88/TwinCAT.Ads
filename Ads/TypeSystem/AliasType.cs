// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AliasType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Alias DataType</summary>
  public sealed class AliasType : DataType, IAliasType, IDataType, IBitSize
  {
    private AdsDataTypeId _baseTypeId;
    private string _baseTypeName = string.Empty;
    private IDataType? _baseType;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.IAliasType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal AliasType(AdsDataTypeEntry entry)
      : base((DataTypeCategory) 2, entry)
    {
      this._baseTypeId = entry != null ? entry.BaseTypeId : throw new ArgumentNullException(nameof (entry));
      this._baseTypeName = entry.TypeName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.IAliasType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="baseType">Type of the base.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AliasType(string name, DataType baseType)
      : base((DataTypeCategory) 2)
    {
      if (baseType == null)
        throw new ArgumentNullException(nameof (baseType));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      this.Size = baseType.Size;
      this.ManagedType = baseType.ManagedType;
      this.Namespace = baseType.Namespace;
      this.resolver = baseType.resolver;
      this.DataTypeId = (AdsDataTypeId) 65;
      this.Name = name;
      this._baseTypeId = baseType.DataTypeId;
      this._baseType = (IDataType) baseType;
      this._baseTypeName = baseType.Name;
    }

    /// <summary>Gets the BaseType name</summary>
    public string BaseTypeName => this._baseTypeName;

    /// <summary>Gets the Base Type</summary>
    public IDataType? BaseType
    {
      get
      {
        if (!this.IsBindingResolved(false) && this.resolver != null)
          ((IBindable2) this).ResolveWithBinder(false, this.resolver);
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

    /// <summary>Handler function resolving the DataType binding</summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The binder.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exclude />
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
      AliasType aliasType = this;
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool ret = true;
      if (!aliasType.IsBindingResolved(recurse))
      {
        if (aliasType._baseType == null)
        {
          ResultValue<IDataType> resultValue = await ((IDataTypeResolver) binder).ResolveTypeAsync(aliasType._baseTypeName, cancel).ConfigureAwait(false);
          if (((ResultAds) resultValue).Succeeded)
            aliasType._baseType = resultValue.Value;
        }
        if (aliasType._baseType != null & recurse)
          ret = await ((IBindable2) aliasType._baseType).ResolveWithBinderAsync(recurse, binder, cancel).ConfigureAwait(false);
      }
      return ret;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is primitive
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is primitive; otherwise, <c>false</c>.
    /// </value>
    public override bool IsPrimitive
    {
      get
      {
        DataType baseType = (DataType) this.BaseType;
        return baseType != null && baseType.IsPrimitive;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is a container type
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    /// <seealso cref="P:TwinCAT.TypeSystem.IDataType.Category" />
    /// <remarks>Container Types are all types that contain SubElements like
    /// <list type="bullet"><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Array" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Pointer" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Union" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Struct" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Function" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.FunctionBlock" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Program" /></description></item></list>
    /// And the <see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Alias" /> types, if they have a container type as base type.</remarks>
    public override bool IsContainer
    {
      get
      {
        DataType baseType = (DataType) this.BaseType;
        return baseType != null ? baseType.IsContainer : base.IsContainer;
      }
    }

    /// <summary>Gets the corresponding .NET Type if attached.</summary>
    /// <value>Dot net type.</value>
    public override Type? ManagedType
    {
      get
      {
        if (base.ManagedType == (Type) null)
        {
          DataType baseType = (DataType) this.BaseType;
          if (baseType != null)
            this.ManagedType = baseType.ManagedType;
        }
        return base.ManagedType;
      }
      protected set => base.ManagedType = value;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.Name;
  }
}
