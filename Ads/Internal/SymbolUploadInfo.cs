// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.SymbolUploadInfo
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>ADS Info object describing the SymbolUpload data</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SymbolUploadInfo
  {
    /// <summary>The data Version of the Upload Information.</summary>
    private int _version;
    /// <summary>The symbols</summary>
    private uint _symbolCount;
    /// <summary>The symbol size</summary>
    private uint _symbolsBlockSize;
    /// <summary>The datatypes</summary>
    private uint _dataTypeCount;
    /// <summary>The datatype size</summary>
    private uint _dataTypesBlockSize;
    /// <summary>The maximum dynamic symbols</summary>
    private uint _maxDynamicSymbolCount;
    /// <summary>The used dynamic symbols</summary>
    private uint _usedDynamicSymbolCount;
    /// <summary>Invalid dynamic symbols</summary>
    private uint _invalidDynamicSymbolCount;
    /// <summary>The encoding code page (marshalled from target)</summary>
    private int _encodingCodePage;
    /// <summary>Symbol Upload Flags</summary>
    private SymbolUploadFlags _flags;
    /// <summary>Reserved bytes for future extensions.</summary>
    private uint[] _reserved = Array.Empty<uint>();

    /// <summary>
    /// Calculates the provided version of the <see cref="T:TwinCAT.Ads.Internal.SymbolUploadInfo" /> structure marshalled from target.
    /// </summary>
    /// <param name="readBytes">The read bytes.</param>
    /// <returns>System.Int32.</returns>
    /// <remarks>The version of the struct data is dependent
    /// on the count of returned bytes.</remarks>
    public static int CalcVersion(int readBytes)
    {
      if (readBytes == 8)
        return 1;
      if (readBytes == 24)
        return 2;
      if (readBytes == 64)
        return 3;
      throw new NotImplementedException();
    }

    /// <summary>Gets the marshal size of a specific version.</summary>
    /// <param name="version">The version.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public static int GetMarshalSize(int version)
    {
      switch (version)
      {
        case 1:
          return 8;
        case 2:
          return 24;
        case 3:
          return 64;
        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>Gets the Marshal size.</summary>
    /// <value>The size of the marshal.</value>
    public int MarshalSize => SymbolUploadInfo.GetMarshalSize(this._version);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.SymbolUploadInfo" /> class.
    /// </summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SymbolUploadInfo() => this._version = 1;

    /// <summary>Gets the data Version of the Upload Information.</summary>
    /// <value>The version.</value>
    public int Version => this._version;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.SymbolUploadInfo" /> class.
    /// </summary>
    /// <param name="buffer">The Raw data</param>
    /// <param name="version">Symbol info Version (dependent on the size of the Data in the reader)</param>
    /// <remarks>
    /// Version 1: SymbolUploadInfo (8 bytes)
    /// Version 2: AdsSymbolUploadInfo2 (24 bytes)
    /// Version 3: AdsSymbolUploadInfo3 (64 bytes)
    /// </remarks>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SymbolUploadInfo(int version, ReadOnlySpan<byte> buffer) => this.Unmarshal(version, buffer);

    private int Unmarshal(int version, ReadOnlySpan<byte> buffer)
    {
      this._version = version;
      int start1 = 0;
      this._symbolCount = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start1));
      int start2 = start1 + 4;
      this._symbolsBlockSize = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start2));
      int start3 = start2 + 4;
      if (version >= 2)
      {
        this._dataTypeCount = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start3));
        int start4 = start3 + 4;
        this._dataTypesBlockSize = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start4));
        int start5 = start4 + 4;
        this._maxDynamicSymbolCount = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start5));
        int start6 = start5 + 4;
        this._usedDynamicSymbolCount = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start6));
        start3 = start6 + 4;
        if (version >= 3)
        {
          this._invalidDynamicSymbolCount = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start3));
          int start7 = start3 + 4;
          this._encodingCodePage = (int) BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start7));
          int start8 = start7 + 4;
          this._flags = (SymbolUploadFlags) BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start8));
          start3 = start8 + 4;
          this._reserved = new uint[7];
          for (int index = 0; index < 7; ++index)
          {
            this._reserved[index] = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(start3));
            start3 += 4;
          }
        }
      }
      return start3;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.SymbolUploadInfo" /> class.
    /// </summary>
    /// <param name="symbolCount">The symbol count.</param>
    /// <param name="symbolsBlockSize">Size of the symbols block.</param>
    /// <param name="typeCount">The type count.</param>
    /// <param name="typeBlockSize">Size of the type block.</param>
    /// <param name="maxDynamicSymbolCount">The maximum dynamic symbol count.</param>
    /// <param name="usedDynamicSymbolCount">The used dynamic symbol count.</param>
    /// <param name="symbolPathEncoding">The encoding used for InstancePath marshalling/unmarshalling.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal SymbolUploadInfo(
      int symbolCount,
      int symbolsBlockSize,
      int typeCount,
      int typeBlockSize,
      int maxDynamicSymbolCount,
      int usedDynamicSymbolCount,
      Encoding symbolPathEncoding)
    {
      this._version = 3;
      this._symbolCount = (uint) symbolCount;
      this._symbolsBlockSize = (uint) symbolsBlockSize;
      this._dataTypeCount = (uint) typeCount;
      this._dataTypesBlockSize = (uint) typeBlockSize;
      this._maxDynamicSymbolCount = (uint) maxDynamicSymbolCount;
      this._usedDynamicSymbolCount = (uint) usedDynamicSymbolCount;
      this._invalidDynamicSymbolCount = 0U;
      this._encodingCodePage = symbolPathEncoding.CodePage;
      this._flags = SymbolUploadFlags.IncludesBaseTypes;
      if (Environment.Is64BitProcess)
        this._flags |= SymbolUploadFlags.Is64BitPlatform;
      this._reserved = new uint[7];
    }

    /// <summary>Writes to memory.</summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    public int Marshal(Span<byte> buffer)
    {
      int start1 = 0;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start1, 4), this._symbolCount);
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start2, 4), this._symbolsBlockSize);
      int start3 = start2 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start3, 4), this._dataTypeCount);
      int start4 = start3 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start4, 4), this._dataTypesBlockSize);
      int start5 = start4 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start5, 4), this._maxDynamicSymbolCount);
      int start6 = start5 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start6, 4), this._usedDynamicSymbolCount);
      int start7 = start6 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start7, 4), this._invalidDynamicSymbolCount);
      int start8 = start7 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start8, 4), (uint) this._encodingCodePage);
      int start9 = start8 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start9, 4), (uint) this._flags);
      int start10 = start9 + 4;
      for (int index = 0; index < this._reserved.Length; ++index)
      {
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(start10, 4), this._reserved[index]);
        start10 += 4;
      }
      return start10;
    }

    /// <summary>
    /// Dumps the <see cref="T:TwinCAT.Ads.Internal.SymbolUploadInfo" />
    /// </summary>
    /// <returns>System.String.</returns>
    internal string Dump() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "TypeCount: {0}, TypeSize: {1} bytes, SymbolCount: {2}, SymbolSize: {3} bytes, DynSymbols: {4}, MaxDynSymbols: {5}", (object) this._dataTypeCount, (object) this._dataTypesBlockSize, (object) this._symbolCount, (object) this._symbolsBlockSize, (object) this._usedDynamicSymbolCount, (object) this._maxDynamicSymbolCount);

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) this.GetType().Name, (object) this.Dump());

    /// <summary>Gets the symbol count.</summary>
    /// <value>The symbol count.</value>
    public int SymbolCount => (int) this._symbolCount;

    /// <summary>Gets the size of the symbols block.</summary>
    /// <value>The size of the symbols block.</value>
    public int SymbolsBlockSize => (int) this._symbolsBlockSize;

    /// <summary>Gets the data type count.</summary>
    /// <value>The data type count.</value>
    public int DataTypeCount => (int) this._dataTypeCount;

    /// <summary>Gets the size of the data types block.</summary>
    /// <value>The size of the data types block.</value>
    public int DataTypesBlockSize => (int) this._dataTypesBlockSize;

    /// <summary>Gets the maximum number of the dynamic symbols.</summary>
    /// <value>The maximum dynamic symbol count.</value>
    public int MaxDynamicSymbolCount => (int) this._maxDynamicSymbolCount;

    /// <summary>Gets the number of used dynamic symbols.</summary>
    /// <value>The used dynamic symbol count.</value>
    public int UsedDynamicSymbolCount => (int) this._usedDynamicSymbolCount;

    /// <summary>Gets the number of invalid dynamic symbols.</summary>
    /// <value>The invalid dynamic symbol count.</value>
    public int InvalidDynamicSymbolCount => (int) this._invalidDynamicSymbolCount;

    /// <summary>Gets the string encoding (marshalled from target)</summary>
    /// <value>The string encoding for symbols and data types (Default: <see cref="P:System.Text.Encoding.Default" />)</value>
    public Encoding StringEncoding
    {
      get
      {
        Encoding stringEncoding = StringMarshaler.DefaultEncoding;
        if (this._encodingCodePage != 0)
        {
          if (stringEncoding.CodePage != this._encodingCodePage)
          {
            try
            {
              Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
              stringEncoding = Encoding.GetEncoding(this._encodingCodePage);
            }
            catch (Exception ex)
            {
            }
          }
        }
        return stringEncoding;
      }
    }

    /// <summary>Gets the Symbol Upload Flags</summary>
    /// <value>The flags.</value>
    public SymbolUploadFlags Flags => this._flags;

    /// <summary>Gets the size of the Pointers on the target system.</summary>
    /// <value>The size of the target pointer.</value>
    public int TargetPointerSize
    {
      get
      {
        if (this._version != 3 || this._encodingCodePage == 0)
          return 0;
        return (this._flags & SymbolUploadFlags.Is64BitPlatform) == SymbolUploadFlags.Is64BitPlatform ? 8 : 4;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the symbol server delivers base types also.
    /// </summary>
    /// <value><c>true</c> if base types are in the data types collection; otherwise, <c>false</c>.</value>
    public bool ContainsBaseTypes => (this._flags & SymbolUploadFlags.IncludesBaseTypes) == SymbolUploadFlags.IncludesBaseTypes;
  }
}
