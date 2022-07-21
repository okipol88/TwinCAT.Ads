// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.InstanceValueMarshaler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Symbol Converter marshaller / Value converter</summary>
  /// <exclude />
  internal class InstanceValueMarshaler : ISymbolMarshaler
  {
    /// <summary>The type marshaller</summary>
    private SymbolicAnyTypeMarshaler _typeMarshaller = new SymbolicAnyTypeMarshaler();

    public IDataTypeMarshaler TypeMarshaller => (IDataTypeMarshaler) this._typeMarshaller;

    public Encoding DefaultValueEncoding => this._typeMarshaller.DefaultValueEncoding;

    internal InstanceValueMarshaler()
    {
    }

    public bool CanMarshal(IAttributedInstance symbol, Type? valueType)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      return ((IInstance) symbol).DataType != null && this._typeMarshaller.CanMarshal(((IInstance) symbol).DataType, valueType);
    }

    public int Marshal(IAttributedInstance symbol, object? value, Span<byte> destination)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (destination == (Span<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (destination));
      return this._typeMarshaller.Marshal(symbol, value, destination);
    }

    public int MarshalSize(IInstance symbol) => symbol != null ? ((IBitSize) symbol).ByteSize : throw new ArgumentNullException(nameof (symbol));

    public bool TryGetManagedType(IAttributedInstance symbol, [NotNullWhen(true)] out Type? managed)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (((IInstance) symbol).DataType != null)
        return this._typeMarshaller.TryGetManagedType(((IInstance) symbol).DataType, out managed);
      managed = (Type) null;
      return false;
    }

    /// <summary>Tries to get the corresponding managed type.</summary>
    /// <param name="type">The type.</param>
    /// <param name="managed">The managed.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetManagedType(IDataType type, [NotNullWhen(true)] out Type? managed) => type != null ? this._typeMarshaller.TryGetManagedType(type, out managed) : throw new ArgumentNullException(nameof (type));

    /// <summary>Unmarshals the specified symbol.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="source">The source memory/span.</param>
    /// <param name="valueType">Target type.</param>
    /// <param name="value">The unmarshaled value.</param>
    /// <returns>The number of unmarshaled bytes</returns>
    public int Unmarshal(
      IAttributedInstance symbol,
      ReadOnlySpan<byte> source,
      Type? valueType,
      out object value)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      return this._typeMarshaller.Unmarshal(symbol, source, valueType, out value);
    }
  }
}
