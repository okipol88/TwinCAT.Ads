// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicStructInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Dynamic struct instance</summary>
  public class DynamicStructInstance : 
    DynamicInterfaceInstance,
    IRpcStructInstance,
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
    internal DynamicStructInstance(IStructInstance structInstance)
      : base((IInterfaceInstance) structInstance)
    {
    }
  }
}
