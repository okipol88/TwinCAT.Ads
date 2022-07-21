// Decompiled with JetBrains decompiler
// Type: TwinCAT.SessionException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Runtime.Serialization;


#nullable enable
namespace TwinCAT
{
  /// <summary>Session Exception</summary>
  /// <seealso cref="T:TwinCAT.AdsException" />
  [Serializable]
  public class SessionException : AdsException
  {
    /// <summary>The session</summary>
    [NonSerialized]
    private ISession? _session;

    /// <summary>Gets the session.</summary>
    /// <value>The session.</value>
    public ISession? Session => this._session;

    public SessionException(string message, ISession? session)
      : this(message, session, (Exception) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SessionException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public SessionException(string message)
      : this(message, (ISession) null, (Exception) null)
    {
    }

    public SessionException(string message, ISession? session, Exception? innerException)
      : base(message, innerException)
    {
      this._session = session;
    }

    /// <summary>
    /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">info</exception>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
    /// </PermissionSet>
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      // ISSUE: explicit non-virtual call
      __nonvirtual (((Exception) this).GetObjectData(info, context));
      info.AddValue("Session", (object) this.Session);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SessionException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    protected SessionException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
      this._session = (ISession) serializationInfo.GetValue(nameof (Session), typeof (ISession));
    }
  }
}
