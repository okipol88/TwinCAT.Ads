// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyTypeAttributeCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Read only version of the <see cref="T:TwinCAT.TypeSystem.TypeAttributeCollection" />
  /// </summary>
  public class ReadOnlyTypeAttributeCollection : 
    ReadOnlyCollection<ITypeAttribute>,
    ITypeAttributeCollection,
    IList<ITypeAttribute>,
    ICollection<ITypeAttribute>,
    IEnumerable<ITypeAttribute>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyTypeAttributeCollection" /> class (for internal use only)
    /// </summary>
    /// <param name="coll">The coll.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReadOnlyTypeAttributeCollection(TypeAttributeCollection coll)
      : base((IList<ITypeAttribute>) coll)
    {
    }

    /// <summary>
    /// Determines whether this <see cref="T:TwinCAT.TypeSystem.ReadOnlyTypeAttributeCollection" /> contains the specified attribute.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.</returns>
    public bool Contains(string name) => ((TypeAttributeCollection) this.Items).Contains(name);

    public bool TryGetAttribute(string name, [NotNullWhen(true)] out ITypeAttribute? attribute) => ((TypeAttributeCollection) this.Items).TryGetAttribute(name, out attribute);

    /// <summary>Tries to get the specified Attribute value.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetValue(string name, [NotNullWhen(true)] out string? value) => ((TypeAttributeCollection) this.Items).TryGetValue(name, out value);

    /// <summary>
    /// Gets the <see cref="T:System.String" /> with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public string this[string name] => ((TypeAttributeCollection) this.Items)[name];

    /// <summary>
    /// Returns an empty <see cref="T:TwinCAT.TypeSystem.ReadOnlyTypeAttributeCollection" />
    /// </summary>
    /// <returns>ReadOnlyTypeAttributeCollection.</returns>
    public static ReadOnlyTypeAttributeCollection Empty => TypeAttributeCollection.Empty.AsReadOnly();

    internal AdsAttributeEntry[] ToEntries() => ((TypeAttributeCollection) this.Items).ToEntries();
  }
}
