using System;

namespace CSharpAnalyzer.Attributes
{
    public class VerifyTypeAttribute : Attribute
    {
        /// <summary>
        /// この属性(と同じのもの)を使ってアノテーションのようなことをする。<c>typeof</c> キーワードを使って、型名を入れる使い方を想定。
        /// </summary>
        /// <example>
        /// <code>
        /// [VerifyType(typeof(int),typeof(System.Console))]
        /// </code>
        /// </example>
        public VerifyTypeAttribute(params Type[] a) { }
    }
}
