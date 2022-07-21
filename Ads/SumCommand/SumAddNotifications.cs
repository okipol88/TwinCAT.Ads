// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumAddNotifications
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// SumCommandBase for registering Notifications (Not implemented in TwinCAT yet)
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" />
  /// <exclude />
  internal class SumAddNotifications : SumCommandBase
  {
    private IAdsConnection _connection;
    /// <summary>
    /// Dictionary Handle --&gt; Notification Length (Variable Symbol Byte Size)
    /// </summary>
    private NotificationSettings _settings;
    private uint[] _variableHandles;
    private int[] _variableLengths;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumCreateHandles" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="variableHandles">The variable handles.</param>
    /// <param name="lengths">The lengths.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="stream">The stream.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// connection
    /// or
    /// variableHandles
    /// or
    /// lengths
    /// </exception>
    /// <exception cref="T:System.ArgumentException">Handles/lenghts mismatch!</exception>
    public SumAddNotifications(
      IAdsConnection connection,
      uint[] variableHandles,
      int[] lengths,
      NotificationSettings settings,
      MemoryStream stream)
      : base(connection, (SumCommandMode) 5, (SumAccessMode) 1)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (variableHandles == null)
        throw new ArgumentNullException(nameof (variableHandles));
      if (lengths == null)
        throw new ArgumentNullException(nameof (lengths));
      if (variableHandles.Length != lengths.Length)
        throw new ArgumentException("Handles/lenghts mismatch!");
      int num = ((IEnumerable<int>) lengths).Max();
      if (stream.Length < (long) num)
        throw new ArgumentException("Notification Buffer/Stream is to small");
      this._connection = connection;
      this._variableHandles = variableHandles;
      this._variableLengths = lengths;
      this._settings = settings;
      this.sumEntities = this.CreateSumEntityInfos();
    }

    /// <summary>Creates the sum entity infos.</summary>
    /// <returns>IList&lt;SumDataEntityInfo&gt;.</returns>
    private IList<SumDataEntity> CreateSumEntityInfos()
    {
      List<SumDataEntity> sumEntityInfos = new List<SumDataEntity>();
      for (int index = 0; index < this._variableHandles.Length; ++index)
      {
        NotificationHandleSumEntity notificationHandleSumEntity = new NotificationHandleSumEntity(61445U, this._variableHandles[index], this._settings, this._variableLengths[index]);
        sumEntityInfos.Add((SumDataEntity) notificationHandleSumEntity);
      }
      return (IList<SumDataEntity>) sumEntityInfos;
    }

    /// <summary>Calculates the length of the read Stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcReadLength() => 8 * this.sumEntities.Count;

    /// <summary>Calculates the length of the write stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcWriteLength() => (8 + NotificationSettingsMarshaller.MarshalSize(true)) * this.sumEntities.Count;

    protected override int OnReadReturnData(
      ReadOnlyMemory<byte> reader,
      out IList<ReadOnlyMemory<byte>> readData,
      out int[] readDataSizes,
      out AdsErrorCode[] returnCodes)
    {
      readDataSizes = new int[this.sumEntities.Count];
      returnCodes = new AdsErrorCode[this.sumEntities.Count];
      readData = (IList<ReadOnlyMemory<byte>>) new List<ReadOnlyMemory<byte>>(this.sumEntities.Count);
      int start1 = 0;
      for (int index = 0; index < this.sumEntities.Count; ++index)
      {
        returnCodes[index] = (AdsErrorCode) (int) BinaryPrimitives.ReadUInt32LittleEndian(reader.Span.Slice(start1, 4));
        int start2 = start1 + 4;
        readDataSizes[index] = 4;
        readData.Add(reader.Slice(start2, readDataSizes[index]));
        start1 = start2 + readDataSizes[index];
      }
      return start1;
    }

    protected override int OnWriteSumEntityData(SumDataEntity entity, Span<byte> writer)
    {
      NotificationHandleSumEntity notificationHandleSumEntity = (NotificationHandleSumEntity) entity;
      notificationHandleSumEntity.GetWriteBytes().Span.CopyTo(writer);
      return notificationHandleSumEntity.WriteLength;
    }

    public AdsErrorCode TryCreateHandles(
      out uint[]? handles,
      out AdsErrorCode[]? returnCodes)
    {
      ISumHandleCollection handles1 = (ISumHandleCollection) null;
      AdsErrorCode handles2 = this.TryCreateHandles(out handles1);
      if (AdsErrorCodeExtensions.Succeeded(handles2))
      {
        returnCodes = new AdsErrorCode[handles1.Count];
        handles = new uint[handles1.Count];
        for (int index = 0; index < handles1.Count; ++index)
        {
          SumNotificationHandleEntry notificationHandleEntry = (SumNotificationHandleEntry) handles1[index];
          handles[index] = notificationHandleEntry.NotificationHandle;
          returnCodes[index] = handles1[index].ErrorCode;
        }
      }
      else
      {
        handles = (uint[]) null;
        returnCodes = (AdsErrorCode[]) null;
      }
      return handles2;
    }

    /// <summary>Tries to create the Handles.</summary>
    /// <param name="handles">The handles.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AdsErrorCode TryCreateHandles(out ISumHandleCollection? handles)
    {
      IList<SumDataEntity> sumEntityInfos = this.CreateSumEntityInfos();
      handles = (ISumHandleCollection) null;
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      int[] readDataSizes = (int[]) null;
      AdsErrorCode handles1 = this.Execute(new ReadOnlyMemory<byte>?(), out readData, out readDataSizes, out returnCodes);
      if (handles1 == null)
      {
        handles = (ISumHandleCollection) new SumHandleList();
        for (int index = 0; index < sumEntityInfos.Count; ++index)
        {
          uint notificationHandle = 0;
          if ((int) returnCodes[index] == 0)
            notificationHandle = BinaryPrimitives.ReadUInt32LittleEndian(readData[index].Span);
          handles.Add((SumHandleEntry) new SumNotificationHandleEntry(this._variableHandles[index], notificationHandle, (AdsErrorCode) (int) returnCodes[index]));
        }
      }
      return handles1;
    }

    /// <summary>Create handles as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'CreateHandles' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumHandles" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <exclude />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumAddNotifications.TryCreateHandles(TwinCAT.Ads.SumCommand.ISumHandleCollection@)" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<ResultSumHandles> CreateHandlesAsync(
      CancellationToken cancel)
    {
      SumAddNotifications addNotifications = this;
      IList<SumDataEntity> infos = addNotifications.CreateSumEntityInfos();
      uint[] handles = new uint[addNotifications._variableHandles.Length];
      ResultSumReadRaw resultSumReadRaw = await addNotifications.ExecuteAsync(new ReadOnlyMemory<byte>?(), cancel).ConfigureAwait(false);
      if (((ResultAds) resultSumReadRaw).Succeeded)
      {
        IList<ReadOnlyMemory<byte>> readBlocks = resultSumReadRaw.ReadBlocks;
        for (int index = 0; index < infos.Count; ++index)
        {
          if (AdsErrorCodeExtensions.Succeeded((AdsErrorCode) (int) ((ResultSumCommand) resultSumReadRaw).SubErrors[index]))
            handles[index] = BinaryPrimitives.ReadUInt32LittleEndian(readBlocks[index].Span);
        }
      }
      ResultSumHandles handlesAsync = new ResultSumHandles(((ResultAds) resultSumReadRaw).ErrorCode, ((ResultSumCommand) resultSumReadRaw).SubErrors, handles);
      infos = (IList<SumDataEntity>) null;
      handles = (uint[]) null;
      return handlesAsync;
    }
  }
}
