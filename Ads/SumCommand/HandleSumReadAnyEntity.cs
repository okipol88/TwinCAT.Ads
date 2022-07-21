// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.HandleSumReadAnyEntity
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// SumDataEntity Handle access (read by handle and Primitive/Any type)
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class HandleSumReadAnyEntity : HandleSumEntity
  {
    /// <summary>
    /// Primitive (Any) type that can be marshalled via <see cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" />
    /// </summary>
    public readonly AnyTypeSpecifier TypeSpec;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.HandleSumReadAnyEntity" /> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="tp">The tp.</param>
    /// <param name="converter">The converter.</param>
    public HandleSumReadAnyEntity(uint handle, Type tp, PrimitiveTypeMarshaler converter)
      : base(handle, converter)
    {
      this.readLength = PrimitiveTypeMarshaler.Default.MarshalSize(tp);
      this.writeLength = 0;
      this.TypeSpec = new AnyTypeSpecifier(tp);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.HandleSumReadAnyEntity" /> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="strLen">Length of the string.</param>
    /// <param name="converter">The converter.</param>
    public HandleSumReadAnyEntity(uint handle, int strLen, PrimitiveTypeMarshaler converter)
      : base(handle, -1, 0, converter)
    {
      this.TypeSpec = new AnyTypeSpecifier(typeof (string), strLen);
      this.readLength = converter.MarshalSize(strLen);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.HandleSumReadAnyEntity" /> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="arrayType">Type of the array.</param>
    /// <param name="anyType">Any type.</param>
    /// <param name="converter">The converter.</param>
    /// <exclude />
    public HandleSumReadAnyEntity(
      uint handle,
      Type arrayType,
      AnyTypeSpecifier anyType,
      PrimitiveTypeMarshaler converter)
      : base(handle, -1, 0, converter)
    {
      this.TypeSpec = anyType;
      int size = 0;
      converter.TryGetArrayMarshalSize(this.TypeSpec, converter.DefaultValueEncoding, out size);
      this.readLength = size;
    }
  }
}
