// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.EnumValueCollection`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Collection of <see cref="T:TwinCAT.TypeSystem.EnumValue`1">EnumValues</see>
  /// </summary>
  /// <typeparam name="T">Base type of enum</typeparam>
  public class EnumValueCollection<T> : 
    IList<EnumValue<T>>,
    ICollection<EnumValue<T>>,
    IEnumerable<EnumValue<T>>,
    IEnumerable,
    IEnumValueCollection<EnumValue<T>, T>
    where T : struct, IConvertible
  {
    /// <summary>Internal list organizing the Index</summary>
    private List<EnumValue<T>> _list = new List<EnumValue<T>>();
    /// <summary>Dictionary Name --&gt; EnumValue for fast find</summary>
    private Dictionary<string, EnumValue<T>> _nameValueDict = new Dictionary<string, EnumValue<T>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.EnumValueCollection`1" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    /// <exception cref="T:System.ArgumentNullException">coll</exception>
    internal EnumValueCollection(AdsEnumInfoEntry[] coll)
    {
      if (coll == null)
        throw new ArgumentNullException(nameof (coll));
      foreach (AdsEnumInfoEntry entry in coll)
        this.Add(new EnumValue<T>(entry));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.EnumValueCollection`1" /> class.
    /// </summary>
    public EnumValueCollection()
    {
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
    public int IndexOf(EnumValue<T> item) => this._list.IndexOf(item);

    /// <summary>
    /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
    /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
    public void Insert(int index, EnumValue<T> item)
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
      EnumValue<T> enumValue = this._list[index];
      this._list.RemoveAt(index);
      this._nameValueDict.Remove(enumValue.Name);
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <returns>EnumValue&lt;T&gt;.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public EnumValue<T> this[int index]
    {
      get => this._list[index];
      set => throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the enumeration value <paramref name="str" /> from the string representation.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public T this[string str]
    {
      get
      {
        StringComparer cmp = StringComparer.OrdinalIgnoreCase;
        return this._list.Single<EnumValue<T>>((Func<EnumValue<T>, bool>) (element => cmp.Compare(element.Name, str) == 0)).Primitive;
      }
    }

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public void Add(EnumValue<T> item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this._nameValueDict.Add(item.Name, item);
      this._list.Add(item);
    }

    /// <summary>Adds the value.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>EnumValueCollection&lt;T&gt;.</returns>
    public EnumValueCollection<T> AddValue(string name, T value)
    {
      this.Add(new EnumValue<T>(name, value, Marshal.SizeOf(typeof (T))));
      return this;
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
    public bool Contains(EnumValue<T> item)
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
    public bool Contains(T value)
    {
      for (int index = 0; index < this.Count; ++index)
      {
        if (this._list[index].Primitive.Equals((object) value))
          return true;
      }
      return false;
    }

    /// <summary>Tries the get information.</summary>
    /// <param name="val">The value.</param>
    /// <param name="ei">The ei.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetInfo(T val, [NotNullWhen(true)] out EnumValue<T>? ei)
    {
      for (int index = 0; index < this.Count; ++index)
      {
        if (this._list[index].Primitive.Equals((object) val))
        {
          ei = this._list[index];
          return true;
        }
      }
      ei = (EnumValue<T>) null;
      return false;
    }

    /// <summary>Parse the specified string to the enum value.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string name, [NotNullWhen(true)] out T value)
    {
      EnumValue<T> enumValue = (EnumValue<T>) null;
      if (this._nameValueDict.TryGetValue(name, out enumValue))
      {
        value = enumValue.Primitive;
        return true;
      }
      value = default (T);
      return false;
    }

    /// <summary>Parse the specified string to the enum value.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string name, [NotNullWhen(true)] out EnumValue<T>? value) => this._nameValueDict.TryGetValue(name, out value);

    /// <summary>Parses the specified string to the Enum value.</summary>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">name</exception>
    public T Parse(string name)
    {
      T obj = default (T);
      if (this.TryParse(name, out obj))
        return obj;
      string str = string.Join(",", this.GetNames());
      throw new ArgumentOutOfRangeException(nameof (name), (object) name, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The value '{0}' is not one of the valid enum values '{1}'", (object) name, (object) str));
    }

    /// <summary>Copies the entire list.</summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(EnumValue<T>[] array, int arrayIndex) => this._list.CopyTo(array, arrayIndex);

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
    public bool Remove(EnumValue<T> item) => throw new NotImplementedException();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<EnumValue<T>> GetEnumerator() => (IEnumerator<EnumValue<T>>) this._list.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._list.GetEnumerator();

    internal AdsEnumInfoEntry[] ToArray(IStringMarshaler marshaler)
    {
      AdsEnumInfoEntry[] array = new AdsEnumInfoEntry[this.Count];
      for (int index = 0; index < this.Count; ++index)
        array[index] = this[index].ToAdsEnumEntry(marshaler);
      return array;
    }

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
    public T[] GetValues()
    {
      T[] values = new T[this.Count];
      for (int index = 0; index < this.Count; ++index)
        values[index] = this._list[index].Primitive;
      return values;
    }

    /// <summary>Gets as read only.</summary>
    /// <returns>ReadOnlyEnumValueCollection&lt;T&gt;.</returns>
    /// <value>As read only.</value>
    public ReadOnlyEnumValueCollection<T> AsReadOnly() => new ReadOnlyEnumValueCollection<T>(this);

    /// <summary>
    /// Performs an explicit conversion from <see cref="T:TwinCAT.TypeSystem.EnumValueCollection`1" /> to <see cref="T:TwinCAT.TypeSystem.EnumValueCollection" />.
    /// </summary>
    /// <param name="coll">The coll.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator EnumValueCollection(
      EnumValueCollection<T> coll)
    {
      IEnumValue[] coll1 = coll != null ? new IEnumValue[coll.Count] : throw new ArgumentNullException(nameof (coll));
      for (int index = 0; index < coll.Count; ++index)
        coll1[index] = (IEnumValue) coll[index];
      return new EnumValueCollection((IEnumerable<IEnumValue>) coll1);
    }

    /// <summary>Return an Empty Collection.</summary>
    /// <returns>EnumValueCollection&lt;T&gt;.</returns>
    public static EnumValueCollection<T> Empty => new EnumValueCollection<T>(Array.Empty<AdsEnumInfoEntry>());
  }
}
