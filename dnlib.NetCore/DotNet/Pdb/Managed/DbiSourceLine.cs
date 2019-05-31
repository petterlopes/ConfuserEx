// dnlib: See LICENSE.txt for more info

namespace dnlib.NetCore.DotNet.Pdb.Managed
{
    internal struct DbiSourceLine
    {
        public DbiDocument Document;
        public uint Offset;
        public uint LineBegin;
        public uint LineEnd;
        public uint ColumnBegin;
        public uint ColumnEnd;
    }
}