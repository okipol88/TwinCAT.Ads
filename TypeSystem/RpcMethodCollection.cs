// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.RpcMethodCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads;
using TwinCAT.Ads.Internal;
using TwinCAT.Ads.TypeSystem;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Collection of <see cref="T:TwinCAT.TypeSystem.IRpcMethod">RpcMethods.</see>
  /// </summary>
  public class RpcMethodCollection : 
    IRpcMethodCollection,
    IList<IRpcMethod>,
    ICollection<IRpcMethod>,
    IEnumerable<IRpcMethod>,
    IEnumerable
  {
    /// <summary>Internal list (organizing the list index)</summary>
    private List<IRpcMethod> _list = new List<IRpcMethod>();
    /// <summary>Dictionary MethodName--&gt;RpcMethod for fast search</summary>
    private Dictionary<string, IRpcMethod> _dict = new Dictionary<string, IRpcMethod>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static RpcMethodCollection? s_empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.RpcMethodCollection" /> class.
    /// </summary>
    public RpcMethodCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.RpcMethodCollection" /> class.
    /// </summary>
    /// <param name="methods">The methods.</param>
    public RpcMethodCollection(IEnumerable<IRpcMethod> methods) => this.AddRange(methods);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.RpcMethodCollection" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    internal RpcMethodCollection(AdsMethodEntry[]? coll)
    {
      if (coll == null)
        return;
      for (int index = 0; index < coll.Length; ++index)
      {
        IRpcMethod irpcMethod = (IRpcMethod) new RpcMethod(coll[index]);
        if (!this.Contains(irpcMethod))
          this.Add(irpcMethod);
        else
          AdsModule.Trace.TraceWarning("RpcMethod '{0}' already contained in collection. Double definition in AdsMethodEntry", new object[1]
          {
            (object) irpcMethod.Name
          });
      }
    }

    /// <summary>Gets the empty collection</summary>
    /// <value>The empty collection</value>
    public static RpcMethodCollection Empty
    {
      get
      {
        if (RpcMethodCollection.s_empty == null)
          RpcMethodCollection.s_empty = new RpcMethodCollection();
        return RpcMethodCollection.s_empty;
      }
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
    public int IndexOf(IRpcMethod item) => this._list.IndexOf(item);

    /// <summary>
    /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
    /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    public void Insert(int index, IRpcMethod item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._dict.Add(item.Name, item);
      this._list.Insert(index, item);
    }

    /// <summary>
    /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
      this._dict.Remove(this._list[index].Name);
      this._list.RemoveAt(index);
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <returns>RpcMethod.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public IRpcMethod this[int index]
    {
      get => this._list[index];
      set => throw new NotImplementedException();
    }

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public void Add(IRpcMethod item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._dict.Add(item.Name, item);
      this._list.Add(item);
    }

    /// <summary>Adds the range of Methods.</summary>
    /// <param name="methods">The methods.</param>
    public void AddRange(IEnumerable<IRpcMethod> methods)
    {
      foreach (IRpcMethod method in methods)
        this.Add(method);
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    public void Clear()
    {
      this._dict.Clear();
      this._list.Clear();
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
    public bool Contains(IRpcMethod item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      return this._dict.ContainsKey(item.Name);
    }

    /// <summary>
    /// Determines whether this collection contains the specified method name.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <returns><c>true</c> if contained.; otherwise, <c>false</c>.</returns>
    public bool Contains(string methodName)
    {
      IRpcMethod method = (IRpcMethod) null;
      return this.TryGetMethod(methodName, out method);
    }

    public bool TryGetMethod(string methodName, [NotNullWhen(true)] out IRpcMethod? method) => this._dict.TryGetValue(methodName, out method);

    public bool TryGetMethod(int vTableIndex, [NotNullWhen(true)] out IRpcMethod? method)
    {
      if (vTableIndex >= 0 && vTableIndex < this._list.Count)
      {
        method = this._list[vTableIndex];
        return true;
      }
      method = (IRpcMethod) null;
      return false;
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IRpcMethod" /> with the specified method name.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>RpcMethod.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException"></exception>
    public IRpcMethod this[string methodName]
    {
      get
      {
        IRpcMethod method = (IRpcMethod) null;
        if (!this.TryGetMethod(methodName, out method))
          throw new KeyNotFoundException();
        return method;
      }
    }

    /// <summary>Copies to.</summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(IRpcMethod[] array, int arrayIndex) => this._list.CopyTo(array, arrayIndex);

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <value>The count.</value>
    public int Count => this._list.Count;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => false;

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public bool Remove(IRpcMethod item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._dict.Remove(item.Name);
      return this._list.Remove(item);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<IRpcMethod> GetEnumerator() => (IEnumerator<IRpcMethod>) this._list.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._list.GetEnumerator();

    /// <summary>
    /// Gets a read only collection of this <see cref="T:TwinCAT.TypeSystem.RpcMethodCollection" />
    /// </summary>
    /// <returns>ReadOnlyRpcMethodCollection.</returns>
    /// <value>Returns a read only version of this <see cref="T:TwinCAT.TypeSystem.RpcMethodCollection" /></value>
    public ReadOnlyRpcMethodCollection AsReadOnly() => new ReadOnlyRpcMethodCollection(this);
  }
}
