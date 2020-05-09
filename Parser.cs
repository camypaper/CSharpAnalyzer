using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpAnalyzer.Collectors;
using CSharpAnalyzer.Attributes;
using CSharpAnalyzer.Logging;
namespace CSharpAnalyzer
{
    public class Parser
    {
        private string normalize(string filePath, string binaryPath)
        {
            if (Path.IsPathFullyQualified(binaryPath)) return binaryPath;
            var file = new FileInfo(filePath);
            return Path.Combine(file.DirectoryName, binaryPath);
        }
        public Parser(string filePath)
        {
            FilePath = filePath;
        }
        public string FilePath { get; private set; }
        public string Text { get; private set; }
        public CompilationUnitSyntax Root { get; private set; }
        public SemanticModel Model { get; private set; }
        public IReadOnlyCollection<string> DependendClasses { get; private set; }
        private IEnumerable<string> analyze(AttributeSyntax attribute)
        {
            var collector = new TypeOfExpressionCollector();
            collector.Visit(attribute);
            foreach (var x in collector.Types)
            {
                var typeInfo = Model.GetTypeInfo(x);
                var typeSymbol = (INamedTypeSymbol)typeInfo.Type;
                if (typeSymbol.SpecialType != SpecialType.None)
                {
                    Logger.Warn("Special type(int, string, etc.) is not supported");
                }
                else
                {
                    yield return $"{typeSymbol.ContainingNamespace.ToDisplayString()}.{typeSymbol.OriginalDefinition.MetadataName}";
                }
            }
        }
        public void Parse()
        {
            Logger.Info($"Start to parse '{this.FilePath}'");
            this.Text = File.ReadAllText(this.FilePath);
            var tree = CSharpSyntaxTree.ParseText(this.Text);
            this.Root = (CompilationUnitSyntax)tree.GetRoot();

            //reference 
            var refCollector = new ReferenceDirectiveTriviaCollector();
            refCollector.Visit(Root);
            Logger.Info($"{refCollector.Reference.Count} references found. {string.Join(',', refCollector.Reference)}");

            var compilation = CSharpCompilation.Create("temp")
                                                           .AddReferences(
                                                                MetadataReference.CreateFromFile(
                                                                    typeof(object).Assembly.Location))
                                                           .AddReferences(
                                                               refCollector.Reference.Select(x => normalize(this.FilePath, x)).Select(x => MetadataReference.CreateFromFile(x))
                                                           )
                                                           .AddSyntaxTrees(tree);
            this.Model = compilation.GetSemanticModel(tree);
            Logger.Info("Success to construct semantic model.");

            var attributeCollector = new AttributeCollector();
            attributeCollector.Visit(Root);
            Logger.Info($"{attributeCollector.Attributes.Count} attributes found. {string.Join(',', attributeCollector.Attributes)}");
            this.DependendClasses = attributeCollector.Attributes
                            .Where(x => nameof(VerifyTypeAttribute).Contains(x.Name.ToString()))
                            .Select(analyze)
                            .SelectMany(x => x)
                            .Distinct().ToList().AsReadOnly();
            Logger.Info($"{this.DependendClasses.Count} dependend classes found. {string.Join(',', this.DependendClasses)}");
        }
    }
}
