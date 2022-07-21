// Decompiled with JetBrains decompiler
// Type: TwinCAT.ClientNotConnectedException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Runtime.Serialization;
using TwinCAT.Ads;


#nullable enable
namespace TwinCAT
{
  /// <summary>Class ClientNotConnectedException</summary>
  [Serializable]
  public class ClientNotConnectedException : AdsException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ClientNotConnectedException" /> class.
    /// </summary>
    public ClientNotConnectedException()
      : base(ResMan.GetString("NotConnected_message"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ClientNotConnectedException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    protected ClientNotConnectedException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
