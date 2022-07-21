// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ArrayElementSymbolCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads.TypeSystem;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>This collection represents the SubItems of an Array</summary>
  /// <remarks>This <see cref="T:TwinCAT.TypeSystem.ISymbolCollection`1" /> is an optimization for <see cref="T:TwinCAT.TypeSystem.SymbolCollection" /> which doesn't cache the
  /// contained Elements. Here in the <see cref="T:TwinCAT.TypeSystem.ArrayElementSymbolCollection" /> the array elements will be created 'On-Access'.
  /// Additionally, this collection is Read-Only.
  /// </remarks>
  internal class ArrayElementSymbolCollection : 
    ISymbolCollection<ISymbol>,
    IInstanceCollection<ISymbol>,
    IList<ISymbol>,
    ICollection<ISymbol>,
    IEnumerable<ISymbol>,
    IEnumerable
  {
    /// <summary>
    /// The array instance (can be ArrayInstance, Union Instance or Reference instance!)
    /// </summary>
    private ISymbol _arrayInstance;
    /// <summary>
    /// The array type (Can be Array, Union Array, or Reference to Array)
    /// </summary>
    private IArrayType _arrayType;
    /// <summary>The used symbol factory</summary>
    private ISymbolFactory _symbolFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ArrayElementSymbolCollection" /> class organized with InstancePath.
    /// </summary>
    internal ArrayElementSymbolCollection(
      ISymbol arrayInstance,
      IArrayType arrayType,
      ISymbolFactory factory)
    {
      this._arrayInstance = arrayInstance;
      this._arrayType = arrayType;
      this._symbolFactory = factory;
    }

    private bool IsOversampled => this._arrayInstance is IOversamplingArrayInstance;

    /// <summary>Gets the ArrayElementInstance at the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <returns>ISymbol.</returns>
    /// <remarks>The Index Setter will throw an exception.</remarks>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public ISymbol this[int index]
    {
      get => this.IsOversampled && ArrayIndexConverter.IsOversamplingElement(index, this._arrayType.Dimensions.LowerBounds, this._arrayType.Dimensions.UpperBounds) ? ((ISymbolFactoryOversampled) this._symbolFactory).CreateOversamplingElement(this._arrayInstance) : this._symbolFactory.CreateArrayElement(this._arrayType, ArrayIndexConverter.SubIndexToIndices(index, this._arrayType), this._arrayInstance);
      set => throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the Array element instance with the specified instance path.
    /// </summary>
    /// <param name="instancePath">The instance path.</param>
    /// <returns>ISymbol.</returns>
    /// <exception cref="T:System.ArgumentException">Symbol not found!</exception>
    public ISymbol this[string instancePath] => this.GetInstance(instancePath) ?? throw new ArgumentException("Symbol not found!");

    /// <summary>Gets the number of contained array element instances.</summary>
    /// <value>The count.</value>
    public int Count
    {
      get
      {
        int elementCount = this._arrayType.Dimensions.ElementCount;
        if (this.IsOversampled)
          ++elementCount;
        return elementCount;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is read only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => true;

    /// <summary>Gets the mode.</summary>
    /// <value>The mode.</value>
    public InstanceCollectionMode Mode => (InstanceCollectionMode) 0;

    /// <summary>Adds the specified item.</summary>
    /// <param name="item">The item.</param>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public void Add(ISymbol item) => throw new NotImplementedException();

    /// <summary>Clears this instance.</summary>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public void Clear() => throw new NotImplementedException();

    /// <summary>
    /// Determines whether the specified ArrayElement instance is contained
    /// </summary>
    /// <param name="element">The item.</param>
    /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
    public bool Contains(ISymbol element) => this.Contains(((IInstance) element).InstancePath);

    /// <summary>
    /// Determines whether this collection contains the array element with the specified path.
    /// </summary>
    /// <param name="instancePath">The instance path.</param>
    /// <returns><c>true</c> if [contains] [the specified instance path]; otherwise, <c>false</c>.</returns>
    public bool Contains(string instancePath)
    {
      if (instancePath.StartsWith(((IInstance) this._arrayInstance).InstancePath, StringComparison.OrdinalIgnoreCase))
      {
        string indicesStr = instancePath.Substring(((IInstance) this._arrayInstance).InstancePath.Length);
        IList<int[]> jaggedIndices = (IList<int[]>) null;
        if (SymbolParser.TryParseIndices(indicesStr, out jaggedIndices, out ArrayIndexType? _))
          return ArrayIndexConverter.TryCheckIndices(jaggedIndices, this._arrayType);
      }
      return false;
    }

    /// <summary>
    /// Determines whether this collection contains an ArrayElement with the specified name with indices.
    /// </summary>
    /// <param name="instanceNameWithIndices">The instance name with indices.</param>
    /// <returns><c>true</c> if the specified instance name with indices contains name; otherwise, <c>false</c>.</returns>
    public bool ContainsName(string instanceNameWithIndices)
    {
      string indicesStr = (string) null;
      string instanceName = (string) null;
      string fullPathWithoutIndexes;
      IList<int[]> jaggedIndices;
      return SymbolParser.TryParseArrayElement(instanceNameWithIndices, out fullPathWithoutIndexes, out instanceName, out indicesStr, out jaggedIndices, out ArrayIndexType? _) && StringComparer.OrdinalIgnoreCase.Compare(fullPathWithoutIndexes, ((IInstance) this._arrayInstance).InstanceName) == 0 && ArrayIndexConverter.TryCheckIndices(jaggedIndices, this._arrayType);
    }

    /// <summary>Copies to.</summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public void CopyTo(ISymbol[] array, int arrayIndex) => throw new NotImplementedException();

    /// <summary>Gets the enumerator.</summary>
    /// <returns>IEnumerator&lt;ISymbol&gt;.</returns>
    public IEnumerator<ISymbol> GetEnumerator()
    {
      ArrayIndexIterator arrayIndexIterator = (ArrayIndexIterator) null;
      if (this._arrayInstance is IArrayInstanceVariableSize)
      {
        ArrayInstanceVariableSize arrayInstance = (ArrayInstanceVariableSize) this._arrayInstance;
        if (arrayInstance.TryUpdateDimensions() == null)
        {
          IArrayType dataType = (IArrayType) arrayInstance.DataType;
          if (dataType != null)
            arrayIndexIterator = new ArrayIndexIterator(dataType);
        }
      }
      else
        arrayIndexIterator = new ArrayIndexIterator(this._arrayType);
      if (arrayIndexIterator != null)
      {
        foreach (int[] numArray in arrayIndexIterator)
          yield return this._symbolFactory.CreateArrayElement(this._arrayType, numArray, this._arrayInstance);
        if (this.IsOversampled)
          yield return ((ISymbolFactoryOversampled) this._symbolFactory).CreateOversamplingElement(this._arrayInstance);
      }
    }

    /// <summary>Gets the instance.</summary>
    /// <param name="instancePath">The instance path.</param>
    /// <returns>ISymbol.</returns>
    public ISymbol GetInstance(string instancePath)
    {
      if (string.IsNullOrEmpty(instancePath))
        throw new ArgumentException("InstancePath not specified!", nameof (instancePath));
      ISymbol symbol = (ISymbol) null;
      this.TryGetInstance(instancePath, out symbol);
      return symbol != null ? symbol : throw new SymbolException("Cannot find symbol '" + instancePath + "'!", symbol);
    }

    /// <summary>Gets the name of the instance by.</summary>
    /// <param name="instanceNameWithIndices">The instance name with indices.</param>
    /// <returns>IList&lt;ISymbol&gt;.</returns>
    public IList<ISymbol> GetInstanceByName(string instanceNameWithIndices)
    {
      if (string.IsNullOrEmpty(instanceNameWithIndices))
        throw new ArgumentException("InstanceName not specified!", nameof (instanceNameWithIndices));
      IList<ISymbol> symbols = (IList<ISymbol>) null;
      this.TryGetInstanceByName(instanceNameWithIndices, out symbols);
      return symbols != null ? symbols : throw new SymbolException("Cannot find symbol '" + instanceNameWithIndices + "'!", (ISymbol) null);
    }

    /// <summary>
    /// Gets the SubIndex of the specified Array Element instance.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">item</exception>
    public int IndexOf(ISymbol item)
    {
      string instancePath = ((IInstance) item).InstancePath;
      if (instancePath.StartsWith(((IInstance) this._arrayInstance).InstancePath, StringComparison.OrdinalIgnoreCase))
      {
        string indicesStr = instancePath.Substring(((IInstance) this._arrayInstance).InstancePath.Length);
        IList<int[]> jaggedIndices = (IList<int[]>) null;
        if (SymbolParser.TryParseIndices(indicesStr, out jaggedIndices, out ArrayIndexType? _))
          return ArrayIndexConverter.IndicesToSubIndex(jaggedIndices[0], this._arrayType);
      }
      throw new ArgumentOutOfRangeException(nameof (item));
    }

    /// <summary>Inserts the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <param name="item">The item.</param>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public void Insert(int index, ISymbol item) => throw new NotImplementedException();

    /// <summary>Removes the specified item.</summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public bool Remove(ISymbol item) => throw new NotImplementedException();

    /// <summary>Removes at.</summary>
    /// <param name="index">The index.</param>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public void RemoveAt(int index) => throw new NotImplementedException();

    public bool TryGetInstance(string instancePath, [NotNullWhen(true)] out ISymbol? symbol)
    {
      symbol = (ISymbol) null;
      if (instancePath.StartsWith(((IInstance) this._arrayInstance).InstancePath, StringComparison.OrdinalIgnoreCase))
      {
        string indicesStr = instancePath.Substring(((IInstance) this._arrayInstance).InstancePath.Length);
        IList<int[]> jaggedIndices = (IList<int[]>) null;
        if (SymbolParser.TryParseIndices(indicesStr, out jaggedIndices, out ArrayIndexType? _))
        {
          if (this.IsOversampled && ArrayIndexConverter.IsOversamplingIndex(jaggedIndices[0], this._arrayType))
          {
            symbol = ((ISymbolFactoryOversampled) this._symbolFactory).CreateOversamplingElement(this._arrayInstance);
          }
          else
          {
            ISymbol isymbol = this._arrayInstance;
            IArrayType iarrayType = this._arrayType;
            for (int index = 0; index < jaggedIndices.Count; ++index)
            {
              symbol = this._symbolFactory.CreateArrayElement(iarrayType, jaggedIndices[index], isymbol);
              isymbol = symbol;
              iarrayType = (IArrayType) ((IInstance) symbol).DataType;
              if (iarrayType == null)
                throw new CannotResolveDataTypeException(((IInstance) symbol).TypeName);
            }
          }
        }
      }
      return symbol != null;
    }

    /// <summary>Tries the name of the get instance by.</summary>
    /// <param name="instanceNameWithIndices">The instance name with indices.</param>
    /// <param name="symbols">The symbols.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetInstanceByName(string instanceNameWithIndices, [NotNullWhen(true)] out IList<ISymbol>? symbols)
    {
      ISymbol isymbol1 = (ISymbol) null;
      symbols = (IList<ISymbol>) new List<ISymbol>();
      string indicesStr = (string) null;
      string fullPathWithoutIndexes;
      IList<int[]> jaggedIndices;
      if (SymbolParser.TryParseArrayElement(instanceNameWithIndices, out fullPathWithoutIndexes, out string _, out indicesStr, out jaggedIndices, out ArrayIndexType? _) && StringComparer.OrdinalIgnoreCase.Compare(fullPathWithoutIndexes, ((IInstance) this._arrayInstance).InstanceName) == 0)
      {
        ISymbol isymbol2 = this._arrayInstance;
        iarrayType = this._arrayType;
        for (int index = 0; index < jaggedIndices.Count; ++index)
        {
          isymbol1 = this._symbolFactory.CreateArrayElement(iarrayType, jaggedIndices[index], isymbol2);
          isymbol2 = isymbol1;
          if (!(((IInstance) isymbol2).DataType is IArrayType iarrayType))
            throw new CannotResolveDataTypeException(((IInstance) isymbol2).TypeName);
        }
      }
      if (isymbol1 != null)
        ((ICollection<ISymbol>) symbols).Add(isymbol1);
      return ((ICollection<ISymbol>) symbols).Count > 0;
    }

    /// <summary>Gets the enumerator.</summary>
    /// <returns>IEnumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
