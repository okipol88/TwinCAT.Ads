// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumDeleteNotifications
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// SumCommandBase for Deleting Notifications (Not implemented in TwinCAT yet)
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" />
  internal class SumDeleteNotifications : SumCommandBase
  {
    private uint[] _notificationHandles;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumCreateHandles" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="notificationHandles">The notification handles.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// connection
    /// or
    /// variableHandles
    /// or
    /// lengths
    /// </exception>
    /// <exception cref="T:System.ArgumentException">Handles/lenghts mismatch!</exception>
    public SumDeleteNotifications(IAdsConnection connection, uint[] notificationHandles)
      : base(connection, (SumCommandMode) 6, (SumAccessMode) 4)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      this._notificationHandles = notificationHandles != null ? notificationHandles : throw new ArgumentNullException(nameof (notificationHandles));
      for (int index = 0; index < notificationHandles.Length; ++index)
        this.sumEntities.Add((SumDataEntity) new NotificationHandleReleaseSumEntity(notificationHandles[index]));
    }

    /// <summary>Calculates the length of the read Stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcReadLength() => 4 * this.sumEntities.Count;

    /// <summary>Calculates the length of the write stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcWriteLength() => 4 * this.sumEntities.Count;

    protected override int OnReadReturnData(
      ReadOnlyMemory<byte> reader,
      out IList<ReadOnlyMemory<byte>> readData,
      out int[] readDataSizes,
      out AdsErrorCode[] returnCodes)
    {
      readDataSizes = new int[this.sumEntities.Count];
      returnCodes = new AdsErrorCode[this.sumEntities.Count];
      readData = (IList<ReadOnlyMemory<byte>>) new List<ReadOnlyMemory<byte>>();
      int start = 0;
      for (int index = 0; index < this.sumEntities.Count; ++index)
      {
        returnCodes[index] = (AdsErrorCode) (int) BinaryPrimitives.ReadUInt32LittleEndian(reader.Span.Slice(start, 4));
        start += 4;
      }
      return start;
    }

    protected override int OnWriteSumEntityData(SumDataEntity entity, Span<byte> writer)
    {
      BinaryPrimitives.WriteUInt32LittleEndian(writer.Slice(0, 4), ((NotificationHandleReleaseSumEntity) entity).Handle);
      return 4;
    }

    public AdsErrorCode TryReleaseHandles(out AdsErrorCode[]? returnCodes)
    {
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      int[] readDataSizes = (int[]) null;
      AdsErrorCode adsErrorCode = this.Execute(new ReadOnlyMemory<byte>?(), out readData, out readDataSizes, out returnCodes);
      if (adsErrorCode == null)
      {
        for (int index = 0; index < this.sumEntities.Count; ++index)
        {
          uint num = 0;
          if ((int) returnCodes[index] == 0)
            num = BinaryPrimitives.ReadUInt32LittleEndian(readData[index].Span);
        }
      }
      return adsErrorCode;
    }

    /// <summary>release handles as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'CreateHandles' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumCommand" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumDeleteNotifications.ReleaseHandles" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumDeleteNotifications.TryReleaseHandles(TwinCAT.Ads.AdsErrorCode[]@)" />
    public async Task<ResultSumCommand> ReleaseHandlesAsync(
      CancellationToken cancel)
    {
      SumDeleteNotifications deleteNotifications = this;
      ResultSumReadRaw resultSumReadRaw = await deleteNotifications.ExecuteAsync(new ReadOnlyMemory<byte>?(), cancel).ConfigureAwait(false);
      if (((ResultAds) resultSumReadRaw).Succeeded)
      {
        IList<ReadOnlyMemory<byte>> readBlocks = resultSumReadRaw.ReadBlocks;
        for (int index = 0; index < deleteNotifications.sumEntities.Count; ++index)
        {
          uint num = 0;
          if (AdsErrorCodeExtensions.Succeeded((AdsErrorCode) (int) ((ResultSumCommand) resultSumReadRaw).SubErrors[index]))
            num = BinaryPrimitives.ReadUInt32LittleEndian(readBlocks[index].Span);
        }
      }
      return (ResultSumCommand) resultSumReadRaw;
    }

    /// <summary>Releases the handles.</summary>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumReleaseHandlesCommand failed!</exception>
    public void ReleaseHandles()
    {
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      this.TryReleaseHandles(out returnCodes);
      if (this.Failed)
        throw new AdsSumCommandException("SumReleaseHandles failed!", (ISumCommand) this);
    }
  }
}
