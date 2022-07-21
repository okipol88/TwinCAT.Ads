// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.FailFastHandlerResetter
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
  /// <summary>Class FailFastHandlerResetter.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class FailFastHandlerResetter
  {
    /// <summary>
    /// Resets the <see cref="T:TwinCAT.Ads.IFailFastHandler" /> for the specified connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <exception cref="T:System.ArgumentNullException">connection</exception>
    /// <remarks>This can be used to reset the FailFast errors state before timeout.</remarks>
    /// <returns>true, if the connection has a FailFastHandler, otherwise false.</returns>
    public static bool Reset(IConnection connection)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (connection is AdsConnection adsConnection && adsConnection.IsConnected)
      {
        IInterceptedClient client = (IInterceptedClient) adsConnection.Client;
        if (client.Interceptors != null)
        {
          IFailFastHandler failFastHandler = (IFailFastHandler) client.Interceptors.CombinedInterceptors.FirstOrDefault<ICommunicationInterceptor>((Func<ICommunicationInterceptor, bool>) (item => item is IFailFastHandler));
          if (failFastHandler != null)
          {
            failFastHandler.Reset();
            return true;
          }
        }
      }
      return false;
    }
  }
}
