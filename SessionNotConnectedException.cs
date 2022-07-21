// Decompiled with JetBrains decompiler
// Type: TwinCAT.SessionNotConnectedException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Globalization;
using System.Runtime.Serialization;
using TwinCAT.Ads;


#nullable enable
namespace TwinCAT
{
  /// <summary>Class SessionNotConnectedException.</summary>
  /// <seealso cref="T:TwinCAT.SessionException" />
  [Serializable]
  public class SessionNotConnectedException : SessionException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SessionNotConnectedException" /> class.
    /// </summary>
    /// <param name="session">The session.</param>
    public SessionNotConnectedException(ISession session)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResMan.GetString("SessionNotConnected_Message1"), (object) session?.Id, (object) session?.AddressSpecifier), session)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SessionNotConnectedException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="session">The session.</param>
    public SessionNotConnectedException(string message, ISession session)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResMan.GetString("SessionNotConnected_Message2"), (object) session?.Id, (object) session?.AddressSpecifier, (object) message), session)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SessionNotConnectedException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    protected SessionNotConnectedException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
