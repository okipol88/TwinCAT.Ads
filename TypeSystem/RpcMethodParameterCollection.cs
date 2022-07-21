// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.RpcMethodParameterCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Collection of RPC method parameters</summary>
  public class RpcMethodParameterCollection : 
    IRpcMethodParameterCollection,
    IList<IRpcMethodParameter>,
    ICollection<IRpcMethodParameter>,
    IEnumerable<IRpcMethodParameter>,
    IEnumerable
  {
    /// <summary>Internal list organizing the index of the parameters.</summary>
    private List<IRpcMethodParameter> _list = new List<IRpcMethodParameter>();
    /// <summary>
    /// Dictionary of ParamaterName-&gt;RpcMethodParameter for fast find.
    /// </summary>
    private Dictionary<string, IRpcMethodParameter> _dict = new Dictionary<string, IRpcMethodParameter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.RpcMethodParameterCollection" /> class.
    /// </summary>
    public RpcMethodParameterCollection()
    {
    }

    /// <summary>
    /// Gets an Empty <see cref="T:TwinCAT.TypeSystem.RpcMethodParameterCollection" />
    /// </summary>
    /// <value>The empty.</value>
    public static RpcMethodParameterCollection Empty => new RpcMethodParameterCollection();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.RpcMethodParameterCollection" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    public RpcMethodParameterCollection(IEnumerable<IRpcMethodParameter> coll)
    {
      if (coll == null)
        throw new ArgumentNullException(nameof (coll));
      foreach (IRpcMethodParameter irpcMethodParameter in coll)
        this.Add(irpcMethodParameter);
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
    public int IndexOf(IRpcMethodParameter item) => this._list.IndexOf(item);

    /// <summary>
    /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
    /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    public void Insert(int index, IRpcMethodParameter item)
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
      this._dict.Remove(this[index].Name);
      this._list.RemoveAt(index);
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <returns>RpcMethodParameter.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public IRpcMethodParameter this[int index]
    {
      get => this._list[index];
      set => throw new NotImplementedException();
    }

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public void Add(IRpcMethodParameter item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._dict.Add(item.Name, item);
      this._list.Add(item);
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
    public bool Contains(IRpcMethodParameter item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      return this._dict.ContainsKey(item.Name);
    }

    /// <summary>Copies to.</summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(IRpcMethodParameter[] array, int arrayIndex) => this._list.CopyTo(array, arrayIndex);

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
    public bool Remove(IRpcMethodParameter item)
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
    public IEnumerator<IRpcMethodParameter> GetEnumerator() => (IEnumerator<IRpcMethodParameter>) this._list.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._list.GetEnumerator();

    /// <summary>
    /// Returns a read only version of this <see cref="T:TwinCAT.TypeSystem.RpcMethodParameterCollection" />
    /// </summary>
    /// <returns>ReadOnlyMethodParameterCollection.</returns>
    /// <value>Collection as read only version.</value>
    public ReadOnlyMethodParameterCollection AsReadOnly() => new ReadOnlyMethodParameterCollection(this);

    /// <summary>Gets the length is parameter.</summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>IRpcMethodParameter.</returns>
    /// <exception cref="T:System.ArgumentNullException">parameter</exception>
    /// <exception cref="T:System.ArgumentException">Parameter is not contained in ParameterList - parameter</exception>
    public IRpcMethodParameter? GetLengthIsParameter(
      IRpcMethodParameter parameter)
    {
      if (parameter == null)
        throw new ArgumentNullException(nameof (parameter));
      if (parameter.LengthIsParameterIndex <= 0)
        return (IRpcMethodParameter) null;
      if (!this.Contains(parameter))
        throw new ArgumentException("Parameter is not contained in ParameterList", nameof (parameter));
      return this._list[parameter.LengthIsParameterIndex - 1];
    }
  }
}
