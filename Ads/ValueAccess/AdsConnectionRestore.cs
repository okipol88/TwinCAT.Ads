// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ValueAccess.AdsConnectionRestore
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads.ValueAccess
{
  /// <summary>Restores an AdsConnection.</summary>
  /// <seealso cref="T:System.IDisposable" />
  internal class AdsConnectionRestore : IDisposable
  {
    protected IAdsConnection? connection;
    protected bool disconnectOnDispose;
    private bool _isDisposed;

    internal AdsConnectionRestore(AdsValueAccessorBase accessor)
    {
      if (accessor == null)
        throw new ArgumentNullException(nameof (accessor));
      if (!accessor.AutomaticReconnection)
        return;
      this.connection = (IAdsConnection) accessor.Connection;
      if (this.connection == null)
        throw new ArgumentException("No connection established!", nameof (accessor));
      if (this.connection == null)
        throw new ClientNotConnectedException();
      if (((IConnection) this.connection).IsConnected)
        return;
      this.connection = ((IConnection) this.connection).Session != null ? (IAdsConnection) ((IConnection) this.connection).Session.Connect() : throw new ClientNotConnectedException();
      this.disconnectOnDispose = true;
    }

    public void Dispose()
    {
      if (this._isDisposed)
        return;
      this.Dispose(true);
      this._isDisposed = true;
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || !this.disconnectOnDispose || this.connection == null)
        return;
      ((IConnection) this.connection).Disconnect();
    }
  }
}
