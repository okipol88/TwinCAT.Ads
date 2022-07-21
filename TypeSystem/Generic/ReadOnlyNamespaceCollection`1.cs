// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.ReadOnlyNamespaceCollection`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem.Generic
{
  /// <summary>Read Only namespace collection</summary>
  /// <typeparam name="T"></typeparam>
  public class ReadOnlyNamespaceCollection<T> : 
    ReadOnlyCollection<INamespace<T>>,
    INamespaceCollection<T>,
    ICollection<INamespace<T>>,
    IEnumerable<INamespace<T>>,
    IEnumerable,
    INamespaceContainer<T>
    where T : class, IDataType
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.ReadOnlyNamespaceCollection`1" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    public ReadOnlyNamespaceCollection(NamespaceCollection<T> coll)
      : base((IList<INamespace<T>>) coll)
    {
    }

    /// <summary>
    /// Determines whether this collection contains a namespace with the specified name.
    /// </summary>
    /// <param name="name">The name of the namespace</param>
    /// <returns>
    ///   <c>true</c> if the namespace is contained; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsNamespace(string name) => ((NamespaceCollection<T>) this.Items).ContainsNamespace(name);

    public bool TryGetNamespace(string name, [NotNullWhen(true)] out INamespace<T>? nspace) => ((NamespaceCollection<T>) this.Items).TryGetNamespace(name, out nspace);

    /// <summary>Gets the element at the specified index.</summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public INamespace<T> this[string name] => ((NamespaceCollection<T>) this.Items)[name];

    /// <summary>Tries to get the specified data type.</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="dataType">Data Type (out-parameter).</param>
    /// <returns>true if found, false if not contained.</returns>
    public bool TryGetType(string typeName, [NotNullWhen(true)] out T? dataType) => ((NamespaceCollection<T>) this.Items).TryGetType(typeName, out dataType);

    /// <summary>Tries to get the specified type (by fullName)</summary>
    /// <param name="fullname">FullName of the data type.</param>
    /// <param name="dataType">Found Data type (out-parameter).</param>
    /// <returns>true if found, false if not contained.</returns>
    public bool TryGetTypeByFullName(string fullname, [NotNullWhen(true)] out T? dataType) => ((NamespaceCollection<T>) this.Items).TryGetTypeByFullName(fullname, out dataType);

    /// <summary>Gets all types included in all namespaces.</summary>
    /// <value>All types.</value>
    public IDataTypeCollection<T> AllTypes => ((NamespaceCollection<T>) this.Items).AllTypes;
  }
}
