// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.InsufficientAccessRightsException
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
  /// <summary>Insufficient rights for access</summary>
  [Serializable]
  public sealed class InsufficientAccessRightsException : SymbolException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.InsufficientAccessRightsException" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="requested">The requested.</param>
    public InsufficientAccessRightsException(IValueSymbol symbol, SymbolAccessRights requested)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The requested rights '{0}' for symbol '{1}' are not sufficient (Current rights: {2})!", (object) requested, (object) ((IInstance) symbol)?.InstanceName, (object) symbol?.AccessRights), (ISymbol) symbol)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.InsufficientAccessRightsException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    private InsufficientAccessRightsException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
