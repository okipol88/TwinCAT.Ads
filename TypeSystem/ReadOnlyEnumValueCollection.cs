// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyEnumValueCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Read only version of the <see cref="T:TwinCAT.TypeSystem.EnumValueCollection`1" />
  /// </summary>
  public class ReadOnlyEnumValueCollection : 
    ReadOnlyCollection<IEnumValue>,
    IEnumValueCollection,
    IEnumValueCollection<IEnumValue, IConvertible>,
    ICollection<IEnumValue>,
    IEnumerable<IEnumValue>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyEnumValueCollection`1" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    public ReadOnlyEnumValueCollection(EnumValueCollection coll)
      : base((IList<IEnumValue>) coll)
    {
    }

    /// <summary>Determines whether [contains] [the specified name].</summary>
    /// <param name="value">Value</param>
    /// <returns><c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.</returns>
    public bool Contains(string value) => ((EnumValueCollection) this.Items).Contains(value);

    /// <summary>Tries to pars the string value of the Enum.</summary>
    /// <param name="strValue">The Value in string represention.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string strValue, out IConvertible value) => ((EnumValueCollection) this.Items).TryParse(strValue, out value);

    public bool TryParse(string strValue, [NotNullWhen(true)] out IEnumValue? value) => ((EnumValueCollection) this.Items).TryParse(strValue, out value);

    /// <summary>Parses the specified name.</summary>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    public IConvertible Parse(string name) => ((EnumValueCollection) this.Items).Parse(name);

    /// <summary>Gets the Value Names.</summary>
    /// <returns>System.String[].</returns>
    public string[] GetNames() => ((EnumValueCollection) this.Items).GetNames();

    /// <summary>Gets the values.</summary>
    /// <returns>T[].</returns>
    public IConvertible[] GetValues() => ((EnumValueCollection) this.Items).GetValues();

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="name">The name of the value</param>
    /// <returns>EnumValue&lt;T&gt;.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public IConvertible this[string name] => ((EnumValueCollection) this.Items)[name];
  }
}
