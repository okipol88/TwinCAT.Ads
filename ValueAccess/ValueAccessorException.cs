// Decompiled with JetBrains decompiler
// Type: TwinCAT.ValueAccess.ValueAccessorException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;


#nullable enable
namespace TwinCAT.ValueAccess
{
  /// <summary>Value Accessor Exceptions</summary>
  /// <exclude />
  [Serializable]
  public sealed class ValueAccessorException : AdsException
  {
    /// <summary>
    /// Symbol that is bound to the <see cref="T:TwinCAT.ValueAccess.IAccessorRawValue" />
    /// </summary>
    [NonSerialized]
    private IAccessorRawValue? _accessor;

    /// <summary>
    /// Symbol that is bound to the <see cref="T:TwinCAT.ValueAccess.IAccessorRawValue" />
    /// </summary>
    /// <value>The accessor.</value>
    public IAccessorRawValue? Accessor => this._accessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.ValueAccessorException" /> class.
    /// </summary>
    /// <param name="accessor">The accessor.</param>
    /// <param name="innerException">The inner exception.</param>
    public ValueAccessorException(IAccessorRawValue accessor, Exception innerException)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
      interpolatedStringHandler.AppendLiteral("Value Accessor '");
      interpolatedStringHandler.AppendFormatted<IAccessorRawValue>(accessor);
      interpolatedStringHandler.AppendLiteral("' failed!");
      // ISSUE: explicit constructor call
      this.\u002Ector(interpolatedStringHandler.ToStringAndClear(), accessor, innerException);
    }

    public ValueAccessorException(string message, IAccessorRawValue? accessor)
      : base(message)
    {
      this._accessor = accessor;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.ValueAccessorException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="accessor">The accessor.</param>
    /// <param name="innerException">The inner exception.</param>
    public ValueAccessorException(
      string message,
      IAccessorRawValue accessor,
      Exception? innerException)
      : base(message, innerException)
    {
      this._accessor = accessor;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.ValueAccessorException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    private ValueAccessorException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
