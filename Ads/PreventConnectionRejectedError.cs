// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.PreventConnectionRejectedError
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Linq;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Helper class for preventing ConnectionRejected Errors (preventing the Error Trip on WSA_ConnRefused
  /// </summary>
  /// <exclude />
  /// <seealso cref="T:System.IDisposable" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PreventConnectionRejectedError : IDisposable
  {
    private bool _old;
    private bool _used;
    private AdsConnection _connection;
    /// <summary>Disposed flag</summary>
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsTimeoutSetter" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <exception cref="T:System.ArgumentNullException">client</exception>
    public PreventConnectionRejectedError(AdsConnection connection)
    {
      this._connection = connection != null ? connection : throw new ArgumentNullException(nameof (connection));
      this._old = this.getCurrentValue();
      this.register(true);
      this._used = true;
    }

    private bool getCurrentValue()
    {
      bool currentValue = false;
      if (this._connection.IsConnected)
      {
        IInterceptedClient client = (IInterceptedClient) this._connection.Client;
        if (client.Interceptors != null && client.Interceptors.CombinedInterceptors.Count > 0)
        {
          IPreventRejected preventRejected = (IPreventRejected) client.Interceptors.CombinedInterceptors.FirstOrDefault<ICommunicationInterceptor>((Func<ICommunicationInterceptor, bool>) (item => item is IPreventRejected));
          if (preventRejected != null)
            currentValue = preventRejected.PreventRejectedConnection;
        }
      }
      return currentValue;
    }

    private void register(bool set)
    {
      IInterceptedClient client = (IInterceptedClient) this._connection.Client;
      if (!this._connection.IsConnected || client.Interceptors == null)
        return;
      foreach (ICommunicationInterceptor combinedInterceptor in client.Interceptors.CombinedInterceptors)
      {
        if (combinedInterceptor is IPreventRejected preventRejected)
          preventRejected.PreventRejectedConnection = set;
      }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.AdsTimeoutSetter" /> class.
    /// </summary>
    ~PreventConnectionRejectedError() => this.Dispose(false);

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
        this.register(this._old);
        this._used = false;
      }
      this._disposed = true;
    }
  }
}
