// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.IAdsBinder
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Interface IAdsTypeBinder</summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.IBinder" />
  /// <exclude />
  internal interface IAdsBinder : IBinder, IDataTypeResolver
  {
    /// <summary>
    /// Gets the <see cref="T:TwinCAT.Ads.AmsAddress" /> of the Process Image
    /// </summary>
    /// <value>The address.</value>
    AmsAddress ImageBaseAddress { get; }
  }
}
