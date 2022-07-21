// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyRpcMethodCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Read only <see cref="T:TwinCAT.TypeSystem.RpcMethodCollection" />
  /// </summary>
  public class ReadOnlyRpcMethodCollection : 
    ReadOnlyCollection<IRpcMethod>,
    IRpcMethodCollection,
    IList<IRpcMethod>,
    ICollection<IRpcMethod>,
    IEnumerable<IRpcMethod>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyRpcMethodCollection" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    internal ReadOnlyRpcMethodCollection(RpcMethodCollection coll)
      : base((IList<IRpcMethod>) coll)
    {
    }

    /// <summary>Gets the empty collection..</summary>
    /// <value>The empty collection.</value>
    internal static ReadOnlyRpcMethodCollection Empty => RpcMethodCollection.Empty.AsReadOnly();

    /// <summary>
    /// Determines whether this collection contains the specified method name.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <returns><c>true</c> if contained.; otherwise, <c>false</c>.</returns>
    public bool Contains(string methodName) => ((RpcMethodCollection) this.Items).Contains(methodName);

    public bool TryGetMethod(string methodName, [NotNullWhen(true)] out IRpcMethod? method) => ((RpcMethodCollection) this.Items).TryGetMethod(methodName, out method);

    public bool TryGetMethod(int vTableIndex, [NotNullWhen(true)] out IRpcMethod? method) => ((RpcMethodCollection) this.Items).TryGetMethod(vTableIndex, out method);

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IRpcMethod" /> with the specified method name.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>RpcMethod.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException"></exception>
    public IRpcMethod this[string methodName] => ((RpcMethodCollection) this.Items)[methodName];
  }
}
