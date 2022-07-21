// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumInstancePathCommand`2
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// SumCommand that uses the instance path as symbol identifier (Abstract).
  /// </summary>
  /// <remarks></remarks>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class SumInstancePathCommand<T, I> : SumCommandWrapper<T> where T : class, ISumCommand
  {
    /// <summary>The connection</summary>
    private IAdsConnection _connection;
    /// <summary>The symbols</summary>
    protected IList<(I instancePath, Type type, int[]? args)> instancePaths;

    /// <summary>Gets the connection object.</summary>
    /// <returns>IAdsConnection.</returns>
    protected override IAdsConnection OnGetConnection() => this._connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolCommand`1" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="typeSpecs">The type specs.</param>
    public SumInstancePathCommand(
      IAdsConnection connection,
      IList<(I instancePath, Type type, int[]? args)> typeSpecs)
    {
      this._connection = connection;
      this.instancePaths = (IList<(I, Type, int[])>) typeSpecs.Select<(I, Type, int[]), (I, Type, int[])>((Func<(I, Type, int[]), (I, Type, int[])>) (e => (e.instancePath, e.type, e.args))).ToList<(I, Type, int[])>();
    }

    /// <summary>The symbols</summary>
    protected IList<I> InstancePaths => (IList<I>) this.instancePaths.Select<(I, Type, int[]), I>((Func<(I, Type, int[]), I>) (x => x.instancePath)).ToList<I>().AsReadOnly();

    /// <summary>Creates the information list.</summary>
    /// <returns>IList&lt;SumSymbolInfo&gt;.</returns>
    protected abstract IList<SumDataEntity> CreateSumEntityInfos();
  }
}
