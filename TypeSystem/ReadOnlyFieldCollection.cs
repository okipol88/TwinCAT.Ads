// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyFieldCollection
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
  /// Read only collection of <see cref="T:TwinCAT.TypeSystem.IField" /> objects
  /// </summary>
  public class ReadOnlyFieldCollection : 
    ReadOnlyInstanceCollection<IField>,
    IFieldCollection,
    IInstanceCollection<IField>,
    IList<IField>,
    ICollection<IField>,
    IEnumerable<IField>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyMemberCollection" /> class.
    /// </summary>
    /// <param name="members">The members.</param>
    public ReadOnlyFieldCollection(FieldCollection members)
      : base((IInstanceCollection<IField>) members)
    {
    }

    public bool TryGetMember(string fieldName, [NotNullWhen(true)] out IField? symbol) => ((FieldCollection) this.Items).TryGetMember(fieldName, out symbol);
  }
}
