using Devsense.PHP.Errors;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tphp
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = File.ReadAllText(@"D:\wamp64\www\tachyon\PHPParser\tphp\tphp\a.php");
            var sourceUnit = new CodeSourceUnit(code, @"D:\wamp64\www\tachyon\PHPParser\tphp\tphp\a.php", Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Php71Set);
            var factory = new BasicNodesFactory(sourceUnit);
            GlobalCode ast = null;
            var errors = new TestErrorSink();
            Parser parser = new Parser();
            using (StringReader source_reader = new StringReader(code))
            {
                sourceUnit.Parse(factory, errors);
                ast = sourceUnit.Ast;
            }
            var visitor = new TreeVisitorCheck();
            visitor.VisitElement(ast);
            Console.Read();
        }
    }

    /// <summary>
    /// Helper visitor checking every node has a containing element.
    /// </summary>
    sealed class TreeVisitorCheck : TreeVisitor
    {
        public override void VisitIncludingEx(IncludingEx x)
        {
            Console.WriteLine("INCEX");

            base.VisitIncludingEx(x);
        }
        public override void VisitIncDecEx(IncDecEx x)
        {
            Console.WriteLine("INCDEX");
            base.VisitIncDecEx(x);
        }
    }

        sealed internal class TestErrorSink : IErrorSink<Span>
    {
        public class ErrorInstance
        {
            public Span Span;
            public ErrorInfo Error;
            public string[] Args;
        }

        public readonly List<ErrorInstance> Errors = new List<ErrorInstance>();

        public int Count => this.Errors.Count;

        public void Error(Span span, ErrorInfo info, params string[] argsOpt)
        {
            Errors.Add(new ErrorInstance()
            {
                Span = span,
                Error = info,
                Args = argsOpt,
            });
        }
    }
}
