// Decompiled with JetBrains decompiler
// Type: TwinCAT.StatisticExtensions
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using TwinCAT.Ads;


#nullable enable
namespace TwinCAT
{
  /// <summary>Class StatisticExtensions.</summary>
  /// <remarks>This class extends the <see cref="T:TwinCAT.Ads.AdsConnection" /> class by Cycles/Errors per second.</remarks>
  /// <exclude />
  public static class StatisticExtensions
  {
    /// <summary>Polls the statistics.</summary>
    /// <param name="connection">The ADS connection.</param>
    /// <param name="pollingPeriod">The polling period (timespan between calculating each distinct value)</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>IObservable&lt;StatisticsPerSecond&gt;.</returns>
    public static IObservable<TwinCAT.CyclesPerSecond> PollCyclesPerSecond(
      this AdsConnection connection,
      TimeSpan pollingPeriod,
      TimeSpan timeout)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (connection.ConnectionObserver == null)
        throw new ArgumentException("Connection doesn't have an ConnectionObserver!");
      return Observable.Timer(DateTimeOffset.UtcNow, pollingPeriod, (IScheduler) Scheduler.CurrentThread).Select<long, ConnectionStatisticsInfo>((Func<long, ConnectionStatisticsInfo>) (i => connection.ConnectionObserver.GetConnectionStatistics())).Buffer<ConnectionStatisticsInfo>(2, 1).Select<IList<ConnectionStatisticsInfo>, TwinCAT.CyclesPerSecond>((Func<IList<ConnectionStatisticsInfo>, TwinCAT.CyclesPerSecond>) (b => StatisticExtensions.CyclesPerSecond(b, pollingPeriod)));
    }

    /// <summary>
    /// Calc the Values/Second in the specified period of time.
    /// </summary>
    /// <param name="valueBuffer">The value buffer.</param>
    /// <param name="period">The period.</param>
    /// <returns>System.Double.</returns>
    internal static double ValuesPerSecond(IList<int> valueBuffer, TimeSpan period)
    {
      int num = valueBuffer[0];
      return (double) valueBuffer[valueBuffer.Count - 1] - (double) num / period.TotalSeconds;
    }

    /// <summary>
    /// Calc the Values/Second in the specified period of time.
    /// </summary>
    /// <param name="valueBuffer">The value buffer.</param>
    /// <param name="period">The period.</param>
    /// <returns>System.ValueTuple&lt;System.Double, System.Double&gt;.</returns>
    internal static (double, double) ValuesPerSecond(IList<(int, int)> valueBuffer, TimeSpan period)
    {
      (int, int) tuple1 = valueBuffer[0];
      (int, int) tuple2 = valueBuffer[valueBuffer.Count - 1];
      return ((double) (tuple2.Item1 - tuple1.Item1) / period.TotalSeconds, (double) (tuple2.Item2 - tuple1.Item2) / period.TotalSeconds);
    }

    /// <summary>
    /// Calcs the Values/Second in the specified period of time.
    /// </summary>
    /// <param name="valueBuffer">The value buffer.</param>
    /// <param name="period">The period.</param>
    /// <returns>System.ValueTuple&lt;System.Double, System.Double, System.Double&gt;.</returns>
    internal static (double, double, double) ValuesPerSecond(
      IList<(int, int, int)> valueBuffer,
      TimeSpan period)
    {
      (int, int, int) tuple1 = valueBuffer[0];
      (int, int, int) tuple2 = valueBuffer[valueBuffer.Count - 1];
      return ((double) (tuple2.Item1 - tuple1.Item1) / period.TotalSeconds, (double) (tuple2.Item2 - tuple1.Item2) / period.TotalSeconds, (double) (tuple2.Item3 - tuple1.Item3) / period.TotalSeconds);
    }

    /// <summary>
    /// Calc the Values/Second in the specified period of time.
    /// </summary>
    /// <param name="valueBuffer">The value buffer.</param>
    /// <param name="period">The period.</param>
    /// <returns>System.ValueTuple&lt;System.Double, System.Double, System.Double&gt;.</returns>
    internal static (double, double, double) ValuesPerSecond(
      IList<ConnectionStatisticsInfo> valueBuffer,
      TimeSpan period)
    {
      ConnectionStatisticsInfo connectionStatisticsInfo1 = valueBuffer[0];
      ConnectionStatisticsInfo connectionStatisticsInfo2 = valueBuffer[((ICollection<ConnectionStatisticsInfo>) valueBuffer).Count - 1];
      return ((double) (connectionStatisticsInfo2.TotalCycles - connectionStatisticsInfo1.TotalCycles) / period.TotalSeconds, (double) (connectionStatisticsInfo2.TotalSucceeded - connectionStatisticsInfo1.TotalSucceeded) / period.TotalSeconds, (double) (connectionStatisticsInfo2.TotalCommunicationErrors - connectionStatisticsInfo1.TotalCommunicationErrors) / period.TotalSeconds);
    }

    /// <summary>
    /// Calc the Cycles/Second in the specified period of time
    /// </summary>
    /// <param name="statBuffers">The stat buffers.</param>
    /// <param name="period">The period.</param>
    /// <returns>CyclesPerSecond.</returns>
    internal static TwinCAT.CyclesPerSecond CyclesPerSecond(
      IList<(int, int, int)> statBuffers,
      TimeSpan period)
    {
      (double, double, double) tuple = StatisticExtensions.ValuesPerSecond(statBuffers, period);
      return new TwinCAT.CyclesPerSecond(tuple.Item1, tuple.Item2, tuple.Item3);
    }

    /// <summary>
    /// Calc the Cycles/Second in the specified period of time
    /// </summary>
    /// <param name="statBuffers">The stat buffers.</param>
    /// <param name="period">The period.</param>
    /// <returns>CyclesPerSecond.</returns>
    internal static TwinCAT.CyclesPerSecond CyclesPerSecond(
      IList<ConnectionStatisticsInfo> statBuffers,
      TimeSpan period)
    {
      (double, double, double) tuple = StatisticExtensions.ValuesPerSecond(statBuffers, period);
      return new TwinCAT.CyclesPerSecond(tuple.Item1, tuple.Item2, tuple.Item3);
    }
  }
}
