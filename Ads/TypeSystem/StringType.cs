// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.StringType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>String DataType</summary>
  public sealed class StringType : DataType, IStringType, IDataType, IBitSize
  {
    /// <summary>
    /// The length of the <see cref="T:TwinCAT.Ads.TypeSystem.StringType" />
    /// </summary>
    /// <exclude />
    private int length;
    private Encoding _encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StringType" /> class.
    /// </summary>
    /// <param name="length">The length.</param>
    /// <param name="encoding">The encoding.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StringType(int length, Encoding encoding)
    {
      string stringAndClear;
      if (encoding != Encoding.Unicode)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
        interpolatedStringHandler.AppendLiteral("STRING(");
        interpolatedStringHandler.AppendFormatted<int>(length);
        interpolatedStringHandler.AppendLiteral(")");
        stringAndClear = interpolatedStringHandler.ToStringAndClear();
      }
      else
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
        interpolatedStringHandler.AppendLiteral("WSTRING(");
        interpolatedStringHandler.AppendFormatted<int>(length);
        interpolatedStringHandler.AppendLiteral(")");
        stringAndClear = interpolatedStringHandler.ToStringAndClear();
      }
      Type dotnetType = typeof (string);
      // ISSUE: explicit constructor call
      base.\u002Ector(stringAndClear, (AdsDataTypeId) 30, (DataTypeCategory) 10, 0, dotnetType);
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      if (encoding != Encoding.Unicode)
      {
        int num = encoding.IsSingleByte ? 1 : 0;
      }
      this._encoding = encoding;
      int byteCount = encoding.GetByteCount("a");
      this.DataTypeId = (AdsDataTypeId) 30;
      this.Size = (length + 1) * byteCount;
      this.flags = (AdsDataTypeFlags) 1;
      this.length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StringType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="encoding">The encoding.</param>
    internal StringType(AdsDataTypeEntry entry, Encoding encoding)
      : base((DataTypeCategory) 10, entry)
    {
      this._encoding = encoding;
      this.ManagedType = typeof (string);
    }

    /// <summary>Gets the number of characters within the string.</summary>
    /// <value>The length.</value>
    public int Length => this.length;

    /// <summary>
    /// Gets a value indicating whether the string is of fixed length.
    /// </summary>
    /// <value><c>true</c> if this instance is fixed length; otherwise, <c>false</c>.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public bool IsFixedLength => this.length >= 0;

    /// <summary>
    /// Gets the encoding of the String (Encoding.Default (Ansi Codepage, STRING) or Encoding.UNICODE (WSTRING))
    /// </summary>
    /// <value>The encoding.</value>
    public Encoding Encoding => this._encoding;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.Name;
  }
}
