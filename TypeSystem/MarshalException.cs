// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.MarshalException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TwinCAT.Ads;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Common Marshalling Exception</summary>
  [Serializable]
  public sealed class MarshalException : AdsException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    public MarshalException()
      : base(ResMan.GetString("MarshalException_Message"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public MarshalException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new Instance of the AdsException class.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public MarshalException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    public MarshalException(IDataType source, Type target)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResMan.GetString("MarshalException_Message2"), (object) source, (object) target))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    /// <param name="message">The message.</param>
    public MarshalException(IDataType source, Type target, string message)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResMan.GetString("MarshalException_Message3"), (object) source, (object) target, (object) message))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="source">The source dataType.</param>
    public MarshalException(IDataType source)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResMan.GetString("MarshalException_Message2"), (object) source, (object) "Unknown"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="tp">The type.</param>
    /// <param name="member">The member.</param>
    public MarshalException(IInstance instance, Type tp, MemberInfo member)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResMan.GetString("MarshalException_Message4"), (object) instance, (object) tp, (object) member?.Name))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    private MarshalException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="target">The datatype information.</param>
    /// <param name="value">The value.</param>
    public MarshalException(IDataType target, object? value)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResMan.GetString("MarshalException_Message4"), (object) value?.ToString(), (object) target?.Name))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="spec">The spec.</param>
    internal MarshalException(AnyTypeSpecifier spec)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
      interpolatedStringHandler.AppendLiteral("Cannot marshal/unmarshal ANY_TYPE specifier '");
      interpolatedStringHandler.AppendFormatted<Type>(spec.Type);
      interpolatedStringHandler.AppendLiteral("' (Category: ");
      interpolatedStringHandler.AppendFormatted<DataTypeCategory>(spec.Category);
      interpolatedStringHandler.AppendLiteral(")");
      // ISSUE: explicit constructor call
      base.\u002Ector(interpolatedStringHandler.ToStringAndClear());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MarshalException" /> class.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="value">The value.</param>
    internal MarshalException(IInstance instance, string typeName, object? value)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 3);
      interpolatedStringHandler.AppendLiteral("Cannot marshal/unmarshal value '");
      interpolatedStringHandler.AppendFormatted<object>(value);
      interpolatedStringHandler.AppendLiteral("' to symbol '");
      interpolatedStringHandler.AppendFormatted(instance.InstancePath);
      interpolatedStringHandler.AppendLiteral("' (DataType: ");
      interpolatedStringHandler.AppendFormatted(typeName);
      interpolatedStringHandler.AppendLiteral(")!");
      // ISSUE: explicit constructor call
      base.\u002Ector(interpolatedStringHandler.ToStringAndClear());
    }
  }
}
