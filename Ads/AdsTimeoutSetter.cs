// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsTimeoutSetter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Helper class for overwrite the Timeout on <see cref="T:TwinCAT.Ads.AdsClient" /> temporarily.
  /// </summary>
  /// <seealso cref="T:System.IDisposable" />
  /// <exclude />
  public sealed class AdsTimeoutSetter : IDisposable
  {
    private int _newTimeout = -1;
    private int _oldTimeout = -1;
    private IConnection _connection;
    private bool _used;
    /// <summary>Disposed flag</summary>
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsTimeoutSetter" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:System.ArgumentNullException">client</exception>
    public AdsTimeoutSetter(IConnection connection, int timeout)
    {
      this._connection = connection != null ? connection : throw new ArgumentNullException(nameof (connection));
      this._newTimeout = timeout;
      if (!connection.IsConnected || this._newTimeout <= 0)
        return;
      this._oldTimeout = this._connection.Timeout;
      this._connection.Timeout = this._newTimeout;
      this._used = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsTimeoutSetter" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="timeout">The timeout.</param>
    public AdsTimeoutSetter(IConnection connection, TimeSpan timeout)
      : this(connection, (int) timeout.TotalMilliseconds)
    {
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.AdsTimeoutSetter" /> class.
    /// </summary>
    ~AdsTimeoutSetter() => this.Dispose(false);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing && this._used)
      {
        if (this._connection != null)
          this._connection.Timeout = this._oldTimeout;
        this._used = false;
      }
      this._disposed = true;
    }
  }
}
