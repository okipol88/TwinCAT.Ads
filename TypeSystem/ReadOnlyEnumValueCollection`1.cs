// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyEnumValueCollection`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Read only version of the <see cref="T:TwinCAT.TypeSystem.EnumValueCollection`1" />
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ReadOnlyEnumValueCollection<T> : 
    ReadOnlyCollection<EnumValue<T>>,
    IEnumValueCollection<EnumValue<T>, T>,
    ICollection<EnumValue<T>>,
    IEnumerable<EnumValue<T>>,
    IEnumerable
    where T : struct, IConvertible
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyEnumValueCollection`1" /> class.
    /// </summary>
    /// <param name="coll">The coll.</param>
    public ReadOnlyEnumValueCollection(EnumValueCollection<T> coll)
      : base((IList<EnumValue<T>>) coll)
    {
    }

    /// <summary>Determines whether [contains] [the specified name].</summary>
    /// <param name="value">Value</param>
    /// <returns><c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.</returns>
    public bool Contains(string value) => ((EnumValueCollection<T>) this.Items).Contains(value);

    /// <summary>Tries to parse the string value of the Enum.</summary>
    /// <param name="strValue">The Value in string represention.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string strValue, out T value) => ((EnumValueCollection<T>) this.Items).TryParse(strValue, out value);

    /// <summary>Tries to parse the string value of the Enum.</summary>
    /// <param name="strValue">The Value in string represention.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryParse(string strValue, [NotNullWhen(true)] out EnumValue<T>? value) => ((EnumValueCollection<T>) this.Items).TryParse(strValue, out value);

    /// <summary>Parses the specified name.</summary>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    public T Parse(string name) => ((EnumValueCollection<T>) this.Items).Parse(name);

    /// <summary>Gets the Value Names.</summary>
    /// <returns>System.String[].</returns>
    public string[] GetNames() => ((EnumValueCollection<T>) this.Items).GetNames();

    /// <summary>Gets the values.</summary>
    /// <returns>T[].</returns>
    public T[] GetValues() => ((EnumValueCollection<T>) this.Items).GetValues();

    /// <summary>
    /// Gets the enumeration value <typeparamref name="T" /> from its string representation.
    /// </summary>
    /// <param name="name">The name of the enum value.</param>
    /// <returns>T.</returns>
    public T this[string name] => this.Items.First<EnumValue<T>>((Func<EnumValue<T>, bool>) (field => StringComparer.OrdinalIgnoreCase.Compare(field.Name, name) == 0)).Primitive;
  }
}
