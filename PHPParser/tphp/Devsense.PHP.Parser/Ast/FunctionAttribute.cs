using Devsense.PHP.Syntax.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Syntax;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    public class FunctionAttribute : Statement
    {
        private readonly Span _span;
        private readonly Name _name;
        private readonly CallSignature _args;
        /// <summary>
        /// Position of the name.
        /// </summary>
        public Span Span => _span;

        /// <summary>
        /// Variable name.
        /// </summary>
        public Name Name => _name;

        public CallSignature Args => _args;
        public FunctionAttribute(Span span, Name name, CallSignature args) : base(span)
        {
            _span = span;
            _name = name;
            _args = args;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
           
        }
    }
}
