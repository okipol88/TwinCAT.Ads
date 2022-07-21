// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.ReferenceType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Represents a reference type</summary>
  public sealed class ReferenceType : DataType, IReferenceType, IDataType, IBitSize
  {
    private string referencedTypeName = string.Empty;
    private IDataType? referencedType;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="referencedTypeName">Name of the referenced type.</param>
    /// <param name="size">The size.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">size</exception>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReferenceType(string? name, string referencedTypeName, int size)
      : base(name ?? "REFERENCE TO " + referencedTypeName, (AdsDataTypeId) 65, (DataTypeCategory) 15, size, (Type) null)
    {
      if (size != 0 && size != 4 && size != 8)
        throw new ArgumentOutOfRangeException(nameof (size), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Reference size is {0}!", (object) size));
      if (string.IsNullOrEmpty(referencedTypeName))
        throw new ArgumentOutOfRangeException(nameof (referencedTypeName));
      if (string.IsNullOrEmpty(name))
        this.Name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "REFERENCE TO {0}", (object) referencedTypeName);
      this.referencedTypeName = referencedTypeName;
      switch (size)
      {
        case 4:
          this.ManagedType = typeof (uint);
          break;
        case 8:
          this.ManagedType = typeof (ulong);
          break;
      }
      this.flags = (AdsDataTypeFlags) 5;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceType" /> class.
    /// </summary>
    /// <param name="referenced">The referenced.</param>
    /// <param name="platformPointerSize">Size of the platform pointer.</param>
    public ReferenceType(IDataType referenced, int platformPointerSize)
      : this((string) null, referenced.Name, platformPointerSize)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="referencedTypeName">Name of the referenced type.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal ReferenceType(AdsDataTypeEntry entry, string referencedTypeName)
      : base((DataTypeCategory) 15, entry)
    {
      this.referencedTypeName = referencedTypeName;
      if (entry.Size == 4)
      {
        this.ManagedType = typeof (uint);
      }
      else
      {
        if (entry.Size != 8)
          return;
        this.ManagedType = typeof (ulong);
      }
    }

    /// <summary>
    /// Called when this <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> is bound via the type binder.
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void OnBound(IBinder binder)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      if (this.Size == 4 || this.Size == 8)
      {
        if (((IDataTypeResolver) binder).PlatformPointerSize > 0)
          return;
        ((Binder) binder).SetPlatformPointerSize(this.Size);
      }
      else
      {
        this.Size = ((IDataTypeResolver) binder).PlatformPointerSize;
        if (this.Size == 4)
          this.ManagedType = typeof (uint);
        if (this.Size != 8)
          return;
        this.ManagedType = typeof (ulong);
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is a container type
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    /// <seealso cref="P:TwinCAT.TypeSystem.IDataType.Category" />
    /// <remarks>Container Types are all types that contain SubElements like
    /// <list type="bullet"><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Array" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Pointer" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Union" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Struct" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Function" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.FunctionBlock" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Program" /></description></item></list>
    /// and the <see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Alias" /> and <see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Reference" /> types, if they have a container type as base type.</remarks>
    public override bool IsContainer
    {
      get
      {
        bool isContainer = false;
        IDataType referencedType = this.ReferencedType;
        if (referencedType != null)
          isContainer = referencedType.IsContainer;
        return isContainer;
      }
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
        DataType referencedType = (DataType) this.ReferencedType;
        return referencedType != null && referencedType.IsPrimitive;
      }
    }

    /// <summary>Gets the corresponding .NET Type if attached.</summary>
    /// <value>Dot net type.</value>
    public override Type? ManagedType
    {
      get
      {
        if (base.ManagedType == (Type) null && this.resolver != null)
        {
          switch (((IDataTypeResolver) this.resolver).PlatformPointerSize)
          {
            case 4:
              this.ManagedType = typeof (uint);
              break;
            case 8:
              this.ManagedType = typeof (ulong);
              break;
          }
        }
        return base.ManagedType;
      }
    }

    /// <summary>Gets the name of the referenced type.</summary>
    /// <value>The name of the referenced type.</value>
    public string ReferencedTypeName => this.referencedTypeName;

    /// <summary>Gets the the referenced type.</summary>
    /// <value>The type of the referenced.</value>
    public IDataType? ReferencedType
    {
      get
      {
        if (this.resolver != null && !this.IsBindingResolved(false))
          ((IBindable2) this).ResolveWithBinder(false, this.resolver);
        return this.referencedType;
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
      if (this.referencedType != null)
        flag = !recurse || ((IBindable2) this.referencedType).IsBindingResolved(recurse);
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
        if (this.referencedType == null)
          flag = ((IDataTypeResolver) binder).TryResolveType(this.referencedTypeName, ref this.referencedType);
        if (this.referencedType != null & recurse)
          flag = ((IBindable2) this.referencedType).ResolveWithBinder(recurse, binder);
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
      ReferenceType referenceType = this;
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool ret = true;
      if (!referenceType.IsBindingResolved(recurse))
      {
        if (referenceType.referencedType == null)
        {
          ResultValue<IDataType> resultValue = await ((IDataTypeResolver) binder).ResolveTypeAsync(referenceType.referencedTypeName, cancel).ConfigureAwait(false);
          if (((ResultAds) resultValue).Succeeded)
            referenceType.referencedType = resultValue.Value;
        }
        if (referenceType.referencedType != null & recurse)
          ret = await ((IBindable2) referenceType.referencedType).ResolveWithBinderAsync(recurse, binder, cancel).ConfigureAwait(false);
      }
      return ret;
    }

    /// <summary>Gets the resolved category.</summary>
    /// <value>The resolved category.</value>
    public DataTypeCategory ResolvedCategory
    {
      get
      {
        IDataType resolvedType = this.ResolvedType;
        return resolvedType != null ? resolvedType.Category : (DataTypeCategory) 0;
      }
    }

    /// <summary>Gets the size of the resolved byte.</summary>
    /// <value>The size of the resolved byte.</value>
    public int ResolvedByteSize
    {
      get
      {
        IDataType resolvedType = this.ResolvedType;
        return resolvedType != null ? ((IBitSize) resolvedType).ByteSize : 0;
      }
    }

    /// <summary>Gets the type of the resolved.</summary>
    /// <value>The type of the resolved.</value>
    public IDataType ResolvedType => this.ResolveType((DataTypeResolveStrategy) 1);
  }
}
