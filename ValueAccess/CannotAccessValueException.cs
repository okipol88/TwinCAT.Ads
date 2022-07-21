// Decompiled with JetBrains decompiler
// Type: TwinCAT.ValueAccess.CannotAccessValueException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.ValueAccess
{
  /// <summary>
  /// Class CannotAccessValueException. This class cannot be inherited.
  /// Implements the <see cref="T:TwinCAT.TypeSystem.SymbolException" />
  /// </summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.SymbolException" />
  [Serializable]
  public sealed class CannotAccessValueException : SymbolException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.CannotAccessValueException" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public CannotAccessValueException(ISymbol symbol)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 1);
      interpolatedStringHandler.AppendLiteral("Accessor not available. Cannot access symbol '");
      interpolatedStringHandler.AppendFormatted<ISymbol>(symbol);
      interpolatedStringHandler.AppendLiteral("'");
      // ISSUE: explicit constructor call
      base.\u002Ector(interpolatedStringHandler.ToStringAndClear(), symbol);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.CannotAccessValueException" /> class.
    /// </summary>
    public CannotAccessValueException()
      : base("Accessor not available!", (ISymbol) null)
    {
    }

    private CannotAccessValueException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
