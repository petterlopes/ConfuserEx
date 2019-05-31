﻿// dnlib: See LICENSE.txt for more info

using dnlib.NetCore.IO;
using System;

namespace dnlib.NetCore.DotNet.Pdb.Managed
{
    internal sealed class MsfStream
    {
        public MsfStream(IImageStream[] pages, uint length)
        {
            byte[] buf = new byte[length];
            int offset = 0;
            foreach (var page in pages)
            {
                page.Position = 0;
                int len = Math.Min((int)page.Length, (int)(length - offset));
                offset += page.Read(buf, offset, len);
            }
            Content = new MemoryImageStream(0, buf, 0, buf.Length);
        }

        public IImageStream Content { get; set; }
    }
}