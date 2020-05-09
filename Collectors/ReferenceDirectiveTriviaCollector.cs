using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace CSharpAnalyzer.Collectors
{
    public class ReferenceDirectiveTriviaCollector : CSharpSyntaxWalker
    {
        public ReferenceDirectiveTriviaCollector() : base(SyntaxWalkerDepth.StructuredTrivia) { }
        private readonly List<string> reference = new List<string>();
        public IReadOnlyCollection<string> Reference { get { return reference.AsReadOnly(); } }
        public override void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
            this.reference.Add(node.File.ValueText);
        }
    }
    // TODO あとでテストプロジェクトに移す
    public class ReferenceDirectiveTriviaCollectorTest
    {
        [Fact(DisplayName = "Success to parse a dll")]
        public void f()
        {
            var input =
@"#r ""./a/b.dll""
using System;
";
            var tree = CSharpSyntaxTree.ParseText(input);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var collector = new ReferenceDirectiveTriviaCollector();
            collector.Visit(root);
            Assert.Equal(1, collector.Reference.Count);
            Assert.Equal("./a/b.dll", collector.Reference.First());

        }
        [Fact(DisplayName = "Success to parse dlls")]
        public void g()
        {
            var input =
@"#r ""./a/b.dll""
#r ""./c/d.dll""
#r ""./e.dll""
using System;
";
            var tree = CSharpSyntaxTree.ParseText(input);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var collector = new ReferenceDirectiveTriviaCollector();
            collector.Visit(root);
            var ret = new string[] { "./a/b.dll", "./c/d.dll", "./e.dll" }.ToHashSet();
            Assert.Equal(3, collector.Reference.Count);
            foreach (var x in collector.Reference)
                Assert.Contains(x, ret);

        }

    }
}
