// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.UnionType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Represents a union type</summary>
  public sealed class UnionType : DataType, IUnionType, IDataType, IBitSize
  {
    /// <summary>The Union Fields</summary>
    private FieldCollection _fields = new FieldCollection();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.UnionType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal UnionType(AdsDataTypeEntry entry)
      : base((DataTypeCategory) 14, entry)
    {
      for (int index = 0; index < entry.SubItemCount; ++index)
      {
        AdsFieldEntry subEntry = entry.SubEntries[index];
        if (subEntry != null)
        {
          Member instance = new Member((DataType) this, subEntry);
          if (!this._fields.isUnique((IInstance) instance))
            ((IInstanceInternal) instance).SetInstanceName(this._fields.createUniquepathName((IInstance) instance));
          this._fields.Add((IField) instance);
        }
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
      foreach (Instance field in (InstanceCollection<IField>) this._fields)
        field.Bind(binder);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is resolved.
    /// </summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <value><c>true</c> if this instance is resolved; otherwise, <c>false</c>.</value>
    /// <exclude />
    public override bool IsBindingResolved(bool recurse) => this._fields.Cast<Member>().All<Member>((Func<Member, bool>) (m => m.IsBindingResolved(recurse)));

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
      if (!this.IsBindingResolved(recurse) && flag)
        flag = this._fields.Cast<IBindable2>().Select<IBindable2, bool>((Func<IBindable2, bool>) (b => b.ResolveWithBinder(recurse, binder))).All<bool>((Func<bool, bool>) (b => b));
      return flag;
    }

    /// <summary>on resolve with binder as an asynchronous operation.</summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The binder.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exception cref="T:System.ArgumentNullException">binder</exception>
    protected override async Task<bool> OnResolveWithBinderAsync(
      bool recurse,
      IBinder binder,
      CancellationToken cancel)
    {
      UnionType unionType = this;
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool flag1 = true;
      if (!unionType.IsBindingResolved(recurse) && flag1)
      {
        foreach (Field field in (InstanceCollection<IField>) unionType._fields)
        {
          bool flag = flag1;
          flag1 = flag | await ((IBindable2) field).ResolveWithBinderAsync(true, binder, cancel).ConfigureAwait(false);
        }
      }
      return flag1;
    }

    /// <summary>
    /// Gets a read only collection of the <see cref="T:TwinCAT.TypeSystem.IField">Fields</see> of the <see cref="T:TwinCAT.TypeSystem.IUnionType" />.
    /// </summary>
    /// <value>The members as read only collection.</value>
    public IFieldCollection Fields => (IFieldCollection) new ReadOnlyFieldCollection(this._fields);
  }
}
