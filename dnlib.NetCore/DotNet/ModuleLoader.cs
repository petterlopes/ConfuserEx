﻿// dnlib: See LICENSE.txt for more info

using dnlib.NetCore.DotNet.Emit;
using dnlib.NetCore.DotNet.MD;
using dnlib.NetCore.DotNet.Pdb;
using dnlib.NetCore.IO;
using dnlib.NetCore.PE;
using dnlib.NetCore.Threading;
using dnlib.NetCore.W32Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace dnlib.NetCore.DotNet
{
    internal struct ModuleLoader
    {
        private readonly ModuleDef module;
        private readonly ICancellationToken cancellationToken;
        private readonly Dictionary<object, bool> seen;
        private readonly Stack<object> stack;

        private ModuleLoader(ModuleDef module, ICancellationToken cancellationToken)
        {
            const int CAPACITY = 0x4000;
            this.module = module;
            this.cancellationToken = cancellationToken;
            this.seen = new Dictionary<object, bool>(CAPACITY);
            this.stack = new Stack<object>(CAPACITY);
        }

        public static void LoadAll(ModuleDef module, ICancellationToken cancellationToken)
        {
            new ModuleLoader(module, cancellationToken).Load();
        }

        private void Add(UTF8String a)
        { }

        private void Add(Guid? a)
        { }

        private void Add(ushort a)
        { }

        private void Add(AssemblyHashAlgorithm a)
        { }

        private void Add(Version a)
        { }

        private void Add(AssemblyAttributes a)
        { }

        private void Add(PublicKeyBase a)
        { }

        private void Add(RVA a)
        { }

        private void Add(IManagedEntryPoint a)
        { }

        private void Add(string a)
        { }

        private void Add(WinMDStatus a)
        { }

        private void Add(TypeAttributes a)
        { }

        private void Add(FieldAttributes a)
        { }

        private void Add(uint? a)
        { }

        private void Add(byte[] a)
        { }

        private void Add(MethodImplAttributes a)
        { }

        private void Add(MethodAttributes a)
        { }

        private void Add(MethodSemanticsAttributes a)
        { }

        private void Add(ParamAttributes a)
        { }

        private void Add(ElementType a)
        { }

        private void Add(SecurityAction a)
        { }

        private void Add(EventAttributes a)
        { }

        private void Add(PropertyAttributes a)
        { }

        private void Add(PInvokeAttributes a)
        { }

        private void Add(FileAttributes a)
        { }

        private void Add(ManifestResourceAttributes a)
        { }

        private void Add(GenericParamAttributes a)
        { }

        private void Add(NativeType a)
        { }

        private void Load()
        {
            LoadAllTables();
            Load(module);
            Process();
        }

        private void Process()
        {
            while (stack.Count != 0)
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();
                var o = stack.Pop();
                LoadObj(o);
            }
        }

        private void LoadAllTables()
        {
            var resolver = module as ITokenResolver;
            if (resolver == null)
                return;
            for (Table tbl = 0; tbl <= Table.GenericParamConstraint; tbl++)
            {
                for (uint rid = 1; ; rid++)
                {
                    var o = resolver.ResolveToken(new MDToken(tbl, rid).Raw, new GenericParamContext());
                    if (o == null)
                        break;
                    Add(o);
                    Process();
                }
            }
        }

        private void LoadObj(object o)
        {
            var ts = o as TypeSig;
            if (ts != null)
            {
                Load(ts);
                return;
            }

            var mdt = o as IMDTokenProvider;
            if (mdt != null)
            {
                Load(mdt);
                return;
            }

            var ca = o as CustomAttribute;
            if (ca != null)
            {
                Load(ca);
                return;
            }

            var sa = o as SecurityAttribute;
            if (sa != null)
            {
                Load(sa);
                return;
            }

            var na = o as CANamedArgument;
            if (na != null)
            {
                Load(na);
                return;
            }

            var p = o as Parameter;
            if (p != null)
            {
                Load(p);
                return;
            }

            var scope = o as PdbScope;
            if (scope != null)
            {
                Load(scope);
                return;
            }

            var rd = o as ResourceDirectory;
            if (rd != null)
            {
                Load(rd);
                return;
            }

            var rdata = o as ResourceData;
            if (rdata != null)
            {
                Load(rdata);
                return;
            }

            Debug.Fail("Unknown type");
        }

        private void Load(TypeSig ts)
        {
            if (ts == null)
                return;
            Add(ts.Next);

            switch (ts.ElementType)
            {
                case ElementType.Void:
                case ElementType.Boolean:
                case ElementType.Char:
                case ElementType.I1:
                case ElementType.U1:
                case ElementType.I2:
                case ElementType.U2:
                case ElementType.I4:
                case ElementType.U4:
                case ElementType.I8:
                case ElementType.U8:
                case ElementType.R4:
                case ElementType.R8:
                case ElementType.String:
                case ElementType.ValueType:
                case ElementType.Class:
                case ElementType.TypedByRef:
                case ElementType.I:
                case ElementType.U:
                case ElementType.Object:
                    Add(((TypeDefOrRefSig)ts).TypeDefOrRef);
                    break;

                case ElementType.Var:
                case ElementType.MVar:
                    var vsig = (GenericSig)ts;
                    Add(vsig.OwnerType);
                    Add(vsig.OwnerMethod);
                    break;

                case ElementType.GenericInst:
                    var gis = (GenericInstSig)ts;
                    Add(gis.GenericType);
                    Add(gis.GenericArguments);
                    break;

                case ElementType.FnPtr:
                    var fpsig = (FnPtrSig)ts;
                    Add(fpsig.Signature);
                    break;

                case ElementType.CModReqd:
                case ElementType.CModOpt:
                    var cmod = (ModifierSig)ts;
                    Add(cmod.Modifier);
                    break;

                case ElementType.End:
                case ElementType.Ptr:
                case ElementType.ByRef:
                case ElementType.Array:
                case ElementType.ValueArray:
                case ElementType.SZArray:
                case ElementType.Module:
                case ElementType.Pinned:
                case ElementType.Sentinel:
                case ElementType.R:
                case ElementType.Internal:
                default:
                    break;
            }
        }

        private void Load(IMDTokenProvider mdt)
        {
            if (mdt == null)
                return;
            switch (mdt.MDToken.Table)
            {
                case Table.Module: Load((ModuleDef)mdt); break;
                case Table.TypeRef: Load((TypeRef)mdt); break;
                case Table.TypeDef: Load((TypeDef)mdt); break;
                case Table.Field: Load((FieldDef)mdt); break;
                case Table.Method: Load((MethodDef)mdt); break;
                case Table.Param: Load((ParamDef)mdt); break;
                case Table.InterfaceImpl: Load((InterfaceImpl)mdt); break;
                case Table.MemberRef: Load((MemberRef)mdt); break;
                case Table.Constant: Load((Constant)mdt); break;
                case Table.DeclSecurity: Load((DeclSecurity)mdt); break;
                case Table.ClassLayout: Load((ClassLayout)mdt); break;
                case Table.StandAloneSig: Load((StandAloneSig)mdt); break;
                case Table.Event: Load((EventDef)mdt); break;
                case Table.Property: Load((PropertyDef)mdt); break;
                case Table.ModuleRef: Load((ModuleRef)mdt); break;
                case Table.TypeSpec: Load((TypeSpec)mdt); break;
                case Table.ImplMap: Load((ImplMap)mdt); break;
                case Table.Assembly: Load((AssemblyDef)mdt); break;
                case Table.AssemblyRef: Load((AssemblyRef)mdt); break;
                case Table.File: Load((FileDef)mdt); break;
                case Table.ExportedType: Load((ExportedType)mdt); break;
                case Table.GenericParam: Load((GenericParam)mdt); break;
                case Table.MethodSpec: Load((MethodSpec)mdt); break;
                case Table.GenericParamConstraint: Load((GenericParamConstraint)mdt); break;

                case Table.ManifestResource:
                    var rsrc = mdt as Resource;
                    if (rsrc != null)
                    {
                        Load(rsrc);
                        break;
                    }

                    var mr = mdt as ManifestResource;
                    if (mr != null)
                    {
                        Load(mr);
                        break;
                    }

                    Debug.Fail("Unknown ManifestResource");
                    break;

                case Table.FieldPtr:
                case Table.MethodPtr:
                case Table.ParamPtr:
                case Table.CustomAttribute:
                case Table.FieldMarshal:
                case Table.FieldLayout:
                case Table.EventMap:
                case Table.EventPtr:
                case Table.PropertyMap:
                case Table.PropertyPtr:
                case Table.MethodSemantics:
                case Table.MethodImpl:
                case Table.FieldRVA:
                case Table.ENCLog:
                case Table.ENCMap:
                case Table.AssemblyProcessor:
                case Table.AssemblyOS:
                case Table.AssemblyRefProcessor:
                case Table.AssemblyRefOS:
                case Table.NestedClass:
                    break;

                default:
                    Debug.Fail("Unknown type");
                    break;
            }
        }

        private void Load(ModuleDef obj)
        {
            if (obj == null || obj != this.module)
                return;
            Add(obj.Generation);
            Add(obj.Name);
            Add(obj.Mvid);
            Add(obj.EncId);
            Add(obj.EncBaseId);
            Add(obj.CustomAttributes);
            Add(obj.Assembly);
            Add(obj.Types);
            Add(obj.ExportedTypes);
            Add(obj.NativeEntryPoint);
            Add(obj.ManagedEntryPoint);
            Add(obj.Resources);
            Add(obj.VTableFixups);
            Add(obj.Location);
            Add(obj.Win32Resources);
            Add(obj.RuntimeVersion);
            Add(obj.WinMDStatus);
            Add(obj.RuntimeVersionWinMD);
            Add(obj.WinMDVersion);
            Add(obj.PdbState);
        }

        private void Load(TypeRef obj)
        {
            if (obj == null)
                return;
            Add(obj.ResolutionScope);
            Add(obj.Name);
            Add(obj.Namespace);
            Add(obj.CustomAttributes);
        }

        private void Load(TypeDef obj)
        {
            if (obj == null)
                return;
            Add(obj.Module2);
            Add(obj.Attributes);
            Add(obj.Name);
            Add(obj.Namespace);
            Add(obj.BaseType);
            Add(obj.Fields);
            Add(obj.Methods);
            Add(obj.GenericParameters);
            Add(obj.Interfaces);
            Add(obj.DeclSecurities);
            Add(obj.ClassLayout);
            Add(obj.DeclaringType);
            Add(obj.DeclaringType2);
            Add(obj.NestedTypes);
            Add(obj.Events);
            Add(obj.Properties);
            Add(obj.CustomAttributes);
        }

        private void Load(FieldDef obj)
        {
            if (obj == null)
                return;
            Add(obj.CustomAttributes);
            Add(obj.Attributes);
            Add(obj.Name);
            Add(obj.Signature);
            Add(obj.FieldOffset);
            Add(obj.MarshalType);
            Add(obj.RVA);
            Add(obj.InitialValue);
            Add(obj.ImplMap);
            Add(obj.Constant);
            Add(obj.DeclaringType);
        }

        private void Load(MethodDef obj)
        {
            if (obj == null)
                return;
            Add(obj.RVA);
            Add(obj.ImplAttributes);
            Add(obj.Attributes);
            Add(obj.Name);
            Add(obj.Signature);
            Add(obj.ParamDefs);
            Add(obj.GenericParameters);
            Add(obj.DeclSecurities);
            Add(obj.ImplMap);
            Add(obj.MethodBody);
            Add(obj.CustomAttributes);
            Add(obj.Overrides);
            Add(obj.DeclaringType);
            Add(obj.Parameters);
            Add(obj.SemanticsAttributes);
        }

        private void Load(ParamDef obj)
        {
            if (obj == null)
                return;
            Add(obj.DeclaringMethod);
            Add(obj.Attributes);
            Add(obj.Sequence);
            Add(obj.Name);
            Add(obj.MarshalType);
            Add(obj.Constant);
            Add(obj.CustomAttributes);
        }

        private void Load(InterfaceImpl obj)
        {
            if (obj == null)
                return;
            Add(obj.Interface);
            Add(obj.CustomAttributes);
        }

        private void Load(MemberRef obj)
        {
            if (obj == null)
                return;
            Add(obj.Class);
            Add(obj.Name);
            Add(obj.Signature);
            Add(obj.CustomAttributes);
        }

        private void Load(Constant obj)
        {
            if (obj == null)
                return;
            Add(obj.Type);
            var o = obj.Value;
        }

        private void Load(DeclSecurity obj)
        {
            if (obj == null)
                return;
            Add(obj.Action);
            Add(obj.SecurityAttributes);
            Add(obj.CustomAttributes);
            obj.GetBlob();
        }

        private void Load(ClassLayout obj)
        {
            if (obj == null)
                return;
            Add(obj.PackingSize);
            Add(obj.ClassSize);
        }

        private void Load(StandAloneSig obj)
        {
            if (obj == null)
                return;
            Add(obj.Signature);
            Add(obj.CustomAttributes);
        }

        private void Load(EventDef obj)
        {
            if (obj == null)
                return;
            Add(obj.Attributes);
            Add(obj.Name);
            Add(obj.EventType);
            Add(obj.CustomAttributes);
            Add(obj.AddMethod);
            Add(obj.InvokeMethod);
            Add(obj.RemoveMethod);
            Add(obj.OtherMethods);
            Add(obj.DeclaringType);
        }

        private void Load(PropertyDef obj)
        {
            if (obj == null)
                return;
            Add(obj.Attributes);
            Add(obj.Name);
            Add(obj.Type);
            Add(obj.Constant);
            Add(obj.CustomAttributes);
            Add(obj.GetMethods);
            Add(obj.SetMethods);
            Add(obj.OtherMethods);
            Add(obj.DeclaringType);
        }

        private void Load(ModuleRef obj)
        {
            if (obj == null)
                return;
            Add(obj.Name);
            Add(obj.CustomAttributes);
        }

        private void Load(TypeSpec obj)
        {
            if (obj == null)
                return;
            Add(obj.TypeSig);
            Add(obj.ExtraData);
            Add(obj.CustomAttributes);
        }

        private void Load(ImplMap obj)
        {
            if (obj == null)
                return;
            Add(obj.Attributes);
            Add(obj.Name);
            Add(obj.Module);
        }

        private void Load(AssemblyDef obj)
        {
            if (obj == null)
                return;
            if (obj.ManifestModule != module)
                return;
            Add(obj.HashAlgorithm);
            Add(obj.Version);
            Add(obj.Attributes);
            Add(obj.PublicKey);
            Add(obj.Name);
            Add(obj.Culture);
            Add(obj.DeclSecurities);
            Add(obj.Modules);
            Add(obj.CustomAttributes);
        }

        private void Load(AssemblyRef obj)
        {
            if (obj == null)
                return;
            Add(obj.Version);
            Add(obj.Attributes);
            Add(obj.PublicKeyOrToken);
            Add(obj.Name);
            Add(obj.Culture);
            Add(obj.Hash);
            Add(obj.CustomAttributes);
        }

        private void Load(FileDef obj)
        {
            if (obj == null)
                return;
            Add(obj.Flags);
            Add(obj.Name);
            Add(obj.HashValue);
            Add(obj.CustomAttributes);
        }

        private void Load(ExportedType obj)
        {
            if (obj == null)
                return;
            Add(obj.CustomAttributes);
            Add(obj.Attributes);
            Add(obj.TypeDefId);
            Add(obj.TypeName);
            Add(obj.TypeNamespace);
            Add(obj.Implementation);
        }

        private void Load(Resource obj)
        {
            if (obj == null)
                return;

            Add(obj.Offset);
            Add(obj.Name);
            Add(obj.Attributes);

            switch (obj.ResourceType)
            {
                case ResourceType.Embedded:
                    var er = (EmbeddedResource)obj;
                    // Make sure data is cached
                    if (!(er.Data is MemoryImageStream))
                        er.Data = MemoryImageStream.Create(er.GetClonedResourceStream().ReadAllBytes());
                    break;

                case ResourceType.AssemblyLinked:
                    var ar = (AssemblyLinkedResource)obj;
                    Add(ar.Assembly);
                    break;

                case ResourceType.Linked:
                    var lr = (LinkedResource)obj;
                    Add(lr.File);
                    Add(lr.Hash);
                    break;

                default:
                    Debug.Fail("Unknown resource");
                    break;
            }
        }

        private void Load(ManifestResource obj)
        {
            if (obj == null)
                return;
            Add(obj.Offset);
            Add(obj.Flags);
            Add(obj.Name);
            Add(obj.Implementation);
            Add(obj.CustomAttributes);
        }

        private void Load(GenericParam obj)
        {
            if (obj == null)
                return;
            Add(obj.Owner);
            Add(obj.Number);
            Add(obj.Flags);
            Add(obj.Name);
            Add(obj.Kind);
            Add(obj.GenericParamConstraints);
            Add(obj.CustomAttributes);
        }

        private void Load(MethodSpec obj)
        {
            if (obj == null)
                return;
            Add(obj.Method);
            Add(obj.Instantiation);
            Add(obj.CustomAttributes);
        }

        private void Load(GenericParamConstraint obj)
        {
            if (obj == null)
                return;
            Add(obj.Owner);
            Add(obj.Constraint);
            Add(obj.CustomAttributes);
        }

        private void Load(CANamedArgument obj)
        {
            if (obj == null)
                return;
            Add(obj.Type);
            Add(obj.Name);
            Load(obj.Argument);
        }

        private void Load(Parameter obj)
        {
            if (obj == null)
                return;
            Add(obj.Type);
        }

        private void Load(SecurityAttribute obj)
        {
            if (obj == null)
                return;
            Add(obj.AttributeType);
            Add(obj.NamedArguments);
        }

        private void Load(CustomAttribute obj)
        {
            if (obj == null)
                return;
            Add(obj.Constructor);
            Add(obj.RawData);
            Add(obj.ConstructorArguments);
            Add(obj.NamedArguments);
        }

        private void Load(MethodOverride obj)
        {
            Add(obj.MethodBody);
            Add(obj.MethodDeclaration);
        }

        private void AddCAValue(object obj)
        {
            if (obj is CAArgument)
            {
                Load((CAArgument)obj);
                return;
            }

            var list = obj as IList<CAArgument>;
            if (list != null)
            {
                Add(list);
                return;
            }

            var md = obj as IMDTokenProvider;
            if (md != null)
            {
                Add(md);
                return;
            }
        }

        private void Load(CAArgument obj)
        {
            Add(obj.Type);
            AddCAValue(obj.Value);
        }

        private void Load(PdbScope obj)
        {
            if (obj == null)
                return;
            Add(obj.Start);
            Add(obj.End);
            Add(obj.Scopes);
            Add(obj.Variables);
        }

        private void Load(ResourceDirectory obj)
        {
            if (obj == null)
                return;
            Add(obj.Directories);
            Add(obj.Data);
        }

        private void Load(ResourceData obj)
        {
            if (obj == null)
                return;
            var data = obj.Data;
            if (data != null && !(data is MemoryImageStream))
                obj.Data = MemoryImageStream.Create(data.ReadAllBytes());
        }

        private void AddToStack<T>(T t) where T : class
        {
            if (t == null)
                return;
            if (seen.ContainsKey(t))
                return;
            seen[t] = true;
            stack.Push(t);
        }

        private void Add(CustomAttribute obj)
        {
            AddToStack(obj);
        }

        private void Add(SecurityAttribute obj)
        {
            AddToStack(obj);
        }

        private void Add(CANamedArgument obj)
        {
            AddToStack(obj);
        }

        private void Add(Parameter obj)
        {
            AddToStack(obj);
        }

        private void Add(IMDTokenProvider o)
        {
            AddToStack(o);
        }

        private void Add(PdbScope scope)
        {
            AddToStack(scope);
        }

        private void Add(TypeSig ts)
        {
            AddToStack(ts);
        }

        private void Add(ResourceDirectory rd)
        {
            AddToStack(rd);
        }

        private void Add(ResourceData rd)
        {
            AddToStack(rd);
        }

        private void Add<T>(IList<T> list) where T : IMDTokenProvider
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<TypeSig> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<CustomAttribute> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<SecurityAttribute> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<MethodOverride> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Load(item);
        }

        private void Add(IList<CAArgument> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Load(item);
        }

        private void Add(IList<CANamedArgument> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(ParameterList list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<Instruction> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<ExceptionHandler> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<Local> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<PdbScope> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<ResourceDirectory> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(IList<ResourceData> list)
        {
            if (list == null)
                return;
            foreach (var item in list.GetSafeEnumerable())
                Add(item);
        }

        private void Add(VTableFixups vtf)
        {
            if (vtf == null)
                return;
            foreach (var fixup in vtf)
            {
                foreach (var method in fixup)
                    Add(method);
            }
        }

        private void Add(Win32Resources vtf)
        {
            if (vtf == null)
                return;
            Add(vtf.Root);
        }

        private void Add(CallingConventionSig sig)
        {
            var msig = sig as MethodBaseSig;
            if (msig != null)
            {
                Add(msig);
                return;
            }

            var fsig = sig as FieldSig;
            if (fsig != null)
            {
                Add(fsig);
                return;
            }

            var lsig = sig as LocalSig;
            if (lsig != null)
            {
                Add(lsig);
                return;
            }

            var gsig = sig as GenericInstMethodSig;
            if (gsig != null)
            {
                Add(gsig);
                return;
            }

            Debug.Assert(sig == null);
        }

        private void Add(MethodBaseSig msig)
        {
            if (msig == null)
                return;
            Add(msig.ExtraData);
            Add(msig.RetType);
            Add(msig.Params);
            Add(msig.ParamsAfterSentinel);
        }

        private void Add(FieldSig fsig)
        {
            if (fsig == null)
                return;
            Add(fsig.ExtraData);
            Add(fsig.Type);
        }

        private void Add(LocalSig lsig)
        {
            if (lsig == null)
                return;
            Add(lsig.ExtraData);
            Add(lsig.Locals);
        }

        private void Add(GenericInstMethodSig gsig)
        {
            if (gsig == null)
                return;
            Add(gsig.ExtraData);
            Add(gsig.GenericArguments);
        }

        private void Add(MarshalType mt)
        {
            if (mt == null)
                return;
            Add(mt.NativeType);
        }

        private void Add(MethodBody mb)
        {
            var cilBody = mb as CilBody;
            if (cilBody != null)
            {
                Add(cilBody);
                return;
            }

            var nb = mb as NativeMethodBody;
            if (nb != null)
            {
                Add(nb);
                return;
            }

            Debug.Assert(mb == null, "Unknown method body");
        }

        private void Add(NativeMethodBody body)
        {
            if (body == null)
                return;
            Add(body.RVA);
        }

        private void Add(CilBody body)
        {
            if (body == null)
                return;
            Add(body.Instructions);
            Add(body.ExceptionHandlers);
            Add(body.Variables);
            Add(body.Scope);
        }

        private void Add(Instruction instr)
        {
            if (instr == null)
                return;

            var mdt = instr.Operand as IMDTokenProvider;
            if (mdt != null)
            {
                Add(mdt);
                return;
            }

            var p = instr.Operand as Parameter;
            if (p != null)
            {
                Add(p);
                return;
            }

            var l = instr.Operand as Local;
            if (l != null)
            {
                Add(l);
                return;
            }

            var csig = instr.Operand as CallingConventionSig;
            if (csig != null)
            {
                Add(csig);
                return;
            }
        }

        private void Add(ExceptionHandler eh)
        {
            if (eh == null)
                return;
            Add(eh.CatchType);
        }

        private void Add(Local local)
        {
            if (local == null)
                return;
            Add(local.Type);
        }

        private void Add(PdbState state)
        {
            if (state == null)
                return;
            Add(state.UserEntryPoint);
        }
    }
}