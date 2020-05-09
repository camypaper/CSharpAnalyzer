using System;
using System.IO;
using System.Linq;
using CSharpAnalyzer.Records;
namespace CSharpAnalyzer
{
    class Program
    {
        static void Main()
        {
            var result = new Meta();
            foreach (var x in Directory.EnumerateFiles(".", "*", SearchOption.AllDirectories)
                                        .Where(x => x.EndsWith(".cs") || x.EndsWith(".csx")))
            {
                var parser = new Parser(x);
                parser.Parse();
                foreach (var y in parser.DependendClasses)
                    result.Add(y, x);
            }
            Console.WriteLine(result.PrintJson(true));
        }
    }
}
