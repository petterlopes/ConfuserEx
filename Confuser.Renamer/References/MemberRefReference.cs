using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
    public class MemberRefReference : INameReference<ISDnlibDef>
    {
        private readonly ISDnlibDef memberDef;
        private readonly MemberRef memberRef;

        public MemberRefReference(MemberRef memberRef, ISDnlibDef memberDef)
        {
            this.memberRef = memberRef;
            this.memberDef = memberDef;
        }

        public bool UpdateNameReference(ConfuserContext context, INameService service)
        {
            memberRef.Name = memberDef.Name;
            return true;
        }

        public bool ShouldCancelRename()
        {
            return false;
        }
    }
}