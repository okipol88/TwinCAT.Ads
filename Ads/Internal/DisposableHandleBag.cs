// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.DisposableHandleBag
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.Threading;
using TwinCAT.Ads.SumCommand;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class DisposableHandleBag.</summary>
  /// <remarks>
  /// This helper class is used to support multiple registration of Variable handles in one step by SumCommand. With Disposing, all the registered handles will be
  /// be released again.
  /// </remarks>
  /// <exclude />
  public class DisposableHandleBag : DisposableHandleBag<string>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.DisposableHandleBag" /> class.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="symbolPaths">The symbol paths.</param>
    /// <exception cref="T:System.ArgumentNullException">symbolPaths</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">symbolPaths</exception>
    public DisposableHandleBag(IAdsConnection client, IList<string> symbolPaths)
      : base(client, symbolPaths)
    {
    }

    /// <summary>Creates the handles.</summary>
    /// <returns>System.Int32.</returns>
    public override int CreateHandles()
    {
      object sync = this.sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        base.CreateHandles();
        int num = 0;
        int handles = 0;
        if (new SumCreateHandles(this.connection, this.sourceObjects).TryCreateHandles(out this.sumCommandHandles) == null)
        {
          foreach (SumHandleInstancePathEntry sumCommandHandle in (IEnumerable<SumHandleEntry>) this.sumCommandHandles)
          {
            this.sourceHandlesDict.Add(sumCommandHandle.InstancePath, sumCommandHandle.Handle);
            if (sumCommandHandle.ErrorCode == null)
              ++handles;
            else
              ++num;
          }
        }
        this.handlesInvalidated = false;
        return handles;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>Releases the handles.</summary>
    public override void ReleaseHandles()
    {
      object sync = this.sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        new SumReleaseHandles(this.connection, this.sumCommandHandles.ValidHandles).TryReleaseHandles(out AdsErrorCode[] _);
        this.sumCommandHandles = (ISumHandleCollection) null;
        base.ReleaseHandles();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.Internal.DisposableHandleBag" /> class.
    /// </summary>
    ~DisposableHandleBag() => this.Dispose(false);
  }
}
