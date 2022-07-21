// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.SubRangeType`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Represents a SubRangType</summary>
  public sealed class SubRangeType<T> : 
    DataType,
    ISubRangeType<T>,
    ISubRangeType,
    IDataType,
    IBitSize
    where T : struct
  {
    private string baseTypeName = string.Empty;
    private IDataType? baseType;
    private T _lowerBound;
    private T _upperBound;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.SubRangeType`1" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="baseType">Type of the base.</param>
    /// <param name="size">The size.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal SubRangeType(string name, string baseType, int size, T lowerBound, T upperBound)
      : base(name, (AdsDataTypeId) 0, (DataTypeCategory) 9, size, typeof (T))
    {
      AdsDataTypeId typeId;
      PrimitiveTypeMarshaler.TryGetDataTypeId(typeof (T), out typeId);
      this.DataTypeId = typeId;
      this.baseTypeName = baseType;
      this._lowerBound = lowerBound;
      this._upperBound = upperBound;
    }

    /// <summary>Gets the corresponding .NET Type if attached.</summary>
    /// <value>Dot net type.</value>
    public override Type ManagedType => typeof (T);

    /// <summary>Gets the name of the base type.</summary>
    /// <value>The name of the base type.</value>
    public string BaseTypeName => this.baseTypeName;

    /// <summary>Gets the the base type.</summary>
    /// <value>The type of the referenced.</value>
    public IDataType? BaseType
    {
      get
      {
        if (!this.IsBindingResolved(false) && this.resolver != null)
        {
          IDataType idataType = (IDataType) null;
          if (((IDataTypeResolver) this.resolver).TryResolveType(this.baseTypeName, ref idataType))
          {
            this.baseType = idataType;
            DataType baseType = (DataType) this.baseType;
            if (baseType.IsBitType)
              this.Size = baseType.BitSize;
            else
              this.Size = baseType.Size;
            this.flags = baseType.Flags;
          }
        }
        return this.baseType;
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
      if (this.baseType != null)
        flag = !recurse || ((IBindable2) this.baseType).IsBindingResolved(true);
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
        if (this.baseType == null)
          flag = ((IDataTypeResolver) binder).TryResolveType(this.baseTypeName, ref this.baseType);
        if (this.baseType != null & recurse)
          flag = ((IBindable2) this.baseType).ResolveWithBinder(recurse, binder);
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
      SubRangeType<T> subRangeType = this;
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool ret = true;
      if (!subRangeType.IsBindingResolved(recurse))
      {
        if (subRangeType.baseType == null)
        {
          ResultValue<IDataType> resultValue = await ((IDataTypeResolver) binder).ResolveTypeAsync(subRangeType.baseTypeName, cancel).ConfigureAwait(false);
          if (((ResultAds) resultValue).Succeeded)
            subRangeType.baseType = resultValue.Value;
        }
        if (subRangeType.baseType != null & recurse)
          ret = await ((IBindable2) subRangeType.baseType).ResolveWithBinderAsync(recurse, binder, cancel).ConfigureAwait(false);
      }
      return ret;
    }

    /// <summary>Gets the lower bound.</summary>
    /// <value>The lower bound.</value>
    public T LowerBound => this._lowerBound;

    /// <summary>Gets the upper bound.</summary>
    /// <value>The upper bound.</value>
    public T UpperBound => this._upperBound;
  }
}
