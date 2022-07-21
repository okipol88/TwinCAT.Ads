// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsSumCommandException
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Runtime.Serialization;
using TwinCAT.Ads.SumCommand;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// The exception that is thrown when an ADS SumCommandBase error occurs.
  /// </summary>
  [Serializable]
  public sealed class AdsSumCommandException : AdsErrorException
  {
    [NonSerialized]
    private AdsErrorCode[] entityExceptions = Array.Empty<AdsErrorCode>();
    [NonSerialized]
    private ISumCommand command;

    /// <summary>
    /// Initializes a new Instance of the AdsErrorException class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="command">The command.</param>
    public AdsSumCommandException(string message, ISumCommand command)
      : base(message, AdsSumCommandException.PassThroughNonNull(command).Result)
    {
      this.command = command;
      this.entityExceptions = command.SubResults;
    }

    private static ISumCommand PassThroughNonNull(ISumCommand command) => command != null ? command : throw new ArgumentNullException(nameof (command));

    /// <summary>Gets the sum command.</summary>
    /// <value>The sum command.</value>
    public ISumCommand SumCommand => this.command;

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
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      base.GetObjectData(info, context);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSumCommandException" /> class.
    /// </summary>
    /// <param name="info">The information.</param>
    /// <param name="streamingContext">The streaming context.</param>
    private AdsSumCommandException(SerializationInfo info, StreamingContext streamingContext)
      : base(info, streamingContext)
    {
    }
  }
}
