// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.PrimitiveType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class PrimitiveType.</summary>
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.DataType" />
  public class PrimitiveType : DataType, IPrimitiveType, IDataType, IBitSize
  {
    private PrimitiveTypeFlags _primitiveFlags;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PrimitiveType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="dataTypeId">The data type identifier.</param>
    /// <param name="byteSize">Size of the byte.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="dotnetType">Type of the dotnet.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal PrimitiveType(
      string name,
      AdsDataTypeId dataTypeId,
      int byteSize,
      PrimitiveTypeFlags flags,
      Type dotnetType)
      : base(name, dataTypeId, (DataTypeCategory) 1, byteSize, dotnetType)
    {
      this._primitiveFlags = flags;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PrimitiveType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="dotnetType">Type of the dotnet.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal PrimitiveType(string name, PrimitiveTypeFlags flags, Type dotnetType)
      : base(name, (DataTypeCategory) 1, dotnetType)
    {
      this._primitiveFlags = flags;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PrimitiveType" /> class.
    /// </summary>
    /// <param name="name">The name of the data type.</param>
    /// <param name="dotnetType">The mapped dotnet type.</param>
    public PrimitiveType(string name, Type dotnetType)
      : base(name, (DataTypeCategory) 1, dotnetType)
    {
      this._primitiveFlags = PrimitiveTypeMarshaler.GetPrimitiveFlags(dotnetType);
    }

    internal PrimitiveType(AdsDataTypeEntry entry, PrimitiveTypeFlags flags, Type? dotnetType)
      : base((DataTypeCategory) 1, entry)
    {
      this._primitiveFlags = flags;
      this.ManagedType = dotnetType;
    }

    /// <summary>
    /// Indicates types of different PrimitiveTypes with flags.
    /// </summary>
    /// <value>The primitive flags.</value>
    public PrimitiveTypeFlags PrimitiveFlags => this._primitiveFlags;
  }
}
