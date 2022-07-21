// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ResMan
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Resource Manager (static)</summary>
  internal class ResMan
  {
    private static ResourceManager? _rm;

    /// <summary>
    /// Gets the error string of the specified <see cref="T:TwinCAT.Ads.AdsErrorCode" />.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="adsErrorCode">The ads error code.</param>
    /// <returns>System.String.</returns>
    internal static string GetString(string? message, AdsErrorCode adsErrorCode)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
      interpolatedStringHandler.AppendLiteral("AdsError_");
      interpolatedStringHandler.AppendFormatted<uint>((uint) adsErrorCode);
      string stringAndClear = ResMan.GetString(interpolatedStringHandler.ToStringAndClear());
      if (stringAndClear == null)
      {
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
        interpolatedStringHandler.AppendLiteral("'");
        interpolatedStringHandler.AppendFormatted<AdsErrorCode>(adsErrorCode);
        interpolatedStringHandler.AppendLiteral("' has occurred");
        stringAndClear = interpolatedStringHandler.ToStringAndClear();
      }
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} (AdsErrorCode: {1}, 0x{1:X})", (object) stringAndClear, (object) (uint) adsErrorCode, (object) stringAndClear);
      string empty = string.Empty;
      return !string.IsNullOrEmpty(message) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) message, (object) str) : str;
    }

    internal static string GetString(AdsErrorCode adsErrorCode) => ResMan.GetString((string) null, adsErrorCode);

    /// <summary>Gets the (internal) resource manager.</summary>
    /// <value>The resource manager.</value>
    private static ResourceManager ResourceManager
    {
      get
      {
        if (ResMan._rm == null)
          ResMan._rm = new ResourceManager("TwinCAT.Ads.Resource", Assembly.GetExecutingAssembly());
        return ResMan._rm;
      }
    }

    /// <summary>Gets the string.</summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    internal static string GetString(string name) => ResMan.ResourceManager.GetString(name, CultureInfo.CurrentUICulture);
  }
}
