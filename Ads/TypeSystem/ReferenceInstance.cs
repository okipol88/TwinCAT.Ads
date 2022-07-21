// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.ReferenceInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Reference instance.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ReferenceInstance : 
    Symbol,
    IReferenceInstanceAccess,
    IReferenceInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IIndexedAccess
  {
    internal ReferenceInstance(
      AdsSymbolEntry entry,
      IReferenceType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 15;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal ReferenceInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 15;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">Indicates, that the oversample Symbol is to be created.</param>
    /// <param name="parent">The parent.</param>
    internal ReferenceInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 15;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal ReferenceInstance(
      string instanceName,
      IReferenceType type,
      ISymbol parent,
      int fieldOffset)
      : base(instanceName, (IDataType) type, parent, fieldOffset, ((ISymbolFactoryServicesProvider) parent).FactoryServices)
    {
      this.Category = (DataTypeCategory) 15;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal ReferenceInstance(
      string instanceName,
      string? instancePath,
      uint indexGroup,
      uint indexOffset,
      IReferenceType? type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, indexGroup, indexOffset, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 15;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is primitive.
    /// </summary>
    /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
    public override bool IsPrimitiveType => base.IsPrimitiveType;

    /// <summary>
    /// Gets a value indicating whether this Symbol is a container/complex type.
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    public override bool IsContainerType
    {
      get
      {
        DataType dataType = (DataType) ((DataType) this.DataType)?.ResolveType((DataTypeResolveStrategy) 1);
        return dataType != null && dataType.IsContainer;
      }
    }

    /// <summary>Gets the Category of the Referenced Symbol.</summary>
    /// <value>The resolved category.</value>
    public DataTypeCategory ResolvedCategory
    {
      get
      {
        IDataType resolvedType = this.ResolvedType;
        return resolvedType != null ? resolvedType.Category : (DataTypeCategory) 0;
      }
    }

    /// <summary>Gets the type of the referenced.</summary>
    /// <value>The type of the referenced.</value>
    public IDataType? ReferencedType => ((ReferenceType) this.DataType)?.ReferencedType;

    /// <summary>Gets the type of the resolved.</summary>
    /// <value>The type of the resolved.</value>
    public IDataType? ResolvedType
    {
      get
      {
        IDataType resolvedType = (IDataType) null;
        ReferenceType dataType = (ReferenceType) this.DataType;
        if (dataType != null)
          resolvedType = dataType.ResolveType((DataTypeResolveStrategy) 1);
        return resolvedType;
      }
    }

    /// <summary>Gets the size of the resolved byte.</summary>
    /// <value>The size of the resolved byte.</value>
    public int ResolvedByteSize
    {
      get
      {
        IDataType resolvedType = this.ResolvedType;
        return resolvedType != null ? ((IBitSize) resolvedType).ByteSize : 0;
      }
    }

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol)
    {
      int subSymbolCount = 0;
      DataType dataType = (DataType) ((DataType) this.DataType)?.ResolveType((DataTypeResolveStrategy) 1);
      if (dataType != null && dataType.Size > 0)
      {
        if (dataType.Category == 5)
          subSymbolCount = ((ICollection<IMember>) ((IInterfaceType) dataType).AllMembers).Count;
        else if (dataType.Category == 4)
          subSymbolCount = ((IArrayType) dataType).Dimensions.ElementCount;
        else if (dataType.Category == 14)
          subSymbolCount = ((ICollection<IField>) ((IUnionType) dataType).Fields).Count;
        else if (dataType.Category == 13)
          subSymbolCount = 1;
      }
      return subSymbolCount;
    }

    /// <summary>Creates the sub symbols collection.</summary>
    internal override ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentInstance)
    {
      ISymbolCollection<ISymbol> subSymbols = base.OnCreateSubSymbols(parentInstance);
      try
      {
        ReferenceType dataType = (ReferenceType) this.DataType;
        DataType resolvedType = (DataType) this.ResolvedType;
        if (resolvedType != null)
        {
          if (resolvedType.Size > 0)
          {
            if (resolvedType.Category == 5)
            {
              StructType structType = (StructType) resolvedType;
              subSymbols = structType == null ? (ISymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 0) : ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory.CreateFieldInstances((ISymbol) this, (IDataType) structType);
            }
            else if (resolvedType.Category == 4)
            {
              ArrayType arrayType = (ArrayType) resolvedType;
              subSymbols = ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory.CreateArrayElementInstances(parentInstance, (IArrayType) arrayType);
            }
            else if (resolvedType.Category == 14)
            {
              UnionType unionType = (UnionType) resolvedType;
              subSymbols = unionType == null ? (ISymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 0) : ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory.CreateFieldInstances((ISymbol) this, (IDataType) unionType);
            }
            else if (resolvedType.Category == 13)
            {
              ISymbol referenceInstance = ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory.CreateReferenceInstance((IPointerType) resolvedType, (ISymbol) this);
              if (referenceInstance != null)
              {
                if (resolvedType.Size > 0)
                  ((ICollection<ISymbol>) subSymbols).Add(referenceInstance);
              }
            }
          }
        }
        else
          AdsModule.Trace.TraceWarning("Cannot resolve reference type '{0}'!", new object[1]
          {
            (object) ((IInstance) parentInstance).InstancePath
          });
      }
      catch (Exception ex)
      {
        AdsModule.Trace.TraceError(ex);
      }
      return subSymbols;
    }

    bool IIndexedAccess.TryGetElement(int[] indices, [NotNullWhen(true)] out ISymbol? symbol)
    {
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (this.ResolvedCategory != 4)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot dereference indices. The reference symbol '{0}' is not referencing an array type!", (object) this.InstancePath), (ISymbol) this);
      ArrayType resolvedType = (ArrayType) this.ResolvedType;
      if (resolvedType != null && ArrayType.AreIndicesValid(indices, (IArrayType) resolvedType, false))
      {
        int elementPosition = resolvedType.GetElementPosition(indices);
        symbol = ((IList<ISymbol>) this.SubSymbols)[elementPosition];
        return true;
      }
      symbol = (ISymbol) null;
      return false;
    }

    bool IIndexedAccess.TryGetElement(IList<int[]> jaggedIndices, [NotNullWhen(true)] out ISymbol? symbol)
    {
      if (jaggedIndices == null)
        throw new ArgumentNullException(nameof (jaggedIndices));
      if (this.ResolvedCategory != 4)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot dereference indices. The reference symbol '{0}' is not referencing an array type!", (object) this.InstancePath), (ISymbol) this);
      ArrayType resolvedType = (ArrayType) this.ResolvedType;
      if (resolvedType == null)
        throw new CannotResolveDataTypeException((IInstance) this);
      if (resolvedType.JaggedLevel != jaggedIndices.Count)
        throw new ArgumentOutOfRangeException(nameof (jaggedIndices));
      bool flag = true;
      ISymbol isymbol1 = (ISymbol) this;
      ISymbol isymbol2 = (ISymbol) null;
      symbol = (ISymbol) null;
      for (int index = 0; index < jaggedIndices.Count; ++index)
      {
        if (isymbol1 is IIndexedAccess)
          flag &= ((IIndexedAccess) isymbol1).TryGetElement(jaggedIndices[index], ref isymbol2);
        if (flag)
          isymbol1 = isymbol2;
        else
          break;
      }
      if (flag)
        symbol = isymbol2;
      return symbol != null;
    }
  }
}
