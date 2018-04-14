﻿// dnlib: See LICENSE.txt for more info

using dnlib.IO;
using System;
using System.Runtime.InteropServices;

namespace dnlib.DotNet.Pdb.Dss
{
    /// <summary>
    /// Pins a metadata stream in memory
    /// </summary>
    internal sealed class PinnedMetaData : IDisposable
    {
        private GCHandle gcHandle;
        private readonly IImageStream stream;
        private readonly byte[] streamData;
        private readonly IntPtr address;

        /// <summary>
        /// Gets the address
        /// </summary>
        public IntPtr Address
        {
            get { return address; }
        }

        /// <summary>
        /// Gets the size
        /// </summary>
        public int Size
        {
            get { return (int)stream.Length; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Metadata stream</param>
        public PinnedMetaData(IImageStream stream)
        {
            this.stream = stream;

            var umStream = stream as UnmanagedMemoryImageStream;
            if (umStream != null)
            {
                this.address = umStream.StartAddress;
                GC.SuppressFinalize(this);  // no GCHandle so finalizer isn't needed
            }
            else
            {
                var memStream = stream as MemoryImageStream;
                if (memStream != null)
                {
                    this.streamData = memStream.DataArray;
                    this.gcHandle = GCHandle.Alloc(this.streamData, GCHandleType.Pinned);
                    this.address = new IntPtr(this.gcHandle.AddrOfPinnedObject().ToInt64() + memStream.DataOffset);
                }
                else
                {
                    this.streamData = stream.ReadAllBytes();
                    this.gcHandle = GCHandle.Alloc(this.streamData, GCHandleType.Pinned);
                    this.address = this.gcHandle.AddrOfPinnedObject();
                }
            }
        }

        ~PinnedMetaData()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (gcHandle.IsAllocated)
            {
                try
                {
                    gcHandle.Free();
                }
                catch (InvalidOperationException)
                {
                }
            }
            if (disposing)
                stream.Dispose();
        }
    }
}