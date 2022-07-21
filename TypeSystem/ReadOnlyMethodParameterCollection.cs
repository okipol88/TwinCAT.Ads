// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyMethodParameterCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Read only <see cref="T:TwinCAT.TypeSystem.RpcMethodParameterCollection" />.
  /// </summary>
  public class ReadOnlyMethodParameterCollection : 
    ReadOnlyCollection<IRpcMethodParameter>,
    IRpcMethodParameterCollection,
    IList<IRpcMethodParameter>,
    ICollection<IRpcMethodParameter>,
    IEnumerable<IRpcMethodParameter>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyMethodParameterCollection" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    internal ReadOnlyMethodParameterCollection(RpcMethodParameterCollection coll)
      : base((IList<IRpcMethodParameter>) coll)
    {
    }

    /// <summary>Gets the corresponding LengthIs parameter.</summary>
    /// <param name="parameter">The value parameter</param>
    /// <returns>The LengthIs Parameter</returns>
    /// <seealso cref="P:TwinCAT.TypeSystem.IRpcMethodParameter.LengthIsParameterIndex" />
    /// <seealso cref="P:TwinCAT.TypeSystem.IRpcMethodParameter.HasLengthIsParameter" />
    public IRpcMethodParameter? GetLengthIsParameter(
      IRpcMethodParameter parameter)
    {
      return ((RpcMethodParameterCollection) this.Items).GetLengthIsParameter(parameter);
    }
  }
}
