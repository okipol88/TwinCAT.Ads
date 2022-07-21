// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ClientNetIdResearcher
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads.Server;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Class ClientNetIdResearcher.
  /// Implements the <see cref="T:TwinCAT.Ads.LocalNetIdResearcher" />
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.LocalNetIdResearcher" />
  /// <exclude />
  public class ClientNetIdResearcher : LocalNetIdResearcher
  {
    public virtual bool TryGetLocalNetId([NotNullWhen(true)] out AmsNetId? local) => base.TryGetLocalNetId(ref local);

    protected static bool TryGetFromRouter([NotNullWhen(true)] out AmsNetId? local)
    {
      local = (AmsNetId) null;
      AmsNetId localNetId = AmsServerNet.LocalNetId;
      if (!AmsNetId.op_Inequality(localNetId, (AmsNetId) null) || !AmsNetId.op_Inequality(localNetId, AmsNetId.Empty))
        return false;
      local = localNetId;
      return true;
    }
  }
}
