﻿// dnlib: See LICENSE.txt for more info

using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;

namespace dnlib.DotNet.Pdb
{
    /// <summary>
    /// PDB writer
    /// </summary>
    /// <remarks>This class is not thread safe because it's a writer class</remarks>
    public sealed class PdbWriter : IDisposable
    {
        private ISymbolWriter2 writer;
        private readonly PdbState pdbState;
        private readonly ModuleDef module;
        private readonly MetaData metaData;
        private readonly Dictionary<PdbDocument, ISymbolDocumentWriter> pdbDocs = new Dictionary<PdbDocument, ISymbolDocumentWriter>();
        private readonly SequencePointHelper seqPointsHelper = new SequencePointHelper();

        /// <summary>
        /// Gets/sets the logger
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">Symbol writer</param>
        /// <param name="pdbState">PDB state</param>
        /// <param name="metaData">Meta data</param>
        public PdbWriter(ISymbolWriter2 writer, PdbState pdbState, MetaData metaData)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (pdbState == null)
                throw new ArgumentNullException("pdbState");
            if (metaData == null)
                throw new ArgumentNullException("metaData");
            this.writer = writer;
            this.pdbState = pdbState;
            this.metaData = metaData;
            this.module = metaData.Module;
            writer.Initialize(metaData);
        }

        /// <summary>
        /// Adds <paramref name="pdbDoc"/> if it doesn't exist
        /// </summary>
        /// <param name="pdbDoc">PDB document</param>
        /// <returns>A <see cref="ISymbolDocumentWriter"/> instance</returns>
        private ISymbolDocumentWriter Add(PdbDocument pdbDoc)
        {
            ISymbolDocumentWriter docWriter;
            if (pdbDocs.TryGetValue(pdbDoc, out docWriter))
                return docWriter;
            docWriter = writer.DefineDocument(pdbDoc.Url, pdbDoc.Language, pdbDoc.LanguageVendor, pdbDoc.DocumentType);
            docWriter.SetCheckSum(pdbDoc.CheckSumAlgorithmId, pdbDoc.CheckSum);
            pdbDocs.Add(pdbDoc, docWriter);
            return docWriter;
        }

        /// <summary>
        /// Writes the PDB file
        /// </summary>
        public void Write()
        {
            writer.SetUserEntryPoint(new SymbolToken(GetUserEntryPointToken()));

            foreach (var type in module.GetTypes())
            {
                if (type == null)
                    continue;
                foreach (var method in type.Methods)
                {
                    if (method == null)
                        continue;
                    if (!ShouldAddMethod(method))
                        continue;
                    Write(method);
                }
            }
        }

        private bool ShouldAddMethod(MethodDef method)
        {
            var body = method.Body;
            if (body == null)
                return false;

            if (body.HasScope)
                return true;

            foreach (var local in body.Variables)
            {
                // Don't check whether it's the empty string. Only check for null.
                if (local.Name != null)
                    return true;
                if (local.PdbAttributes != 0)
                    return true;
            }

            foreach (var instr in body.Instructions)
            {
                if (instr.SequencePoint != null)
                    return true;
            }

            return false;
        }

        private sealed class SequencePointHelper
        {
            private readonly Dictionary<PdbDocument, bool> checkedPdbDocs = new Dictionary<PdbDocument, bool>();
            private int[] instrOffsets = new int[0];
            private int[] startLines;
            private int[] startColumns;
            private int[] endLines;
            private int[] endColumns;

            public void Write(PdbWriter pdbWriter, IList<Instruction> instrs)
            {
                checkedPdbDocs.Clear();
                while (true)
                {
                    PdbDocument currPdbDoc = null;
                    bool otherDocsAvailable = false;
                    int index = 0, instrOffset = 0;
                    Instruction instr = null;
                    for (int i = 0; i < instrs.Count; i++, instrOffset += instr.GetSize())
                    {
                        instr = instrs[i];
                        var seqp = instr.SequencePoint;
                        if (seqp == null || seqp.Document == null)
                            continue;
                        if (checkedPdbDocs.ContainsKey(seqp.Document))
                            continue;
                        if (currPdbDoc == null)
                            currPdbDoc = seqp.Document;
                        else if (currPdbDoc != seqp.Document)
                        {
                            otherDocsAvailable = true;
                            continue;
                        }

                        if (index >= instrOffsets.Length)
                        {
                            int newSize = index * 2;
                            if (newSize < 64)
                                newSize = 64;
                            Array.Resize(ref instrOffsets, newSize);
                            Array.Resize(ref startLines, newSize);
                            Array.Resize(ref startColumns, newSize);
                            Array.Resize(ref endLines, newSize);
                            Array.Resize(ref endColumns, newSize);
                        }

                        instrOffsets[index] = instrOffset;
                        startLines[index] = seqp.StartLine;
                        startColumns[index] = seqp.StartColumn;
                        endLines[index] = seqp.EndLine;
                        endColumns[index] = seqp.EndColumn;
                        index++;
                    }
                    if (index != 0)
                        pdbWriter.writer.DefineSequencePoints(pdbWriter.Add(currPdbDoc), (uint)index, instrOffsets, startLines, startColumns, endLines, endColumns);

                    if (!otherDocsAvailable)
                        break;
                    if (currPdbDoc != null)
                        checkedPdbDocs.Add(currPdbDoc, true);
                }
            }
        }

