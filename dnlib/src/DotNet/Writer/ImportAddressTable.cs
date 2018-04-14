// dnlib: See LICENSE.txt for more info

using dnlib.IO;
using dnlib.PE;
using System.IO;

namespace dnlib.DotNet.Writer
{
    /// <summary>
    /// Import address table chunk
    /// </summary>
    public sealed class ImportAddressTable : IChunk
    {
        private FileOffset offset;
        private RVA rva;

        /// <summary>
        /// Gets/sets the <see cref="ImportDirectory"/>
        /// </summary>
        public ImportDirectory ImportDirectory { get; set; }

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
            return 8;
        }

        /// <inheritdoc/>
        public uint GetVirtualSize()
        {
            return GetFileLength();
        }

        /// <inheritdoc/>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((uint)ImportDirectory.CorXxxMainRVA);
            writer.Write(0);
        }
    }
}