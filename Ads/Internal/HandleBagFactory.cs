// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.HandleBagFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Factory class for <see cref="T:TwinCAT.Ads.Internal.IDisposableHandleBag" />.
  /// </summary>
  /// <exclude />
  public static class HandleBagFactory
  {
    /// <summary>Creates the notification handle bag.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="symbols">The symbols.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">The user data.</param>
    /// <returns>IDisposableSymbolHandleBag.</returns>
    public static IDisposableHandleBag<ISymbol> CreateNotificationHandleBag(
      IAdsConnection connection,
      ISymbol[] symbols,
      NotificationSettings settings,
      object[]? userData)
    {
      return (IDisposableHandleBag<ISymbol>) new DisposableNotificationHandleBag(connection, (IList<ISymbol>) symbols, settings, (object) userData);
    }

    /// <summary>Creates the variable handle bag.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="instancePaths">The instance paths.</param>
    /// <returns>IDisposableHandleBag.</returns>
    public static IDisposableHandleBag<string> CreateVariableHandleBag(
      IAdsConnection connection,
      string[] instancePaths)
    {
      return (IDisposableHandleBag<string>) new DisposableHandleBag(connection, (IList<string>) instancePaths);
    }

    /// <summary>Creates the NotificationEx. handle bag.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="symbols">The symbols.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">The user data.</param>
    /// <returns>IDisposableHandleBag.</returns>
    public static IDisposableHandleBag<AnySymbolSpecifier> CreateNotificationExHandleBag(
      IAdsConnection connection,
      IList<AnySymbolSpecifier> symbols,
      NotificationSettings settings,
      object[]? userData)
    {
      return (IDisposableHandleBag<AnySymbolSpecifier>) new DisposableNotificationExHandleBag(connection, symbols, settings, (object) userData);
    }
  }
}
