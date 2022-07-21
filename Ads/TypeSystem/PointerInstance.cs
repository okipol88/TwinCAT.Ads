// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.PointerInstance
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
  /// <summary>Pointer Instance</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PointerInstance : 
    Symbol,
    IPointerInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize
  {
    internal PointerInstance(
      AdsSymbolEntry entry,
      IPointerType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 13;
      if (entry.Size <= 0U)
        return;
      ((Binder) factoryServices.Binder).SetPlatformPointerSize((int) entry.Size);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PointerInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">Indicates, that the oversample Symbol is to be created.</param>
    /// <param name="parent">The parent.</param>
    internal PointerInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 13;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PointerInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal PointerInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 13;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PointerInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal PointerInstance(
      string instanceName,
      IPointerType type,
      ISymbol parent,
      int fieldOffset)
      : base(instanceName, (IDataType) type, parent, fieldOffset, ((ISymbolFactoryServicesProvider) parent).FactoryServices)
    {
      this.Category = (DataTypeCategory) 13;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal PointerInstance(
      string instanceName,
      string? instancePath,
      uint indexGroup,
      uint indexOffset,
      IPointerType? type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, indexGroup, indexOffset, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 13;
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
    public override bool IsContainerType => true;

    internal bool IsAnySizeArray
    {
      get
      {
        AdsSymbolFlags flags = this.Flags;
        return (this.MemberFlags & 1048576) == 1048576;
      }
    }

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol)
    {
      int num = 0;
      IPointerType dataType = (IPointerType) this.DataType;
      if (dataType != null && ((object) dataType).GetType() == typeof (PVoidType))
        return 0;
      IDataType referencedType = dataType?.ReferencedType;
      return this.IsAnySizeArray || referencedType != null && ((IBitSize) referencedType).Size > 0 ? 1 : num;
    }

    /// <summary>Creates the sub symbols collection.</summary>
    internal override ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentInstance)
    {
      ISymbolCollection<ISymbol> subSymbols = base.OnCreateSubSymbols(parentInstance);
      try
      {
        IPointerType dataType = (IPointerType) this.DataType;
        switch (dataType)
        {
          case null:
          case PVoidType _:
            break;
          default:
            ISymbol referenceInstance = ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory.CreateReferenceInstance(dataType, parentInstance);
            if (referenceInstance != null)
            {
              if (!this.IsAnySizeArray)
              {
                if (((IBitSize) referenceInstance).Size <= 0)
                  break;
              }
              ((ICollection<ISymbol>) subSymbols).Add(referenceInstance);
              break;
            }
            break;
        }
      }
      catch (Exception ex)
      {
        AdsModule.Trace.TraceError(ex);
      }
      return subSymbols;
    }

    /// <summary>Gets the resolved reference of Pointer / Reference</summary>
    /// <value>The reference symbol or NULL if PVOID Pointer.</value>
    public ISymbol? Reference => this.SubSymbolCount > 0 ? ((IList<ISymbol>) this.SubSymbols)[0] : (ISymbol) null;
  }
}
