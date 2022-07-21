// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AdsBinder
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class AdsTypeBinder. This class cannot be inherited.</summary>
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.IAdsBinder" />
  /// <exclude />
  internal class AdsBinder : Binder, IAdsBinder, IBinder, IDataTypeResolver
  {
    private AmsAddress _imageBaseAddress;

    internal AdsBinder(
      AmsAddress imageBaseAddress,
      IInternalSymbolProvider provider,
      ISymbolFactory symbolFactory,
      bool useVirtualInstance)
      : base(provider, symbolFactory, useVirtualInstance)
    {
      this._imageBaseAddress = !AmsAddress.op_Equality(imageBaseAddress, (AmsAddress) null) ? imageBaseAddress : throw new ArgumentNullException(nameof (imageBaseAddress));
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.Ads.AmsAddress" /> of the Process Image
    /// </summary>
    /// <value>The address.</value>
    public AmsAddress ImageBaseAddress => this._imageBaseAddress;
  }
}
