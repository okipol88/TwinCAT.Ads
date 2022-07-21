// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.NamespaceCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Collection of Namespaces (internally using <see cref="T:TwinCAT.TypeSystem.Generic.INamespace`1" />
  /// </summary>
  /// <exclude />
  public class NamespaceCollection : 
    NamespaceCollection<IDataType>,
    INamespaceCollection,
    INamespaceCollection<IDataType>,
    ICollection<INamespace<IDataType>>,
    IEnumerable<INamespace<IDataType>>,
    IEnumerable,
    INamespaceContainer<IDataType>
  {
    /// <summary>
    /// Creates a read-only version of this <see cref="T:TwinCAT.TypeSystem.NamespaceCollection" />.
    /// </summary>
    /// <returns>A readonly <see cref="T:TwinCAT.TypeSystem.NamespaceCollection" />.</returns>
    public ReadOnlyNamespaceCollection AsReadOnly() => new ReadOnlyNamespaceCollection(this);

    /// <summary>Registers a type on its namespace</summary>
    /// <param name="type">The type.</param>
    /// <remarks>Creates a new namespace if not existing in the collection.</remarks>
    public void RegisterType(IDataType type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      INamespace<IDataType> nspace = (INamespace<IDataType>) null;
      INamespaceInternal<IDataType> inamespaceInternal;
      if (!this.TryGetNamespace(type.Namespace, out nspace))
      {
        inamespaceInternal = (INamespaceInternal<IDataType>) new Namespace(type.Namespace);
        this.Add((INamespace<IDataType>) inamespaceInternal);
      }
      else
        inamespaceInternal = (INamespaceInternal<IDataType>) nspace;
      if (!inamespaceInternal.RegisterType(type))
        return;
      this.InnerAllTypes.Add(type.FullName, type);
    }
  }
}
