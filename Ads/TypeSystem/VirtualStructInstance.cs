// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.VirtualStructInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;
using System.Diagnostics;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class VirtualStructInstance.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DebuggerDisplay("Path = { InstancePath } (Virtual), Category = {category}")]
  internal sealed class VirtualStructInstance : 
    StructInstance,
    IVirtualStructInstance,
    IStructInstance,
    IInterfaceInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IRpcCallableInstance
  {
    /// <summary>Virtual members (used as SubSymbols)</summary>
    /// <remarks>These virtual members are stored as full reference that they are
    /// not lost over time.
    /// /// </remarks>
    private SymbolCollection _virtualMembers = new SymbolCollection((InstanceCollectionMode) 0);

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal VirtualStructInstance(
      string instanceName,
      string instancePath,
      ISymbol? parent,
      ISymbolFactoryServices services)
      : base(instanceName, instancePath, services)
    {
      this.Category = (DataTypeCategory) 5;
      this.SetParent(parent);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.VirtualStructInstance" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="services">The services.</param>
    internal VirtualStructInstance(
      string name,
      uint indexGroup,
      uint indexOffset,
      ISymbolFactoryServices services)
      : base(name, name, indexGroup, indexOffset, (IStructType) null, (ISymbol) null, services)
    {
      this.Category = (DataTypeCategory) 5;
    }

    /// <summary>Adds the member.</summary>
    /// <param name="memberInstance">The member instance.</param>
    /// <param name="parent">The parent struct instance. Usually the this pointer.</param>
    public bool AddMember(ISymbol memberInstance, IVirtualStructInstance parent)
    {
      bool flag;
      if (!this._virtualMembers.isUnique((IInstance) memberInstance))
      {
        AdsModule.Trace.TraceWarning("Cannot add ambiguous instance '{0}' to virtual parent '{1}!", new object[2]
        {
          (object) memberInstance,
          (object) parent
        });
        flag = false;
      }
      else
      {
        this._virtualMembers.Add(memberInstance);
        flag = true;
      }
      return flag;
    }

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol) => this._virtualMembers.Count;

    /// <summary>Called when the SubSymbols are (re)created)</summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <returns>TwinCAT.TypeSystem.SymbolCollection.</returns>
    internal override ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentInstance)
    {
      return (ISymbolCollection<ISymbol>) this._virtualMembers;
    }

    /// <summary>
    /// Gets the size of the <see cref="T:TwinCAT.Ads.TypeSystem.Instance" /> in bits.
    /// </summary>
    /// <value>The size of the bit.</value>
    public override int BitSize => -1;

    /// <summary>
    /// Gets a value indicating whether this instance has a value.
    /// </summary>
    /// <value><c>true</c> if this instance has value; otherwise, <c>false</c>.</value>
    public override bool HasValue => false;

    /// <summary>
    /// Tries to resolve the <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override bool TryResolveType() => false;
  }
}
