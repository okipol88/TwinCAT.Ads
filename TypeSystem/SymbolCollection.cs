// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.SymbolCollection
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
  /// Interface represents a collection of <see cref="T:TwinCAT.TypeSystem.ISymbol" /> objects.
  /// </summary>
  public class SymbolCollection : 
    SymbolCollection<ISymbol>,
    ISymbolCollection,
    ISymbolCollection<ISymbol>,
    IInstanceCollection<ISymbol>,
    IList<ISymbol>,
    ICollection<ISymbol>,
    IEnumerable<ISymbol>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.SymbolCollection" /> class organized with InstancePath.
    /// </summary>
    public SymbolCollection()
      : this((InstanceCollectionMode) 1)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.SymbolCollection" /> class.
    /// </summary>
    /// <param name="mode">The mode.</param>
    /// <exclude />
    public SymbolCollection(InstanceCollectionMode mode)
      : base(mode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.SymbolCollection" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    public SymbolCollection(IEnumerable<ISymbol> coll)
      : base(coll, (InstanceCollectionMode) 1)
    {
    }

    /// <summary>
    /// Returns a Read only version of this collection (shallow copy).
    /// </summary>
    /// <returns>Read only collection.</returns>
    public ReadOnlySymbolCollection AsReadOnly() => new ReadOnlySymbolCollection((IInstanceCollection<ISymbol>) this);

    /// <summary>Clones this instance.</summary>
    /// <returns>Cloned <see cref="T:TwinCAT.TypeSystem.SymbolCollection" />.</returns>
    public SymbolCollection Clone() => new SymbolCollection((IEnumerable<ISymbol>) this);

    /// <summary>Returns an Empty Collection.</summary>
    /// <returns>SymbolCollection.</returns>
    public static SymbolCollection Empty => new SymbolCollection();
  }
}
