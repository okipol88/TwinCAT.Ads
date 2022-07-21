// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.SymbolIterator`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;


#nullable enable
namespace TwinCAT.TypeSystem.Generic
{
  /// <summary>
  /// Iterator class for enumerations of <see cref="T:TwinCAT.TypeSystem.ISymbol">Symbols</see>.
  /// </summary>
  /// <typeparam name="T">Concrete <see cref="T:TwinCAT.TypeSystem.ISymbol" /> type.</typeparam>
  /// <seealso cref="T:System.Collections.Generic.IEnumerable`1" />
  /// <remarks>This iterator class can be used to iterate over collections of symbol trees (root symbols + sub symbols).
  /// By constructor the user can choose if the iterator works recursively within the symbol
  /// tree and optionally a filter function to select only specific symbols (predicate).
  /// </remarks>
  /// <example>
  /// The following example shows how to determine, browse and filter symbols.
  /// <code language="C#" title="Browsing and filtering Symbols" source="..\..\Samples\TwinCAT.ADS.NET_Samples\01_ADS.NET_ReadWriteFlag\Form1.cs" region="CODE_SAMPLE_SYMBOLBROWSER_ASYNC" />
  /// </example>
  public class SymbolIterator<T> : IEnumerable<T>, IEnumerable where T : class, ISymbol
  {
    /// <summary>Symbols enumeration</summary>
    private IEnumerable<T> _symbols;
    /// <summary>Symbol Iterator mask</summary>
    private SymbolIterationMask _mask = SymbolIterationMask.All;
    private bool _symbolRecursionDetection = true;
    private bool _recurse;
    /// <summary>The Filter handler Function</summary>
    private Func<T, bool>? _selectorPredicate;
    /// <summary>Handler function indicating if children are browsed</summary>
    private Func<T, bool>? _areChildsSelectedPredicate;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.SymbolIterator`1" /> class.
    /// </summary>
    /// <param name="symbols">Input collection (root objects).</param>
    /// <param name="recurse">if set to <c>true</c>, the iterator works recursively over all subsymbols.</param>
    /// <param name="mask">Specifies a mask over the symbols, that filters out specific symbol categories. The default is <see cref="F:TwinCAT.TypeSystem.SymbolIterationMask.All" /> and all symbols are shown.</param>
    /// <param name="selector">Predicate function indicating that symbols are visible for the iteration. The default value null returns all symbols (of the specified mask).</param>
    /// <param name="areChildsIterated">Predicate function indicating that childs of the actual symbol should be iterated (in recurse mode). The default value iterates all child (of the specified mask).</param>
    public SymbolIterator(
      IEnumerable<T> symbols,
      bool recurse,
      SymbolIterationMask mask,
      Func<T, bool>? selector,
      Func<T, bool>? areChildsIterated)
    {
      this._symbols = symbols;
      this._recurse = recurse;
      this._selectorPredicate = selector;
      this._areChildsSelectedPredicate = areChildsIterated;
      this._mask = mask;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.SymbolIterator`1" /> class.
    /// </summary>
    /// <param name="symbols">Input collection (root objects).</param>
    /// <param name="recurse">if set to <c>true</c>, the iterator works recursively over all subsymbols.</param>
    /// <param name="selector">Predicate function indicating that symbols are visible for the iteration. The default value null returns all symbols (of the specified mask).</param>
    public SymbolIterator(IEnumerable<T> symbols, bool recurse, Func<T, bool>? selector)
      : this(symbols, recurse, SymbolIterationMask.All, selector, (Func<T, bool>) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.SymbolIterator`1" /> class.
    /// </summary>
    /// <param name="symbols">The root collection</param>
    /// <param name="recurse">if set to <c>true</c>, the iterator works recursively over all subsymbols.</param>
    public SymbolIterator(IEnumerable<T> symbols, bool recurse)
      : this(symbols, recurse, SymbolIterationMask.All, (Func<T, bool>) null, (Func<T, bool>) null)
    {
    }

    public SymbolIterator(IInstanceCollection<T> symbols, Func<T, bool>? selector)
      : this((IEnumerable<T>) symbols, false, SymbolIterationMask.All, selector, (Func<T, bool>) null)
    {
      if (symbols == null)
        throw new ArgumentNullException(nameof (symbols));
      switch ((int) symbols.Mode)
      {
        case 0:
          this._recurse = false;
          break;
        case 1:
          this._recurse = false;
          break;
        case 2:
          this._recurse = true;
          break;
        default:
          throw new NotSupportedException();
      }
    }

    public SymbolIterator(IInstanceCollection<T> symbols)
      : this((IEnumerable<T>) symbols, false, SymbolIterationMask.All, (Func<T, bool>) null, (Func<T, bool>) null)
    {
    }

    /// <summary>
    /// Gets or sets the <see cref="T:TwinCAT.TypeSystem.SymbolIterationMask" />
    /// </summary>
    /// <value>The mask.</value>
    /// <remarks>This property can be used for prefiltering the iterator
    /// without using a predicate function.
    /// </remarks>
    public SymbolIterationMask Mask
    {
      get => this._mask;
      set => this._mask = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the iterator checks for Symbol recursions (true by default).
    /// </summary>
    /// <value><c>true</c> if recursion checking, <c>false</c> switched off check.</value>
    public bool SymbolRecursionDetection
    {
      get => this._symbolRecursionDetection;
      set => this._symbolRecursionDetection = value;
    }

    /// <summary>
    /// Gets the enumerator that enumerates through a collection
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator() => this.getFilteredEnumerable(this._symbols).GetEnumerator();

    private IEnumerable<T> getFilteredEnumerable(IEnumerable<T> parentColl)
    {
      foreach (T obj in parentColl)
      {
        T parent = obj;
        if (this._selectorPredicate == null || this._selectorPredicate(parent))
          yield return parent;
        bool flag = this._symbolRecursionDetection && ((ISymbol) (object) parent).IsRecursive;
        if (this._recurse && !flag && this.iterateSubSymbols(parent))
        {
          foreach (T filtered in this.getFilteredEnumerable((IEnumerable<T>) ((ISymbol) (object) parent).SubSymbols))
            yield return filtered;
        }
        parent = default (T);
      }
    }

    /// <summary>
    /// Indicates that the subsymbols of the parent should be iterated.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private bool iterateSubSymbols(IDataType type)
    {
      if (this._mask == SymbolIterationMask.All)
        return true;
      switch ((int) type.Category)
      {
        case 2:
          IAliasType ialiasType = (IAliasType) type;
          return ialiasType.BaseType == null || this.iterateSubSymbols(ialiasType.BaseType);
        case 4:
          return (this._mask & SymbolIterationMask.Arrays) > SymbolIterationMask.None;
        case 5:
          return (this._mask & SymbolIterationMask.Structures) > SymbolIterationMask.None;
        case 13:
          return (this._mask & SymbolIterationMask.Pointer) > SymbolIterationMask.None;
        case 14:
          return (this._mask & SymbolIterationMask.Unions) > SymbolIterationMask.None;
        case 15:
          return (this._mask & SymbolIterationMask.References) > SymbolIterationMask.None;
        default:
          return true;
      }
    }

    /// <summary>
    /// Indicates that the subsymbols of the parent should be iterated.
    /// </summary>
    /// <param name="parent">The type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private bool iterateSubSymbols(T parent)
    {
      if (this._areChildsSelectedPredicate != null && !this._areChildsSelectedPredicate(parent))
        return false;
      if (this._mask == SymbolIterationMask.All)
        return true;
      switch ((int) ((ISymbol) (object) parent).Category)
      {
        case 2:
          IAliasType dataType = (IAliasType) ((IInstance) (object) parent).DataType;
          return dataType == null || dataType.BaseType == null || this.iterateSubSymbols(dataType.BaseType);
        case 4:
          return (this._mask & SymbolIterationMask.Arrays) > SymbolIterationMask.None;
        case 5:
          return (this._mask & SymbolIterationMask.Structures) > SymbolIterationMask.None;
        case 13:
          return (this._mask & SymbolIterationMask.Pointer) > SymbolIterationMask.None;
        case 14:
          return (this._mask & SymbolIterationMask.Unions) > SymbolIterationMask.None;
        case 15:
          return (this._mask & SymbolIterationMask.References) > SymbolIterationMask.None;
        default:
          return true;
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
