// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyDataTypeCollection
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
  /// ReadOnly Collection of <see cref="T:TwinCAT.TypeSystem.IDataType" /> objects.
  /// </summary>
  public class ReadOnlyDataTypeCollection : 
    ReadOnlyDataTypeCollection<IDataType>,
    IDataTypeCollection,
    IDataTypeCollection<IDataType>,
    IList<IDataType>,
    ICollection<IDataType>,
    IEnumerable<IDataType>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyDataTypeCollection" /> class.
    /// </summary>
    /// <param name="coll">Collection of types.</param>
    public ReadOnlyDataTypeCollection(DataTypeCollection<IDataType> coll)
      : base(coll)
    {
    }

    /// <summary>Gets the empty collection.</summary>
    /// <value>The empty.</value>
    public static ReadOnlyDataTypeCollection Empty => DataTypeCollection.Empty.AsReadOnly();
  }
}
