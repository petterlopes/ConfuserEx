using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
    public interface IRenamer
    {
        void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, ISDnlibDef def);

        void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, ISDnlibDef def);

        void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, ISDnlibDef def);
    }
}