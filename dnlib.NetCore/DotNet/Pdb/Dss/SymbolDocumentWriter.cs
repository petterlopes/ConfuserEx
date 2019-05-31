// dnlib: See LICENSE.txt for more info

using System;
using System.Diagnostics.SymbolStore;

namespace dnlib.NetCore.DotNet.Pdb.Dss
{
    internal sealed class SymbolDocumentWriter : ISymbolDocumentWriter
    {
        private readonly ISymUnmanagedDocumentWriter writer;

        public ISymUnmanagedDocumentWriter SymUnmanagedDocumentWriter
        {
            get { return writer; }
        }

        public SymbolDocumentWriter(ISymUnmanagedDocumentWriter writer)
        {
            this.writer = writer;
        }

        public void SetCheckSum(Guid algorithmId, byte[] checkSum)
        {
            writer.SetCheckSum(algorithmId, (uint)(checkSum == null ? 0 : checkSum.Length), checkSum);
        }

        public void SetSource(byte[] source)
        {
            writer.SetSource((uint)source.Length, source);
        }
    }
}