// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TaskResultExtensions
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Extension class for <see cref="T:TwinCAT.Ads.ResultAds" />
  /// </summary>
  /// <exclude />
  public static class TaskResultExtensions
  {
    /// <summary>
    /// Throws an error, if the <see cref="T:TwinCAT.Ads.ResultAds" /> has failed.
    /// </summary>
    /// <param name="result">The result.</param>
    public static void ThrowOnError(this ResultAds result)
    {
      if (result == null)
        throw new ArgumentNullException(nameof (result));
      result.ErrorCode.ThrowOnError();
    }
  }
}
