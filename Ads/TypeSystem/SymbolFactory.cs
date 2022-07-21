// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.SymbolFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Symbol factory (static objects) (for internal use only)
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SymbolFactory : SymbolFactoryBase, ISymbolFactoryOversampled
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.SymbolFactory" /> class (for internal use only)
    /// </summary>
    /// <param name="nonCachedArrayElements">if set to <c>true</c> [non cached array elements].</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SymbolFactory(bool nonCachedArrayElements)
      : base(nonCachedArrayElements)
    {
    }

    /// <summary>Handler function creating a new Array Element Symbol.</summary>
    /// <param name="arrayType">Resolved array type.</param>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>ISymbol.</returns>
    protected override ISymbol OnCreateArrayElement(
      IArrayType arrayType,
      int[] currentIndex,
      ISymbol parent)
    {
      return this.createArrayElement(currentIndex, false, parent, arrayType);
    }

    /// <summary>Creates the array element.</summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">if set to <c>true</c> [oversample].</param>
    /// <param name="parent">The parent.</param>
    /// <param name="arrayType">Resolved array type.</param>
    /// <returns>ISymbol.</returns>
    /// <exception cref="T:System.ArgumentNullException">parent</exception>
    /// <exception cref="T:System.ArgumentNullException">arrType
    /// or
    /// parent</exception>
    private ISymbol createArrayElement(
      int[] currentIndex,
      bool oversample,
      ISymbol parent,
      IArrayType arrayType)
    {
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      return (ISymbol) this.createArrayElement(currentIndex, oversample, (ArrayType) arrayType, (arrayType != null ? (DataType) arrayType.ElementType : throw new ArgumentNullException(nameof (arrayType))) ?? throw new CannotResolveDataTypeException((IInstance) parent), parent);
    }

    /// <summary>Creates an Array element instance.</summary>
    /// <param name="currentIndex">Indices of the array element</param>
    /// <param name="oversample">Oversampling indicator.</param>
    /// <param name="arrType">Array type.</param>
    /// <param name="elementType">The element type.</param>
    /// <param name="parent">The parent instance (here the array instance)</param>
    /// <returns>Symbol.</returns>
    /// <exception cref="T:System.NotSupportedException"></exception>
    private Symbol createArrayElement(
      int[] currentIndex,
      bool oversample,
      ArrayType arrType,
      DataType elementType,
      ISymbol parent)
    {
      if (currentIndex == null)
        throw new ArgumentNullException(nameof (currentIndex));
      if (arrType == null)
        throw new ArgumentNullException(nameof (arrType));
      if (elementType == null)
        throw new ArgumentNullException(nameof (elementType));
      bool flag1 = true;
      bool flag2 = true;
      DataTypeCategory category = elementType.Category;
      Symbol arrayElement;
      if (category == 12)
        arrayElement = new Symbol(currentIndex, oversample, parent);
      else if (category == 1)
        arrayElement = new Symbol(currentIndex, oversample, parent);
      else if (category == 4)
        arrayElement = !arrType.IsOversampled ? (Symbol) new ArrayInstance(currentIndex, false, parent) : (Symbol) new OversamplingArrayInstance(currentIndex, oversample, parent);
      else if (category == 5)
        arrayElement = (Symbol) new StructInstance(currentIndex, oversample, parent);
      else if (category == 16)
        arrayElement = (Symbol) new InterfaceInstance(currentIndex, oversample, parent);
      else if (category == 14)
        arrayElement = (Symbol) new UnionInstance(currentIndex, oversample, parent);
      else if (category == 15)
      {
        IDataType referencedType = ((IReferenceType) elementType).ReferencedType;
        if (referencedType == null)
          throw new CannotResolveDataTypeException(elementType.Name);
        arrayElement = referencedType.Category != 5 ? (Symbol) new ReferenceInstance(currentIndex, oversample, parent) : (Symbol) new RpcStructReferenceInstance(currentIndex, oversample, parent);
      }
      else if (category == 13)
        arrayElement = (Symbol) new PointerInstance(currentIndex, oversample, parent);
      else if (category == 14)
        arrayElement = new Symbol(currentIndex, oversample, parent);
      else if (category == 3)
        arrayElement = new Symbol(currentIndex, oversample, parent);
      else if (category == 2)
        arrayElement = (Symbol) new AliasInstance(currentIndex, oversample, parent);
      else if (category == 10)
        arrayElement = (Symbol) new StringInstance(currentIndex, oversample, parent);
      else
        arrayElement = new Symbol(currentIndex, oversample, parent);
      if (flag1)
        arrayElement.SetComment(((IInstance) parent).Comment);
      if (flag2 && ((ICollection<ITypeAttribute>) ((IAttributedInstance) parent).Attributes).Count > 0)
        arrayElement.AddDerivedAttributes((IEnumerable<ITypeAttribute>) ((IAttributedInstance) parent).Attributes);
      return arrayElement;
    }

    /// <summary>Creates the oversampling array Element.</summary>
    /// <param name="parent">Array Instance.</param>
    /// <returns>ISymbol.</returns>
    public ISymbol CreateOversamplingElement(ISymbol parent)
    {
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      ArrayType arrayType = (ArrayType) ((DataType) ((DataType) ((IInstance) parent).DataType)?.ResolveType((DataTypeResolveStrategy) 1) ?? throw new CannotResolveDataTypeException(((IInstance) parent).TypeName));
      int[] currentIndex = new int[((ICollection<IDimension>) arrayType.Dimensions).Count];
      for (int index = 0; index < ((ICollection<IDimension>) arrayType.Dimensions).Count; ++index)
        currentIndex[index] = ((IList<IDimension>) arrayType.Dimensions)[index].ElementCount;
      return this.createArrayElement(currentIndex, true, parent, (IArrayType) arrayType);
    }

    /// <summary>
    /// Handler function creating a new <see cref="T:TwinCAT.TypeSystem.IStructInstance" /> member
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>ISymbol.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// member
    /// or
    /// parent
    /// </exception>
    protected override ISymbol OnCreateFieldInstance(IField field, ISymbol parent)
    {
      if (field == null)
        throw new ArgumentNullException(nameof (field));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      Member member = (Member) field;
      IDataType dataType1 = member.DataType;
      if (dataType1 == null)
        throw new CannotResolveDataTypeException(member.TypeName);
      Symbol fieldInstance;
      if (member.Category == null)
      {
        AdsModule.Trace.TraceWarning("Category of member '{0}.{1}' is Unknown", new object[2]
        {
          (object) ((IInstance) parent).InstancePath,
          (object) member.InstanceName
        });
        fieldInstance = new Symbol(member, parent);
      }
      else if (member.Category == 4)
        fieldInstance = !((ArrayType) dataType1).IsOversampled ? (Symbol) new ArrayInstance(member, parent) : (Symbol) new OversamplingArrayInstance(member, parent);
      else if (member.Category == 5)
        fieldInstance = (Symbol) new StructInstance(member, parent);
      else if (member.Category == 16)
        fieldInstance = (Symbol) new InterfaceInstance(member, parent);
      else if (member.Category == 14)
        fieldInstance = (Symbol) new UnionInstance(member, parent);
      else if (member.Category == 15)
      {
        IReferenceType ireferenceType = (IReferenceType) dataType1;
        IDataType referencedType = ireferenceType.ReferencedType;
        if (referencedType == null)
          throw new CannotResolveDataTypeException(((IDataType) ireferenceType).Name);
        fieldInstance = referencedType.Category != 5 ? (Symbol) new ReferenceInstance(member, parent) : (Symbol) new RpcStructReferenceInstance(member, parent);
      }
      else if (member.Category == 13)
        fieldInstance = (Symbol) new PointerInstance(member, parent);
      else if (member.Category == 2)
      {
        DataType dataType2 = (DataType) ((DataType) dataType1).ResolveType((DataTypeResolveStrategy) 0);
        fieldInstance = (Symbol) new AliasInstance(member, parent);
      }
      else if (member.Category == 10)
      {
        fieldInstance = (Symbol) new StringInstance(member, parent);
      }
      else
      {
        if (member.Category == 14)
          AdsModule.Trace.TraceWarning("Category of member '{0}.{1}' is Union. This is not supported yet!", new object[2]
          {
            (object) ((IInstance) parent).InstancePath,
            (object) member.InstanceName
          });
        fieldInstance = new Symbol(member, parent);
      }
      return (ISymbol) fieldInstance;
    }

    protected override IStructInstance OnCreateStruct(
      ISymbolInfo entry,
      IStructType structType,
      ISymbol? parent)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      if (structType == null)
        throw new ArgumentNullException(nameof (structType));
      return (IStructInstance) new StructInstance((AdsSymbolEntry) entry, structType, parent, this.Services);
    }

    protected override IInterfaceInstance OnCreateInterface(
      ISymbolInfo entry,
      IInterfaceType interfaceType,
      ISymbol? parent)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      if (interfaceType == null)
        throw new ArgumentNullException(nameof (interfaceType));
      return (IInterfaceInstance) new InterfaceInstance((AdsSymbolEntry) entry, interfaceType, parent, this.Services);
    }

    protected override IUnionInstance OnCreateUnion(
      ISymbolInfo entry,
      IUnionType unionType,
      ISymbol? parent)
    {
      return (IUnionInstance) new UnionInstance((AdsSymbolEntry) entry, unionType, parent, this.Services);
    }

    protected override IArrayInstance OnCreateArrayInstance(
      ISymbolInfo entry,
      IArrayType type,
      ISymbol? parent)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      ArrayType type1 = type != null ? (ArrayType) type : throw new ArgumentNullException(nameof (type));
      return !type1.IsOversampled ? (IArrayInstance) new ArrayInstance((AdsSymbolEntry) entry, (IArrayType) type1, parent, this.Services) : (IArrayInstance) new OversamplingArrayInstance((AdsSymbolEntry) entry, (IArrayType) type1, parent, this.Services);
    }

    protected override IAliasInstance OnCreateAlias(
      ISymbolInfo entry,
      IAliasType aliasType,
      ISymbol? parent)
    {
      return (IAliasInstance) new AliasInstance((AdsSymbolEntry) entry, aliasType, parent, this.Services);
    }

    protected override IPointerInstance OnCreatePointerInstance(
      ISymbolInfo entry,
      IPointerType pointerType,
      ISymbol? parent)
    {
      return (IPointerInstance) new PointerInstance((AdsSymbolEntry) entry, pointerType, parent, this.Services);
    }

    protected override IReferenceInstance OnCreateReferenceInstance(
      ISymbolInfo entry,
      IReferenceType referenceType,
      ISymbol? parent)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      IDataType idataType = referenceType != null ? referenceType.ReferencedType : throw new ArgumentNullException(nameof (referenceType));
      if (idataType == null)
        throw new CannotResolveDataTypeException(((IDataType) referenceType).Name);
      return idataType.Category == 5 ? (IReferenceInstance) new RpcStructReferenceInstance((AdsSymbolEntry) entry, referenceType, parent, this.Services) : (IReferenceInstance) new ReferenceInstance((AdsSymbolEntry) entry, referenceType, parent, this.Services);
    }

    protected override ISymbol OnCreateString(
      ISymbolInfo entry,
      IStringType stringType,
      ISymbol? parent)
    {
      return (ISymbol) new StringInstance((AdsSymbolEntry) entry, stringType, parent, this.Services);
    }

    protected override ISymbol OnCreatePrimitive(
      ISymbolInfo entry,
      IDataType? dataType,
      ISymbol? parent)
    {
      return dataType != null ? (ISymbol) new Symbol((AdsSymbolEntry) entry, dataType, parent, this.Services) : (ISymbol) new Symbol((AdsSymbolEntry) entry, parent, this.Services);
    }

    /// <summary>Handler function creating a new Reference Instance.</summary>
    /// <param name="pointerType">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>ISymbol.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// type
    /// or
    /// parent
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">type
    /// or
    /// parent</exception>
    protected override ISymbol? OnCreateReference(IPointerType pointerType, ISymbol parent)
    {
      if (pointerType == null)
        throw new ArgumentNullException(nameof (pointerType));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      if (((object) pointerType).GetType() == typeof (PVoidType))
        return (ISymbol) null;
      int fieldOffset = 0;
      DataType referencedType1 = (DataType) pointerType.ReferencedType;
      DataType dataType1 = (DataType) ((IInstance) parent).DataType;
      if (dataType1 == null)
        throw new CannotResolveDataTypeException(((IInstance) parent).TypeName);
      if (referencedType1 == null)
        throw new CannotResolveDataTypeException(((IDataType) pointerType).Name);
      bool flag1 = (referencedType1.Flags & 1048576) == 1048576;
      bool flag2 = (((Symbol) parent).MemberFlags & 1048576) == 1048576;
      if (referencedType1 == null || referencedType1.Size == 0 && !flag2)
        throw new DataTypeException("Cannot create reference!", (IDataType) pointerType);
      string instanceName = ((IInstance) parent).InstanceName;
      bool isPointer = dataType1.ResolveType((DataTypeResolveStrategy) 1).IsPointer;
      bool isBitType = referencedType1.IsBitType;
      if (isPointer)
        instanceName += "^";
      ISymbol reference;
      if (referencedType1.Category == 5)
      {
        StructType type = (StructType) referencedType1;
        reference = (ISymbol) new StructInstance(instanceName, (IStructType) type, parent, fieldOffset);
      }
      else if (referencedType1.Category == 16)
      {
        InterfaceType type = (InterfaceType) referencedType1;
        reference = (ISymbol) new InterfaceInstance(instanceName, (IInterfaceType) type, parent, fieldOffset);
      }
      else if (referencedType1.Category == 4)
        reference = !flag2 ? (ISymbol) new ArrayInstance(instanceName, (IArrayType) referencedType1, parent, fieldOffset) : (ISymbol) new ArrayInstanceVariableSize(instanceName, (IArrayType) referencedType1, parent, fieldOffset);
      else if (referencedType1.Category == 15)
      {
        IDataType referencedType2 = ((ReferenceType) referencedType1).ReferencedType;
        reference = referencedType2 == null || referencedType2.Category != 5 ? (ISymbol) new ReferenceInstance(instanceName, (IReferenceType) referencedType1, parent, fieldOffset) : (ISymbol) new RpcStructReferenceInstance(parent, (IReferenceType) referencedType1, instanceName, fieldOffset);
      }
      else if (referencedType1.Category == 13)
        reference = (ISymbol) new PointerInstance(instanceName, (IPointerType) referencedType1, parent, fieldOffset);
      else if (referencedType1.Category == 2)
      {
        AliasType type = (AliasType) referencedType1;
        DataType dataType2 = (DataType) type.ResolveType((DataTypeResolveStrategy) 0);
        reference = (ISymbol) new AliasInstance(instanceName, (IAliasType) type, parent, fieldOffset);
      }
      else if (referencedType1.Category == 14)
        reference = (ISymbol) new UnionInstance(instanceName, (IUnionType) referencedType1, parent, fieldOffset);
      else if (referencedType1.Category == 10)
      {
        reference = (ISymbol) new StringInstance(instanceName, (IStringType) referencedType1, parent, fieldOffset);
      }
      else
      {
        if (referencedType1.Category == 14)
          AdsModule.Trace.TraceError("Unions not supported yet!");
        reference = (ISymbol) new Symbol(instanceName, (IDataType) referencedType1, parent, fieldOffset, ((ISymbolFactoryServicesProvider) parent).FactoryServices);
      }
      return reference;
    }

    protected override ISymbol OnCreateVirtualStruct(
      string instanceName,
      string instancePath,
      ISymbol? parent)
    {
      return (ISymbol) new VirtualStructInstance(instanceName, instancePath, parent, this.Services);
    }
  }
}
