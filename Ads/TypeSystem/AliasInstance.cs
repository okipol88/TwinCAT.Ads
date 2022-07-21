// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AliasInstance
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
  /// <summary>Class AliasInstance.</summary>
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.Symbol" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IAliasInstance" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AliasInstance : 
    Symbol,
    IAliasInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IIndexedAccess
  {
    internal AliasInstance(
      AdsSymbolEntry entry,
      IAliasType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 2;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal AliasInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 2;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">Indicates, that the oversample Symbol is to be created.</param>
    /// <param name="parent">The parent.</param>
    internal AliasInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 2;
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
    internal AliasInstance(string instanceName, IAliasType type, ISymbol parent, int fieldOffset)
      : base(instanceName, (IDataType) type, parent, fieldOffset, ((ISymbolFactoryServicesProvider) parent).FactoryServices)
    {
      this.Category = (DataTypeCategory) 2;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal AliasInstance(
      string instanceName,
      string? instancePath,
      uint indexGroup,
      uint indexOffset,
      IAliasType? type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, indexGroup, indexOffset, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 2;
    }

    /// <summary>Gets the Category of the fully resolved alias.</summary>
    /// <value>The resolved category.</value>
    public DataTypeCategory ResolvedCategory
    {
      get
      {
        IDataType resolvedType = this.ResolvedType;
        return resolvedType != null ? resolvedType.Category : (DataTypeCategory) 0;
      }
    }

    /// <summary>Gets the Datatype of the fully resolved Alias</summary>
    /// <value>The resolved alias type.</value>
    public IDataType? ResolvedType => ((DataType) this.DataType)?.ResolveType((DataTypeResolveStrategy) 1);

    /// <summary>
    /// Gets a value indicating whether this instance is primitive.
    /// </summary>
    /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
    public override bool IsPrimitiveType
    {
      get
      {
        DataType resolvedType = (DataType) this.ResolvedType;
        return resolvedType != null && resolvedType.IsPrimitive;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this Symbol is a container/complex type.
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    public override bool IsContainerType
    {
      get
      {
        DataType resolvedType = (DataType) this.ResolvedType;
        return resolvedType != null && resolvedType.IsContainer;
      }
    }

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol)
    {
      int subSymbolCount = 0;
      IDataType resolvedType = this.ResolvedType;
      if (resolvedType != null)
      {
        DataTypeCategory category = resolvedType.Category;
        if (category <= 5)
        {
          if (category != 4)
          {
            if (category == 5)
              subSymbolCount = ((ICollection<IMember>) ((IInterfaceType) resolvedType).AllMembers).Count;
          }
          else
            subSymbolCount = ((IArrayType) resolvedType).Dimensions.ElementCount;
        }
        else if (category != 13)
        {
          if (category == 14)
            subSymbolCount = ((ICollection<IField>) ((IUnionType) resolvedType).Fields).Count;
        }
        else
        {
          IDataType referencedType = ((IPointerType) resolvedType).ReferencedType;
          if (referencedType == null || referencedType.Category != 4 && ((IBitSize) referencedType).Size == 0)
            return 0;
          subSymbolCount = 1;
        }
      }
      return subSymbolCount;
    }

    /// <summary>Creates the sub symbols collection.</summary>
    internal override ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentInstance)
    {
      IDataType resolvedType = this.ResolvedType;
      ISymbolCollection<ISymbol> subSymbols = base.OnCreateSubSymbols(parentInstance);
      try
      {
        ISymbolFactory symbolFactory = ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory;
        if (resolvedType != null)
        {
          DataTypeCategory category = resolvedType.Category;
          if (category <= 5)
          {
            if (category != 4)
            {
              if (category == 5)
                subSymbols = symbolFactory.CreateFieldInstances(parentInstance, resolvedType);
            }
            else
              subSymbols = symbolFactory.CreateArrayElementInstances(parentInstance, (IArrayType) resolvedType);
          }
          else if (category != 13)
          {
            if (category == 14)
              subSymbols = symbolFactory.CreateFieldInstances(parentInstance, resolvedType);
          }
          else
          {
            ISymbol referenceInstance = symbolFactory.CreateReferenceInstance((IPointerType) resolvedType, parentInstance);
            if (referenceInstance != null)
              ((ICollection<ISymbol>) subSymbols).Add(referenceInstance);
          }
        }
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
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot resolve indices. The alias symbol '{0}' is not aliasing an array type!", (object) this.InstancePath), (ISymbol) this);
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
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot resolve indices. The alias symbol '{0}' is not aliasing an array type!", (object) this.InstancePath), (ISymbol) this);
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
