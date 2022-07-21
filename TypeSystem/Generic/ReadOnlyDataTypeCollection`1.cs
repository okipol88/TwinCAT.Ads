// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.ReadOnlyDataTypeCollection`1
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
  /// <summary>ReadOnly DataType collection</summary>
  /// <typeparam name="T"></typeparam>
  public class ReadOnlyDataTypeCollection<T> : 
    ReadOnlyCollection<T>,
    IDataTypeCollection<T>,
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
    where T : class, IDataType
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.ReadOnlyDataTypeCollection`1" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    public ReadOnlyDataTypeCollection(ReadOnlyDataTypeCollection<T> coll)
      : base(coll.Items)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Generic.ReadOnlyDataTypeCollection`1" /> class.
    /// </summary>
    /// <param name="coll">The collection.</param>
    public ReadOnlyDataTypeCollection(DataTypeCollection<T> coll)
      : base((IList<T>) coll)
    {
    }

    /// <summary>Determines whether the specified name contains type.</summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if the specified name contains type; otherwise, <c>false</c>.</returns>
    public bool ContainsType(string name) => ((DataTypeCollection<T>) this.Items).ContainsType(name);

    /// <summary>
    /// Tries to get the Type with the specified name out of the collection.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <returns>true if found</returns>
    public bool TryGetType(string name, [NotNullWhen(true)] out T? type) => ((DataTypeCollection<T>) this.Items).TryGetType(name, out type);

    /// <summary>Gets the element with the specified type name.</summary>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    public T this[string name] => ((DataTypeCollection<T>) this.Items)[name];
  }
}
