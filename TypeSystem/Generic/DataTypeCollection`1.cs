// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.DataTypeCollection`1
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
  /// <summary>Data type collection</summary>
  /// <typeparam name="T"></typeparam>
  public class DataTypeCollection<T> : 
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable,
    IDataTypeCollection<T>
    where T : class, IDataType
  {
    /// <summary>Internal list of data types</summary>
    protected List<T> list = new List<T>();
    /// <summary>Dictionary (Type Name --&gt; DataType)</summary>
    private Dictionary<string, T> nameDict = new Dictionary<string, T>();
    /// <summary>
    /// Indicates that the <see cref="T:TwinCAT.TypeSystem.Generic.DataTypeCollection`1" /> is readonly
    /// </summary>
    private bool readOnly;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" /> class.
    /// </summary>
    public DataTypeCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.DataTypeCollection`1" /> class.
    /// </summary>
    /// <param name="types">The types.</param>
    public DataTypeCollection(IEnumerable<T> types) => this.AddRange(types);

    /// <summary>Clones this instance.</summary>
    /// <returns>DataTypeCollection&lt;T&gt;.</returns>
    public DataTypeCollection<T> Clone() => new DataTypeCollection<T>((IEnumerable<T>) this);

    /// <summary>
    /// Determines the Index of the specified <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item) => this.list.IndexOf(item);

    /// <summary>
    /// Inserts an <see cref="T:TwinCAT.TypeSystem.IDataType" /> into the <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" />.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="item">The item.</param>
    public void Insert(int index, T item)
    {
      this.nameDict.Add(((IDataType) (object) item).Name, item);
      this.list.Add(item);
    }

    /// <summary>
    /// Removes the <see cref="T:TwinCAT.TypeSystem.IDataType" /> object at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    public void RemoveAt(int index)
    {
      this.nameDict.Remove(((IDataType) (object) this.list[index]).Name);
      this.list.RemoveAt(index);
    }

    /// <summary>
    /// Gets or sets the <see cref="T:TwinCAT.TypeSystem.IDataType" /> at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public T this[int index]
    {
      get => this.list[index];
      set => throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IDataType" /> with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    public T this[string name] => this.nameDict[name];

    /// <summary>
    /// Determines whether the container contains the specified <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if contained; otherwise, <c>false</c>.</returns>
    public bool ContainsType(string name) => this.nameDict.ContainsKey(name);

    /// <summary>
    /// Tries to get the specified <see cref="T:TwinCAT.TypeSystem.IDataType" /> from the <see cref="T:TwinCAT.TypeSystem.IDataTypeCollection`1" />.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type (Out parameter)</param>
    /// <returns>true if found</returns>
    public bool TryGetType(string name, [NotNullWhen(true)] out T? type) => this.nameDict.TryGetValue(name, out type);

    /// <summary>
    /// Determines the specified <see cref="T:TwinCAT.TypeSystem.IDataType" />
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The <see cref="T:TwinCAT.TypeSystem.IDataType" /> if found, otherwise <c>NULL</c></returns>
    public T? LookupType(string name)
    {
      T type = default (T);
      this.TryGetType(name, out type);
      return type;
    }

    /// <summary>Adds the specified item to the collection.</summary>
    /// <param name="item">The item.</param>
    public void Add(T item)
    {
      this.nameDict.Add(((IDataType) (object) item).Name, item);
      this.list.Add(item);
    }

    /// <summary>Adds a range of types</summary>
    /// <param name="types">The types.</param>
    public void AddRange(IEnumerable<T> types)
    {
      if (types == null)
        throw new ArgumentNullException(nameof (types));
      foreach (T type in types)
        this.Add(type);
    }

    /// <summary>Clears the collection.</summary>
    public void Clear()
    {
      this.nameDict.Clear();
      this.list.Clear();
    }

    /// <summary>
    /// Determines whether this <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" /> contains the specified <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
    public bool Contains(T item) => this.ContainsType(((IDataType) (object) item).Name);

    /// <summary>
    /// Copies the data types to the specified array, starting at the array index.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(T[] array, int arrayIndex) => this.list.CopyTo(array, arrayIndex);

    /// <summary>
    /// Gets the count of contained <see cref="T:TwinCAT.TypeSystem.IDataType" />s.
    /// </summary>
    /// <value>The count.</value>
    public int Count => this.list.Count;

    /// <summary>
    /// Gets a value indicating whether this instance is read only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => this.readOnly;

    /// <summary>
    /// Removes the specified <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public bool Remove(T item)
    {
      this.nameDict.Remove(((IDataType) (object) item).Name);
      return this.list.Remove(item);
    }

    /// <summary>Gets the enumerator.</summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.list.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.list.GetEnumerator();

    /// <summary>
    /// Converts the <see cref="T:TwinCAT.TypeSystem.Generic.DataTypeCollection`1" /> into a <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />
    /// </summary>
    /// <returns>ReadOnlyDataTypeCollection&lt;T&gt;.</returns>
    public ReadOnlyDataTypeCollection<T> AsReadOnly() => new ReadOnlyDataTypeCollection<T>(this);
  }
}
