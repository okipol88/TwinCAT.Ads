// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsInvalidNotificationException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Runtime.Serialization;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// This AdsInvalidNotificationException is created if the length of the notification data is 0.
  /// This indicates that the notification handle is not valid any more. This exception is passed
  /// to the AdsNotificationErrorEvent.
  /// </summary>
  [Serializable]
  public sealed class AdsInvalidNotificationException : AdsException
  {
    /// <summary>The _handle</summary>
    private uint _handle;
    /// <summary>The _time stamp</summary>
    private DateTimeOffset _timeStamp;

    /// <summary>
    /// Initializes the class AdsInvalidNotificationException.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="timeStamp">The time stamp.</param>
    internal AdsInvalidNotificationException(uint handle, DateTimeOffset timeStamp)
    {
      this._handle = handle;
      this._timeStamp = timeStamp;
    }

    /// <summary>Handle of the notification.</summary>
    /// <value>The handle.</value>
    public uint Handle => this._handle;

    /// <summary>Gets the Time stamp as long</summary>
    /// <value>The time stamp.</value>
    public DateTimeOffset TimeStamp => this._timeStamp;

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
      info.AddValue("Handle", this._handle);
      info.AddValue("TimeStamp", (object) this._timeStamp);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsInvalidNotificationException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    private AdsInvalidNotificationException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
      this._handle = serializationInfo.GetUInt32(nameof (Handle));
      this._timeStamp = (DateTimeOffset) serializationInfo.GetValue(nameof (TimeStamp), typeof (DateTimeOffset));
    }
  }
}
