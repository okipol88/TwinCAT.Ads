// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsOnDemandBinder
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Read Type Binder.</summary>
  /// <remarks>
  /// This is a binder implementation that reads the Data types on demand from the target.
  /// </remarks>
  /// <exclude />
  internal class AdsOnDemandBinder : AdsBinder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsOnDemandBinder" /> class.
    /// </summary>
    /// <param name="imageBaseAddress">The image base address.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="symbolFactory">The symbol factory.</param>
    /// <param name="useVirtualInstance">if set to <c>true</c> [use virtual instance].</param>
    internal AdsOnDemandBinder(
      AmsAddress imageBaseAddress,
      IInternalSymbolProvider provider,
      ISymbolFactory symbolFactory,
      bool useVirtualInstance)
      : base(imageBaseAddress, provider, symbolFactory, useVirtualInstance)
    {
    }

    public override bool TryResolveType(string name, [NotNullWhen(true)] out IDataType? type)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (!AdsErrorCodeExtensions.Succeeded(((ISymbolInfoTable) this.Provider).TryReadType(name, true, ref type)))
        return false;
      DataTypeCategory category = type.Category;
      return true;
    }

    /// <summary>Resolves the type asynchronous.</summary>
    /// <param name="name">The name.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultValue&lt;IDataType&gt;&gt;.</returns>
    public override async Task<ResultValue<IDataType>> ResolveTypeAsync(
      string name,
      CancellationToken cancel)
    {
      // ISSUE: reference to a compiler-generated method
      ResultValue<IDataType> resultValue = await ((ISymbolInfoTable) this.\u003C\u003En__0()).ReadTypeAsync(name, true, cancel).ConfigureAwait(false);
      if (((ResultAds) resultValue).Succeeded)
      {
        DataTypeCategory category = resultValue.Value.Category;
      }
      return resultValue;
    }
  }
}
