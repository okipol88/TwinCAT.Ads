// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsSessionProvider
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>ADS Session Provider class.</summary>
  /// <seealso cref="T:TwinCAT.SessionProvider`3" />
  /// <Exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Export(typeof (ISessionProvider))]
  [ExportMetadata("SessionProvider", "Ads")]
  public class AdsSessionProvider : SessionProvider<AdsSessionBase, AmsAddress, SessionSettings>
  {
    /// <summary>Gets the name of the SessionProvider</summary>
    /// <value>The name.</value>
    public override string Name => "ADS";

    public override ISession Create(object? address, ISessionSettings? settings)
    {
      AmsAddress address1 = (AmsAddress) null;
      SessionSettings settings1 = (SessionSettings) null;
      if (address is AmsAddress)
        address1 = (AmsAddress) address;
      else if (address is string)
        address1 = AmsAddress.Parse((string) address);
      if (AmsAddress.op_Equality(address1, (AmsAddress) null))
        throw new ArgumentOutOfRangeException(nameof (address));
      if (settings != null)
        settings1 = (SessionSettings) settings;
      return (ISession) this.Create(address1, settings1);
    }

    /// <summary>Creates the specified address.</summary>
    /// <param name="address">The address.</param>
    /// <param name="settings">The settings.</param>
    /// <returns>S.</returns>
    public override AdsSessionBase Create(
      AmsAddress address,
      SessionSettings? settings)
    {
      if (settings == null)
        settings = SessionSettings.Default;
      return (AdsSessionBase) new AdsSession(address, settings);
    }

    /// <summary>Gets the Singleton instance</summary>
    /// <value>The self.</value>
    /// <exclude />
    public static AdsSessionProvider Self
    {
      get
      {
        if (SessionProvider<AdsSessionBase, AmsAddress, SessionSettings>.s_self == null)
          SessionProvider<AdsSessionBase, AmsAddress, SessionSettings>.s_self = (ISessionProvider<AdsSessionBase, AmsAddress, SessionSettings>) new AdsSessionProvider();
        return (AdsSessionProvider) SessionProvider<AdsSessionBase, AmsAddress, SessionSettings>.s_self;
      }
    }
  }
}
