// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicVirtualStructInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Dynamic struct instance</summary>
  public sealed class DynamicVirtualStructInstance : 
    DynamicStructInstance,
    IVirtualStructInstance,
    IStructInstance,
    IInterfaceInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IRpcCallableInstance
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicStructInstance" /> class.
    /// </summary>
    /// <param name="structInstance">The struct instance.</param>
    internal DynamicVirtualStructInstance(IVirtualStructInstance structInstance)
      : base((IStructInstance) structInstance)
    {
    }

    /// <summary>Adds an member instance.</summary>
    /// <param name="memberInstance">The member instance.</param>
    /// <param name="parent">The parent struct instance. Usually the this pointer.</param>
    public bool AddMember(ISymbol memberInstance, IVirtualStructInstance parent)
    {
      if (memberInstance == null)
        throw new ArgumentNullException(nameof (memberInstance));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      bool flag = ((IVirtualStructInstance) this._InnerSymbol).AddMember(memberInstance, parent);
      string instanceName = ((IInstance) memberInstance).InstanceName;
      this._weakNormalizedNames = (WeakReference<IDictionary<string, ISymbol>>) null;
      return flag;
    }

    /// <summary>Handler function for reading ADS 'Any' Values.</summary>
    /// <param name="managedType">Managed type to read.</param>
    /// <returns>System.Object.</returns>
    protected override object OnReadAnyValue(Type managedType) => base.OnReadAnyValue(managedType);
  }
}
