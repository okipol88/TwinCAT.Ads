// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.WStringType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Represents an Unicode string (Wide string)</summary>
  public sealed class WStringType : DataType, IStringType, IDataType, IBitSize
  {
    /// <summary>
    /// The length of the <see cref="T:TwinCAT.Ads.TypeSystem.WStringType" />
    /// </summary>
    /// <exclude />
    private int length;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.WStringType" /> class.
    /// </summary>
    /// <param name="length">The length.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public WStringType(int length)
      : base(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WSTRING({0})", (object) length), (AdsDataTypeId) 31, (DataTypeCategory) 10, 0, typeof (string))
    {
      int byteCount = Encoding.Unicode.GetByteCount("a");
      this.Size = (length + 1) * byteCount;
      this.flags = (AdsDataTypeFlags) 1;
      this.length = length;
    }

    /// <summary>Gets the number of characters within the string.</summary>
    /// <value>The length.</value>
    public int Length => this.length;

    /// <summary>
    /// Gets the encoding of the String (Encoding.Default (STRING) or Encoding.UNICODE (WSTRING))
    /// </summary>
    /// <value>The encoding.</value>
    public Encoding Encoding => StringMarshaler.UnicodeEncoding;

    /// <summary>
    /// Gets a value indicating whether the string is of fixed length.
    /// </summary>
    /// <value><c>true</c> if this instance is fixed length; otherwise, <c>false</c>.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public bool IsFixedLength => this.length >= 0;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.Name;
  }
}
