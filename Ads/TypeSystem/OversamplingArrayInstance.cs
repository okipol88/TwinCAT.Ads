// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.OversamplingArrayInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class OversamplingArrayInstance.</summary>
  internal sealed class OversamplingArrayInstance : 
    ArrayInstance,
    IOversamplingArrayInstance,
    IArrayInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IIndexedAccess
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">Oversampling instance.</param>
    /// <param name="parent">The parent.</param>
    internal OversamplingArrayInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 4;
    }

    internal OversamplingArrayInstance(
      AdsSymbolEntry entry,
      IArrayType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 4;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal OversamplingArrayInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 4;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.OversamplingArrayInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    internal OversamplingArrayInstance(
      string instanceName,
      IArrayType type,
      ISymbol parent,
      int fieldOffset)
      : base(instanceName, type, parent, fieldOffset)
    {
      this.Category = (DataTypeCategory) 4;
    }

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol) => base.OnGetSubSymbolCount(parentSymbol) + 1;

    /// <summary>Creates the sub symbols collection.</summary>
    internal override ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentInstance)
    {
      ISymbolCollection<ISymbol> subSymbols = base.OnCreateSubSymbols(parentInstance);
      try
      {
        ArrayType dataType = (ArrayType) this.DataType;
        ISymbolFactoryOversampled symbolFactory = (ISymbolFactoryOversampled) ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory;
      }
      catch (Exception ex)
      {
        AdsModule.Trace.TraceError(ex);
      }
      return subSymbols;
    }

    public bool TryGetOversamplingElement([NotNullWhen(true)] out ISymbol? symbol)
    {
      ArrayType dataType = (ArrayType) this.DataType;
      if (dataType != null && dataType.IsOversampled)
      {
        ISymbolCollection<ISymbol> subSymbols = this.SubSymbols;
        symbol = ((IList<ISymbol>) subSymbols)[((ICollection<ISymbol>) subSymbols).Count - 1];
        return true;
      }
      symbol = (ISymbol) null;
      return false;
    }

    /// <summary>Gets the oversampling element.</summary>
    /// <value>The oversampling element.</value>
    public ISymbol OversamplingElement
    {
      get
      {
        ISymbol symbol = (ISymbol) null;
        this.TryGetOversamplingElement(out symbol);
        return symbol;
      }
    }

    /// <summary>
    /// Get the Elements Collection (SubItems without Virtual oversampling element)
    /// </summary>
    /// <returns>ReadOnlySymbolCollection.</returns>
    protected override ISymbolCollection<ISymbol> OnGetElements()
    {
      SymbolCollection symbolCollection = new SymbolCollection((IEnumerable<ISymbol>) ((ISymbolInternal) this).SubSymbolsInternal);
      symbolCollection.RemoveAt(symbolCollection.Count - 1);
      return (ISymbolCollection<ISymbol>) symbolCollection.AsReadOnly();
    }
  }
}
