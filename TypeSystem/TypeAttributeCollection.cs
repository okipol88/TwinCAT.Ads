// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.TypeAttributeCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Collection of <see cref="T:TwinCAT.TypeSystem.ITypeAttribute">AdsAttributes</see>
  /// </summary>
  public class TypeAttributeCollection : 
    ITypeAttributeCollection,
    IList<ITypeAttribute>,
    ICollection<ITypeAttribute>,
    IEnumerable<ITypeAttribute>,
    IEnumerable
  {
    /// <summary>List of Attributes</summary>
    private List<ITypeAttribute> list = new List<ITypeAttribute>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.TypeAttributeCollection" /> class.
    /// </summary>
    public TypeAttributeCollection()
    {
    }

    /// <summary>
    /// Returns an Empty <see cref="T:TwinCAT.TypeSystem.TypeAttributeCollection" />.
    /// </summary>
    /// <returns>TypeAttributeCollection.</returns>
    public static TypeAttributeCollection Empty => new TypeAttributeCollection();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.TypeAttributeCollection" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    public TypeAttributeCollection(IEnumerable<ITypeAttribute> coll)
    {
      if (coll == null || coll == null)
        return;
      this.AddRange(coll);
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
    public int IndexOf(ITypeAttribute item) => this.list.IndexOf(item);

    /// <summary>
    /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
    /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    public void Insert(int index, ITypeAttribute item) => this.list.Insert(index, item);

    /// <summary>
    /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
      ITypeAttribute itypeAttribute = this.list[index];
      this.list.RemoveAt(index);
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <returns>AdsAttribute.</returns>
    public ITypeAttribute this[int index]
    {
      get => this.list[index];
      set
      {
        ITypeAttribute itypeAttribute = this.list[index];
        this.list[index] = value;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.String" /> with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException"></exception>
    public string this[string name]
    {
      get
      {
        ITypeAttribute att = (ITypeAttribute) null;
        if (this.TryGetAttribute(name, out att))
          return att.Value;
        throw new KeyNotFoundException();
      }
    }

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public void Add(ITypeAttribute item) => this.list.Add(item);

    /// <summary>Adds the range.</summary>
    /// <param name="items">The items.</param>
    public void AddRange(IEnumerable<ITypeAttribute> items) => this.list.AddRange(items);

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    public void Clear() => this.list.Clear();

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
    public bool Contains(ITypeAttribute item) => this.list.Contains(item);

    /// <summary>
    /// Determines whether this <see cref="T:TwinCAT.TypeSystem.TypeAttributeCollection" /> contains the <see cref="T:TwinCAT.TypeSystem.ITypeAttribute" /> with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.</returns>
    public bool Contains(string name)
    {
      StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
      foreach (ITypeAttribute itypeAttribute in this.list)
      {
        if (ordinalIgnoreCase.Compare(name, itypeAttribute.Name) == 0)
          return true;
      }
      return false;
    }

    /// <summary>Copies to.</summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(ITypeAttribute[] array, int arrayIndex) => this.list.CopyTo(array, arrayIndex);

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <value>The count.</value>
    public int Count => this.list.Count;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => true;

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public bool Remove(ITypeAttribute item) => this.list.Remove(item);

    /// <summary>
    /// Removes the specified <see cref="T:TwinCAT.TypeSystem.ITypeAttribute" /> from the <see cref="T:TwinCAT.TypeSystem.TypeAttributeCollection" />
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Remove(string name)
    {
      StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
      ITypeAttribute itypeAttribute = (ITypeAttribute) null;
      int index1 = -1;
      for (int index2 = 0; index2 < this.list.Count; ++index2)
      {
        if (ordinalIgnoreCase.Compare(name, this.list[index2].Name) == 0)
        {
          itypeAttribute = this.list[index2];
          index1 = index2;
          break;
        }
      }
      if (itypeAttribute != null)
        this.list.RemoveAt(index1);
      return itypeAttribute != null;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<ITypeAttribute> GetEnumerator() => (IEnumerator<ITypeAttribute>) this.list.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.list.GetEnumerator();

    /// <summary>
    /// Gets a read only version of this <see cref="T:TwinCAT.TypeSystem.TypeAttributeCollection" />
    /// </summary>
    /// <returns>ReadOnlyAttributeCollection.</returns>
    /// <value>As read only.</value>
    public ReadOnlyTypeAttributeCollection AsReadOnly() => new ReadOnlyTypeAttributeCollection(this);

    public bool TryGetAttribute(string name, [NotNullWhen(true)] out ITypeAttribute? att)
    {
      StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
      att = (ITypeAttribute) null;
      foreach (ITypeAttribute itypeAttribute in this.list)
      {
        if (ordinalIgnoreCase.Compare(name, itypeAttribute.Name) == 0)
        {
          att = itypeAttribute;
          return true;
        }
      }
      return false;
    }

    /// <summary>Tries to get the specified Attribute value.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetValue(string name, [NotNullWhen(true)] out string? value)
    {
      ITypeAttribute att;
      if (this.TryGetAttribute(name, out att))
      {
        value = att.Value;
        return true;
      }
      value = (string) null;
      return false;
    }

    internal AdsAttributeEntry[] ToEntries()
    {
      AdsAttributeEntry[] entries = new AdsAttributeEntry[this.list.Count];
      for (int index = 0; index < this.list.Count; ++index)
        entries[index] = new AdsAttributeEntry(this.list[index]);
      return entries;
    }
  }
}
