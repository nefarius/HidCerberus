using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HidCerberus.Srv.Util;
using PInvoke;

namespace HidCerberus.Srv.Core
{
    public delegate void OpenPermissionRequestedEventHandler(object sender, OpenPermissionRequestedEventArgs args);

    public class OpenPermissionRequestedEventArgs : EventArgs
    {
        public bool IsAllowed { get; set; }
    }

    public partial class HidGuardianControlDevice : IDisposable
    {
        public static string DevicePath => "\\\\.\\HidGuardian";

        private readonly Kernel32.SafeObjectHandle _deviceHandle;
        private readonly CancellationTokenSource _invertedCallTokenSource = new CancellationTokenSource();
        private readonly Random _randGen = new Random();

        public HidGuardianControlDevice()
        {
            _deviceHandle = Kernel32.CreateFile(DevicePath,
                Kernel32.ACCESS_MASK.GenericRight.GENERIC_READ | Kernel32.ACCESS_MASK.GenericRight.GENERIC_WRITE,
                Kernel32.FileShare.FILE_SHARE_READ | Kernel32.FileShare.FILE_SHARE_WRITE,
                IntPtr.Zero, Kernel32.CreationDisposition.OPEN_EXISTING,
                Kernel32.CreateFileFlags.FILE_ATTRIBUTE_NORMAL
                | Kernel32.CreateFileFlags.FILE_FLAG_NO_BUFFERING
                | Kernel32.CreateFileFlags.FILE_FLAG_WRITE_THROUGH
                | Kernel32.CreateFileFlags.FILE_FLAG_OVERLAPPED,
                Kernel32.SafeObjectHandle.Null
            );

            if (_deviceHandle.IsInvalid)
                throw new ArgumentException($"Couldn't open device {DevicePath}");

            for (int i = 0; i < 20; i++)
            {
                Task.Factory.StartNew(InvertedCallSupplierWorker, _invertedCallTokenSource.Token);
            }
        }

        private void InvertedCallSupplierWorker(object cancellationToken)
        {
            var token = (CancellationToken)cancellationToken;
            var requestSize = Marshal.SizeOf<HidGuardianGetCreateRequest>();
            var requestBuffer = Marshal.AllocHGlobal(requestSize);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var requestId = (uint) _randGen.Next();

                    Marshal.StructureToPtr(
                        new HidGuardianGetCreateRequest
                        {
                            RequestId = requestId
                        }, 
                        requestBuffer, false);

                    var ret = _deviceHandle.OverlappedDeviceIoControl(
                        IoctlHidguardianGetCreateRequest,
                        requestBuffer,
                        requestSize,
                        requestBuffer,
                        requestSize,
                        out var _);

                    var request = Marshal.PtrToStructure<HidGuardianGetCreateRequest>(requestBuffer);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(requestBuffer);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                _deviceHandle?.Close();

                disposedValue = true;
            }
        }

        ~HidGuardianControlDevice() {
          // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
          Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);            
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}