// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.SymbolLoaderFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.Ads.ValueAccess;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// The class <see cref="T:TwinCAT.Ads.TypeSystem.SymbolLoaderFactory" /> is used to create a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolLoader" /> initialized to the parametrized mode (SymbolBrowser V2, new Version)
  /// </summary>
  /// <remarks>The Symbol Loader (V2) supports the following <see cref="T:TwinCAT.SymbolsLoadMode">modes</see>.
  /// <list type="Table"><listheader><term></term><description></description></listheader><item><term><see cref="F:TwinCAT.SymbolsLoadMode.Flat" /></term><description>The flat mode organizes the Symbols in a flat list. This mode is available in all .NET versions.
  /// </description></item><item><term><see cref="F:TwinCAT.SymbolsLoadMode.VirtualTree" /></term><description>The virtual tree mode organizes the Symbols hierarchically with parent-child relationships. This mode is available in all .NET Versions.
  /// </description></item><item><term><see cref="F:TwinCAT.SymbolsLoadMode.DynamicTree" /></term><description>The Dynamic tree mode organizes the Symbols hierarchically and (dynamically) creates struct members,
  /// array elements and enum fields on the fly. This feature is only available on platforms that support the Dynamic
  /// Language Runtime (DLR), actually all .NET Framework Version larger than 4.0.
  /// </description></item></list>
  /// Virtual instances means, that all Symbols are ordered within a tree structure. For that symbol nodes that are not located on a fixed address, a Virtual Symbol will be created.
  /// Setting the virtualInstance parameter to 'false' means, that the located symbols will be returned in a flattened list.</remarks>
  /// <seealso cref="T:TwinCAT.SymbolLoaderSettings" />
  public static class SymbolLoaderFactory
  {
    /// <summary>Creates the specified connection.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="settings">The settings.</param>
    /// <returns>ISymbolLoader.</returns>
    /// <example>
    /// The following sample shows how to create a dynamic version of the SymbolLoader V2. The dynamic symbol loader makes use of the Dynamic Language Runtime (DLR) of the .NET Framework.
    /// That means Structures, Arrays and Enumeration types and instances are generated 'on-the-fly' during symbol Browsing. These created dynamic objects are a one to one representation
    /// of the Symbol Server target objects (e.g the IEC61131 types on the PLC).
    /// Dynamic language features are only available from .NET4 upwards.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE" />
    /// The following sample shows how to create a static (non dynamic) version of the SymbolLoader V2.
    /// The static symbol loader in version 2 is a nearly code compatible version of the Dynamic Loader, only the dynamic creation of objects is not available. The reason for supporting
    /// this mode is that .NET Framework Versions lower than Version 4.0 (CLR2) doesn't support the Dynamic Language Runtime (DLR).
    /// The SymbolLoader V2 static object is supported from .NET 2.0 on.
    /// <code language="C#" title="Virtual Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE" /></example>
    /// <example>
    /// The SymbolLoader V2 static object is supported from .NET 2.0 on.
    /// <code language="C#" title="Flat Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2Flat.cs" region="CODE_SAMPLE" /></example>
    /// <example>
    ///   <code language="C#" title="Argument Parser" source="..\..\Samples\Sample.Ads.AdsClientCore\ArgParser.cs" region="CODE_SAMPLE" />
    ///   <code language="C#" title="Dumping Symbols" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolDump.cs" region="CODE_SAMPLE" />
    /// </example>
    public static ISymbolLoader Create(
      IConnection connection,
      ISymbolLoaderSettings settings)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      IAccessorValue accessor = settings != null ? SymbolLoaderFactory.createValueAccessor((IAdsConnection) connection, (SymbolLoaderSettings) settings) : throw new ArgumentNullException(nameof (settings));
      return (ISymbolLoader) new AdsSymbolLoader((IAdsConnection) connection, (SymbolLoaderSettings) settings, (IAccessorRawValue) accessor, connection.Session);
    }

    /// <summary>Gets the value accessor.</summary>
    /// <returns>IRawValueAccessor.</returns>
    internal static IAccessorValue createValueAccessor(
      IAdsConnection connection,
      SymbolLoaderSettings settings)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (settings == null)
        throw new ArgumentNullException(nameof (settings));
      IAccessorValueFactory iaccessorValueFactory;
      AdsValueAccessor inner;
      IAccessorValue valueAccessor;
      if (settings.SymbolsLoadMode == 2)
      {
        iaccessorValueFactory = (IAccessorValueFactory) new DynamicValueFactory(settings);
        inner = new AdsValueAccessor(connection, settings.ValueAccessMode, iaccessorValueFactory, NotificationSettings.Default);
        valueAccessor = (IAccessorValue) new DynamicValueAccessor((IAccessorValue) inner, iaccessorValueFactory, settings.ValueCreation);
      }
      else
      {
        iaccessorValueFactory = (IAccessorValueFactory) new ValueFactory(settings.ValueCreation);
        inner = new AdsValueAccessor(connection, settings.ValueAccessMode, iaccessorValueFactory, NotificationSettings.Default);
        valueAccessor = (IAccessorValue) inner;
      }
      if (iaccessorValueFactory is IAccessorValueFactory2 iaccessorValueFactory2)
        iaccessorValueFactory2.SetValueAccessor((IAccessorRawValue) valueAccessor);
      inner.AutomaticReconnection = settings.AutomaticReconnection;
      return valueAccessor;
    }

    /// <summary>Reads the symbol upload information.</summary>
    internal static AdsErrorCode TryReadSymbolUploadInfo(
      IAdsConnection connection,
      out SymbolUploadInfo? symbolInfo)
    {
      AdsErrorCode adsErrorCode = connection != null ? AdsSymbolProvider.loadUploadInfoSync(connection, out symbolInfo) : throw new ArgumentNullException(nameof (connection));
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
      {
        if (!(connection is IAdsSymbolTableProvider symbolTableProvider))
          symbolTableProvider = ((AdsConnection) connection).Client as IAdsSymbolTableProvider;
        symbolTableProvider?.SetSymbolEncoding(symbolInfo.StringEncoding);
      }
      return adsErrorCode;
    }

    /// <summary>Reads the symbol upload information.</summary>
    internal static async Task<ResultValue<SymbolUploadInfo>> readSymbolUploadInfoAsync(
      IAdsConnection connection,
      CancellationToken cancel)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      ResultValue<SymbolUploadInfo> resultValue = await AdsSymbolProvider.loadUploadInfoAsync(connection, cancel).ConfigureAwait(false);
      if (((ResultAds) resultValue).Succeeded)
      {
        if (!(connection is IAdsSymbolTableProvider symbolTableProvider))
          symbolTableProvider = ((AdsConnection) connection).Client as IAdsSymbolTableProvider;
        symbolTableProvider?.SetSymbolEncoding(resultValue.Value.StringEncoding);
      }
      return resultValue;
    }
  }
}
