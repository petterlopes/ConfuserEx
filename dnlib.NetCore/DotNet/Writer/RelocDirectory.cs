// dnlib: See LICENSE.txt for more info

using dnlib.NetCore.IO;
using dnlib.NetCore.PE;
using System.IO;

namespace dnlib.NetCore.DotNet.Writer
{
    /// <summary>
    /// Relocations directory
    /// </summary>
    public sealed class RelocDirectory : IChunk
    {
        private FileOffset offset;
        private RVA rva;

        /// <summary>
        /// Gets/sets the <see cref="StartupStub"/>
        /// </summary>
        public StartupStub StartupStub { get; set; }

        /// <inheritdoc/>
        public FileOffset FileOffset
        {
            get { return offset; }
        }

        /// <inheritdoc/>
        public RVA RVA
        {
            get { return rva; }
        }

        /// <inheritdoc/>
        public void SetOffset(FileOffset offset, RVA rva)
        {
            this.offset = offset;
            this.rva = rva;
        }

        /// <inheritdoc/>
        public uint GetFileLength()
        {
            return 12;
        }

        /// <inheritdoc/>
        public uint GetVirtualSize()
        {
            return GetFileLength();
        }

        /// <inheritdoc/>
        public void WriteTo(BinaryWriter writer)
        {
            uint rva = (uint)StartupStub.RelocRVA;
            writer.Write(rva & ~0xFFFU);
            writer.Write(12);
            writer.Write((ushort)(0x3000 | (rva & 0xFFF)));
            writer.Write((ushort)0);
        }
    }
}