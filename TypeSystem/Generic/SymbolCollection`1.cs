// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.SymbolCollection`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem.Generic
{
  /// <summary>
  /// Interface represents a collection of <see cref="T:TwinCAT.TypeSystem.ISymbol" /> objects.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SymbolCollection<T> : 
    InstanceCollection<T>,
    ISymbolCollection<T>,
    IInstanceCollection<T>,
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
    where T : class, ISymbol
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.SymbolCollection`1" /> class.
    /// </summary>
    /// <param name="mode">The mode.</param>
    internal SymbolCollection(InstanceCollectionMode mode)
      : base(mode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.SymbolCollection" /> class.
    /// </summary>
    /// <param name="coll">The collection to be copied.</param>
    /// <param name="mode">The mode.</param>
    internal SymbolCollection(IEnumerable<T> coll, InstanceCollectionMode mode)
      : base(coll, mode)
    {
    }

    /// <summary>
    /// Returns a Read only version of this collection (shallow copy).
    /// </summary>
    /// <returns>ReadOnlySymbolCollection&lt;T&gt;.</returns>
    public ReadOnlySymbolCollection<T> AsReadOnly() => new ReadOnlySymbolCollection<T>((IInstanceCollection<T>) this);

    /// <summary>Clones this instance.</summary>
    /// <returns>SymbolCollection&lt;T&gt;.</returns>
    public SymbolCollection<T> Clone() => new SymbolCollection<T>((IEnumerable<T>) this, this.Mode);

    /// <summary>
    /// Creates an Empty <see cref="T:TwinCAT.TypeSystem.Generic.SymbolCollection`1" />
    /// </summary>
    /// <returns>SymbolCollection&lt;T&gt;.</returns>
    public static SymbolCollection<T> Empty => new SymbolCollection<T>((InstanceCollectionMode) 1);

    /// <summary>Try to get instances with predicate function</summary>
    /// <param name="predicate">The predicate function</param>
    /// <param name="recurse">if set to <c>true</c> the symbol hierarchy will be searched recursively.</param>
    /// <param name="instances">The instances.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetInstances(Func<T, bool> predicate, bool recurse, [NotNullWhen(true)] out IList<T>? instances)
    {
      instances = (IList<T>) new List<T>();
      foreach (T obj in new SymbolIterator<T>((IEnumerable<T>) this.InnerList, recurse))
      {
        if (predicate(obj))
          instances.Add(obj);
      }
      if (instances.Count > 0)
        return true;
      instances = (IList<T>) null;
      return false;
    }
  }
}
