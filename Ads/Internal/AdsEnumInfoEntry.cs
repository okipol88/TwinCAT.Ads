// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsEnumInfoEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class AdsEnumInfoEntry.</summary>
  internal class AdsEnumInfoEntry : IAdsEnumCustomMarshal<AdsEnumInfoEntry>
  {
    /// <summary>The name length</summary>
    private byte nameLength;
    /// <summary>The name</summary>
    private string name = string.Empty;
    /// <summary>The value</summary>
    private byte[] value = Array.Empty<byte>();

    public byte NameLength => this.nameLength;

    public string Name => this.name;

    public byte[] Value => this.value;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsEnumInfoEntry" /> class.
    /// </summary>
    public AdsEnumInfoEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsEnumInfoEntry" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="marshaler">The marshaler.</param>
    internal AdsEnumInfoEntry(string name, byte[] value, IStringMarshaler marshaler)
    {
      this.name = name;
      int num = marshaler.MarshalSize("");
      this.nameLength = (byte) (marshaler.MarshalSize(name) - num);
      this.value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsEnumInfoEntry" /> class.
    /// </summary>
    /// <param name="valueSize">Size of the value.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    internal AdsEnumInfoEntry(uint valueSize, StringMarshaler marshaler, ReadOnlySpan<byte> span) => this.Unmarshal(valueSize, (IStringMarshaler) marshaler, span);

    /// <summary>Unmarshals the specified value size.</summary>
    /// <param name="valueSize">Size of the value.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="span">The span.</param>
    /// <returns>System.ValueTuple&lt;System.Boolean, System.Int32&gt;.</returns>
    public (bool valid, int readBytes) Unmarshal(
      uint valueSize,
      IStringMarshaler marshaler,
      ReadOnlySpan<byte> span)
    {
      this.nameLength = span[0];
      int start1 = 1;
      int num = marshaler.MarshalSize("");
      int start2 = start1 + marshaler.Unmarshal(span.Slice(start1, (int) this.nameLength + num), ref this.name);
      this.value = new byte[(int) valueSize];
      span.Slice(start2, (int) valueSize).CopyTo((Span<byte>) this.Value);
      return (true, start2 + (int) valueSize);
    }

    /// <summary>
    /// Marshals the content of this <see cref="T:TwinCAT.Ads.Internal.AdsEnumInfoEntry" /> object.
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>The amount of marshalled bytes.</returns>
    public int Marshal(IStringMarshaler marshaler, Span<byte> buffer)
    {
      int num1 = 0;
      int num2 = marshaler.MarshalSize("");
      buffer[0] = (byte) (marshaler.MarshalSize(this.name) - num2);
      int start1 = num1 + 1;
      int start2 = start1 + marshaler.Marshal(this.Name, buffer.Slice(start1));
      this.value.CopyTo<byte>(buffer.Slice(start2));
      return start2 + this.value.Length;
    }

    /// <summary>
    /// Gets the marshal size of this <see cref="T:TwinCAT.Ads.Internal.AdsEnumInfoEntry" /> object.
    /// </summary>
    /// <param name="marshaler">The symbol string marshaler</param>
    /// <returns>The marshal size of the type <paramref name="marshaler" />.</returns>
    public int MarshalSize(IStringMarshaler marshaler) => 1 + marshaler.MarshalSize(this.Name) + this.value.Length;
  }
}
