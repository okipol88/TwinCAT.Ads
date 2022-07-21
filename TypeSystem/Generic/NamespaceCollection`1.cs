// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.NamespaceCollection`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem.Generic
{
  /// <summary>Generic class for Namespace collections</summary>
  /// <typeparam name="T"></typeparam>
  public class NamespaceCollection<T> : 
    IList<INamespace<T>>,
    ICollection<INamespace<T>>,
    IEnumerable<INamespace<T>>,
    IEnumerable,
    INamespaceContainer<T>
    where T : class, IDataType
  {
    /// <summary>List of Namespaces</summary>
    private List<INamespace<T>> list = new List<INamespace<T>>();
    /// <summary>Dictionary NamespaceName --&gt; INamespace</summary>
    private Dictionary<string, INamespace<T>> namespaceDict = new Dictionary<string, INamespace<T>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    /// <summary>Dictionary FullPath -&gt; IDataType</summary>
    private Dictionary<string, T> allTypes = new Dictionary<string, T>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    /// <summary>Read only indicator</summary>
    private bool readOnly;

    /// <summary>Dictionary FullPath -&gt; IDataType</summary>
    protected IDictionary<string, T> InnerAllTypes => (IDictionary<string, T>) this.allTypes;

    public int IndexOf(INamespace<T> item) => this.list.IndexOf(item);

    public void Insert(int index, INamespace<T> item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this.namespaceDict.Add(item.Name, item);
      this.list.Insert(index, item);
    }

    /// <summary>
    /// Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
      INamespace<T> inamespace = this.list[index];
      this.list.RemoveAt(index);
      this.namespaceDict.Remove(inamespace.Name);
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public INamespace<T> this[int index]
    {
      get => this.list[index];
      set => throw new NotImplementedException();
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="str">The STR.</param>
    /// <returns></returns>
    public INamespace<T> this[string str] => this.namespaceDict[str];

    public void Add(INamespace<T> item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this.namespaceDict.Add(item.Name, item);
      this.list.Add(item);
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    /// </summary>
    public void Clear()
    {
      this.namespaceDict.Clear();
      this.list.Clear();
    }

    public bool Contains(INamespace<T> item) => this.list.Contains(item);

    public void CopyTo(INamespace<T>[] array, int arrayIndex) => this.list.CopyTo(array, arrayIndex);

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    /// </summary>
    /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
    public int Count => this.list.Count;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
    /// </summary>
    /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
    public bool IsReadOnly => this.readOnly;

    public bool Remove(INamespace<T> item)
    {
      bool flag = item != null ? this.list.Remove(item) : throw new ArgumentNullException(nameof (item));
      this.namespaceDict.Remove(item.Name);
      return flag;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<INamespace<T>> GetEnumerator() => (IEnumerator<INamespace<T>>) this.list.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.list.GetEnumerator();

    /// <summary>
    /// Determines whether the specified name contains namespace.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>
    ///   <c>true</c> if the specified name contains namespace; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsNamespace(string name) => this.namespaceDict.ContainsKey(name);

    public bool TryGetNamespace(string name, [NotNullWhen(true)] out INamespace<T>? nspace) => this.namespaceDict.TryGetValue(name, out nspace);

    /// <summary>Tries to get the specified type.</summary>
    /// <param name="typeName">Data type name</param>
    /// <param name="dataType">The found data type (out-parameter).</param>
    /// <returns>true if found, false if not contained.</returns>
    /// <exception cref="T:System.ArgumentNullException">typeName</exception>
    /// <exception cref="T:System.ArgumentException"></exception>
    public bool TryGetType(string typeName, [NotNullWhen(true)] out T? dataType)
    {
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentNullException(nameof (typeName));
      foreach (INamespace<T> inamespace in this)
      {
        if (inamespace.DataTypes.TryGetType(typeName, ref dataType))
          return true;
      }
      dataType = default (T);
      return false;
    }

    /// <summary>Tries to get the data type by full name.</summary>
    /// <param name="fullname">DataTypes full name.</param>
    /// <param name="dataType">Found data type (out-parameter).</param>
    /// <returns>true if found, false if not contained.</returns>
    public bool TryGetTypeByFullName(string fullname, [NotNullWhen(true)] out T? dataType) => this.allTypes.TryGetValue(fullname, out dataType);

    /// <summary>Gets all types included in all namespaces.</summary>
    /// <value>All types.</value>
    public IDataTypeCollection<T> AllTypes => (IDataTypeCollection<T>) this.AllTypesInternal.AsReadOnly();

    /// <summary>Gets all types included in all namespaces</summary>
    /// <value>All types internal.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public DataTypeCollection<T> AllTypesInternal => new DataTypeCollection<T>((IEnumerable<T>) this.allTypes.Values);
  }
}
