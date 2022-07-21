// Decompiled with JetBrains decompiler
// Type: TwinCAT.SessionProvider`3
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT
{
  /// <summary>Abstract base class for a Custom Session provider</summary>
  /// <typeparam name="TSession">SessionType</typeparam>
  /// <typeparam name="TAddress">Address type</typeparam>
  /// <typeparam name="TSettings">Communication settings type</typeparam>
  /// <seealso cref="T:TwinCAT.ISessionProvider`3" />
  /// <Exclude />
  public abstract class SessionProvider<TSession, TAddress, TSettings> : 
    ISessionProvider<TSession, TAddress, TSettings>,
    ISessionProvider
    where TSession : ISession
    where TSettings : class
  {
    /// <summary>
    /// The capabilities of the <see cref="T:TwinCAT.ISessionProvider`3" />
    /// </summary>
    private SessionProviderCapabilities capabilities = (SessionProviderCapabilities) 31;
    /// <summary>Singleton Instance.</summary>
    protected static ISessionProvider<TSession, TAddress, TSettings>? s_self;

    /// <summary>Gets the capabilities.</summary>
    /// <value>The capabilities.</value>
    public SessionProviderCapabilities Capabilities => this.capabilities;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SessionProvider`3" /> class.
    /// </summary>
    /// <exception cref="T:System.Exception">Session provider already instantiated!</exception>
    protected SessionProvider() => SessionProvider<TSession, TAddress, TSettings>.s_self = SessionProvider<TSession, TAddress, TSettings>.s_self == null ? (ISessionProvider<TSession, TAddress, TSettings>) this : throw new Exception("Session provider already instantiated!");

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SessionProvider`3" /> class.
    /// </summary>
    /// <exception cref="T:System.Exception">Session provider already instantiated!</exception>
    protected SessionProvider(SessionProviderCapabilities cap)
    {
      if (SessionProvider<TSession, TAddress, TSettings>.s_self != null)
        throw new Exception("Session provider already instantiated!");
      this.capabilities = cap;
      SessionProvider<TSession, TAddress, TSettings>.s_self = (ISessionProvider<TSession, TAddress, TSettings>) this;
    }

    /// <summary>Gets the name of the SessionProvider</summary>
    /// <value>The name.</value>
    public abstract string Name { get; }

    public virtual ISession Create(object address, ISessionSettings? settings)
    {
      TSettings settings1 = (TSettings) settings;
      return (ISession) (object) this.Create((TAddress) address, settings1);
    }

    /// <summary>
    /// Creates the Session with specified address and communication settings.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="settings">The communicationSettings.</param>
    /// <returns>S.</returns>
    public abstract TSession Create(TAddress address, TSettings? settings);
  }
}
