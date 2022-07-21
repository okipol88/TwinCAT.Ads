// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.BitMappingType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Helper Data Type to implement Bit mapping types.</summary>
  public sealed class BitMappingType : DataType
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.BitMappingType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="bitSize">The size of the type in bits.</param>
    /// <param name="dotnetType">Type of the dotnet.</param>
    public BitMappingType(string name, int bitSize, Type dotnetType)
      : base(name, (AdsDataTypeId) 33, (DataTypeCategory) 1, bitSize, dotnetType, (AdsDataTypeFlags) 33)
    {
    }
  }
}
