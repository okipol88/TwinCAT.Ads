// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.HandleSumWriteAnyEntity
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
  /// SumDataEntity Handle access (write by handle and Primitive/Any type)
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class HandleSumWriteAnyEntity : HandleSumEntity
  {
    /// <summary>
    /// Primitive (Any) type that can be marshalled via <see cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" />
    /// </summary>
    public readonly Type Type;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.HandleSumWriteAnyEntity" /> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="tp">The tp.</param>
    /// <param name="converter">The converter.</param>
    public HandleSumWriteAnyEntity(uint handle, Type tp, PrimitiveTypeMarshaler converter)
      : base(handle, 0, -1, converter)
    {
      this.Type = tp;
      if (tp == typeof (string))
        this.writeLength = -1;
      else
        this.writeLength = this.Converter.MarshalSize(tp);
    }
  }
}
