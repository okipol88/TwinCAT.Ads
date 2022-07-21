// Decompiled with JetBrains decompiler
// Type: TwinCAT.SymbolLoaderSettings
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using TwinCAT.Ads.ValueAccess;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT
{
  /// <summary>Settings object for the SymbolLoader initialization.</summary>
  /// <seealso cref="T:TwinCAT.ISymbolLoaderSettings" />
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.SymbolLoaderFactory" />
  /// <seealso cref="T:TwinCAT.TypeSystem.ISymbolLoader" />
  /// <seealso cref="T:TwinCAT.SymbolsLoadMode" />
  /// <seealso cref="T:TwinCAT.Ads.ValueAccess.ValueAccessMode" />
  /// <remarks>This settings object is used for the initialization of the internal Symbol loader object.</remarks>
  public class SymbolLoaderSettings : ISymbolLoaderSettings
  {
    private ValueCreationModes _creationMode = (ValueCreationModes) 1;
    /// <summary>The value update mode</summary>
    private ValueUpdateMode _valueUpdateMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SymbolLoaderSettings" /> class with <see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.IndexGroupOffsetPreferred" />.
    /// </summary>
    /// <param name="loadMode">The load mode.</param>
    public SymbolLoaderSettings(SymbolsLoadMode loadMode)
      : this(loadMode, ValueAccessMode.Symbolic)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SymbolLoaderSettings" /> class.
    /// </summary>
    /// <param name="loadMode">The load mode.</param>
    /// <param name="valueAccess">The value access.</param>
    public SymbolLoaderSettings(SymbolsLoadMode loadMode, ValueAccessMode valueAccess)
    {
      this.SymbolsLoadMode = loadMode;
      this.ValueAccessMode = valueAccess;
      this.NonCachedArrayElements = true;
      this.ValueCreation = (ValueCreationModes) 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.SymbolLoaderSettings" /> class.
    /// </summary>
    /// <param name="loadMode">The load mode.</param>
    /// <param name="valueCreation">The dynamic value creation.</param>
    /// <param name="valueAccess">The value access.</param>
    public SymbolLoaderSettings(
      SymbolsLoadMode loadMode,
      ValueCreationModes valueCreation,
      ValueAccessMode valueAccess)
    {
      this.SymbolsLoadMode = loadMode;
      this.ValueAccessMode = valueAccess;
      this._creationMode = valueCreation;
      this.NonCachedArrayElements = true;
    }

    /// <summary>Gets or sets the symbols load mode.</summary>
    /// <value>The symbols load mode.</value>
    public SymbolsLoadMode SymbolsLoadMode { get; set; }

    /// <summary>Gets or sets the value access mode.</summary>
    /// <value>The value access mode.</value>
    public ValueAccessMode ValueAccessMode { get; set; }

    /// <summary>
    /// Gets or sets the setting to create ArrayElements "On-The-Fly" (Default True)
    /// </summary>
    /// <value>The value access mode.</value>
    public bool NonCachedArrayElements { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Disconnect connections can be reconnected.
    /// </summary>
    /// <value><c>true</c> if Disconnect connections can be reconnecte; otherwise, <c>false</c>.</value>
    public bool AutomaticReconnection { get; set; }

    /// <summary>Gets or sets the value creation mode.</summary>
    /// <value>The dynamic value mode.</value>
    public ValueCreationModes ValueCreation
    {
      get => this._creationMode;
      set => this._creationMode = value;
    }

    /// <summary>Gets or sets the value update mode.</summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader><term>Mode</term><description>Description</description></listheader>
    /// <item><term><see cref="F:TwinCAT.ValueUpdateMode.Immediately" /></term><description>Writes the values of this <see cref="T:TwinCAT.TypeSystem.DynamicValue" />"/&gt; instantly when setting its value or the value
    /// of its child members/elements.</description></item>
    /// <item><term><see cref="F:TwinCAT.ValueUpdateMode.Triggered" /></term><description>Caches internally the value of this <see cref="T:TwinCAT.TypeSystem.DynamicValue" />"/&gt; until the <see cref="M:TwinCAT.TypeSystem.DynamicValue.Write" /> method is called. This reduces
    /// ADS rountrips, if one or more member/element values should be changed. Furthermore the write on the destination system happens
    /// consistently in one ADS Write operation, which could be important for dependent properties/members/elements.</description></item>
    /// </list>
    /// </remarks>
    /// <value>The value update mode.</value>
    public ValueUpdateMode ValueUpdateMode
    {
      get => this._valueUpdateMode;
      set => this._valueUpdateMode = value;
    }

    /// <summary>
    /// Gets the default settings object for standard symbols.
    /// </summary>
    /// <remarks>
    /// The following defaults are set here:
    /// <list type="table">
    /// <listheader><term>Setting</term><description>Description</description></listheader>
    ///     <item><term>Symbols load mode (<see cref="P:TwinCAT.SymbolLoaderSettings.SymbolsLoadMode" />)</term><description>Create virtual tree (<see cref="F:TwinCAT.SymbolsLoadMode.VirtualTree" />.</description></item>
    ///     <item><term>Value access mode (<see cref="P:TwinCAT.SymbolLoaderSettings.ValueAccessMode" />)</term><description>Prefer Symbolic access of values (<see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.Symbolic" />).</description></item>
    ///     <item><term>Value creation mode <see cref="P:TwinCAT.SymbolLoaderSettings.ValueCreation" /></term><description>Create .NET integral primitives if possible (<see cref="F:TwinCAT.ValueAccess.ValueCreationModes.Default" />.</description></item>
    /// </list>
    /// </remarks>
    /// <value>The default settings object.</value>
    /// <seealso cref="P:TwinCAT.SymbolLoaderSettings.DefaultDynamic" />
    public static SymbolLoaderSettings Default => new SymbolLoaderSettings((SymbolsLoadMode) 1, ValueAccessMode.Symbolic);

    /// <summary>Gets the default settings object for Dynamic symbols.</summary>
    /// <remarks>
    /// The following defaults are set here:
    /// <list type="table">
    /// <listheader><term>Setting</term><description>Description</description></listheader>
    ///     <item><term>Symbols load mode (<see cref="P:TwinCAT.SymbolLoaderSettings.SymbolsLoadMode" />)</term><description>Create dynamic tree (<see cref="F:TwinCAT.SymbolsLoadMode.DynamicTree" />.</description></item>
    ///     <item><term>Value access mode (<see cref="P:TwinCAT.SymbolLoaderSettings.ValueAccessMode" />)</term><description>Prefer Symbolic access of values (<see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.Symbolic" />).</description></item>
    ///     <item><term>Value creation mode <see cref="P:TwinCAT.SymbolLoaderSettings.ValueCreation" /></term><description>Create .NET integral primitives if possible (<see cref="F:TwinCAT.ValueAccess.ValueCreationModes.Default" />.</description></item>
    /// </list>
    /// </remarks>
    /// <value>The dynamic default settings object.</value>
    /// <seealso cref="P:TwinCAT.SymbolLoaderSettings.Default" />
    public static SymbolLoaderSettings DefaultDynamic => new SymbolLoaderSettings((SymbolsLoadMode) 2, (ValueCreationModes) 1, ValueAccessMode.Symbolic)
    {
      ValueUpdateMode = (ValueUpdateMode) 1
    };
  }
}