        private void Write(MethodDef method)
        {
            uint rid = metaData.GetRid(method);
            if (rid == 0)
            {
                Error("Method {0} ({1:X8}) is not defined in this module ({2})", method, method.MDToken.Raw, module);
                return;
            }

            var body = method.Body;
            uint methodSize = GetSizeOfBody(body);

            writer.OpenMethod(new SymbolToken((int)new MDToken(MD.Table.Method, metaData.GetRid(method)).Raw));
            writer.OpenScope(0);
            AddLocals(method, body.Variables, 0, methodSize);
            seqPointsHelper.Write(this, body.Instructions);
            foreach (var scope in GetScopes(body.Scope))
            {
                foreach (var ns in scope.Namespaces)
                    writer.UsingNamespace(ns);
            }
            writer.CloseScope((int)methodSize);
            writer.CloseMethod();
        }

        private IEnumerable<PdbScope> GetScopes(PdbScope root)
        {
            if (root == null)
                return new PdbScope[0];
            return GetScopes(new PdbScope[1] { root });
        }

        private IEnumerable<PdbScope> GetScopes(IEnumerable<PdbScope> scopes)
        {
            var visited = new Dictionary<PdbScope, bool>();
            var stack = new Stack<IEnumerator<PdbScope>>();
            if (scopes != null)
                stack.Push(scopes.GetEnumerator());
            while (stack.Count > 0)
            {
                var enumerator = stack.Pop();
                while (enumerator.MoveNext())
                {
                    var type = enumerator.Current;
                    if (visited.ContainsKey(type))
                    {
                        Error("PdbScope present more than once");
                        continue;
                    }
                    visited[type] = true;
                    yield return type;
                    if (type.Scopes.Count > 0)
                    {
                        stack.Push(enumerator);
                        enumerator = type.Scopes.GetEnumerator();
                    }
                }
            }
        }

        private void AddLocals(MethodDef method, IList<Local> locals, uint startOffset, uint endOffset)
        {
            if (locals.Count == 0)
                return;
            uint token = metaData.GetLocalVarSigToken(method);
            if (token == 0)
            {
                Error("Method {0} ({1:X8}) has no local signature token", method, method.MDToken.Raw);
                return;
            }
            foreach (var local in locals)
            {
                if (local.Name == null && local.PdbAttributes == 0)
                    continue;
                writer.DefineLocalVariable2(local.Name ?? string.Empty, (uint)local.PdbAttributes,
                                token, 1, (uint)local.Index, 0, 0, startOffset, endOffset);
            }
        }

        private uint GetSizeOfBody(CilBody body)
        {
            uint offset = 0;
            foreach (var instr in body.Instructions)
                offset += (uint)instr.GetSize();
            return offset;
        }

        private int GetUserEntryPointToken()
        {
            var ep = pdbState.UserEntryPoint;
            if (ep == null)
                return 0;
            uint rid = metaData.GetRid(ep);
            if (rid == 0)
            {
                Error("PDB user entry point method {0} ({1:X8}) is not defined in this module ({2})", ep, ep.MDToken.Raw, module);
                return 0;
            }
            return new MDToken(MD.Table.Method, rid).ToInt32();
        }

        /// <summary>
        /// Gets the <see cref="IMAGE_DEBUG_DIRECTORY"/> and debug data that should be written to
        /// the PE file.
        /// </summary>
        /// <param name="idd">Updated with new values</param>
        /// <returns>Debug data</returns>
        public byte[] GetDebugInfo(out IMAGE_DEBUG_DIRECTORY idd)
        {
            return writer.GetDebugInfo(out idd);
        }

        /// <summary>
        /// Closes the PDB writer
        /// </summary>
        public void Close()
        {
            writer.Close();
        }

        private ILogger GetLogger()
        {
            return Logger ?? DummyLogger.ThrowModuleWriterExceptionOnErrorInstance;
        }

        private void Error(string message, params object[] args)
        {
            GetLogger().Log(this, LoggerEvent.Error, message, args);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (writer != null)
                Close();
            writer = null;
        }
    }
}