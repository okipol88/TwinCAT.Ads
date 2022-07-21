// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsSession
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using Microsoft.Extensions.Logging;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>AdsSession class</summary>
  /// <remarks>On top of the well known <see cref="T:TwinCAT.Ads.AdsClient" /> class that is used traditionally for ADS communication,
  /// the <see cref="T:TwinCAT.Ads.AdsSession" /> class provides the following additionally abilities out of the box:
  /// 
  /// These are used to provide more stable connections to ADS Servers than the <see cref="T:TwinCAT.Ads.AdsClient" /> can provide. The main issues are Resurrection / Self-Healing
  /// after communication timeouts, faster and less error prone reaction to communication errors (not necessarily waiting for communication timeouts) und enhanced
  /// communication diagnosis.
  /// 
  /// These enhanced features are provided by the following additions to the TwinCAT.Ads API:
  /// <list type="bullet">
  /// <item><description><see cref="T:TwinCAT.Ads.AdsConnection" /> class.</description></item>
  /// <item><description>Enhanced diagnosis in form of communication statistics <see cref="P:TwinCAT.Ads.AdsSessionBase.Statistics" /></description></item>
  /// <item><description>(semi-automatic) Resurrectable client communication with <see cref="T:TwinCAT.Ads.AdsConnection" /> objects.</description></item>
  /// <item><description>Symbol caching <see cref="P:TwinCAT.Session.SymbolServer" /></description></item>
  /// <item><description>Fail fast handler for connection stabilization <see cref="T:TwinCAT.Ads.IFailFastHandler" /></description></item>
  /// </list>
  /// 
  /// The <see cref="T:TwinCAT.Ads.AdsConnection" /> is established by calling the <see cref="M:TwinCAT.Session.Connect" /> method. The returned <see cref="T:TwinCAT.Ads.AdsConnection" /> can be used
  /// as long the <see cref="T:TwinCAT.Ads.AdsSessionBase" /> exists.
  /// </remarks>
  /// <example>
  /// The following sample shows a simple use of the <see cref="T:TwinCAT.Ads.AdsSessionBase" /> object. The AdsSession object (and the dynamic SymbolLoader features) are only
  /// available from .NET 4 and upwards.
  /// <code language="C#" title="Use Session (async)" source="..\..\Samples\Sample.Ads.AdsClientCore\SessionAsync.cs" region="CODE_SAMPLE" />
  /// <code language="C#" title="Use Session (sync)" source="..\..\Samples\Sample.Ads.AdsClientCore\Session.cs" region="CODE_SAMPLE" />
  /// </example>
  /// <seealso cref="T:TwinCAT.Session" />
  /// <seealso cref="T:TwinCAT.Ads.IAdsSession" />
  /// <seealso cref="T:TwinCAT.Ads.IInterceptionFactory" />
  public class AdsSession : AdsSessionBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="logger">The logger interface.</param>
    /// <param name="owner">The session owner</param>
    public AdsSession(AmsAddress address, SessionSettings settings, ILogger? logger, object? owner)
      : base(address, settings, (IAdsClientFactory) new ClientFactory(), logger, owner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="owner">The session owner</param>
    public AdsSession(AmsAddress address, SessionSettings settings, object? owner)
      : this(address, settings, (ILogger) null, owner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="netId">The net identifier.</param>
    /// <param name="port">The port.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="logger">The logger interface.</param>
    public AdsSession(AmsNetId netId, int port, SessionSettings settings, ILogger? logger)
      : this(new AmsAddress(netId, port), settings, logger, (object) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="netId">The net identifier.</param>
    /// <param name="port">The port.</param>
    /// <param name="settings">The settings.</param>
    public AdsSession(AmsNetId netId, int port, SessionSettings settings)
      : this(new AmsAddress(netId, port), settings, (ILogger) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="logger">The logger interface.</param>
    public AdsSession(AmsAddress address, SessionSettings settings, ILogger? logger)
      : this(address, settings, logger, (object) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="settings">The settings.</param>
    public AdsSession(AmsAddress address, SessionSettings settings)
      : this(address, settings, (ILogger) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="netId">The net identifier.</param>
    /// <param name="port">The port.</param>
    /// <param name="logger">The logger interface.</param>
    public AdsSession(AmsNetId netId, int port, ILogger? logger)
      : this(netId, port, SessionSettings.Default, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="netId">The net identifier.</param>
    /// <param name="port">The port.</param>
    public AdsSession(AmsNetId netId, int port)
      : this(netId, port, (ILogger) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="logger">The logger interface.</param>
    public AdsSession(AmsAddress address, ILogger? logger)
      : this(address, SessionSettings.Default, logger, (object) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSession" /> class.
    /// </summary>
    /// <param name="address">The address.</param>
    public AdsSession(AmsAddress address)
      : this(address, SessionSettings.Default, (ILogger) null, (object) null)
    {
    }
  }
}
