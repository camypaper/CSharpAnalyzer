using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using Xunit;
namespace CSharpAnalyzer.Records
{
    public class Meta
    {
        Dictionary<string, HashSet<string>> data = new Dictionary<string, HashSet<string>>();
        public Meta() { }
        public void Add(string target, string source)
        {
            if (!data.ContainsKey(target)) data[target] = new HashSet<string>();
            data[target].Add(source);
        }
        public string PrintJson(bool beautify = false)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = beautify
            };
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            return Regex.Unescape(JsonSerializer.Serialize(data, options));
        }
    }
    // TODO あとでテストプロジェクトに移す
    public class MetaTest
    {
        [Fact(DisplayName = "Success to print Json")]
        public void f()
        {
            var meta = new Meta();
            for (int i = 0; i < 3; i++) meta.Add("key0", $"value{i % 2}");
            for (int i = 0; i < 3; i++) meta.Add("key1", $"value{i}");
            const string expected = "{\"key0\":[\"value0\",\"value1\"],\"key1\":[\"value0\",\"value1\",\"value2\"]}";
            Assert.Equal(expected, meta.PrintJson());
        }
        [Fact(DisplayName = "Success to print Json with unescape")]
        public void g()
        {
            var meta = new Meta();
            meta.Add("key", $"`<>");
            const string expected = "{\"key\":[\"`<>\"]}";
            Assert.Equal(expected, meta.PrintJson());
        }
    }
}
