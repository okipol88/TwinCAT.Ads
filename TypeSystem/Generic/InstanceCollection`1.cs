// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.InstanceCollection`1
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
using System.Runtime.CompilerServices;


#nullable enable
namespace TwinCAT.TypeSystem.Generic
{
  /// <summary>
  /// Base class for <see cref="T:TwinCAT.TypeSystem.IInstance" /> object collections (abstract).
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class InstanceCollection<T> : 
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable,
    IInstanceCollection<T>
    where T : class, IInstance
  {
    /// <summary>List of Instances</summary>
    private List<T> _list = new List<T>();
    /// <summary>The _path dictionary</summary>
    private Dictionary<string, T> _pathDict = new Dictionary<string, T>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// The mode this <see cref="T:TwinCAT.TypeSystem.Generic.InstanceCollection`1" /> is working in.
    /// </summary>
    private InstanceCollectionMode mode = (InstanceCollectionMode) 1;

    /// <summary>Gets the LIst of instances.</summary>
    /// <value>The inner list.</value>
    protected IList<T> InnerList => (IList<T>) this._list;

    /// <summary>The Path dictionary</summary>
    protected IDictionary<string, T> InnerPathDict => (IDictionary<string, T>) this._pathDict;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.InstanceCollection`1" /> class.
    /// </summary>
    /// <param name="mode">The mode.</param>
    protected InstanceCollection(InstanceCollectionMode mode) => this.mode = mode;

    /// <summary>
    /// The mode this <see cref="T:TwinCAT.TypeSystem.Generic.InstanceCollection`1" /> is working in.
    /// </summary>
    public InstanceCollectionMode Mode => this.mode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.InstanceCollection`1" /> class.
    /// </summary>
    /// <param name="coll">The copy collection</param>
    /// <param name="mode">The mode.</param>
    protected InstanceCollection(IEnumerable<T> coll, InstanceCollectionMode mode)
      : this(mode)
    {
      this.AddRange(coll);
    }

    /// <summary>
    /// Determines the index of the specified <see cref="T:TwinCAT.TypeSystem.IInstance" />.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item) => this._list.IndexOf(item);

    /// <summary>
    /// Inserts the specified <see cref="T:TwinCAT.TypeSystem.IInstance" /> at the specified index.
    /// </summary>
    /// <param name="index">The instance.</param>
    /// <param name="instance">The item.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// index
    /// or
    /// index
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    public virtual void Insert(int index, T instance)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index));
      if ((object) instance == null)
        throw new ArgumentNullException(nameof (instance));
      if (index > this.Count)
        throw new ArgumentOutOfRangeException(nameof (index));
      if (this.mode == 2)
      {
        string str;
        if (SymbolParser.TryParseParentPath((IInstance) (object) instance, out string _, out str))
          throw new ArgumentOutOfRangeException(nameof (instance), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Instance '{0}'is not a root object. Cannot Insert!", (object) ((IInstance) (object) instance).InstancePath));
        string indicesStr = (string) null;
        string instanceName = (string) null;
        if (SymbolParser.TryParseArrayElement(((IInstance) (object) instance).InstancePath, out str, out instanceName, out indicesStr, out IList<int[]> _, out ArrayIndexType? _))
          throw new ArgumentOutOfRangeException(nameof (instance), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Instance '{0}'is not a root object. Cannot Insert!", (object) ((IInstance) (object) instance).InstancePath));
      }
      this._pathDict.Add(((IInstance) (object) instance).InstancePath, instance);
      this._list.Insert(index, instance);
    }

    /// <summary>
    /// Removes the <see cref="T:TwinCAT.TypeSystem.IInstance" /> at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    public virtual void RemoveAt(int index)
    {
      T obj = this._list[index];
      this._list.RemoveAt(index);
      this._pathDict.Remove(((IInstance) (object) obj).InstancePath);
    }

    /// <summary>
    /// Gets or sets the <see cref="T:TwinCAT.TypeSystem.IInstance" /> at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public T this[int index]
    {
      get => this._list[index];
      set => throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IInstance" /> with the specified instance path.
    /// </summary>
    /// <param name="instanceSpecifier">The instance path or Instance Name (dependent of <see cref="P:TwinCAT.TypeSystem.Generic.InstanceCollection`1.Mode" /> setting)</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    /// <exception cref="T:System.ArgumentException"></exception>
    /// <remarks>Dependent what this <see cref="T:TwinCAT.TypeSystem.Generic.InstanceCollection`1" /> contains configured by the <see cref="T:TwinCAT.TypeSystem.InstanceCollectionMode" />
    /// the instance specifier should be the <see cref="P:TwinCAT.TypeSystem.IInstance.InstanceName" /> or the <see cref="P:TwinCAT.TypeSystem.IInstance.InstancePath" />.
    /// </remarks>
    public T this[string instanceSpecifier]
    {
      get
      {
        T symbol = default (T);
        if (!this.TryGetInstance(instanceSpecifier, out symbol))
          throw new KeyNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "InstancePath '{0}' not found in InstanceCollection!", (object) instanceSpecifier));
        return symbol;
      }
    }

    internal string createUniquepathName(IInstance instance)
    {
      string str1 = this.mode != null ? instance.InstancePath : instance.InstanceName;
      int num = 0;
      string str2 = str1;
      string key;
      DefaultInterpolatedStringHandler interpolatedStringHandler;
      for (key = str1; this._pathDict.ContainsKey(key); key = interpolatedStringHandler.ToStringAndClear())
      {
        ++num;
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
        interpolatedStringHandler.AppendFormatted(str2);
        interpolatedStringHandler.AppendLiteral("_");
        interpolatedStringHandler.AppendFormatted<int>(num);
      }
      return key;
    }

    internal bool isUnique(IInstance instance) => this.mode == null ? !this._pathDict.ContainsKey(instance.InstanceName) : !this._pathDict.ContainsKey(instance.InstancePath);

    /// <summary>Adds the specified item.</summary>
    /// <param name="item">The item.</param>
    public virtual void Add(T item)
    {
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
      if (this.mode == null)
      {
        this._pathDict.Add(((IInstance) (object) item).InstanceName, item);
        this._list.Add(item);
      }
      else if (this.mode == 1)
      {
        this._pathDict.Add(((IInstance) (object) item).InstancePath, item);
        this._list.Add(item);
      }
      else
      {
        if (this.mode != 2)
          throw new NotSupportedException();
        IList<int[]> jaggedIndices = (IList<int[]>) null;
        string indicesStr = (string) null;
        string instanceName = (string) null;
        ArrayIndexType? type = new ArrayIndexType?();
        string str;
        if (SymbolParser.TryParseParentPath((IInstance) (object) item, out string _, out str) || SymbolParser.TryParseArrayElement(((IInstance) (object) item).InstancePath, out str, out instanceName, out indicesStr, out jaggedIndices, out type))
          return;
        this._list.Add(item);
        string key = ((IInstance) (object) item).InstanceName;
        if (((IInstance) (object) item).InstancePath[0] == '.')
          key = key.Insert(0, ".");
        this._pathDict.Add(key, item);
      }
    }

    /// <summary>Adds the specified items to this collection.</summary>
    /// <param name="items">The items.</param>
    public void AddRange(IEnumerable<T> items)
    {
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      foreach (T obj in items)
        this.Add(obj);
    }

    /// <summary>Clears this instance.</summary>
    public void Clear()
    {
      this._pathDict.Clear();
      this._list.Clear();
    }

    /// <summary>
    /// Determines whether this collection contains the specified <see cref="T:TwinCAT.TypeSystem.IInstance" />
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
    public bool Contains(T item)
    {
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
      return this._pathDict.ContainsKey(((IInstance) (object) item).InstancePath);
    }

    /// <summary>
    /// Determines whether this collection contains an <see cref="T:TwinCAT.TypeSystem.IInstance" /> with the specified InstanceName / InstancePath
    /// </summary>
    /// <param name="instanceSpecifier">The instance path or Instance Name (dependent of <see cref="P:TwinCAT.TypeSystem.Generic.InstanceCollection`1.Mode" /> setting)</param>
    /// <returns><c>true</c> if [contains] [the specified instance path]; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">instancePath</exception>
    /// <exception cref="T:System.ArgumentException"></exception>
    public bool Contains(string instanceSpecifier)
    {
      T symbol = default (T);
      return this.TryGetInstance(instanceSpecifier, out symbol);
    }

    /// <summary>
    /// Tries to get the <see cref="T:TwinCAT.TypeSystem.IInstance" />. of the specified path.
    /// </summary>
    /// <param name="instanceSpecifier">The instance path or Instance Name (dependent of <see cref="P:TwinCAT.TypeSystem.Generic.InstanceCollection`1.Mode" /> setting)</param>
    /// <param name="symbol">The symbol.</param>
    /// <returns><c>true</c> if the <see cref="T:TwinCAT.TypeSystem.IInstance" /> is found; otherwise, <c>false</c></returns>
    /// <exception cref="T:System.ArgumentNullException">instancePath</exception>
    /// <exception cref="T:System.ArgumentException"></exception>
    public bool TryGetInstance(string instanceSpecifier, [NotNullWhen(true)] out T? symbol)
    {
      if (string.IsNullOrEmpty(instanceSpecifier))
        throw new ArgumentOutOfRangeException(nameof (instanceSpecifier));
      switch ((int) this.mode)
      {
        case 0:
          return this._pathDict.TryGetValue(instanceSpecifier, out symbol);
        case 1:
          return this._pathDict.TryGetValue(instanceSpecifier, out symbol);
        case 2:
          return this.TryGetInstanceHierarchically(instanceSpecifier, out symbol);
        default:
          throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Tries to get the Symbol Hierarchically from a dotted instance path.
    /// </summary>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="symbol">The symbol.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <remarks>If we have provider.SymbolsInternal root object, the Roots are organized from different (virtual) namespaces.
    /// The need is to find the symbol independent independent of the root has a dotted instance path, or it is organized (bound) in a ChildInstance level.
    /// </remarks>
    internal bool TryGetInstanceHierarchically(string instancePath, [NotNullWhen(true)] out T? symbol)
    {
      if (string.IsNullOrEmpty(instancePath))
        throw new ArgumentOutOfRangeException(nameof (instancePath));
      if (this.mode == 1 && this._pathDict.TryGetValue(instancePath, out symbol))
        return true;
      bool flag = false;
      if (instancePath.Length > 0 && instancePath[0] == '.')
        flag = true;
      string[] strArray;
      if (flag)
      {
        string[] sourceArray = instancePath.Split(new char[1]
        {
          '.'
        });
        if (sourceArray.Length > 1)
          sourceArray[1] = sourceArray[1].Insert(0, ".");
        strArray = new string[sourceArray.Length - 1];
        Array.Copy((Array) sourceArray, 1, (Array) strArray, 0, sourceArray.Length - 1);
      }
      else
        strArray = instancePath.Split(new char[1]{ '.' });
      return InstanceCollection<T>.TryGetSubItem((IInstanceCollection<T>) this, strArray, 0, out symbol);
    }

    /// <summary>
    /// Determines whether the specified instance name is a Dereferencing Instance Name
    /// </summary>
    /// <param name="instanceName">InstanceName.</param>
    /// <param name="dereferencedInstance">The dereferenced instance name.</param>
    /// <param name="refLevels">The reference levels.</param>
    /// <returns><c>true</c> if the specified instance name is reference; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">instanceName</exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    private static bool IsReference(
      string instanceName,
      [NotNullWhen(true)] out string? dereferencedInstance,
      out int refLevels)
    {
      int length = !string.IsNullOrEmpty(instanceName) ? instanceName.IndexOf('^') : throw new ArgumentNullException(nameof (instanceName));
      dereferencedInstance = (string) null;
      refLevels = 0;
      if (length <= 0)
        return false;
      dereferencedInstance = instanceName.Substring(0, length);
      refLevels = 1;
      for (int index = length + 1; index < instanceName.Length; ++index)
      {
        if (instanceName[index] != '^')
          throw new NotSupportedException();
        ++refLevels;
      }
      return true;
    }

    private static T Dereference(T pointerSymbol, int refLevels)
    {
      T obj = pointerSymbol;
      for (int index = 0; index < refLevels; ++index)
        obj = (T) ((IList<ISymbol>) ((ISymbolInternal) (object) obj).SubSymbolsInternal)[0];
      return obj;
    }

    internal static bool TryGetSubItem(
      IInstanceCollection<T> coll,
      string[] pathSplit,
      int splitIndex,
      [NotNullWhen(true)] out T? symbol)
    {
      T obj1 = default (T);
      symbol = default (T);
      string str = pathSplit[splitIndex];
      string dereferencedInstance;
      int refLevels;
      bool flag1 = InstanceCollection<T>.IsReference(str, out dereferencedInstance, out refLevels);
      if (flag1)
        str = dereferencedInstance;
      IList<T> objList = (IList<T>) null;
      bool instanceByName = coll.TryGetInstanceByName(str, ref objList);
      IList<int[]> jaggedIndices = (IList<int[]>) null;
      bool flag2 = false;
      if (!instanceByName)
      {
        string fullPathWithoutIndexes = (string) null;
        string indicesStr = (string) null;
        string instanceName = (string) null;
        flag2 = SymbolParser.TryParseArrayElement(str, out fullPathWithoutIndexes, out instanceName, out indicesStr, out jaggedIndices, out ArrayIndexType? _);
        if (flag2)
          instanceByName = coll.TryGetInstanceByName(fullPathWithoutIndexes, ref objList);
      }
      if (instanceByName)
      {
        T obj2 = objList[0];
        if (flag1)
          obj2 = InstanceCollection<T>.Dereference(objList[0], refLevels);
        if (flag2)
        {
          ISymbol isymbol = (ISymbol) null;
          if (obj2 is IIndexedAccess iindexedAccess)
            iindexedAccess.TryGetElement(jaggedIndices, ref isymbol);
          obj2 = (T) isymbol;
        }
        if ((object) obj2 != null && splitIndex < pathSplit.Length - 1)
        {
          ISymbol isymbol = (ISymbol) (object) obj2;
          if (isymbol.IsContainerType)
          {
            ISymbolCollection<ISymbol> subSymbolsInternal = ((ISymbolInternal) isymbol).SubSymbolsInternal;
            if (subSymbolsInternal != null && ((ICollection<ISymbol>) subSymbolsInternal).Count > 0)
            {
              T symbol1 = default (T);
              ++splitIndex;
              symbol = !InstanceCollection<T>.TryGetSubItem((IInstanceCollection<T>) subSymbolsInternal, pathSplit, splitIndex, out symbol1) ? default (T) : symbol1;
            }
          }
        }
        else
          symbol = obj2;
      }
      return (object) symbol != null;
    }

    /// <summary>
    /// Copies this <see cref="T:TwinCAT.TypeSystem.Generic.InstanceCollection`1" /> to the specified array.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(T[] array, int arrayIndex) => this._list.CopyTo(array, arrayIndex);

    /// <summary>Gets the collection count.</summary>
    /// <value>The count.</value>
    public int Count => this._list.Count;

    /// <summary>
    /// Gets a value indicating whether this instance is read only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => true;

    /// <summary>Removes the specified item.</summary>
    /// <param name="item">The item.</param>
    /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public bool Remove(T item)
    {
      string str = (object) item != null ? ((IInstance) (object) item).InstancePath : throw new ArgumentNullException(nameof (item));
      string instanceName = ((IInstance) (object) item).InstanceName;
      int index = this.IndexOf(item);
      if (index < 0)
        return false;
      this.RemoveAt(index);
      return true;
    }

    /// <summary>Gets the enumerator.</summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this._list.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._list.GetEnumerator();

    /// <summary>
    /// Converts the <see cref="T:TwinCAT.TypeSystem.Generic.InstanceCollection`1" /> to an <see cref="T:TwinCAT.TypeSystem.Generic.ReadOnlyInstanceCollection`1" />
    /// </summary>
    /// <returns>ReadOnlyInstanceCollection&lt;T&gt;.</returns>
    public ReadOnlyInstanceCollection<T> AsReadOnly() => new ReadOnlyInstanceCollection<T>((IInstanceCollection<T>) this);

    /// <summary>Tries to get Instances by name.</summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="instances">The instances found.</param>
    /// <returns><c>true</c> if the <see cref="T:TwinCAT.TypeSystem.IInstance" /> is found; otherwise, <c>false</c></returns>
    public virtual bool TryGetInstanceByName(string instanceName, [NotNullWhen(true)] out IList<T>? instances)
    {
      if (string.IsNullOrEmpty(instanceName))
        throw new ArgumentNullException(nameof (instanceName));
      List<T> source = (List<T>) null;
      bool flag = instanceName.EndsWith("^", StringComparison.OrdinalIgnoreCase);
      string actName = instanceName;
      if (flag)
        actName = instanceName.Substring(0, instanceName.Length - 1);
      if (this.mode == 1)
      {
        StringComparer cmp = StringComparer.OrdinalIgnoreCase;
        source = new List<T>(this._pathDict.Values.Where<T>((Func<T, bool>) (p => cmp.Compare(((IInstance) (object) p).InstanceName, actName) == 0)));
        if (flag)
          source = new List<T>(((IEnumerable) source.Where<T>((Func<T, bool>) (p => (object) p is IPointerInstance)).Cast<IPointerInstance>().Select<IPointerInstance, ISymbol>((Func<IPointerInstance, ISymbol>) (pi => pi.Reference)).Where<ISymbol>((Func<ISymbol, bool>) (r => r != null))).Cast<T>());
      }
      else
      {
        T obj = default (T);
        if (this._pathDict.TryGetValue(actName, out obj))
        {
          source = new List<T>();
          if (flag)
          {
            if (obj is IPointerInstance ipointerInstance)
            {
              T reference = (T) ipointerInstance.Reference;
              if ((object) reference != null)
                source.Add(reference);
            }
          }
          else
            source.Add(obj);
        }
      }
      if (source != null && source.Count > 0)
      {
        instances = (IList<T>) source.AsReadOnly();
        return true;
      }
      instances = (IList<T>) null;
      return false;
    }

    /// <summary>
    /// Determines whether the specified instance name contains name.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <returns><c>true</c> if the specified instance name contains name; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public bool ContainsName(string instanceName)
    {
      string actName = !string.IsNullOrEmpty(instanceName) ? instanceName : throw new ArgumentNullException(nameof (instanceName));
      bool flag = instanceName.EndsWith("^", StringComparison.OrdinalIgnoreCase);
      if (flag)
        actName = instanceName.Substring(0, instanceName.Length - 1);
      T obj1 = default (T);
      T obj2 = this._list.SingleOrDefault<T>((Func<T, bool>) (p => string.CompareOrdinal(((IInstance) (object) p).InstanceName, actName) == 0));
      foreach (T obj3 in this._list)
      {
        if (string.CompareOrdinal(((IInstance) (object) obj3).InstanceName, actName) == 0)
        {
          obj2 = obj3;
          break;
        }
      }
      if ((object) obj2 != null & flag)
        obj2 = !(obj2 is IPointerInstance ipointerInstance) ? default (T) : (T) ipointerInstance.Reference;
      return (object) obj2 != null;
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IInstance" />by instance path.
    /// </summary>
    /// <param name="instanceSpecifier">The instance path or Instance Name (dependent of <see cref="P:TwinCAT.TypeSystem.Generic.InstanceCollection`1.Mode" /> setting)</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.ArgumentException">Path not found!;instancePath</exception>
    public T GetInstance(string instanceSpecifier)
    {
      T symbol = default (T);
      if (!this.TryGetInstance(instanceSpecifier, out symbol))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "InstancePath '{0}' not found!", (object) instanceSpecifier), nameof (instanceSpecifier));
      return symbol;
    }

    /// <summary>Gets the name of the instance by.</summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <returns>IList&lt;T&gt;.</returns>
    /// <exception cref="T:System.ArgumentException">Name not found!;instanceName</exception>
    public IList<T> GetInstanceByName(string instanceName)
    {
      IList<T> instances = (IList<T>) null;
      if (!this.TryGetInstanceByName(instanceName, out instances))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Name '{0}' not found!", (object) instanceName), nameof (instanceName));
      return instances;
    }
  }
}
