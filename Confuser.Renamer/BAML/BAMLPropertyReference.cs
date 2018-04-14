﻿using Confuser.Core;
using System;
using System.Diagnostics;

namespace Confuser.Renamer.BAML
{
    internal class BAMLPropertyReference : IBAMLReference
    {
        private PropertyRecord rec;

        public BAMLPropertyReference(PropertyRecord rec)
        {
            this.rec = rec;
        }

        public bool CanRename(string oldName, string newName)
        {
            return true;
        }

        public void Rename(string oldName, string newName)
        {
            var value = rec.Value;
            if (value.IndexOf(oldName, StringComparison.OrdinalIgnoreCase) != -1)
                value = newName;
            else if (oldName.EndsWith(".baml"))
            {
                Debug.Assert(newName.EndsWith(".baml"));
                value = newName.Substring(0, newName.Length - 5) + ".xaml";
            }
            else
                throw new UnreachableException();
            rec.Value = "pack://application:,,,/" + value;
        }
    }
}