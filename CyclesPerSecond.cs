// Decompiled with JetBrains decompiler
// Type: TwinCAT.CyclesPerSecond
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

namespace TwinCAT
{
  /// <summary>Class StatisticsPerSecond.</summary>
  /// <exclude />
  public struct CyclesPerSecond
  {
    /// <summary>The requests per second</summary>
    /// <remarks>These are the cycles (per second) of the communication whether they are sucessfull or not.</remarks>
    private readonly double RequestsPerSecond;
    /// <summary>
    /// The succeeded cycles of the ADS communication per second.
    /// </summary>
    private readonly double SucceedsPerSecond;
    /// <summary>
    /// The failed cycles of the ADS communication per second.
    /// </summary>
    private readonly double ErrorsPerSecond;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.CyclesPerSecond" /> struct.
    /// </summary>
    /// <param name="requestsPerSecond">The requests per second.</param>
    /// <param name="succeedsPerSecond">The succeeds per second.</param>
    /// <param name="errorsPerSecond">The errors per second.</param>
    internal CyclesPerSecond(
      double requestsPerSecond,
      double succeedsPerSecond,
      double errorsPerSecond)
    {
      this.RequestsPerSecond = requestsPerSecond;
      this.SucceedsPerSecond = succeedsPerSecond;
      this.ErrorsPerSecond = errorsPerSecond;
    }
  }
}
