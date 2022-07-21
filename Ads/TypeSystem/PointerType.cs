// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.PointerType
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
  /// <summary>Represents a pointer type.</summary>
  public class PointerType : DataType, IPointerType, IDataType, IBitSize
  {
    private string referencedTypeName = string.Empty;
    private IDataType? referencedType;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PointerType" /> class.
    /// </summary>
    /// <param name="referencedTypeName">Name of the referenced type.</param>
    /// <param name="platformPointerSize">Size of the platform pointer.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">size</exception>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public PointerType(string referencedTypeName, int platformPointerSize)
      : base("POINTER TO " + referencedTypeName, (AdsDataTypeId) 65, (DataTypeCategory) 13, platformPointerSize, (Type) null)
    {
      this.referencedTypeName = referencedTypeName;
      this.flags = (AdsDataTypeFlags) 1;
      if (platformPointerSize == 4)
        this.ManagedType = typeof (uint);
      else
        this.ManagedType = typeof (ulong);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PointerType" /> class.
    /// </summary>
    /// <param name="referencedType">Type of the referenced.</param>
    /// <param name="platformPointerSize">Size of the platform pointer.</param>
    public PointerType(IDataType referencedType, int platformPointerSize)
      : this(referencedType.Name, platformPointerSize)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PointerType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="referencedTypeName">Name of the referenced type.</param>
    /// <param name="platformPointerSize">The platform pointer size.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">size</exception>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public PointerType(string name, string referencedTypeName, int platformPointerSize)
      : base(name, (AdsDataTypeId) 65, (DataTypeCategory) 13, platformPointerSize, (Type) null)
    {
      this.referencedTypeName = referencedTypeName;
      this.flags = (AdsDataTypeFlags) 1;
      if (platformPointerSize == 4)
        this.ManagedType = typeof (uint);
      else
        this.ManagedType = typeof (ulong);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PointerType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="referencedTypeName">Name of the referenced type.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal PointerType(AdsDataTypeEntry entry, string referencedTypeName)
      : base((DataTypeCategory) 13, entry)
    {
      this.referencedTypeName = referencedTypeName;
      if (entry.Size == 4)
        this.ManagedType = typeof (uint);
      else
        this.ManagedType = typeof (ulong);
    }

    /// <summary>
    /// Called when this <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> is bound via the type binder.
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void OnBound(IBinder binder)
    {
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
            default:
              AdsModule.Trace.TraceWarning("Couldn't resolve Pointer type '{0}' yet!", new object[1]
              {
                (object) this.Name
              });
              break;
          }
        }
        return base.ManagedType;
      }
    }

    /// <summary>Gets the name of the referenced datatype</summary>
    /// <value>The name of the reference dattype.</value>
    public string ReferenceTypeName => this.referencedTypeName;

    /// <summary>Gets the the referenced type.</summary>
    /// <value>The type of the referenced.</value>
    public IDataType? ReferencedType
    {
      get
      {
        if (this.referencedType == null && this.resolver != null)
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
      PointerType pointerType = this;
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool ret = true;
      if (!pointerType.IsBindingResolved(recurse))
      {
        if (pointerType.referencedType == null)
        {
          ResultValue<IDataType> resultValue = await ((IDataTypeResolver) binder).ResolveTypeAsync(pointerType.referencedTypeName, cancel).ConfigureAwait(false);
          if (((ResultAds) resultValue).Succeeded)
            pointerType.referencedType = resultValue.Value;
        }
        if (pointerType.referencedType != null & recurse)
          ret = await ((IBindable2) pointerType.referencedType).ResolveWithBinderAsync(recurse, binder, cancel).ConfigureAwait(false);
      }
      return ret;
    }
  }
}
