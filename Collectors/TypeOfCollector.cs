using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace CSharpAnalyzer.Collectors
{
    public class TypeOfExpressionCollector : CSharpSyntaxWalker
    {
        public TypeOfExpressionCollector() { }
        readonly List<TypeSyntax> types = new List<TypeSyntax>();
        public IReadOnlyCollection<TypeSyntax> Types { get { return types.AsReadOnly(); } }
        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            types.Add(node.Type);
        }
    }
    // TODO あとでテストプロジェクトに移す
    public class TypeOfExpressionCollectorTest
    {
        [Fact(DisplayName = "Success to parse typeof expression")]
        public void f()
        {
            var input =
@"using System;
[VerifyType(typeof(int))]
";
            var tree = CSharpSyntaxTree.ParseText(input);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var collector = new TypeOfExpressionCollector();
            collector.Visit(root);
            Assert.Equal(1, collector.Types.Count);
            Assert.Equal("int", collector.Types.First().ToString());
        }
        [Fact(DisplayName = "Success to parse generic type")]
        public void g()
        {
            var input =
@"using System;
using System.Collections.Generic;
[VerifyType(typeof(List<int>))]
";
            var tree = CSharpSyntaxTree.ParseText(input);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var collector = new TypeOfExpressionCollector();
            collector.Visit(root);
            Assert.Equal(1, collector.Types.Count);
            Assert.Equal("List<int>", collector.Types.First().ToString());
        }
    }
}
