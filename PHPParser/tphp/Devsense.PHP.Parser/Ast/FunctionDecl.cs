// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace Devsense.PHP.Syntax.Ast
{
	#region FormalParam

	/// <summary>
	/// Represents a formal parameter definition.
	/// </summary>
	public sealed class FormalParam : LangElement
	{
        [Flags]
        public enum Flags
        {
            Default = 0,
            IsByRef = 1,
            IsOut = 2,
            IsVariadic = 4,
        }

        /// <summary>
        /// Flags describing the parameter.
        /// </summary>
        private Flags _flags;

		/// <summary>
		/// Name of the argument.
		/// </summary>
		public VariableNameRef Name { get { return _name; } }
		private VariableNameRef _name;

		/// <summary>
		/// Whether the parameter is &amp;-modified.
		/// </summary>
        public bool PassedByRef { get { return (_flags & Flags.IsByRef) != 0; } }

		/// <summary>
		/// Whether the parameter is an out-parameter. Set by applying the [Out] attribute.
		/// </summary>
        public bool IsOut
        {
            get { return (_flags & Flags.IsOut) != 0; }
            internal set
            {
                if (value) _flags |= Flags.IsOut;
                else _flags &= ~Flags.IsOut;
            }
        }

        /// <summary>
        /// Gets value indicating whether the parameter is variadic and so passed parameters will be packed into the array as passed as one parameter.
        /// </summary>
        public bool IsVariadic { get { return (_flags & Flags.IsVariadic) != 0; } }

		/// <summary>
		/// Initial value expression. Can be <B>null</B>.
		/// </summary>
        public Expression InitValue { get { return _initValue; } internal set { _initValue = value; } }
		private Expression _initValue;

        /// <summary>
        /// Either <see cref="TypeRef"/> or <B>null</B>.
        /// </summary>
        public TypeRef TypeHint { get { return _typeHint; } }
		private TypeRef _typeHint;

        /// <summary>
        /// Gets collection of CLR attributes annotating this statement.
        /// </summary>
        public CustomAttributes Attributes
        {
            get { return this.GetCustomAttributes(); }
            set { this.SetCustomAttributes(value); }
        }

        #region Construction

        public FormalParam(Text.Span span, string/*!*/ name, Text.Span nameSpan, TypeRef typeHint, Flags flags,
				Expression initValue, List<CustomAttribute> attributes)
            : base(span)
		{
			_name = new VariableNameRef(nameSpan, name);
			_typeHint = typeHint;
            _flags = flags;
			_initValue = initValue;
            if (attributes != null && attributes.Count != 0)
            {
                this.Attributes = new CustomAttributes(attributes);
            }
		}

		#endregion

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitFormalParam(this);
        }
	}

	#endregion

	#region Signature

    public struct Signature
	{
		public bool AliasReturn { get { return aliasReturn; } }
		private readonly bool aliasReturn;

		public FormalParam[]/*!*/ FormalParams { get { return formalParams; } }
		private readonly FormalParam[]/*!*/ formalParams;

		public Signature(bool aliasReturn, IList<FormalParam>/*!*/ formalParams)
		{
			this.aliasReturn = aliasReturn;
			this.formalParams = formalParams.AsArray();
		}
	}

	#endregion

	#region FunctionDecl

	/// <summary>
	/// Represents a function declaration.
	/// </summary>
    public sealed class FunctionDecl : Statement
	{ 
		internal override bool IsDeclaration { get { return true; } }
        public List<FunctionAttribute> Attribute { get { return attr; } }
        private readonly List<FunctionAttribute> attr;
        public NameRef Name { get { return name; } }
		private readonly NameRef name;

        public Signature Signature { get { return signature; } }
        private readonly Signature signature;

        public TypeSignature TypeSignature { get { return typeSignature; } }
		private readonly TypeSignature typeSignature;

        public BlockStmt/*!*/ Body { get { return body; } }
        private readonly BlockStmt/*!*/ body;

        /// <summary>
        /// Gets value indicating whether the function is declared conditionally.
        /// </summary>
        public bool IsConditional { get; internal set; }

        /// <summary>
        /// Gets function declaration attributes.
        /// </summary>
        public PhpMemberAttributes MemberAttributes { get; private set; }
        
        /// <summary>
        /// Gets collection of CLR attributes annotating this statement.
        /// </summary>
        public CustomAttributes Attributes
        {
            get { return this.GetCustomAttributes(); }
            set { this.SetCustomAttributes(value); }
        }

        public Text.Span ParametersSpan { get { return parametersSpan; } }
        private Text.Span parametersSpan;

        public Text.Span HeadingSpan => Text.Span.FromBounds(Span.Start, ((returnType != null) ? returnType.Span : ParametersSpan).End);

        public TypeRef ReturnType { get { return returnType; } }
        private TypeRef returnType;

        #region Construction

        public FunctionDecl(
            Text.Span span, List<FunctionAttribute> attr,
			bool isConditional, PhpMemberAttributes memberAttributes, NameRef/*!*/ name,
			bool aliasReturn, List<FormalParam>/*!*/ formalParams, Text.Span paramsSpan, List<FormalTypeParam>/*!*/ genericParams,
            BlockStmt/*!*/ body, List<CustomAttribute> attributes, TypeRef returnType)
			: base(span)
		{
			Debug.Assert(genericParams != null && formalParams != null && body != null);
            this.attr = attr;
            this.name = name;
			this.signature = new Signature(aliasReturn, formalParams);
			this.typeSignature = new TypeSignature(genericParams);
			if (attributes != null && attributes.Count != 0)
                this.Attributes = new CustomAttributes(attributes);
			this.body = body;
            this.parametersSpan = paramsSpan;
            this.IsConditional = isConditional;
            this.MemberAttributes = memberAttributes;
            this.returnType = returnType;
		}

		#endregion

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitFunctionDecl(this);
        }

        /// <summary>
        /// <see cref="PHPDocBlock"/> instance or <c>null</c> reference.
        /// </summary>
        public PHPDocBlock PHPDoc
        {
            get { return this.GetPHPDoc(); }
            set { this.SetPHPDoc(value); }
        }
	}

	#endregion
}
