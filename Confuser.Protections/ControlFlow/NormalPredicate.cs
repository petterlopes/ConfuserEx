﻿using dnlib.DotNet.Emit;
using System.Collections.Generic;

namespace Confuser.Protections.ControlFlow
{
    internal class NormalPredicate : IPredicate
    {
        private readonly CFContext ctx;
        private bool inited;
        private int xorKey;

        public NormalPredicate(CFContext ctx)
        {
            this.ctx = ctx;
        }

        public void Init(CilBody body)
        {
            if (inited)
                return;

            xorKey = ctx.Random.NextInt32();
            inited = true;
        }

        public void EmitSwitchLoad(IList<Instruction> instrs)
        {
            instrs.Add(Instruction.Create(OpCodes.Ldc_I4, xorKey));
            instrs.Add(Instruction.Create(OpCodes.Xor));
        }

        public int GetSwitchKey(int key)
        {
            return key ^ xorKey;
        }
    }
}