// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsAttributeEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class describing attribute entries.</summary>
  /// <exclude />
  public class AdsAttributeEntry : ITypeAttribute, IAdsCustomMarshal<AdsAttributeEntry>
  {
    /// <summary>Attribute Name</summary>
    private string _name = string.Empty;
    /// <summary>Gets the Attribute Value.</summary>
    private string _value = string.Empty;

    /// <summary>Gets the attribute name.</summary>
    /// <value>The name.</value>
    public string Name => this._name;

    /// <summary>Gets the value of the Attribute</summary>
    /// <value>The value.</value>
    public string Value => this._value;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsAttributeEntry" /> class.
    /// </summary>
    public AdsAttributeEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsAttributeEntry" /> class.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    internal AdsAttributeEntry(StringMarshaler marshaler, ReadOnlySpan<byte> span) => this.Unmarshal((IStringMarshaler) marshaler, span);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsAttributeEntry" /> class.
    /// </summary>
    /// <param name="att">The att.</param>
    internal AdsAttributeEntry(ITypeAttribute att)
    {
      this._name = att.Name;
      this._value = att.Value;
    }

    /// <summary>
    /// Unmarshals data into this <see cref="T:TwinCAT.Ads.Internal.AdsAttributeEntry" /> object.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The read buffer.</param>
    /// <returns>A tuple that contains the data validity plus the read bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">marshaler</exception>
    /// <remarks>This method should try to resync the readBytes result to the next valid readable object. Usually
    /// read structures contain their size as first element. If the Read is not valid, than the Unmarshalled object should be ignored.</remarks>
    public (bool valid, int readBytes) Unmarshal(
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span)
    {
      if (marshaler == null)
        throw new ArgumentNullException(nameof (marshaler));
      int num1 = 0;
      byte num2 = span[0];
      byte num3 = span[1];
      int start1 = num1 + 2;
      int start2 = start1 + marshaler.Unmarshal(span.Slice(start1, (int) num2 + 1), ref this._name);
      return (true, start2 + marshaler.Unmarshal(span.Slice(start2, (int) num3 + 1), ref this._value));
    }

    /// <summary>
    /// Marshals the content of the <see cref="T:TwinCAT.Ads.Internal.AdsAttributeEntry" /> object into the buffer.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentNullException">marshaler</exception>
    public int Marshal(IStringMarshaler marshaler, Span<byte> buffer)
    {
      if (marshaler == null)
        throw new ArgumentNullException(nameof (marshaler));
      buffer[0] = (byte) (marshaler.MarshalSize(this.Name) - 1);
      buffer[1] = (byte) (marshaler.MarshalSize(this.Value) - 1);
      int start1 = 2;
      int start2 = start1 + marshaler.Marshal(this.Name, buffer.Slice(start1));
      return start2 + marshaler.Marshal(this.Value, buffer.Slice(start2));
    }

    /// <summary>
    /// Gets the marshal size of this <see cref="T:TwinCAT.Ads.Internal.AdsAttributeEntry" />.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>System.Int32.</returns>
    public int MarshalSize(IStringMarshaler marshaler) => 2 + marshaler.MarshalSize(this.Name) + marshaler.MarshalSize(this.Value);
  }
}
