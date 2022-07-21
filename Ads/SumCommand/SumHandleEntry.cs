// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumHandleEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Class SumHandleEntry.</summary>
  /// <exclude />
  public class SumHandleEntry
  {
    /// <summary>The handle</summary>
    private uint handle;
    /// <summary>The error code</summary>
    private AdsErrorCode errorCode;

    /// <summary>Gets the handle.</summary>
    /// <value>The handle.</value>
    public uint Handle => this.handle;

    /// <summary>Gets the error code.</summary>
    /// <value>The error code.</value>
    public AdsErrorCode ErrorCode => this.errorCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumHandleEntry" /> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="errorCode">The error code.</param>
    internal SumHandleEntry(uint handle, AdsErrorCode errorCode)
    {
      this.handle = handle;
      this.errorCode = errorCode;
    }
  }
}
