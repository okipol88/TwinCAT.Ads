// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.StructInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class representing a Struct Instance</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class StructInstance : 
    InterfaceInstance,
    IStructInstance,
    IInterfaceInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IRpcCallableInstance
  {
    internal StructInstance(
      AdsSymbolEntry entry,
      IStructType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, (IInterfaceType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 5;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="factoryServices">The factory services.</param>
    protected StructInstance(
      string instanceName,
      string instancePath,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, factoryServices)
    {
      this.Category = (DataTypeCategory) 5;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal StructInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 5;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">if set to <c>true</c> [oversample].</param>
    /// <param name="parent">The parent.</param>
    internal StructInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 5;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StructInstance(string instanceName, IStructType type, ISymbol parent, int fieldOffset)
      : base(instanceName, (IInterfaceType) type, parent, fieldOffset)
    {
      this.Category = (DataTypeCategory) 5;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public StructInstance(
      string instanceName,
      string? instancePath,
      uint indexGroup,
      uint indexOffset,
      IStructType? type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, indexGroup, indexOffset, (IInterfaceType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 5;
    }
  }
}
