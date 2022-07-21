// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.NotificationHandleSumEntity
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.ComponentModel;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>SumDataEntity Handle access</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class NotificationHandleSumEntity : IgIoSumEntity
  {
    private NotificationSettings _settings;
    private int _notificationDataLength;

    public NotificationHandleSumEntity(
      uint indexGroup,
      uint indexOffset,
      NotificationSettings settings,
      int dataLength)
      : base(indexGroup, indexOffset, 4, 8 + NotificationSettingsMarshaller.MarshalSize(true))
    {
      this._settings = settings;
      this._notificationDataLength = dataLength;
    }

    private static int MarshalSize => 8 + NotificationSettingsMarshaller.MarshalSize(true);

    public ReadOnlyMemory<byte> GetWriteBytes()
    {
      byte[] array = new byte[NotificationHandleSumEntity.MarshalSize];
      int start1 = 0;
      BinaryPrimitives.WriteUInt32LittleEndian(array.AsSpan<byte>(start1), this.IndexGroup);
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(array.AsSpan<byte>(start2), this.IndexOffset);
      int start3 = start2 + 4;
      int num = start3 + NotificationSettingsMarshaller.Marshal(this._settings, this._notificationDataLength, array.AsSpan<byte>(start3), true);
      return (ReadOnlyMemory<byte>) array;
    }
  }
}
