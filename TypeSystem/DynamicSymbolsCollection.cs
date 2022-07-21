// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicSymbolsCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Dynamic (Expandable) Symbols collection.</summary>
  /// <remarks>The <see cref="T:TwinCAT.TypeSystem.DynamicSymbolsCollection" /> collection adds dynamically its child Symbols as Members (for access like "Main.Symbol")</remarks>
  public sealed class DynamicSymbolsCollection : 
    DynamicObject,
    IDynamicSymbolsCollection,
    IDynamicMetaObjectProvider,
    IEnumerable<ISymbol>,
    IEnumerable
  {
    /// <summary>Internal Symbol Collection</summary>
    private SymbolCollection<ISymbol> _symbols;
    private Dictionary<string, ISymbol> _normalizedDict = new Dictionary<string, ISymbol>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicSymbolsCollection" /> class (for internal use only)
    /// </summary>
    /// <param name="symbols">The symbols.</param>
    /// <exception cref="T:System.ArgumentNullException">symbols</exception>
    public DynamicSymbolsCollection(SymbolCollection<ISymbol> symbols)
    {
      this._symbols = symbols != null ? symbols : throw new ArgumentNullException(nameof (symbols));
      foreach (DynamicSymbol symbol in (InstanceCollection<ISymbol>) symbols)
        this._normalizedDict.Add(symbol.NormalizedName, (ISymbol) symbol);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicSymbolsCollection" /> class (for internal use only)
    /// </summary>
    /// <param name="symbols">The symbols.</param>
    /// <exception cref="T:System.ArgumentNullException">symbols</exception>
    public DynamicSymbolsCollection(IEnumerable<ISymbol> symbols)
    {
      this._symbols = symbols != null ? new SymbolCollection<ISymbol>(symbols, (InstanceCollectionMode) 0) : throw new ArgumentNullException(nameof (symbols));
      foreach (DynamicSymbol symbol in symbols)
        this._normalizedDict.Add(symbol.NormalizedName, (ISymbol) symbol);
    }

    /// <summary>Returns the enumeration of all dynamic member names.</summary>
    /// <returns>A sequence that contains dynamic member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames() => base.GetDynamicMemberNames().Union<string>((IEnumerable<string>) this._normalizedDict.Keys);

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    public override bool TryGetMember(GetMemberBinder binder, [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      ISymbol isymbol = (ISymbol) null;
      if (!this._normalizedDict.TryGetValue(binder.Name, out isymbol))
        return base.TryGetMember(binder, out result);
      result = (object) isymbol;
      return true;
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" /> with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>DynamicSymbol.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Symbol name not found in DynamicSymbols collection!</exception>
    public DynamicSymbol this[string name]
    {
      get
      {
        IList<ISymbol> instances = (IList<ISymbol>) null;
        if (this._symbols.TryGetInstanceByName(name, out instances))
          return (DynamicSymbol) instances[0];
        throw new KeyNotFoundException("Symbol name not found in DynamicSymbols collection!");
      }
    }

    public bool TryGetInstance(string instanceSpecifier, [NotNullWhen(true)] out ISymbol? symbol) => this._symbols.TryGetInstance(instanceSpecifier, out symbol);

    internal bool TryGetInstanceHierarchically(string instancePath, [NotNullWhen(true)] out ISymbol? symbol) => this._symbols.TryGetInstanceHierarchically(instancePath, out symbol);

    /// <summary>Gets the enumerator.</summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<ISymbol> GetEnumerator() => this._symbols.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._symbols.GetEnumerator();

    /// <summary>Gets an empty collection.</summary>
    /// <returns>DynamicSymbolsCollection.</returns>
    public static DynamicSymbolsCollection Empty => new DynamicSymbolsCollection(SymbolCollection<ISymbol>.Empty);
  }
}
