using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
namespace CSharpAnalyzer.Collectors
{
    public class PragmaWarningDirectiveTriviaCollector : CSharpSyntaxWalker
    {
        public PragmaWarningDirectiveTriviaCollector() : base(SyntaxWalkerDepth.StructuredTrivia) { }
        private readonly List<KeyValuePair<string, string>> pragmas = new List<KeyValuePair<string, string>>();
        public IReadOnlyList<KeyValuePair<string, string>> Pragmas { get { return pragmas; } }
        public override void VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
            var res = node.ToString().Split(new char[] { }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (res.Length >= 2)
                this.pragmas.Add(new KeyValuePair<string, string>(res[1], res[2]));
        }
    }

    public class PragmaWarningDirectiveTriviaCollectorTest
    {
        [Fact(DisplayName = "Success to parse a pragma")]
        public void f()
        {
            var input = "#pragma warning disable warning-list";
            var tree = CSharpSyntaxTree.ParseText(input);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var collector = new PragmaWarningDirectiveTriviaCollector();
            collector.Visit(root);
            Assert.Equal(1, collector.Pragmas.Count);
            Assert.Equal("warning", collector.Pragmas.First().Key);
            Assert.Equal("disable warning-list", collector.Pragmas.First().Value);

        }
        [Fact(DisplayName = "Success to parse pragmas")]
        public void g()
        {
            var input =
@"#pragma checksum ""abcde"" ""{406EA660-64CF-4C82-B6F0-42D48172A799}"" ""ab007f1d23d9"" //ignored
#pragma     warning   enable warning-list
#pragma    abc    def
";
            var tree = CSharpSyntaxTree.ParseText(input);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var collector = new PragmaWarningDirectiveTriviaCollector();
            collector.Visit(root);
            var keys = new string[] { "warning", "abc" }.ToHashSet();
            var values = new string[] { "enable warning-list", "def" }.ToHashSet();
            Assert.Equal(2, collector.Pragmas.Count);
            foreach (var x in collector.Pragmas)
            {
                Assert.Contains(x.Key, keys);
                Assert.Contains(x.Value, values);
            }

        }
    }
}
