// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.UnionInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class representing a Union Instance</summary>
  public sealed class UnionInstance : 
    Symbol,
    IUnionInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize
  {
    internal UnionInstance(
      AdsSymbolEntry entry,
      IUnionType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 14;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.UnionInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="factoryServices">The factory services.</param>
    internal UnionInstance(
      string instanceName,
      ISymbol parent,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 14;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.UnionInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="factoryServices">The factory services.</param>
    internal UnionInstance(
      string instanceName,
      string instancePath,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, factoryServices)
    {
      this.Category = (DataTypeCategory) 14;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.UnionInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent of this member instance symbol.</param>
    internal UnionInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 14;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.UnionInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">if set to <c>true</c> [oversample].</param>
    /// <param name="parent">The parent.</param>
    internal UnionInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 14;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.UnionInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    internal UnionInstance(string instanceName, IUnionType type, ISymbol parent, int fieldOffset)
      : base(instanceName, (IDataType) type, parent, fieldOffset, ((ISymbolFactoryServicesProvider) parent).FactoryServices)
    {
      this.Category = (DataTypeCategory) 14;
    }

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol)
    {
      IUnionType dataType = (IUnionType) this.DataType;
      return dataType != null ? ((ICollection<IField>) dataType.Fields).Count : 0;
    }

    /// <summary>Called when [create sub symbols].</summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <returns>SymbolCollection.</returns>
    internal override ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentInstance)
    {
      ISymbolCollection<ISymbol> subSymbols;
      try
      {
        UnionType dataType = (UnionType) this.DataType;
        subSymbols = dataType == null ? (ISymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 0) : ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory.CreateFieldInstances((ISymbol) this, (IDataType) dataType);
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        throw;
      }
      return subSymbols;
    }

    /// <summary>
    /// Gets the member instances of the <see cref="T:TwinCAT.TypeSystem.IStructInstance">Struct Instance</see>.
    /// </summary>
    /// <value>The member instances.</value>
    public ISymbolCollection<ISymbol> FieldInstances => this.SubSymbols;

    /// <summary>
    /// Gets a value indicating whether this instance is primitive.
    /// </summary>
    /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
    public override bool IsPrimitiveType => false;

    /// <summary>
    /// Gets a value indicating whether this Symbol is a container/complex type.
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    public override bool IsContainerType => true;
  }
}
