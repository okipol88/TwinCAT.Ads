// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumHandleRead
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// Read (primitive, Any) values by Handle SumCommandBase.
  /// </summary>
  /// <remarks>This is an ADS Sum Command to access values by handle information. It is always used in combination with <seealso cref="T:TwinCAT.Ads.SumCommand.SumCreateHandles" /> and <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />.
  /// By design (and in contrast to the symbolic access in <see cref="T:TwinCAT.Ads.SumCommand.SumRead" />, <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" />) this access method can act only with ADS ANY Type (Primitive) values (disadvantage).
  /// The Advantage is, that no symbolic information must be loaded before accessing the values, see samples:
  /// </remarks>
  /// <example>
  /// <code language="C#" title="Usage of Sum commands with handles (CreateHandles, Read, Write, ReleaseHandles)" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMCOMMANDBYHANDLE" />
  /// </example>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" />
  public class SumHandleRead : SumRead
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" /> class (Only for internal use).
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="handleTypeDict">The handle type dictionary.</param>
    /// <param name="unicode">if set to <c>true</c> [unicode].</param>
    /// <param name="strlen">The strlen.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SumHandleRead(
      IAdsConnection connection,
      IDictionary<uint, Type> handleTypeDict,
      bool unicode = false,
      int strlen = 256)
      : base(connection, (SumAccessMode) 1)
    {
      if (handleTypeDict == null)
        throw new ArgumentNullException(nameof (handleTypeDict));
      PrimitiveTypeMarshaler unicode1 = PrimitiveTypeMarshaler.Default;
      if (unicode)
        unicode1 = PrimitiveTypeMarshaler.Unicode;
      List<SumDataEntity> sumDataEntityList = new List<SumDataEntity>();
      foreach (KeyValuePair<uint, Type> keyValuePair in (IEnumerable<KeyValuePair<uint, Type>>) handleTypeDict)
      {
        if (keyValuePair.Value == typeof (string))
          sumDataEntityList.Add((SumDataEntity) new HandleSumReadAnyEntity(keyValuePair.Key, strlen, unicode1));
        else
          sumDataEntityList.Add((SumDataEntity) new HandleSumReadAnyEntity(keyValuePair.Key, keyValuePair.Value, unicode1));
      }
      this.sumEntities = (IList<SumDataEntity>) sumDataEntityList;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="serverHandles">The server Handles</param>
    /// <param name="valueTypes">The value types (ANY/Primitive .NET types only)</param>
    /// <exclude />
    public SumHandleRead(IAdsConnection connection, uint[] serverHandles, Type[] valueTypes)
      : this(connection, serverHandles, valueTypes, false, 256)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="serverHandles">The handles.</param>
    /// <param name="valueTypes">The value types (ANY/Primitive .NET types only)</param>
    /// <param name="unicode">if set to <c>true</c> use unicode access.</param>
    /// <param name="strlen">Default (fixed) string len.</param>
    /// <exclude />
    public SumHandleRead(
      IAdsConnection connection,
      uint[] serverHandles,
      Type[] valueTypes,
      bool unicode,
      int strlen)
      : base(connection, (SumAccessMode) 1)
    {
      if (serverHandles == null)
        throw new ArgumentNullException(nameof (serverHandles));
      if (valueTypes == null)
        throw new ArgumentNullException(nameof (valueTypes));
      List<SumDataEntity> sumDataEntityList = new List<SumDataEntity>();
      PrimitiveTypeMarshaler unicode1 = PrimitiveTypeMarshaler.Default;
      if (unicode)
        unicode1 = PrimitiveTypeMarshaler.Unicode;
      for (int index = 0; index < serverHandles.Length; ++index)
      {
        uint serverHandle = serverHandles[index];
        Type valueType = valueTypes[index];
        if (valueType == typeof (string))
          sumDataEntityList.Add((SumDataEntity) new HandleSumReadAnyEntity(serverHandle, strlen, unicode1));
        else
          sumDataEntityList.Add((SumDataEntity) new HandleSumReadAnyEntity(serverHandle, valueType, unicode1));
      }
      this.sumEntities = (IList<SumDataEntity>) sumDataEntityList;
    }

    public AdsErrorCode TryRead(out object[]? values, out AdsErrorCode[]? returnCodes)
    {
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      values = (object[]) null;
      AdsErrorCode adsErrorCode = this.TryReadRaw(out readData, out returnCodes);
      if (adsErrorCode == null)
      {
        values = new object[this.sumEntities.Count];
        int num = 0;
        for (int index = 0; index < this.sumEntities.Count; ++index)
        {
          if ((AdsErrorCode) (int) returnCodes[index] == null)
          {
            HandleSumReadAnyEntity sumEntity = (HandleSumReadAnyEntity) this.sumEntities[index];
            PrimitiveTypeMarshaler converter = sumEntity.Converter;
            num = converter.Unmarshal(sumEntity.TypeSpec, readData[index].Span, converter.DefaultValueEncoding, out values[index]);
          }
        }
      }
      return adsErrorCode;
    }

    /// <summary>Read the values asynchronously.</summary>
    /// <param name="cancel">The cancellation token</param>
    /// <returns>An asynchronous task that represents the 'ReadSymbols' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumValues" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumHandleRead.Read" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumHandleRead.TryRead(System.Object[]@,TwinCAT.Ads.AdsErrorCode[]@)" />
    public async Task<ResultSumValues> ReadAsync(CancellationToken cancel)
    {
      SumHandleRead sumHandleRead = this;
      object[] values = (object[]) null;
      ResultSumReadRaw resultSumReadRaw = await sumHandleRead.ReadRawAsync(cancel).ConfigureAwait(false);
      if (((ResultAds) resultSumReadRaw).Succeeded)
      {
        values = new object[sumHandleRead.sumEntities.Count];
        int num = 0;
        for (int index = 0; index < sumHandleRead.sumEntities.Count; ++index)
        {
          if ((AdsErrorCode) (int) ((ResultSumCommand) resultSumReadRaw).SubErrors[index] == null)
          {
            HandleSumReadAnyEntity sumEntity = (HandleSumReadAnyEntity) sumHandleRead.sumEntities[index];
            PrimitiveTypeMarshaler converter = sumEntity.Converter;
            num = converter.Unmarshal(sumEntity.TypeSpec, resultSumReadRaw.ReadBlocks[index].Span, converter.DefaultValueEncoding, out values[index]);
          }
        }
      }
      ResultSumValues resultSumValues = new ResultSumValues(((ResultAds) resultSumReadRaw).ErrorCode, ((ResultSumCommand) resultSumReadRaw).SubErrors, values);
      values = (object[]) null;
      return resultSumValues;
    }

    /// <summary>Reads the values.</summary>
    /// <returns>System.Object[].</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumAnyReadByHandleCommand failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumHandleRead.TryRead(System.Object[]@,TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumHandleRead.ReadAsync(System.Threading.CancellationToken)" />
    public object[] Read()
    {
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      object[] values = (object[]) null;
      this.TryRead(out values, out returnCodes);
      if (this.Failed)
        throw new AdsSumCommandException("SumHandleRead failed!", (ISumCommand) this);
      return values;
    }
  }
}
