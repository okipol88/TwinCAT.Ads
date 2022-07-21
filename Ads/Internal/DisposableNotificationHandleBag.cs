// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.DisposableNotificationHandleBag
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using TwinCAT.Ads.SumCommand;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Class DisposableNotificationHandleBag. This class cannot be inherited.
  /// </summary>
  /// <remarks>
  /// This helper class is used to support multiple registration of AdsNotification events in one step. With Disposing, all the registered AdsNotification events will
  /// be released again.
  /// </remarks>
  /// <seealso cref="T:TwinCAT.Ads.Internal.DisposableHandleBag" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DisposableNotificationHandleBag : DisposableHandleBag<ISymbol>
  {
    private NotificationSettings _settings = NotificationSettings.Default;
    private object? _userData;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.DisposableNotificationHandleBag" /> class.
    /// </summary>
    /// <param name="connection">The client.</param>
    /// <param name="symbols">The symbols.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">The user data.</param>
    /// <exception cref="T:System.ArgumentNullException">dict</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">dict</exception>
    public DisposableNotificationHandleBag(
      IAdsConnection connection,
      IList<ISymbol> symbols,
      NotificationSettings settings,
      object? userData)
      : base(connection, symbols)
    {
      this._settings = settings;
      this._userData = userData;
    }

    /// <summary>Creates the handles.</summary>
    /// <returns>System.Int32.</returns>
    public override int CreateHandles()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("DisposableHandleBag");
      ((IAdsNotifications) this.connection).AdsNotificationError += new EventHandler<AdsNotificationErrorEventArgs>(this.Connection_AdsNotificationError);
      base.CreateHandles();
      int num1 = 0;
      int handles = 0;
      int num2 = 0;
      foreach (ISymbol sourceObject in (IEnumerable<ISymbol>) this.sourceObjects)
      {
        uint handle = 0;
        int byteSize = ((IBitSize) sourceObject).ByteSize;
        AdsErrorCode errorCode = ((IAdsNotifications) this.connection).TryAddDeviceNotification(((IInstance) sourceObject).InstancePath, byteSize, this._settings, this._userData, ref handle);
        this.sumCommandHandles.Add((SumHandleEntry) new SumHandleInstancePathEntry(((IInstance) sourceObject).InstancePath, handle, errorCode));
        this.sourceHandlesDict.Add(sourceObject, handle);
        num1 += byteSize;
        if (errorCode == null)
          ++handles;
        else
          ++num2;
      }
      return handles;
    }

    /// <summary>Releases the handles.</summary>
    public override void ReleaseHandles()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("DisposableHandleBag");
      ((IAdsNotifications) this.connection).AdsNotificationError -= new EventHandler<AdsNotificationErrorEventArgs>(this.Connection_AdsNotificationError);
      foreach (KeyValuePair<ISymbol, uint> keyValuePair in (IEnumerable<KeyValuePair<ISymbol, uint>>) this.sourceHandlesDict)
      {
        if (keyValuePair.Value != 0U)
          ((IAdsNotifications) this.connection).TryDeleteDeviceNotification(keyValuePair.Value);
      }
      base.ReleaseHandles();
    }

    private void Connection_AdsNotificationError(object? sender, AdsNotificationErrorEventArgs e)
    {
      AdsInvalidNotificationException exception = e.Exception as AdsInvalidNotificationException;
      object sync = this.sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (exception == null || this.sourceHandlesDict == null || !this.sourceHandlesDict.Values.Contains(exception.Handle))
          return;
        this.OnNotificationError(exception);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    private void OnNotificationError(AdsInvalidNotificationException exception)
    {
    }
  }
}
