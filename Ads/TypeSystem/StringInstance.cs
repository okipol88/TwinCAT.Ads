// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.StringInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Pointer Instance</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StringInstance : 
    Symbol,
    IStringInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize
  {
    internal StringInstance(
      AdsSymbolEntry entry,
      IStringType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 10;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StringInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">Indicates, that the oversample Symbol is to be created.</param>
    /// <param name="parent">The parent.</param>
    internal StringInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 10;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StringInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal StringInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 10;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StringInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    internal StringInstance(
      string instanceName,
      IStringType type,
      ISymbol parent,
      int fieldOffset)
      : base(instanceName, (IDataType) type, parent, fieldOffset, ((ISymbolFactoryServicesProvider) parent).FactoryServices)
    {
      this.Category = (DataTypeCategory) 10;
    }

    internal StringInstance(
      string instanceName,
      string? instancePath,
      uint indexGroup,
      uint indexOffset,
      IStringType? type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, indexGroup, indexOffset, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 10;
    }

    /// <summary>
    /// Gets the encoding of the String (Encoding.Default (STRING) or Encoding.UNICODE (WSTRING))
    /// </summary>
    /// <value>The encoding.</value>
    public override Encoding ValueEncoding
    {
      get
      {
        Encoding encoding = (Encoding) null;
        if (this.Attributes == null || !EncodingAttributeConverter.TryGetEncoding((IEnumerable<ITypeAttribute>) this.Attributes, out encoding))
          encoding = ((IStringType) this.DataType)?.Encoding;
        if (encoding == null)
        {
          IBinder binder = this.Binder;
          encoding = binder == null ? StringMarshaler.DefaultEncoding : ((ISymbolServer) binder.Provider).DefaultValueEncoding;
        }
        return encoding;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is fixed length.
    /// </summary>
    /// <value><c>true</c> if this instance is fixed length; otherwise, <c>false</c>.</value>
    public bool IsFixedLength
    {
      get
      {
        IStringType dataType = (IStringType) this.DataType;
        return dataType == null || dataType.IsFixedLength;
      }
    }

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol) => 0;
  }
}
