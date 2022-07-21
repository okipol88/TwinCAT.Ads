// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Namespace
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.Diagnostics;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Namespace object.</summary>
  /// <exclude />
  [DebuggerDisplay("{ Name }, { DataTypes.Count } types")]
  public sealed class Namespace : INamespaceInternal<IDataType>, INamespace<IDataType>
  {
    /// <summary>The namespace name</summary>
    private string _name;
    /// <summary>
    /// Data types of the <see cref="T:TwinCAT.TypeSystem.Namespace" />
    /// </summary>
    private DataTypeCollection _dataTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Namespace" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public Namespace(string name)
    {
      this._name = name;
      this._dataTypes = new DataTypeCollection();
    }

    /// <summary>Gets the name of the namespace.</summary>
    /// <value>The name.</value>
    public string Name => this._name;

    /// <summary>
    /// Gets the data types organized by this <see cref="T:TwinCAT.TypeSystem.Namespace" />
    /// </summary>
    /// <value>The data types.</value>
    public IDataTypeCollection<IDataType> DataTypes => (IDataTypeCollection<IDataType>) new ReadOnlyDataTypeCollection((DataTypeCollection<IDataType>) this._dataTypes);

    IDataTypeCollection INamespaceInternal<IDataType>.DataTypesInternal => (IDataTypeCollection) this._dataTypes;

    bool INamespaceInternal<IDataType>.RegisterType(IDataType type)
    {
      if (this._dataTypes.Contains(type))
        return false;
      this._dataTypes.Add(type);
      return true;
    }

    void INamespaceInternal<IDataType>.RegisterTypes(
      IEnumerable<IDataType> types)
    {
      foreach (IDataType type in types)
        ((INamespaceInternal<IDataType>) this).RegisterType(type);
    }
  }
}
