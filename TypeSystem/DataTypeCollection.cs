// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DataTypeCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections;
using System.Collections.Generic;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Collection of <see cref="T:TwinCAT.TypeSystem.IDataType">DataTypes.</see>
  /// </summary>
  public class DataTypeCollection : 
    DataTypeCollection<IDataType>,
    IDataTypeCollection,
    IDataTypeCollection<IDataType>,
    IList<IDataType>,
    ICollection<IDataType>,
    IEnumerable<IDataType>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" /> class.
    /// </summary>
    public DataTypeCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" /> class (Copy constructor).
    /// </summary>
    /// <param name="coll">The coll.</param>
    public DataTypeCollection(IEnumerable<IDataType> coll)
      : base(coll)
    {
    }

    /// <summary>
    /// Returns A ReadOnly-Version of the  <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" />.
    /// </summary>
    /// <returns>A read only version of this <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" />.</returns>
    public ReadOnlyDataTypeCollection AsReadOnly() => new ReadOnlyDataTypeCollection((DataTypeCollection<IDataType>) this);

    /// <summary>
    /// Clones this <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" /> (Shallow Copy)
    /// </summary>
    /// <returns>A clone of this <see cref="T:TwinCAT.TypeSystem.DataTypeCollection" />.</returns>
    public DataTypeCollection Clone() => new DataTypeCollection((IEnumerable<IDataType>) this);

    /// <summary>Gets the empty collection.</summary>
    /// <value>The empty.</value>
    public static DataTypeCollection Empty => new DataTypeCollection();
  }
}
