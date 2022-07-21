// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.HandleTable
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class SymbolTable.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class HandleTable : IHandleTable, IDisposable, IAdsResurrectHandles
  {
    private ConcurrentDictionary<string, uint> _symbolPathTable;
    private ConcurrentDictionary<uint, HandleTable.SymbolEntry> _symbolTable;
    private StringMarshaler _symbolNameMarshaler = StringMarshaler.Default;
    private IAdsReadWrite _syncPort;
    private bool _disposed = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.HandleTable" /> class.
    /// </summary>
    /// <param name="syncPort">The synchronize port.</param>
    /// <param name="symbolEncoding">The symbol encoding or NULL (StringMarshaler.Default by default).</param>
    public HandleTable(IAdsReadWrite syncPort, Encoding? symbolEncoding)
    {
      if (symbolEncoding != null)
        this._symbolNameMarshaler = new StringMarshaler(symbolEncoding, (StringConvertMode) 1);
      this._symbolTable = new ConcurrentDictionary<uint, HandleTable.SymbolEntry>();
      this._symbolPathTable = new ConcurrentDictionary<string, uint>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this._syncPort = syncPort;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.Internal.HandleTable" /> class.
    /// </summary>
    ~HandleTable() => this.Dispose(false);

    /// <summary>Disposes this instance.</summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
        this.RemoveAll();
      this._disposed = true;
    }

    /// <summary>Tries to create a variable handle</summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="clientHandle">The handle.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryCreateVariableHandle(
      string variableName,
      int timeout,
      out uint clientHandle)
    {
      AdsErrorCode variableHandle = (AdsErrorCode) 0;
      clientHandle = 0U;
      bool flag = this._symbolPathTable.TryGetValue(variableName, out clientHandle);
      uint num1 = clientHandle;
      if (!flag)
      {
        byte[] array1 = new byte[this._symbolNameMarshaler.MarshalSize(variableName)];
        this._symbolNameMarshaler.Marshal(variableName, array1.AsSpan<byte>());
        byte[] array2 = new byte[4];
        int num2 = 0;
        variableHandle = this._syncPort.TryReadWrite(61443U, 0U, array2.AsMemory<byte>(), (ReadOnlyMemory<byte>) array1.AsMemory<byte>(), ref num2);
        if (AdsErrorCodeExtensions.Succeeded(variableHandle))
          num1 = BitConverter.ToUInt32(array2, 0);
      }
      if (AdsErrorCodeExtensions.Succeeded(variableHandle))
      {
        clientHandle = num1;
        if (!flag)
          this._symbolPathTable.TryAdd(variableName, clientHandle);
        Interlocked.Increment(ref this._symbolTable.GetOrAdd(clientHandle, new HandleTable.SymbolEntry(clientHandle, variableName)).referenceCount);
      }
      return variableHandle;
    }

    /// <summary>Creates a variable handle (asynch)</summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultHandle&gt;.</returns>
    public async Task<ResultHandle> CreateVariableHandleAsync(
      string variableName,
      CancellationToken cancel)
    {
      ResultHandle resultHandle = ResultHandle.Empty;
      uint handle = 0;
      bool contained = this._symbolPathTable.TryGetValue(variableName, out handle);
      if (!contained)
      {
        byte[] array = new byte[this._symbolNameMarshaler.MarshalSize(variableName)];
        this._symbolNameMarshaler.Marshal(variableName, array.AsSpan<byte>());
        byte[] readBytes = new byte[4];
        ResultReadWrite resultReadWrite = await this._syncPort.ReadWriteAsync(61443U, 0U, readBytes.AsMemory<byte>(), (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel).ConfigureAwait(false);
        if (((ResultAds) resultReadWrite).ErrorCode == null)
        {
          handle = BitConverter.ToUInt32(readBytes, 0);
          resultHandle = ResultHandle.CreateSuccess(handle, ((ResultAds) resultReadWrite).InvokeId);
        }
        else
          resultHandle = ResultHandle.CreateError(((ResultAds) resultReadWrite).ErrorCode, ((ResultAds) resultReadWrite).InvokeId);
        readBytes = (byte[]) null;
      }
      ResultHandle variableHandleAsync;
      if (((ResultAds) resultHandle).ErrorCode == null)
      {
        if (!contained)
          this._symbolPathTable.TryAdd(variableName, handle);
        Interlocked.Increment(ref this._symbolTable.GetOrAdd(handle, new HandleTable.SymbolEntry(handle, variableName)).referenceCount);
        variableHandleAsync = ResultHandle.CreateSuccess(handle, ((ResultAds) resultHandle).InvokeId);
      }
      else
        variableHandleAsync = ResultHandle.CreateError(((ResultAds) resultHandle).ErrorCode, ((ResultAds) resultHandle).InvokeId);
      return variableHandleAsync;
    }

    /// <summary>Tries to Delete the variable handle</summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryDeleteVariableHandle(uint variableHandle, int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      HandleTable.SymbolEntry symbolEntry = (HandleTable.SymbolEntry) null;
      if (this._symbolTable.TryGetValue(variableHandle, out symbolEntry))
      {
        if (Interlocked.Decrement(ref symbolEntry.referenceCount) == 0)
        {
          adsErrorCode = this._syncPort.TryWrite(61446U, 0U, (ReadOnlyMemory<byte>) BitConverter.GetBytes(symbolEntry.serverHandle).AsMemory<byte>());
          this._symbolTable.TryRemove(variableHandle, out symbolEntry);
          uint maxValue = uint.MaxValue;
          this._symbolPathTable.TryRemove(symbolEntry.symbolPath, out maxValue);
        }
      }
      else
        adsErrorCode = (AdsErrorCode) 1808;
      return adsErrorCode;
    }

    /// <summary>Delete variable handle as an asynchronous operation.</summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;TaskResult&gt;.</returns>
    public async Task<ResultAds> DeleteVariableHandleAsync(
      uint variableHandle,
      CancellationToken cancel)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      HandleTable.SymbolEntry symbolEntry = (HandleTable.SymbolEntry) null;
      if (this._symbolTable.TryGetValue(variableHandle, out symbolEntry))
      {
        if (Interlocked.Decrement(ref symbolEntry.referenceCount) == 0)
        {
          adsErrorCode = ((ResultAds) await this._syncPort.WriteAsync(61446U, 0U, (ReadOnlyMemory<byte>) BitConverter.GetBytes(symbolEntry.serverHandle).AsMemory<byte>(), cancel).ConfigureAwait(false)).ErrorCode;
          this._symbolTable.TryRemove(variableHandle, out symbolEntry);
          uint maxValue = uint.MaxValue;
          this._symbolPathTable.TryRemove(symbolEntry.symbolPath, out maxValue);
        }
      }
      else
        adsErrorCode = (AdsErrorCode) 1808;
      return ResultAds.CreateError(adsErrorCode);
    }

    /// <summary>Removes all handles.</summary>
    public void RemoveAll()
    {
      foreach (KeyValuePair<uint, HandleTable.SymbolEntry> keyValuePair in this._symbolTable)
        this._syncPort.TryWrite(61446U, 0U, (ReadOnlyMemory<byte>) BitConverter.GetBytes(keyValuePair.Value.serverHandle).AsMemory<byte>());
      this._symbolTable.Clear();
      this._symbolPathTable.Clear();
    }

    /// <summary>Remove all handles as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task RemoveAllAsync(CancellationToken cancel)
    {
      foreach (KeyValuePair<uint, HandleTable.SymbolEntry> keyValuePair in this._symbolTable)
      {
        ResultWrite resultWrite = await this._syncPort.WriteAsync(61446U, 0U, (ReadOnlyMemory<byte>) BitConverter.GetBytes(keyValuePair.Value.serverHandle).AsMemory<byte>(), cancel).ConfigureAwait(false);
      }
      this._symbolTable.Clear();
      this._symbolPathTable.Clear();
    }

    /// <summary>Resurrects this instance.</summary>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode Resurrect()
    {
      foreach (uint key in (IEnumerable<uint>) this._symbolTable.Keys)
        this.Resurrect(key);
      return (AdsErrorCode) 0;
    }

    /// <summary>Resurrects this instance the asynchronously.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    public async Task<ResultAds> ResurrectAsync(CancellationToken cancel)
    {
      foreach (uint key in (IEnumerable<uint>) this._symbolTable.Keys)
      {
        ResultAds resultAds = await this.ResurrectAsync(cancel).ConfigureAwait(false);
      }
      return ResultAds.CreateSuccess();
    }

    /// <summary>Resurrects this instance.</summary>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode Resurrect(uint clientHandle)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 1809;
      int timeout = 5000;
      HandleTable.SymbolEntry symbolEntry = (HandleTable.SymbolEntry) null;
      if (this._symbolTable.TryRemove(clientHandle, out symbolEntry))
      {
        adsErrorCode = this._syncPort.TryWrite(61446U, 0U, (ReadOnlyMemory<byte>) BitConverter.GetBytes(symbolEntry.serverHandle).AsMemory<byte>());
        if (adsErrorCode == null || adsErrorCode == 1809)
        {
          this._symbolPathTable.TryRemove(symbolEntry.symbolPath, out uint _);
          adsErrorCode = this.TryCreateVariableHandle(symbolEntry.symbolPath, timeout, out uint _);
        }
      }
      return adsErrorCode;
    }

    /// <summary>Resurrects this instance the asynchronously.</summary>
    /// <param name="clientHandle">The client handle.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    public async Task<ResultAds> ResurrectAsync(
      uint clientHandle,
      CancellationToken cancel)
    {
      HandleTable.SymbolEntry entry = (HandleTable.SymbolEntry) null;
      if (!this._symbolTable.TryRemove(clientHandle, out entry))
        return ResultAds.CreateError((AdsErrorCode) 1809);
      ResultAds resultAds = (ResultAds) await this._syncPort.WriteAsync(61446U, 0U, (ReadOnlyMemory<byte>) BitConverter.GetBytes(entry.serverHandle).AsMemory<byte>(), cancel).ConfigureAwait(false);
      if (resultAds.ErrorCode == null || resultAds.ErrorCode == 1809)
      {
        this._symbolPathTable.TryRemove(entry.symbolPath, out uint _);
        resultAds = (ResultAds) await this.CreateVariableHandleAsync(entry.symbolPath, cancel).ConfigureAwait(false);
        uint handle = ((ResultHandle) resultAds).Handle;
      }
      return resultAds;
    }

    private class SymbolEntry
    {
      public uint serverHandle;
      public int referenceCount;
      public string symbolPath;

      public SymbolEntry(uint serverHandle, string symbolPath)
      {
        this.serverHandle = serverHandle;
        this.symbolPath = symbolPath;
        this.referenceCount = 0;
      }
    }
  }
}
