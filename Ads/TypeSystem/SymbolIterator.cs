// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.SymbolIterator
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using TwinCAT.TypeSystem;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Iterator class for enumerations of <see cref="T:TwinCAT.TypeSystem.ISymbol">Symbols</see>.
  /// </summary>
  /// <remarks>This iterator class can be used to iterate over collections of symbol trees (root symbols + sub symbols).
  /// By constructor the user can choose if the iterator works recursively within the symbol
  /// tree and optionally a filter function to select only specific symbols (predicate).
  /// </remarks>
  /// <example>
  /// The following example shows how to determine, browse and filter symbols.
  /// <code language="C#" title="Browsing and filtering Symbols" source="..\..\Samples\TwinCAT.ADS.NET_Samples\01_ADS.NET_ReadWriteFlag\Form1.cs" region="CODE_SAMPLE_SYMBOLBROWSER_ASYNC" />
  /// </example>
  public class SymbolIterator : SymbolIterator<ISymbol>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.SymbolIterator" /> class.
    /// </summary>
    /// <param name="symbols">The symbol collection.</param>
    /// <param name="recurse">if set to <c>true</c>, the iterator works recursively over all subsymbols.</param>
    /// <param name="mask">Specifies a mask over the symbols, that filters out specific symbol categories. The default is <see cref="F:TwinCAT.TypeSystem.SymbolIterationMask.All" /> and all symbols are shown.</param>
    /// <param name="selector">Predicate function indicating that symbols are visible for the iteration. The default value null returns all symbols (of the specified mask).</param>
    /// <param name="areChildsIterated">Predicate function indicating that childs of the actual symbol should be iterated (in recurse mode). The default value iterates all child (of the specified mask).</param>
    public SymbolIterator(
      IEnumerable<ISymbol> symbols,
      bool recurse,
      SymbolIterationMask mask,
      Func<ISymbol, bool>? selector,
      Func<ISymbol, bool>? areChildsIterated)
      : base(symbols, recurse, mask, selector, areChildsIterated)
    {
    }

    public SymbolIterator(IInstanceCollection<ISymbol> symbols, Func<ISymbol, bool>? selector)
      : base(symbols, selector)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.SymbolIterator" /> class.
    /// </summary>
    /// <param name="symbols">The symbol collection.</param>
    /// <param name="recurse">if set to <c>true</c>, the iterator works recursively over all subsymbols.</param>
    /// <param name="selector">Predicate function indicating that symbols are visible for the iteration. The default value null returns all symbols (of the specified mask).</param>
    public SymbolIterator(IEnumerable<ISymbol> symbols, bool recurse, Func<ISymbol, bool>? selector)
      : base(symbols, recurse, selector)
    {
    }

    public SymbolIterator(IInstanceCollection<ISymbol> symbols)
      : base(symbols, (Func<ISymbol, bool>) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.SymbolIterator" /> class.
    /// </summary>
    /// <param name="symbols">The symbol enumeration.</param>
    /// <param name="recurse">if set to <c>true</c>, the iterator works recursively over all subsymbols.</param>
    public SymbolIterator(IEnumerable<ISymbol> symbols, bool recurse)
      : base(symbols, recurse)
    {
    }
  }
}
