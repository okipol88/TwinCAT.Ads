// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicValue
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Dynamic value (uses RuntimeBinding for <see cref="T:TwinCAT.TypeSystem.ISymbol" /> value reading / writing).
  /// </summary>
  /// <remarks>The <see cref="T:TwinCAT.TypeSystem.DynamicValue" /> adds dynamic run time behaviour to the <see cref="T:TwinCAT.TypeSystem.IValue" />Value/<see cref="T:TwinCAT.TypeSystem.IValue" />.
  /// That means e.g. for struct values that .NET Properties are on-the-fly defined and dispatched at runtime just like defined in the structs
  /// structs data type definition. Another example is the access of Array Element values through indexes.
  /// </remarks>
  /// <example>
  /// Sample for the dynamic resolution of Symbols and reading values:
  /// <code language="C#" title="Dynamic Symbol access" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE_SIMPLEDYNAMIC" />
  /// </example>
  /// <seealso cref="T:TwinCAT.TypeSystem.DynamicSymbol" />
  /// <seealso cref="T:System.Dynamic.DynamicObject" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IValue" />
  public class DynamicValue : 
    DynamicObject,
    IDynamicValue,
    IDynamicMetaObjectProvider,
    IValue,
    IStructValue,
    IArrayValue
  {
    private static Dictionary<string, string>? s_staticProperties;
    private static Dictionary<string, string>? s_staticMethods;
    private static Dictionary<string, string> s_propertyCollisions = new Dictionary<string, string>();
    private static Dictionary<string, string> s_methodCollisions = new Dictionary<string, string>();
    /// <summary>The value factory</summary>
    private IAccessorValueFactory valueFactory;
    /// <summary>
    /// The timestamp of the last successful read of the value.
    /// </summary>
    private DateTimeOffset _timeStamp;
    /// <summary>Symbol that is bound to this value.</summary>
    private ISymbol _symbol;
    private ValueUpdateMode _mode;
    private DynamicValue? _parentValue;
    /// <summary>The cached (raw) data) of the Root Symbol</summary>
    internal byte[]? cachedData;
    private InstanceValueMarshaler s_marshaller = new InstanceValueMarshaler();

    /// <summary>The value factory</summary>
    protected IAccessorValueFactory ValueFactory => this.valueFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicValue" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="data">RawData</param>
    /// <param name="timeStamp">The time stamp (UserMode,</param>
    /// <param name="factory">The factory.</param>
    /// <exception cref="T:System.ArgumentNullException">factory
    /// or
    /// symbol</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
    /// <exception cref="T:System.ArgumentNullException">factory
    /// or
    /// symbol</exception>
    internal DynamicValue(
      ISymbol symbol,
      byte[] data,
      DateTimeOffset timeStamp,
      IAccessorValueFactory factory)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (data.Length != symbol.GetValueMarshalSize())
        throw new ArgumentOutOfRangeException(nameof (data));
      this.valueFactory = factory;
      this._symbol = symbol;
      this._parentValue = (DynamicValue) null;
      this.cachedData = data;
      this._timeStamp = timeStamp;
      this._mode = ((DynamicValueFactory) this.valueFactory).UpdateMode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicValue" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="data">RawData</param>
    /// <param name="parentValue">The parent value.</param>
    /// <exception cref="T:System.ArgumentNullException">parentValue</exception>
    internal DynamicValue(ISymbol symbol, byte[] data, DynamicValue parentValue)
      : this(symbol, data, DateTimeOffset.MinValue, parentValue.valueFactory)
    {
      this._parentValue = parentValue != null ? parentValue : throw new ArgumentNullException(nameof (parentValue));
      this._timeStamp = this._parentValue._timeStamp;
    }

    /// <summary>
    /// Gets the Time stamp of the last successful read of the Value.
    /// </summary>
    /// <value>The read time stamp.</value>
    public DateTimeOffset TimeStamp => this._timeStamp;

    /// <summary>Gets the symbol that is bound to this value.</summary>
    /// <value>The symbol.</value>
    public ISymbol Symbol => this._symbol;

    /// <summary>Gets / Sets the update mode</summary>
    /// <value>The update mode.</value>
    /// <remarks>
    /// The default value is initialized by the creating Value Factory.
    /// <list type="table">
    /// <listheader><term>Mode</term><description>Description</description></listheader>
    /// <item><term><see cref="F:TwinCAT.ValueUpdateMode.Immediately" /></term><description>Writes the values of this <see cref="T:TwinCAT.TypeSystem.DynamicValue" />"/&gt; instantly when setting its value or the value
    /// of its child members/elements.</description></item>
    /// <item><term><see cref="F:TwinCAT.ValueUpdateMode.Triggered" /></term><description>Caches internally the value of this <see cref="T:TwinCAT.TypeSystem.DynamicValue" />"/&gt; until the <see cref="M:TwinCAT.TypeSystem.DynamicValue.Write" /> method is called. This reduces
    /// ADS rountrips, if one or more member/element values should be changed. Furthermore the write on the destination system happens
    /// consistently in one ADS Write operation, which could be important for dependent properties/members/elements.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:TwinCAT.ValueUpdateMode" />
    public ValueUpdateMode UpdateMode
    {
      get => this._mode;
      set => this._mode = value;
    }

    /// <summary>
    /// Byte offset of this value data within the cached data.
    /// </summary>
    private static Dictionary<string, string> CollidingProperties
    {
      get
      {
        if (DynamicValue.s_staticProperties == null)
        {
          DynamicValue.s_staticProperties = new Dictionary<string, string>();
          foreach (PropertyInfo property in typeof (DynamicValue).GetProperties())
            DynamicValue.s_staticProperties.Add(property.Name, property.Name);
        }
        return DynamicValue.s_staticProperties;
      }
    }

    private static Dictionary<string, string> CollidingMethods
    {
      get
      {
        if (DynamicValue.s_staticMethods == null)
        {
          DynamicValue.s_staticMethods = new Dictionary<string, string>();
          foreach (MethodInfo method in typeof (DynamicValue).GetMethods())
          {
            if (!DynamicValue.s_staticMethods.ContainsKey(method.Name))
              DynamicValue.s_staticMethods.Add(method.Name, method.Name);
          }
        }
        return DynamicValue.s_staticMethods;
      }
    }

    private static List<string> createMemberNames(IStructType str)
    {
      List<string> memberNames = new List<string>();
      foreach (IInstance allMember in (IEnumerable<IMember>) ((IInterfaceType) str).AllMembers)
      {
        string instanceName = allMember.InstanceName;
        if (DynamicValue.CollidingMethods.ContainsKey(instanceName))
        {
          memberNames.Add(instanceName + "_1");
          DynamicValue.s_propertyCollisions.Add(instanceName, instanceName + "_1");
        }
        else
          memberNames.Add(instanceName);
      }
      if (((IInterfaceType) str).HasRpcMethods)
      {
        foreach (IRpcMethod rpcMethod in (IEnumerable<IRpcMethod>) ((IRpcCallableType) str).RpcMethods)
        {
          string name = rpcMethod.Name;
          if (DynamicValue.CollidingMethods.ContainsKey(name))
          {
            memberNames.Add(name + "_1");
            DynamicValue.s_methodCollisions.Add(name, name + "_1");
          }
          else
            memberNames.Add(name);
        }
      }
      return memberNames;
    }

    /// <summary>Returns the enumeration of all dynamic member names.</summary>
    /// <returns>A sequence that contains dynamic member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames()
    {
      List<string> dynamicMemberNames = new List<string>();
      IDataType str = ((IInstance) this._symbol).DataType;
      if (this._symbol.Category == 15)
      {
        IResolvableType dataType = (IResolvableType) ((IInstance) this._symbol).DataType;
        if (dataType != null)
          str = dataType.ResolveType((DataTypeResolveStrategy) 1);
      }
      if (str != null && str.Category == 5)
        dynamicMemberNames = DynamicValue.createMemberNames((IStructType) str);
      return (IEnumerable<string>) dynamicMemberNames;
    }

    /// <summary>
    /// Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.
    /// </summary>
    /// <param name="binder">Provides information about the invoke operation.</param>
    /// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args" /> is equal to 100.</param>
    /// <param name="result">The result of the object invocation.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.</returns>
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result) => base.TryInvoke(binder, args, out result);

    /// <summary>Gets the root value.</summary>
    /// <value>The root value.</value>
    /// <remarks>The root value is the value, that is active in terms of ADS communication, the object that requests the data. All subsequent
    /// children are working on the <see cref="P:TwinCAT.TypeSystem.DynamicValue.RootValue" />s cache.</remarks>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DynamicValue RootValue
    {
      get
      {
        DynamicValue rootValue = (DynamicValue) null;
        for (DynamicValue dynamicValue = this; dynamicValue != null; dynamicValue = dynamicValue._parentValue)
          rootValue = dynamicValue;
        return rootValue;
      }
    }

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
    /// </returns>
    public override bool TryGetMember(GetMemberBinder binder, [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      return this.TryGetMemberValue(binder.Name, out result) || base.TryGetMember(binder, out result);
    }

    /// <summary>Tries the get member value.</summary>
    /// <param name="name">The name.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    public virtual bool TryGetMemberValue(string name, [NotNullWhen(true)] out object? result)
    {
      result = (object) null;
      if (this.ResolvedType != null && this.ResolvedType.Category == 5)
      {
        IList<ISymbol> isymbolList = (IList<ISymbol>) null;
        if (DynamicValue.CollidingProperties.ContainsKey(name))
          name = DynamicValue.s_propertyCollisions[name];
        if (((IInstanceCollection<ISymbol>) this._symbol.SubSymbols).TryGetInstanceByName(name, ref isymbolList))
        {
          if (((ICollection<ISymbol>) isymbolList).Count != 1)
          {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 1);
            interpolatedStringHandler.AppendLiteral("Struct Instance members mismatch in StructInstance '");
            interpolatedStringHandler.AppendFormatted<ISymbol>(this.Symbol);
            interpolatedStringHandler.AppendLiteral("'!");
            throw new SymbolException(interpolatedStringHandler.ToStringAndClear(), this.Symbol);
          }
          ISymbol memberInstance = isymbolList[0];
          result = this.ReadMember(memberInstance);
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
    /// </returns>
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      return this.TrySetMemberValue(binder.Name, value) || base.TrySetMember(binder, value);
    }

    /// <summary>Tries to Set a Member/Property Value</summary>
    /// <param name="name">The name of the member</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if succeeded, otherwise <c>false</c> otherwise.</returns>
    public bool TrySetMemberValue(string name, object? value)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      bool flag = false;
      if (((IInstance) this._symbol).DataType != null && ((IInstance) this._symbol).DataType.Category == 5 && this._symbol.Category == 5)
      {
        if (DynamicValue.CollidingProperties.ContainsKey(name))
          name = DynamicValue.s_propertyCollisions[name];
        IStructInstance symbol = (IStructInstance) this._symbol;
        IList<ISymbol> isymbolList = (IList<ISymbol>) null;
        if (((IInstanceCollection<ISymbol>) ((IInterfaceInstance) symbol).MemberInstances).TryGetInstanceByName(name, ref isymbolList))
        {
          ISymbol memberInstance = isymbolList[0];
          flag = true;
          if (memberInstance.IsPrimitiveType)
            this.WriteMember(memberInstance, value);
        }
      }
      return flag;
    }

    /// <summary>
    /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as calling a method.
    /// </summary>
    /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args" />[0] is equal to 100.</param>
    /// <param name="result">The result of the member invocation.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
    /// </returns>
    public override bool TryInvokeMember(
      InvokeMemberBinder binder,
      object?[]? args,
      [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      if (this._symbol.Category != 5 || !(this._symbol is IRpcCallableInstance))
        return base.TryInvokeMember(binder, args, out result);
      IRpcCallableInstance symbol = (IRpcCallableInstance) this._symbol;
      string str = binder.Name;
      if (DynamicValue.CollidingMethods.ContainsKey(binder.Name))
        str = DynamicValue.CollidingMethods[binder.Name];
      object[] objArray = (object[]) null;
      return symbol.TryInvokeRpcMethod(str, args, ref objArray, ref result) == 0;
    }

    /// <summary>
    /// Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for indexing operations.
    /// </summary>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class, <paramref name="indexes" />[0] is equal to 3.</param>
    /// <param name="result">The result of the index operation.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
    /// </returns>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, [NotNullWhen(true)] out object? result) => this.TryGetIndexValue(indexes, out result) || base.TryGetIndex(binder, indexes, out result);

    /// <summary>
    /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
    /// </summary>
    /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Type returns the <see cref="T:System.String" /> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
    /// <param name="result">The result of the type conversion operation.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
    public override bool TryConvert(ConvertBinder binder, [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      result = (object) null;
      if (!this._symbol.IsPrimitiveType)
        return base.TryConvert(binder, out result);
      object primitiveValue = this.valueFactory.CreatePrimitiveValue(this._symbol, (ReadOnlySpan<byte>) this.cachedData, this._timeStamp);
      try
      {
        result = Convert.ChangeType(primitiveValue, binder.Type, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      catch (InvalidCastException ex)
      {
        return false;
      }
      catch (FormatException ex)
      {
        return false;
      }
    }

    /// <summary>Tries the get index value.</summary>
    /// <param name="indexes">The indexes.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetIndexValue(object[] indexes, [NotNullWhen(true)] out object? result)
    {
      if (indexes == null)
        throw new ArgumentNullException(nameof (indexes));
      result = (object) null;
      if (((IInstance) this._symbol).DataType == null || ((IInstance) this._symbol).DataType.Category != 4)
        return false;
      int[] indices = new int[indexes.GetLength(0)];
      for (int index = 0; index < indexes.GetLength(0); ++index)
        indices[index] = (int) indexes[index];
      return this.TryGetIndexValue(indices, out result);
    }

    /// <summary>Reads the specified array element.</summary>
    /// <param name="indices">The indices.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public bool TryGetIndexValue(int[] indices, [NotNullWhen(true)] out object? value)
    {
      IArrayInstance symbol = (IArrayInstance) this._symbol;
      IArrayType dataType = (IArrayType) ((IInstance) symbol).DataType;
      IDataType elementType = dataType?.ElementType;
      if (dataType != null)
      {
        ArrayType.CheckIndices(indices, dataType, false);
        int elementPosition = ArrayType.GetElementPosition(indices, dataType);
        ISymbol subSymbol = ((IList<ISymbol>) ((ISymbol) symbol).SubSymbols)[elementPosition];
        int valueMarshalSize = subSymbol.GetValueMarshalSize();
        if (valueMarshalSize <= 0)
        {
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 2);
          interpolatedStringHandler.AppendLiteral("Cannot determine size of array type '");
          interpolatedStringHandler.AppendFormatted(((IDataType) dataType).Name);
          interpolatedStringHandler.AppendLiteral("' element type '");
          interpolatedStringHandler.AppendFormatted(elementType?.Name);
          interpolatedStringHandler.AppendLiteral("'");
          throw new MarshalException(interpolatedStringHandler.ToStringAndClear());
        }
        int start = elementPosition * valueMarshalSize;
        value = this.valueFactory.CreateValue(subSymbol, (ReadOnlySpan<byte>) this.cachedData.AsSpan<byte>(start, valueMarshalSize), (IValue) this, this._timeStamp);
        return true;
      }
      value = (object) null;
      return false;
    }

    /// <summary>Reads the specified member element.</summary>
    /// <param name="memberInstance">The member instance.</param>
    /// <returns></returns>
    protected internal virtual object ReadMember(ISymbol memberInstance)
    {
      if (memberInstance == null)
        throw new ArgumentNullException(nameof (memberInstance));
      IMember member = ((IInstanceCollection<IMember>) ((IInterfaceType) ((IStructType) this.ResolvedType ?? throw new CannotResolveDataTypeException(((IInstance) this.Symbol).TypeName))).Members)[((IInstance) memberInstance).InstanceName];
      IDataType dataType = ((IInstance) member).DataType;
      return !((IInstance) memberInstance).IsReference ? this.valueFactory.CreateValue(memberInstance, (ReadOnlySpan<byte>) this.cachedData.AsSpan<byte>(member.Offset, ((IBitSize) memberInstance).ByteSize), (IValue) this, this._timeStamp) : this.valueFactory.CreateValue(memberInstance, (ReadOnlySpan<byte>) new byte[memberInstance.GetValueMarshalSize()], (IValue) this, this._timeStamp);
    }

    /// <summary>Writes the specified member element.</summary>
    /// <param name="memberInstance">The member instance.</param>
    /// <param name="value">The value.</param>
    protected virtual void WriteMember(ISymbol memberInstance, object? value)
    {
      if (memberInstance == null)
        throw new ArgumentNullException(nameof (memberInstance));
      IMember member = ((IInstanceCollection<IMember>) ((IInterfaceType) ((IStructType) this.ResolvedType ?? throw new CannotResolveDataTypeException(((IInstance) this.Symbol).TypeName))).Members)[((IInstance) memberInstance).InstanceName];
      IDataType dataType = ((IInstance) member).DataType;
      if (dataType != null && dataType.IsPrimitive)
      {
        this.s_marshaller.TypeMarshaller.Marshal(dataType, ((IAttributedInstance) this._symbol).ValueEncoding, value, this.cachedData.AsSpan<byte>().Slice(member.Offset, ((IBitSize) memberInstance).Size));
      }
      else
      {
        if (!(value is DynamicValue dynamicValue) || dynamicValue.cachedData == null)
          throw new AdsException();
        dynamicValue.cachedData.CopyTo((Array) this.cachedData, member.Offset);
      }
      if (this._mode != 1)
        return;
      this.RootValue.Write();
    }

    /// <summary>
    /// Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations that access objects by a specified index.
    /// </summary>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="indexes" />[0] is equal to 3.</param>
    /// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="value" /> is equal to 10.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
    /// </returns>
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) => this.TrySetIndexValue(indexes, value) || base.TrySetIndex(binder, indexes, value);

    /// <summary>Writes the array element value into the cache.</summary>
    /// <param name="indices">The indices.</param>
    /// <param name="value">The value.</param>
    private bool TryWriteArrayElementCached(int[] indices, object? value)
    {
      IArrayInstance symbol = (IArrayInstance) this._symbol;
      IArrayType dataType = (IArrayType) ((IInstance) symbol).DataType;
      if (dataType == null || !ArrayType.TryCheckIndices(indices, dataType, false))
        return false;
      int elementPosition = ArrayType.GetElementPosition(indices, dataType);
      ISymbol subSymbol = ((IList<ISymbol>) ((ISymbol) symbol).SubSymbols)[elementPosition];
      int valueMarshalSize = subSymbol.GetValueMarshalSize();
      int start = elementPosition * valueMarshalSize;
      this.s_marshaller.Marshal((IAttributedInstance) subSymbol, value, this.cachedData.AsSpan<byte>().Slice(start, valueMarshalSize));
      return true;
    }

    /// <summary>Tries to set the indexed value on Arrays</summary>
    /// <param name="indexes">The indexes.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if succeeded, <c>false</c> otherwise.</returns>
    public bool TrySetIndexValue(object[] indexes, object? value)
    {
      if (indexes == null)
        throw new ArgumentNullException(nameof (indexes));
      if (((IInstance) this._symbol).DataType == null || ((IInstance) this._symbol).DataType.Category != 4)
        return false;
      IDataType elementType = ((IArrayType) ((IInstance) this._symbol).DataType)?.ElementType;
      int[] indices = new int[indexes.GetLength(0)];
      for (int index = 0; index < indexes.GetLength(0); ++index)
        indices[index] = (int) indexes[index];
      return this.TryWriteArrayElementCached(indices, value);
    }

    private IAccessorDynamicValue? ValueAccessor => ((IValueAccessorProvider) this._symbol).ValueAccessor as IAccessorDynamicValue;

    /// <summary>Writes the value (via ADS)</summary>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    public void Write()
    {
      DateTimeOffset? nullable;
      int num = this.ValueAccessor != null ? this.ValueAccessor.TryWriteValue((IDynamicValue) this, ref nullable) : throw new CannotAccessValueException(this.Symbol);
      if (num != 0)
        throw new SymbolException(this.Symbol, num);
      this._timeStamp = nullable.Value;
    }

    /// <summary>Reads the value (via ADS)</summary>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    public void Read()
    {
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException(this.Symbol);
      DateTimeOffset? nullable;
      int num = ((IAccessorRawValue) this.ValueAccessor).TryReadRaw(this._symbol, (Memory<byte>) this.cachedData, ref nullable);
      if (num != 0)
        throw new SymbolException(this.Symbol, num);
      this._timeStamp = nullable.Value;
    }

    /// <summary>write as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;WriteValueResult&gt;.</returns>
    public async Task<ResultWriteAccess> WriteAsync(CancellationToken cancel)
    {
      DynamicValue dynamicValue = this;
      if (dynamicValue.ValueAccessor == null)
      {
        // ISSUE: explicit non-virtual call
        throw new CannotAccessValueException(__nonvirtual (dynamicValue.Symbol));
      }
      ResultWriteAccess resultWriteAccess = await dynamicValue.ValueAccessor.WriteValueAsync((IDynamicValue) dynamicValue, cancel).ConfigureAwait(false);
      if (((ResultAccess) resultWriteAccess).Succeeded)
        dynamicValue._timeStamp = ((ResultAccess) resultWriteAccess).DateTime;
      return resultWriteAccess;
    }

    /// <summary>read as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ReadValueResult&gt;.</returns>
    public async Task<ResultAccess> ReadAsync(CancellationToken cancel)
    {
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException(this.Symbol);
      ResultReadRawAccess resultReadRawAccess = await ((IAccessorRawValue) this.ValueAccessor).ReadRawAsync(this._symbol, (Memory<byte>) this.cachedData, cancel).ConfigureAwait(false);
      if (((ResultAccess) resultReadRawAccess).Succeeded)
        this._timeStamp = ((ResultAccess) resultReadRawAccess).DateTime;
      return (ResultAccess) resultReadRawAccess;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      IDataType resolvedType = this.ResolvedType;
      if (resolvedType == null)
        return "[NOTRESOLVED]";
      if (resolvedType.Category == 5)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("...");
        return stringBuilder.ToString();
      }
      if (resolvedType.Category == 4)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("...");
        return stringBuilder.ToString();
      }
      return resolvedType.IsPrimitive ? this.ResolveValue(false).ToString() : "[INVALID]";
    }

    /// <summary>
    /// Gets the age of the value (last successful read of the value)
    /// </summary>
    /// <value>The age.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <seealso cref="P:TwinCAT.TypeSystem.DynamicValue.TimeStamp" />
    public TimeSpan Age => DateTimeOffset.Now - this.TimeStamp;

    /// <summary>
    /// Gets the data type bound to this <see cref="T:TwinCAT.TypeSystem.IValue" />
    /// </summary>
    /// <value>The type of the data.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public IDataType? DataType => ((IInstance) this._symbol).DataType;

    /// <summary>Gets the cached Raw internal Data.</summary>
    /// <value>The raw cached data.</value>
    public byte[] CachedRaw
    {
      get
      {
        IDataType resolvedType = this.ResolvedType;
        int num = 0;
        if (resolvedType != null)
          num = ((IBitSize) resolvedType).ByteSize;
        return num < 0 ? ((IEnumerable<byte>) this.cachedData).ToArray<byte>() : ((IEnumerable<byte>) this.cachedData).ToArray<byte>();
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IValue" /> is a primitive value.
    /// </summary>
    /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
    public bool IsPrimitive => this.ResolvedType != null && this.ResolvedType.IsPrimitive;

    /// <summary>Gets the resolved type.</summary>
    /// <value>Resolved type.</value>
    protected IDataType? ResolvedType
    {
      get
      {
        IDataType resolvedType = this.DataType;
        if (this.DataType is IResolvableType dataType)
          resolvedType = dataType.ResolveType((DataTypeResolveStrategy) 1);
        return resolvedType;
      }
    }

    /// <summary>Resolves the Value object to its primitive value.</summary>
    /// <param name="resolveEnumToPrimitive">if set to <c>true</c> <see cref="T:TwinCAT.TypeSystem.IEnumValue" />s are resolved to their primitives also.</param>
    /// <returns>System.Object.</returns>
    /// <remarks>If the value is not primitive, this method returns the <see cref="T:TwinCAT.TypeSystem.IValue" /> itself.</remarks>
    public object ResolveValue(bool resolveEnumToPrimitive)
    {
      object obj = (object) null;
      if (!this.TryResolveValue(resolveEnumToPrimitive, out obj))
        throw new AdsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot resolve type '{0}' to primitive value!", (object) ((IInstance) this.Symbol).TypeName));
      return obj;
    }

    /// <summary>
    /// Tries to resolves the Value object to its primitive value.
    /// </summary>
    /// <param name="resolveEnumToPrimitive">if set to <c>true</c> <see cref="T:TwinCAT.TypeSystem.IEnumValue" />s are resolved to their primitives also.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if value can be resolved, <c>false</c> otherwise.</returns>
    public bool TryResolveValue(bool resolveEnumToPrimitive, [NotNullWhen(true)] out object? value)
    {
      IDataType resolvedType = this.ResolvedType;
      if (resolvedType != null && resolvedType.Category == 3)
      {
        IEnumValue ienumValue = EnumValueFactory.Create((IEnumType) resolvedType, (ReadOnlySpan<byte>) this.cachedData.AsSpan<byte>());
        value = !resolveEnumToPrimitive ? (object) ienumValue : ienumValue.Primitive;
        return true;
      }
      if (resolvedType != null && resolvedType.IsPrimitive)
      {
        InstanceValueMarshaler marshaller = this.s_marshaller;
        ISymbol symbol = this.Symbol;
        ReadOnlySpan<byte> source = (ReadOnlySpan<byte>) this.cachedData.AsSpan<byte>();
        Type valueType = resolvedType.GetManagedType();
        if ((object) valueType == null)
          valueType = typeof (byte[]);
        ref object local = ref value;
        marshaller.Unmarshal((IAttributedInstance) symbol, source, valueType, out local);
        return true;
      }
      value = (object) null;
      return false;
    }

    /// <summary>Returns Array Element values.</summary>
    /// <param name="elementValues">The element values.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetArrayElementValues([NotNullWhen(true)] out IEnumerable<object>? elementValues)
    {
      IDataType resolvedType = this.ResolvedType;
      if (resolvedType != null && resolvedType.Category == 4)
      {
        ArrayElementValueIterator elementValueIterator = new ArrayElementValueIterator((IArrayValue) this);
        elementValues = (IEnumerable<object>) elementValueIterator;
        return true;
      }
      elementValues = (IEnumerable<object>) null;
      return false;
    }
  }
}
