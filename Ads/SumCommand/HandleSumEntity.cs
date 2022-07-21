// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.HandleSumEntity
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>SumDataEntity Handle access</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class HandleSumEntity : SumDataEntity
  {
    /// <summary>The ADS handle</summary>
    public readonly uint Handle;
    /// <summary>The Primitive Type converter</summary>
    internal readonly PrimitiveTypeMarshaler Converter;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.HandleSumEntity" /> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="readLength">Length of the read.</param>
    /// <param name="writeLength">Length of the write.</param>
    /// <param name="converter">The converter.</param>
    protected internal HandleSumEntity(
      uint handle,
      int readLength,
      int writeLength,
      PrimitiveTypeMarshaler converter)
      : base(readLength, writeLength)
    {
      this.Handle = handle;
      this.Converter = converter;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.HandleSumEntity" /> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="converter">The converter.</param>
    protected internal HandleSumEntity(uint handle, PrimitiveTypeMarshaler converter)
    {
      this.Handle = handle;
      this.Converter = converter;
    }
  }
}
