// dnlib: See LICENSE.txt for more info

using System.Diagnostics;
using dnlib.NetCore.DotNet.Emit;
using dnlib.NetCore.Threading;

#if THREAD_SAFE
using ThreadSafe = dnlib.NetCore.Threading.Collections;
#else

using ThreadSafe = System.Collections.Generic;

#endif

namespace dnlib.NetCore.DotNet.Pdb
{
    /// <summary>
    /// A PDB scope
    /// </summary>
    [DebuggerDisplay("{Start} - {End}")]
    public sealed class PdbScope
    {
        private readonly ThreadSafe.IList<PdbScope> scopes = ThreadSafeListCreator.Create<PdbScope>();
        private readonly ThreadSafe.IList<Local> locals = ThreadSafeListCreator.Create<Local>();
        private readonly ThreadSafe.IList<string> namespaces = ThreadSafeListCreator.Create<string>();

        /// <summary>
        /// Gets/sets the first instruction
        /// </summary>
        public Instruction Start { get; set; }

        /// <summary>
        /// Gets/sets the last instruction. It's <c>null</c> if it ends at the end of the method.
        /// </summary>
        public Instruction End { get; set; }

        /// <summary>
        /// Gets all child scopes
        /// </summary>
        public ThreadSafe.IList<PdbScope> Scopes
        {
            get { return scopes; }
        }

        /// <summary>
        /// <c>true</c> if <see cref="Scopes"/> is not empty
        /// </summary>
        public bool HasScopes
        {
            get { return scopes.Count > 0; }
        }

        /// <summary>
        /// Gets all locals in this scope
        /// </summary>
        public ThreadSafe.IList<Local> Variables
        {
            get { return locals; }
        }

        /// <summary>
        /// <c>true</c> if <see cref="Variables"/> is not empty
        /// </summary>
        public bool HasVariables
        {
            get { return locals.Count > 0; }
        }

        /// <summary>
        /// Gets all namespaces
        /// </summary>
        public ThreadSafe.IList<string> Namespaces
        {
            get { return namespaces; }
        }

        /// <summary>
        /// <c>true</c> if <see cref="Namespaces"/> is not empty
        /// </summary>
        public bool HasNamespaces
        {
            get { return namespaces.Count > 0; }
        }
    }
}