using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
    internal class CAMemberReference : INameReference<ISDnlibDef>
    {
        private readonly ISDnlibDef definition;
        private readonly CANamedArgument namedArg;

        public CAMemberReference(CANamedArgument namedArg, ISDnlibDef definition)
        {
            this.namedArg = namedArg;
            this.definition = definition;
        }

        public bool UpdateNameReference(ConfuserContext context, INameService service)
        {
            namedArg.Name = definition.Name;
            return true;
        }

        public bool ShouldCancelRename()
        {
            return false;
        }
    }
}