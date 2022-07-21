// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.RpcMethodParaConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Linq;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  internal static class RpcMethodParaConverter
  {
    internal static AdsMethodParaInfo[] ToMethodParameterInfoArray(
      IRpcMethodParameterCollection parameters,
      IStringMarshaler marshaler)
    {
      return ((IEnumerable<IRpcMethodParameter>) parameters).Select<IRpcMethodParameter, AdsMethodParaInfo>((Func<IRpcMethodParameter, AdsMethodParaInfo>) (p => new AdsMethodParaInfo(p, marshaler))).ToArray<AdsMethodParaInfo>();
    }
  }
}
