// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.ISymbolFlagProvider
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using TwinCAT.Ads.Internal;

namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Gives access to the specific Symbol Flags</summary>
  /// <exclude />
  public interface ISymbolFlagProvider
  {
    /// <summary>Gets the Symbol Flags.</summary>
    /// <value>The flags.</value>
    /// <exclude />
    AdsSymbolFlags Flags { get; }

    /// <summary>Gets the ADS Context Mask</summary>
    /// <value>The context mask.</value>
    byte ContextMask { get; }

    /// <summary>Indicates that this instance is read only.</summary>
    /// <remarks>
    /// Actually, this Flag is restricted to TcCOM-Objects readonly Parameters. Within the PLC this is used for the ApplicationName and
    /// ProjectName of PLC instances.
    /// Write-Access on these Modules will create an <see cref="F:TwinCAT.Ads.AdsErrorCode.DeviceAccessDenied" /> error.
    /// </remarks>
    bool IsReadOnly { get; }

    /// <summary>Indicates that this instance is persistent.</summary>
    bool IsPersistent { get; }

    /// <summary>
    /// Indicates that this instance is a TcComInterfacePointer.
    /// </summary>
    bool IsTcComInterfacePointer { get; }

    /// <summary>Indicates that this instance has set TypeGuid flag.</summary>
    bool IsTypeGuid { get; }
  }
}
