// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AdsSymbolParser
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Class parsing Symbols from String or from AdsStream (for internal use only)
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class AdsSymbolParser
  {
    /// <summary>
    /// Parses the symbolStream for Symbols (for internal use only)
    /// </summary>
    /// <param name="span">The buffer to read from..</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="factoryServices">The factory services.</param>
    /// <exception cref="T:System.ArgumentNullException">span</exception>
    /// <exception cref="T:System.ArgumentNullException">factoryServices</exception>
    public static int ParseSymbols(
      ReadOnlySpan<byte> span,
      StringMarshaler marshaler,
      ISymbolFactoryServices factoryServices)
    {
      if (span == (ReadOnlySpan<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (span));
      if (factoryServices == null)
        throw new ArgumentNullException(nameof (factoryServices));
      int start = 0;
      int symbols = 0;
      while (start < span.Length)
      {
        ISymbol symbol1 = (ISymbol) null;
        (bool valid, int readBytes) symbol2 = AdsSymbolParser.ParseSymbol(span.Slice(start), marshaler, factoryServices, out symbol1);
        start += symbol2.readBytes;
        ++symbols;
        if (symbol2.valid && symbol1 != null)
          factoryServices.Binder.Bind((IHierarchicalSymbol) symbol1);
      }
      return symbols;
    }

    internal static (bool valid, int readBytes) ParseSymbol(
      ReadOnlySpan<byte> span,
      StringMarshaler marshaler,
      ISymbolFactoryServices factoryServices,
      out ISymbol? symbol)
    {
      AdsSymbolEntry symbol1 = (AdsSymbolEntry) null;
      (bool valid, int readBytes) symbol2 = AdsSymbolEntry.Parse((IStringMarshaler) marshaler, span, out symbol1);
      symbol = !symbol2.valid ? (ISymbol) null : factoryServices.SymbolFactory.CreateInstance((ISymbolInfo) symbol1, (ISymbol) null);
      return symbol2;
    }

    /// <summary>Parses the symbol.</summary>
    /// <param name="span">The buffer to parse from.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="factoryServices">The factory services.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>ISymbol.</returns>
    internal static Task<ResultValue<ISymbol>> ParseSymbolAsync(
      ReadOnlySpan<byte> span,
      StringMarshaler marshaler,
      ISymbolFactoryServices factoryServices,
      CancellationToken cancel)
    {
      AdsSymbolEntry symbol;
      return AdsSymbolEntry.Parse((IStringMarshaler) marshaler, span, out symbol).valid ? factoryServices.SymbolFactory.CreateInstanceAsync((ISymbolInfo) symbol, (ISymbol) null, cancel) : Task.FromResult<ResultValue<ISymbol>>(ResultValue<ISymbol>.CreateError((AdsErrorCode) 1));
    }

    /// <summary>
    /// Parses the the data types within the specified stream (for internal use only)
    /// </summary>
    /// <param name="span">The buffer to parse from.</param>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="factoryServices">The factory services.</param>
    /// <param name="buildInTypesInStream">if set to <c>true</c> base types are contained in stream.</param>
    /// <param name="buildInTypes">The base types.</param>
    /// <returns>The number of parsed types.</returns>
    /// <exception cref="T:System.ArgumentNullException">marshaler</exception>
    /// <exception cref="T:System.ArgumentNullException">buildInTypes</exception>
    /// <exception cref="T:System.ArgumentNullException">factoryServices</exception>
    /// <exception cref="T:TwinCAT.AdsException">Enum base type mismatch!</exception>
    public static int ParseTypes(
      ReadOnlySpan<byte> span,
      StringMarshaler marshaler,
      ISymbolFactoryServices factoryServices,
      bool buildInTypesInStream,
      DataTypeCollection<IDataType>? buildInTypes)
    {
      if (marshaler == null)
        throw new ArgumentNullException(nameof (marshaler));
      if (factoryServices == null)
        throw new ArgumentNullException(nameof (factoryServices));
      int start = 0;
      int types = 0;
      while (start < span.Length)
      {
        AdsDataTypeEntry entry = new AdsDataTypeEntry();
        (bool valid, int readBytes) tuple = entry.Unmarshal((IStringMarshaler) marshaler, span.Slice(start));
        start += tuple.readBytes;
        ++types;
        if (tuple.valid)
        {
          try
          {
            IDataType type1 = (IDataType) null;
            if (!(buildInTypes != null & buildInTypesInStream) || !buildInTypes.TryGetType(entry.EntryName, out type1))
            {
              DataType type2 = AdsSymbolParser.ParseType(entry, marshaler.Encoding, factoryServices);
              if (type2 != null)
                factoryServices.Binder.RegisterType((IDataType) type2);
            }
          }
          catch (Exception ex)
          {
            AdsModule.Trace.TraceWarning("Cannot parse DataTypeEntry. Skipping dataType", ex);
          }
        }
        else
          AdsModule.Trace.TraceWarning("Cannot parse DataTypeEntry. Skipping dataType");
      }
      return types;
    }

    internal static DataType ParseType(
      AdsDataTypeEntry entry,
      Encoding encoding,
      ISymbolFactoryServices factoryServices)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      return factoryServices != null ? AdsDataTypeFactory.CreateType(entry, factoryServices) : throw new ArgumentNullException(nameof (factoryServices));
    }
  }
}
