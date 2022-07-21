// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.InstancePathSumReadEntity
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class InstancePathSumReadEntity : InstancePathSumEntity
  {
    public InstancePathSumReadEntity(string instancePath, int valueLength)
      : base(instancePath, valueLength, InstancePathSumEntity.s_marshaler.MarshalSize(instancePath))
    {
    }
  }
}
