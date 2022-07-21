// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlySymbolCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections;
using System.Collections.Generic;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// ReadOnly collection containing <see cref="T:TwinCAT.TypeSystem.ISymbol" /> objects.
  /// </summary>
  public class ReadOnlySymbolCollection : 
    ReadOnlySymbolCollection<ISymbol>,
    ISymbolCollection,
    ISymbolCollection<ISymbol>,
    IInstanceCollection<ISymbol>,
    IList<ISymbol>,
    ICollection<ISymbol>,
    IEnumerable<ISymbol>,
    IEnumerable
  {
    public ReadOnlySymbolCollection(IInstanceCollection<ISymbol> symbols)
      : base(symbols)
    {
    }

    /// <summary>Returns an Empty collection.</summary>
    /// <returns>ReadOnlySymbolCollection.</returns>
    public static ReadOnlySymbolCollection Empty => SymbolCollection.Empty.AsReadOnly();
  }
}
