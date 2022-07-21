// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.CannotAccessVirtualSymbolException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Globalization;
using System.Runtime.Serialization;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Cannot access virtual Symbol</summary>
  [Serializable]
  public sealed class CannotAccessVirtualSymbolException : SymbolException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public CannotAccessVirtualSymbolException(ISymbol symbol)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot read/write from/to virtual symbol '{0}'!", (object) ((IInstance) symbol)?.InstanceName), symbol)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    private CannotAccessVirtualSymbolException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
