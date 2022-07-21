// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.InstancePathSumEntity
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
  /// SumDataEntity InstancePath access (Read/Write by Instance Path)
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class InstancePathSumEntity : SumDataEntity
  {
    /// <summary>The instance path</summary>
    public readonly string InstancePath;
    protected static StringMarshaler s_marshaler = StringMarshaler.DefaultVariableLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.InstancePathSumEntity" /> class.
    /// </summary>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="readLength">Length of the value data to read.</param>
    /// <param name="writeLength">Length of the value data to be written.</param>
    public InstancePathSumEntity(string instancePath, int readLength, int writeLength)
      : base(readLength, writeLength)
    {
      this.InstancePath = instancePath;
    }

    /// <summary>Marshals the WriteInformation into the data.</summary>
    /// <remarks>Here the Instance Path will be marshalled into the data stream.</remarks>
    /// <param name="data">The data.</param>
    /// <returns>System.Int32.</returns>
    public int Marshal(Span<byte> data) => InstancePathSumEntity.s_marshaler.Marshal(this.InstancePath, data);
  }
}
