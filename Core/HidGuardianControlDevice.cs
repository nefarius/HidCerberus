using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        private readonly Kernel32.SafeObjectHandle _deviceHandle;
        private readonly CancellationTokenSource _invertedCallTokenSource = new CancellationTokenSource();
        private readonly Random _randGen = new Random();

        public HidGuardianControlDevice()
        {
            Console.WriteLine("Ctor");

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

            ThreadPool.SetMinThreads(20, 20);

            for (var i = 0; i < 20; i++)
                Task.Factory.StartNew(InvertedCallSupplierWorker, _invertedCallTokenSource.Token);
        }

        public static string DevicePath => "\\\\.\\HidGuardian";

        public static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }

        private void InvertedCallSupplierWorker(object cancellationToken)
        {
            Console.WriteLine("Spawned thread");

            var token = (CancellationToken) cancellationToken;

            var invertedCallSize = Marshal.SizeOf<HidGuardianGetCreateRequest>();
            var invertedCallBuffer = Marshal.AllocHGlobal(invertedCallSize);

            Console.WriteLine($"invertedCallSize = {invertedCallSize}");

            var authCallSize = Marshal.SizeOf<HidGuardianSetCreateRequest>();
            var authCallBuffer = Marshal.AllocHGlobal(authCallSize);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var requestId = (uint) _randGen.Next();

                    Marshal.StructureToPtr(
                        new HidGuardianGetCreateRequest
                        {
                            Size = (uint) invertedCallSize,
                            RequestId = requestId
                        },
                        invertedCallBuffer, false);

                    var ret = _deviceHandle.OverlappedDeviceIoControl(
                        IoctlHidguardianGetCreateRequest,
                        invertedCallBuffer,
                        invertedCallSize,
                        invertedCallBuffer,
                        invertedCallSize,
                        out var _);

                    var request = Marshal.PtrToStructure<HidGuardianGetCreateRequest>(invertedCallBuffer);

                    Console.WriteLine($"RequestId: {request.RequestId}");
                    Console.WriteLine($"DeviceIndex: {request.DeviceIndex}");
                    Console.WriteLine($"ProcessId: {request.ProcessId}");

                    foreach (var extractHardwareId in ExtractHardwareIds(request))
                    {
                        Console.WriteLine($"HWID: {extractHardwareId}");
                    }

                    Marshal.StructureToPtr(
                        new HidGuardianSetCreateRequest
                        {
                            RequestId = request.RequestId,
                            DeviceIndex = request.DeviceIndex,
                            IsAllowed = true
                        },
                        authCallBuffer, false);

                    ret = _deviceHandle.OverlappedDeviceIoControl(
                        IoctlHidguardianSetCreateRequest,
                        authCallBuffer,
                        authCallSize,
                        authCallBuffer,
                        authCallSize,
                        out var _);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(invertedCallBuffer);
            }
        }

        private static IEnumerable<string> ExtractHardwareIds(HidGuardianGetCreateRequest request)
        {
            var multibyte = Encoding.Unicode.GetString(request.HardwareIds).TrimEnd('\0');

            return Encoding.UTF8.GetBytes(multibyte).Separate(new byte[] {0x00})
                .Select(chunk => Encoding.UTF8.GetString(chunk)).ToList();
        }

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

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

        ~HidGuardianControlDevice()
        {
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