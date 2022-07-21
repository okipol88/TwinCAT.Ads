// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.ArrayInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Implementation of the <see cref="T:TwinCAT.TypeSystem.IArrayInstance" />.
  /// </summary>
  /// <exclude />
  public class ArrayInstance : 
    Symbol,
    IArrayInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IIndexedAccess
  {
    internal ArrayInstance(
      AdsSymbolEntry entry,
      IArrayType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 4;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal ArrayInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 4;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">Oversampling instance.</param>
    /// <param name="parent">The parent.</param>
    internal ArrayInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 4;
    }

    internal ArrayInstance(
      string instanceName,
      string? instancePath,
      uint indexGroup,
      uint indexOffset,
      IArrayType? type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, indexGroup, indexOffset, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 4;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal ArrayInstance(string instanceName, IArrayType type, ISymbol parent, int fieldOffset)
      : base(instanceName, (IDataType) type, parent, fieldOffset, ((ISymbolFactoryServicesProvider) parent).FactoryServices)
    {
      this.Category = (DataTypeCategory) 4;
    }

    /// <summary>
    /// Handler function is called, when SubSymbols (the elements) are called.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <exclude />
    protected virtual bool OnAccessSubSymbols() => true;

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol)
    {
      this.OnAccessSubSymbols();
      IArrayType dataType = (IArrayType) this.DataType;
      return dataType != null ? dataType.Dimensions.ElementCount : 0;
    }

    /// <summary>Creates the sub symbols collection.</summary>
    internal override ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentInstance)
    {
      ISymbolFactory symbolFactory = ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory;
      this.OnAccessSubSymbols();
      ArrayType dataType = (ArrayType) this.DataType;
      ISymbolCollection<ISymbol> subSymbols;
      try
      {
        subSymbols = dataType == null ? (ISymbolCollection<ISymbol>) SymbolCollection.Empty : symbolFactory.CreateArrayElementInstances(parentInstance, (IArrayType) dataType);
      }
      catch (Exception ex)
      {
        AdsModule.Trace.TraceError(ex);
        throw;
      }
      return subSymbols;
    }

    /// <summary>
    /// Gets the contained Array Elements as read only collection.
    /// </summary>
    /// <value>The elements.</value>
    public ISymbolCollection<ISymbol> Elements => this.OnGetElements();

    /// <summary>
    /// Handler function getting the SubElements of the Array.
    /// </summary>
    /// <returns>ReadOnlySymbolCollection.</returns>
    protected virtual ISymbolCollection<ISymbol> OnGetElements() => this.SubSymbols;

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.ISymbol" /> with the specified indices.
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <returns>ISymbol.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">indices</exception>
    public ISymbol this[int[] indices]
    {
      get
      {
        ISymbol symbol = (ISymbol) null;
        if (!this.TryGetElement(indices, out symbol))
          throw new ArgumentOutOfRangeException(nameof (indices));
        return symbol;
      }
    }

    public virtual bool TryGetElement(int[] indices, [NotNullWhen(true)] out ISymbol? symbol)
    {
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      ArrayType dataType = (ArrayType) this.DataType;
      if (dataType != null && ArrayType.AreIndicesValid(indices, (IArrayType) dataType, false))
      {
        int elementPosition = dataType.GetElementPosition(indices);
        symbol = ((IList<ISymbol>) this.SubSymbols)[elementPosition];
        return true;
      }
      symbol = (ISymbol) null;
      return false;
    }

    public bool TryGetElement(IList<int[]> jaggedIndices, [NotNullWhen(true)] out ISymbol? symbol)
    {
      if (jaggedIndices == null)
        throw new ArgumentNullException(nameof (jaggedIndices));
      IArrayType dataType = (IArrayType) this.DataType;
      if (dataType == null)
        throw new CannotResolveDataTypeException((IInstance) this);
      if (dataType.JaggedLevel < jaggedIndices.Count)
        throw new ArgumentOutOfRangeException(nameof (jaggedIndices));
      bool flag = true;
      IArrayInstance iarrayInstance = (IArrayInstance) this;
      ISymbol isymbol = (ISymbol) null;
      symbol = (ISymbol) null;
      for (int index = 0; index < jaggedIndices.Count; ++index)
      {
        if (iarrayInstance == null)
          throw new ArgumentException("Jagged indices list mismatch!", nameof (jaggedIndices));
        flag &= ((IIndexedAccess) iarrayInstance).TryGetElement(jaggedIndices[index], ref isymbol);
        if (flag)
          iarrayInstance = isymbol as IArrayInstance;
        else
          break;
      }
      if (flag)
        symbol = isymbol;
      return symbol != null;
    }

    /// <summary>Gets the jagged level.</summary>
    /// <value>The jagged level.</value>
    public int JaggedLevel
    {
      get
      {
        IArrayType dataType = (IArrayType) this.DataType;
        return dataType != null ? dataType.JaggedLevel : 1;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this Symbol is a container/complex type.
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    public override bool IsContainerType => true;

    /// <summary>
    /// Gets a value indicating whether this instance is primitive.
    /// </summary>
    /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
    public override bool IsPrimitiveType
    {
      get
      {
        IDataType dataType = this.DataType;
        return dataType != null && dataType.IsPrimitive;
      }
    }

    /// <summary>Gets the dimensions as read only collection.</summary>
    /// <value>The dimensions.</value>
    public IDimensionCollection Dimensions
    {
      get
      {
        IArrayType dataType = (IArrayType) this.DataType;
        return dataType != null ? dataType.Dimensions : (IDimensionCollection) DimensionCollection.Empty.AsReadOnly();
      }
    }

    /// <summary>Gets the type of the contained elements.</summary>
    /// <value>The type of the element.</value>
    public IDataType? ElementType => ((IArrayType) this.DataType)?.ElementType;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" /> is oversampled.
    /// </summary>
    /// <value><c>true</c> if this instance is oversampled; otherwise, <c>false</c>.</value>
    public bool IsOversampled
    {
      get
      {
        ArrayType dataType = (ArrayType) this.DataType;
        return dataType != null && dataType.IsOversampled;
      }
    }
  }
}
