// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.SymbolNavigator`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Navigator class to navigate through a tree of symbols.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SymbolNavigator<T> where T : class, ISymbol
  {
    private IInstanceCollection<T> _symbols;
    /// <summary>The Default path separator</summary>
    public const char DefaultPathSeparator = '.';
    /// <summary>the current path separator.</summary>
    private char _pathSeparator = '.';

    public SymbolNavigator(IInstanceCollection<T> symbols, char sep)
    {
      if (symbols == null)
        throw new ArgumentNullException(nameof (symbols));
      this._pathSeparator = sep;
      this._symbols = symbols;
    }

    public SymbolNavigator(IInstanceCollection<T> symbols) => this._symbols = symbols != null ? symbols : throw new ArgumentNullException(nameof (symbols));

    /// <summary>Gets or sets the path separator character.</summary>
    /// <value>The path separator.</value>
    public char PathSeparator
    {
      get => this._pathSeparator;
      set => this._pathSeparator = value;
    }

    /// <summary>Tries to get the symbol</summary>
    /// <param name="path">The path.</param>
    /// <param name="found">The found instance (out-parameter)</param>
    /// <returns>true if found, false if not contained.</returns>
    public bool TryGetSymbol(string path, [NotNullWhen(true)] out T? found)
    {
      bool flag = !string.IsNullOrEmpty(path) ? path.StartsWith(".", StringComparison.OrdinalIgnoreCase) : throw new ArgumentOutOfRangeException(nameof (path));
      int index1 = 0;
      string[] relativePath = path.Split(new char[1]
      {
        this.PathSeparator
      }, StringSplitOptions.RemoveEmptyEntries);
      if (flag && relativePath.Length != 0)
        relativePath[0] = relativePath[0].Insert(0, ".");
      T obj = default (T);
      if (relativePath.Length != 0)
      {
        IList<T> objList = (IList<T>) null;
        if (this._symbols.TryGetInstanceByName(relativePath[index1], ref objList) && objList.Count > 0)
        {
          T root = objList[0];
          int index2 = index1 + 1;
          if (relativePath.Length > index2)
            return this.TryGetSubSymbol(root, relativePath, index2, out found);
          found = root;
          return true;
        }
      }
      found = default (T);
      return false;
    }

    /// <summary>
    /// Tries to get the Subsymbol specified in the relative path
    /// </summary>
    /// <param name="root">Root instance (anchor instance).</param>
    /// <param name="relativePath">The relative path (relative to the root).</param>
    /// <param name="index">Optional array index.</param>
    /// <param name="found">Found object (out-parameter).</param>
    /// <returns>true if found, false if not contained.</returns>
    private bool TryGetSubSymbol(T root, string[] relativePath, int index, [NotNullWhen(true)] out T? found)
    {
      string fullPathWithoutIndexes = (string) null;
      string instanceName = (string) null;
      string indicesStr = (string) null;
      IList<int[]> jaggedIndices;
      if (SymbolParser.TryParseArrayElement(relativePath[index], out fullPathWithoutIndexes, out instanceName, out indicesStr, out jaggedIndices, out ArrayIndexType? _))
      {
        IList<ISymbol> isymbolList;
        if (!((IInstanceCollection<ISymbol>) ((IInterfaceInstance) (object) root).MemberInstances).TryGetInstanceByName(fullPathWithoutIndexes, ref isymbolList))
          throw new ArgumentException("Cannot resolve Array", nameof (root));
        IArrayInstance root1 = (IArrayInstance) isymbolList[0];
        T found1 = default (T);
        bool flag = true;
        for (int index1 = 0; index1 < jaggedIndices.Count; ++index1)
        {
          flag &= this.TryGetSubSymbol(root1, jaggedIndices[index1], out found1);
          if (flag)
            root1 = (IArrayInstance) (object) found1;
          else
            break;
        }
        found = !flag ? default (T) : found1;
        return (object) found != null;
      }
      if (((ISymbol) (object) root).Category == 5)
        return this.TryGetSubSymbol((IStructInstance) (object) root, relativePath, index, out found);
      found = default (T);
      return false;
    }

    /// <summary>Tries to get the specified subSymbol</summary>
    /// <param name="root">Root array instance.</param>
    /// <param name="indices">The indices.</param>
    /// <param name="found">Found Instance (out-parameter)</param>
    /// <returns>true if found, false if not contained.</returns>
    private bool TryGetSubSymbol(IArrayInstance root, int[] indices, [NotNullWhen(true)] out T? found)
    {
      ISymbol isymbol = (ISymbol) null;
      T obj1 = default (T);
      if (((IIndexedAccess) root).TryGetElement(indices, ref isymbol))
      {
        T obj2 = (T) isymbol;
        found = obj2;
        return true;
      }
      found = default (T);
      return false;
    }

    /// <summary>Try to get Sub Symbol</summary>
    /// <param name="root">Root object.</param>
    /// <param name="relativeInstancePath">The relative instance path.</param>
    /// <param name="index">Array Index</param>
    /// <param name="symbol">Found Symbol (out-parameter)</param>
    /// <returns>true if found, false if not contained.</returns>
    private bool TryGetSubSymbol(
      IStructInstance root,
      string[] relativeInstancePath,
      int index,
      [NotNullWhen(true)] out T? symbol)
    {
      IList<ISymbol> isymbolList = (IList<ISymbol>) null;
      string str = relativeInstancePath[index];
      if (((IInstanceCollection<ISymbol>) ((IInterfaceInstance) root).MemberInstances).TryGetInstanceByName(str, ref isymbolList) && ((ICollection<ISymbol>) isymbolList).Count > 0)
      {
        T root1 = (T) isymbolList[0];
        ++index;
        if (relativeInstancePath.Length > index)
          return this.TryGetSubSymbol(root1, relativeInstancePath, index, out symbol);
        symbol = root1;
        return true;
      }
      symbol = default (T);
      return false;
    }
  }
}
