// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsErrorException
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
  /// The exception that is thrown when an ADS error occurs.
  /// </summary>
  [Serializable]
  public class AdsErrorException : AdsException
  {
    /// <summary>The _error code</summary>
    private AdsErrorCode _errorCode;

    /// <summary>
    /// Initializes a new Instance of the AdsErrorException class.
    /// </summary>
    public AdsErrorException()
    {
    }

    public AdsErrorException(string? message, AdsErrorCode errorCode)
      : base(ResMan.GetString(message, errorCode))
    {
      this._errorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsErrorException" /> class.
    /// </summary>
    /// <param name="info">The information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    protected AdsErrorException(SerializationInfo info, StreamingContext streamingContext)
      : base(info, streamingContext)
    {
      this._errorCode = (AdsErrorCode) info.GetInt32(nameof (ErrorCode));
    }

    /// <summary>Gets the error code of the Exception.</summary>
    /// <value>The error code.</value>
    public AdsErrorCode ErrorCode => this._errorCode;

    /// <summary>Creates the AdsErrorException</summary>
    /// <param name="adsErrorCode">The ads error code.</param>
    /// <returns>AdsErrorException.</returns>
    public static AdsErrorException Create(AdsErrorCode adsErrorCode) => AdsErrorException.CreateException((string) null, adsErrorCode);

    /// <summary>Creates the AdsErrorException</summary>
    /// <param name="message">The message.</param>
    /// <param name="adsErrorCode">The ads error code.</param>
    /// <returns>AdsErrorException.</returns>
    public static AdsErrorException Create(
      string message,
      AdsErrorCode adsErrorCode)
    {
      return AdsErrorException.CreateException(message, adsErrorCode);
    }

    /// <summary>Creates the an exception object from Error Code</summary>
    /// <param name="message">The message.</param>
    /// <param name="adsErrorCode">The ads error code.</param>
    /// <returns>AdsErrorException.</returns>
    /// <exception cref="T:System.ArgumentException">No error indicated!;adsErrorCode</exception>
    private static AdsErrorException CreateException(
      string? message,
      AdsErrorCode adsErrorCode)
    {
      return adsErrorCode != null ? new AdsErrorException(message, adsErrorCode) : throw new ArgumentException("No error indicated!", nameof (adsErrorCode));
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
      info.AddValue("ErrorCode", (int) this._errorCode);
    }
  }
}
