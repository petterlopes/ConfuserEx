using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
    internal class BAMLAttributeReference : INameReference<ISDnlibDef>
    {
        private readonly AttributeInfoRecord attrRec;
        private readonly ISDnlibDef member;
        private readonly PropertyRecord propRec;

        public BAMLAttributeReference(ISDnlibDef member, AttributeInfoRecord rec)
        {
            this.member = member;
            attrRec = rec;
        }

        public BAMLAttributeReference(ISDnlibDef member, PropertyRecord rec)
        {
            this.member = member;
            propRec = rec;
        }

        public bool UpdateNameReference(ConfuserContext context, INameService service)
        {
            if (attrRec != null)
                attrRec.Name = member.Name;
            else
                propRec.Value = member.Name;
            return true;
        }

        public bool ShouldCancelRename()
        {
            return false;
        }
    }
}