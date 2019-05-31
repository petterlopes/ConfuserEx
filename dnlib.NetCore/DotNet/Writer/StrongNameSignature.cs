// dnlib: See LICENSE.txt for more info

using dnlib.NetCore.IO;
using dnlib.NetCore.PE;
using System.IO;

namespace dnlib.NetCore.DotNet.Writer
{
    /// <summary>
    /// Strong name signature chunk
    /// </summary>
    public sealed class StrongNameSignature : IChunk
    {
        private FileOffset offset;
        private RVA rva;
        private int size;

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of strong name signature</param>
        public StrongNameSignature(int size)
        {
            this.size = size;
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
            return (uint)this.size;
        }

        /// <inheritdoc/>
        public uint GetVirtualSize()
        {
            return GetFileLength();
        }

        /// <inheritdoc/>
        public void WriteTo(BinaryWriter writer)
        {
            writer.WriteZeros(size);
        }
    }
}