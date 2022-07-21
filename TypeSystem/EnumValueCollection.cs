// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.EnumValueCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class EnumValueCollection.</summary>
  public class EnumValueCollection : 
    IList<IEnumValue>,
    ICollection<IEnumValue>,
    IEnumerable<IEnumValue>,
    IEnumerable,
    IEnumValueCollection,
    IEnumValueCollection<IEnumValue, IConvertible>
  {
    /// <summary>Internal list organizing the Index</summary>
    private List<IEnumValue> _list = new List<IEnumValue>();
    /// <summary>Dictionary Name --&gt; EnumValue for fast find</summary>
    private Dictionary<string, IEnumValue> _nameValueDict = new Dictionary<string, IEnumValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.EnumValueCollection`1" /> class.
    /// </summary>
    /// <param name="typeId">The type identifier.</param>
    /// <param name="coll">The coll.</param>
    /// <exception cref="T:System.ArgumentNullException">coll</exception>
    internal EnumValueCollection(AdsDataTypeId typeId, AdsEnumInfoEntry[] coll)
    {
      if (coll == null)
        throw new ArgumentNullException(nameof (coll));
      foreach (AdsEnumInfoEntry enumInfo in coll)
        this.Add(EnumValueFactory.Create(typeId, enumInfo));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.EnumValueCollection" /> class (for internal use only).
    /// </summary>
    /// <param name="coll">The coll.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnumValueCollection(IEnumerable<IEnumValue> coll)
    {
      if (coll == null)
        throw new ArgumentNullException(nameof (coll));
      foreach (IEnumValue ienumValue in coll)
        this.Add(ienumValue);
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
    public int IndexOf(IEnumValue item) => this._list.IndexOf(item);

    /// <summary>
    /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
    /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    public void Insert(int index, IEnumValue item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._nameValueDict.Add(item.Name, item);
      this._list.Insert(index, item);
    }

    /// <summary>
    /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
      IEnumValue ienumValue = this._list[index];
      this._list.RemoveAt(index);
      this._nameValueDict.Remove(ienumValue.Name);
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <returns>EnumValue&lt;T&gt;.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public IEnumValue this[int index]
    {
      get => this._list[index];
      set => throw new NotImplementedException();
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="name">The name of the value</param>
    /// <returns>EnumValue&lt;T&gt;.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public IConvertible this[string name] => (IConvertible) this._nameValueDict[name].Primitive;

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public void Add(IEnumValue item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._nameValueDict.Add(item.Name, item);
      this._list.Add(item);
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    public void Clear()
    {
      this._nameValueDict.Clear();
      this._list.Clear();
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
    public bool Contains(IEnumValue item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      return this._nameValueDict.ContainsKey(item.Name);
    }

    /// <summary>Determines whether [contains] [the specified name].</summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.</returns>
    public bool Contains(string name) => this._nameValueDict.ContainsKey(name);

    /// <summary>Determines whether [contains] [the specified value].</summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
    public bool Contains(object value)
    {
      for (int index = 0; index < this.Count; ++index)
      {
        if (this._list[index].Primitive.Equals(value))
          return true;
      }
      return false;
    }

    public bool TryGetInfo(object val, [NotNullWhen(true)] out IEnumValue? ei)
    {
      for (int index = 0; index < this.Count; ++index)
      {
        if (this._list[index].Primitive.Equals(val))
        {
          ei = this._list[index];
          return true;
        }
      }
      ei = (IEnumValue) null;
      return false;
    }

    /// <summary>Parse the specified string to the enum value.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string name, out IConvertible value)
    {
      IEnumValue ienumValue = (IEnumValue) null;
      if (this._nameValueDict.TryGetValue(name, out ienumValue))
      {
        value = (IConvertible) ienumValue.Primitive;
        return true;
      }
      value = (IConvertible) 0;
      return false;
    }

    public bool TryParse(string name, [NotNullWhen(true)] out IEnumValue? value) => this._nameValueDict.TryGetValue(name, out value);

    /// <summary>Parses the specified string to the Enum value.</summary>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">name</exception>
    public IConvertible Parse(string name)
    {
      IConvertible convertible = (IConvertible) null;
      if (this.TryParse(name, out convertible))
        return convertible;
      throw new ArgumentOutOfRangeException(nameof (name));
    }

    /// <summary>Converts to array.</summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <returns>AdsEnumInfoEntry[].</returns>
    internal AdsEnumInfoEntry[] ToArray(IStringMarshaler marshaler)
    {
      AdsEnumInfoEntry[] array = new AdsEnumInfoEntry[this.Count];
      for (int index = 0; index < this.Count; ++index)
        array[index] = new AdsEnumInfoEntry(this._list[index].Name, this._list[index].RawValue, marshaler);
      return array;
    }

    /// <summary>Copies the entire list.</summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(IEnumValue[] array, int arrayIndex) => this._list.CopyTo(array, arrayIndex);

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
    /// <exception cref="T:System.NotImplementedException"></exception>
    public bool Remove(IEnumValue item) => throw new NotImplementedException();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<IEnumValue> GetEnumerator() => (IEnumerator<IEnumValue>) this._list.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._list.GetEnumerator();

    /// <summary>Gets the names.</summary>
    /// <returns>System.String[].</returns>
    public string[] GetNames()
    {
      string[] names = new string[this.Count];
      for (int index = 0; index < this.Count; ++index)
        names[index] = this._list[index].Name;
      return names;
    }

    /// <summary>Gets the values.</summary>
    /// <returns>T[].</returns>
    public IConvertible[] GetValues()
    {
      IConvertible[] values = new IConvertible[this.Count];
      for (int index = 0; index < this.Count; ++index)
        values[index] = (IConvertible) this._list[index].Primitive;
      return values;
    }

    /// <summary>Gets as read only.</summary>
    /// <returns>ReadOnlyEnumValueCollection.</returns>
    /// <value>As read only.</value>
    public ReadOnlyEnumValueCollection AsReadOnly() => new ReadOnlyEnumValueCollection(this);
  }
}
