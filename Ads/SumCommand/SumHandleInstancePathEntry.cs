// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumHandleInstancePathEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Class SumHandleEntry.</summary>
  /// <exclude />
  public class SumHandleInstancePathEntry : SumHandleEntry
  {
    private string _instancePath;

    /// <summary>Gets the instance path.</summary>
    /// <value>The instance path.</value>
    public string InstancePath => this._instancePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumHandleEntry" /> class.
    /// </summary>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="handle">The handle.</param>
    /// <param name="errorCode">The error code.</param>
    internal SumHandleInstancePathEntry(string instancePath, uint handle, AdsErrorCode errorCode)
      : base(handle, errorCode)
    {
      this._instancePath = instancePath;
    }
  }
}
