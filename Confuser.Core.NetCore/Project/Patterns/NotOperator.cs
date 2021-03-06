﻿namespace Confuser.Core.NetCore.Project.Patterns
{
    /// <summary>
    ///     The NOT operator.
    /// </summary>
    public class NotOperator : PatternOperator
    {
        internal const string OpName = "not";

        /// <inheritdoc />
        public override string Name
        {
            get { return OpName; }
        }

        /// <inheritdoc />
        public override bool IsUnary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override object Evaluate(ISDnlibDef definition)
        {
            return !(bool)OperandA.Evaluate(definition);
        }
    }
}