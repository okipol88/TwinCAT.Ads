// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicOversamplingArrayInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Dynamic Array Instance</summary>
  public sealed class DynamicOversamplingArrayInstance : 
    DynamicArrayInstance,
    IOversamplingArrayInstance,
    IArrayInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IIndexedAccess
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicOversamplingArrayInstance" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    internal DynamicOversamplingArrayInstance(IOversamplingArrayInstance symbol)
      : base((IArrayInstance) symbol)
    {
    }

    /// <summary>Gets the oversampling element.</summary>
    /// <value>The oversampling element.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public ISymbol OversamplingElement => ((IOversamplingArrayInstance) this._InnerSymbol).OversamplingElement;
  }
}
