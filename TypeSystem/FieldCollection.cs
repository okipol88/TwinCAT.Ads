// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.FieldCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Collection of <see cref="T:TwinCAT.TypeSystem.IField" /> objects.
  /// </summary>
  public class FieldCollection : 
    InstanceCollection<IField>,
    IFieldCollection,
    IInstanceCollection<IField>,
    IList<IField>,
    ICollection<IField>,
    IEnumerable<IField>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.FieldCollection" /> class.
    /// </summary>
    public FieldCollection()
      : base((InstanceCollectionMode) 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.FieldCollection" /> class (copy constructor)
    /// </summary>
    /// <param name="coll">The coll.</param>
    public FieldCollection(IEnumerable<IField> coll)
      : base(coll, (InstanceCollectionMode) 0)
    {
    }

    /// <summary>
    /// Returns a read only copy of this collection (shallow copy)
    /// </summary>
    /// <returns>The readonly copy.</returns>
    public ReadOnlyFieldCollection AsReadOnly() => new ReadOnlyFieldCollection(this);

    /// <summary>
    /// Clones this <see cref="T:TwinCAT.TypeSystem.FieldCollection" />.
    /// </summary>
    /// <returns>A cloned <see cref="T:TwinCAT.TypeSystem.FieldCollection" />.</returns>
    public FieldCollection Clone() => new FieldCollection((IEnumerable<IField>) this);

    public bool TryGetMember(string fieldName, [NotNullWhen(true)] out IField? symbol) => this.TryGetInstance(fieldName, out symbol);
  }
}
