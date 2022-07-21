// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ValueAccess.AdsValueAccessorBase
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.ValueAccess
{
  /// <summary>Abstract base class for an AdsValue accessor.</summary>
  /// <seealso cref="T:TwinCAT.ValueAccess.RpcNotificationAccessorBase" />
  /// <seealso cref="T:TwinCAT.ValueAccess.IAccessorValueAny" />
  internal abstract class AdsValueAccessorBase : RpcNotificationAccessorBase, IAccessorValueAny
  {
    protected bool automaticReconnection;

    internal bool AutomaticReconnection
    {
      get => this.automaticReconnection;
      set => this.automaticReconnection = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.ValueAccess.AdsValueAccessorBase" /> class.
    /// </summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="session">The session.</param>
    /// <param name="settings">The settings.</param>
    protected AdsValueAccessorBase(
      IAccessorValueFactory valueFactory,
      ISession session,
      NotificationSettings settings)
      : base(valueFactory, session, (INotificationSettings) settings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.ValueAccess.AdsValueAccessorBase" /> class.
    /// </summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="settings">The settings.</param>
    protected AdsValueAccessorBase(
      IAccessorValueFactory valueFactory,
      IConnection connection,
      NotificationSettings settings)
      : base(valueFactory, connection, (INotificationSettings) settings)
    {
    }

    /// <summary>
    /// Tries to read the value of the symbol and returns the value as instance of the specified type.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueType">The value type.</param>
    /// <param name="value">The value.</param>
    /// <param name="utcReadTime">The UTC read time.</param>
    /// <returns>AdsErrorCode.</returns>
    public abstract int TryReadAnyValue(
      ISymbol symbol,
      Type valueType,
      out object? value,
      out DateTimeOffset? utcReadTime);

    public abstract Task<ResultReadValueAccess> ReadAnyValueAsync(
      ISymbol symbol,
      Type valueType,
      CancellationToken cancel);

    /// <summary>
    /// Tries to write the data within the value object as the symbol value.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueObject">The value object.</param>
    /// <param name="utcReadTime">The UTC read time.</param>
    /// <returns>AdsErrorCode.</returns>
    public abstract int TryWriteAnyValue(
      ISymbol symbol,
      object valueObject,
      out DateTimeOffset? utcReadTime);

    public abstract Task<ResultWriteAccess> WriteAnyValueAsync(
      ISymbol symbol,
      object valueObject,
      CancellationToken cancel);

    /// <summary>
    /// Tries to read the value of the symbol and updates the referenced value object with that data
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueObject">The value object.</param>
    /// <param name="utcReadTime">The UTC read time.</param>
    /// <returns>AdsErrorCode.</returns>
    public abstract int TryUpdateAnyValue(
      ISymbol symbol,
      ref object valueObject,
      out DateTimeOffset? utcReadTime);

    /// <summary>
    /// Reads the value of the symbol and updates the referenced value object with that data.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueObject">The value object.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultReadValueAccess&gt;.</returns>
    public abstract Task<ResultReadValueAccess> UpdateAnyValueAsync(
      ISymbol symbol,
      object valueObject,
      CancellationToken cancel);
  }
}
