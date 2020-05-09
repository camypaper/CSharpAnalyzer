using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace CSharpAnalyzer.Collectors
{
    public class AttributeCollector : CSharpSyntaxWalker
    {
        public AttributeCollector() : base(SyntaxWalkerDepth.StructuredTrivia) { }
        readonly List<AttributeSyntax> attributes = new List<AttributeSyntax>();
        public IReadOnlyCollection<AttributeSyntax> Attributes { get { return attributes.AsReadOnly(); } }
        public override void VisitAttribute(AttributeSyntax node)
        {
            attributes.Add(node);
        }
    }
    // TODO あとでテストプロジェクトに移す
    public class AttributeCollectorCollectorTest
    {
        [Fact(DisplayName = "Success to parse an attribute")]
        public void f()
        {
            var input =
    @"using System;
[VerifyType(typeof(int),typeof(string))]
";
            var tree = CSharpSyntaxTree.ParseText(input);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var collector = new AttributeCollector();
            collector.Visit(root);
            Assert.Equal(1, collector.Attributes.Count);
            Assert.Equal("VerifyType", collector.Attributes.First().Name.ToString());

        }
    }
}
