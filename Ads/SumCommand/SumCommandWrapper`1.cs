// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumCommandWrapper`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// SumCommandBase Adapter object to wrap inner SumCommands.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <exclude />
  public class SumCommandWrapper<T> : ISumCommand where T : class, ISumCommand
  {
    /// <summary>The inner sum command.</summary>
    protected ISumCommand? innerCommand;

    /// <summary>
    /// Gets teh <see cref="T:TwinCAT.Ads.AdsErrorCode" /> of the main SumCommandBase ADS Request
    /// </summary>
    /// <value>The result.</value>
    public AdsErrorCode Result => this.innerCommand != null ? this.innerCommand.Result : (AdsErrorCode) 0;

    /// <summary>Gets the sub results of the single Sub Requests.</summary>
    /// <value>The sub results.</value>
    public AdsErrorCode[] SubResults => this.innerCommand != null ? this.innerCommand.SubResults : Array.Empty<AdsErrorCode>();

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Ads.SumCommand.ISumCommand" /> was already executed.
    /// </summary>
    /// <value><c>true</c> if executed; otherwise, <c>false</c>.</value>
    public bool Executed => this.innerCommand != null && this.innerCommand.Executed;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Ads.SumCommand.ISumCommand" /> is succeeded.
    /// </summary>
    /// <value><c>true</c> if succeeded; otherwise, <c>false</c>.</value>
    public bool Succeeded => this.innerCommand != null && this.innerCommand.Succeeded;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Ads.SumCommand.ISumCommand" /> failed.
    /// </summary>
    /// <value><c>true</c> if failed; otherwise, <c>false</c>.</value>
    public bool Failed => this.innerCommand != null && this.innerCommand.Failed;

    /// <summary>The connection used for communication.</summary>
    public IAdsConnection Connection => this.OnGetConnection();

    /// <summary>Gets the connection object.</summary>
    /// <returns>IAdsConnection.</returns>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    protected virtual IAdsConnection OnGetConnection() => this.innerCommand != null ? this.innerCommand.Connection : throw new ClientNotConnectedException();
  }
}
