// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyNamespaceCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>ReadOnly namespace collection</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ReadOnlyNamespaceCollection : ReadOnlyNamespaceCollection<IDataType>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyNamespaceCollection" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    public ReadOnlyNamespaceCollection(NamespaceCollection coll)
      : base((NamespaceCollection<IDataType>) coll)
    {
    }
  }
}
