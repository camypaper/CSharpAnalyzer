### これはなに？
Roslyn を使ってソースコード(`.cs`, `.csx`)を解析して、いろいろ情報を抜き出すやつ

下のようなコードから以下のような情報を(静的解析をして)抜き出す。
- `hoge.dll`
- `pragma` の中身(`abc def`)
- `VerifyTypeAttribute` によって指定されたある特定の型(ここでは `System.Int32` や `System.Console`)

```cs
#r "hoge.dll"
#pragma abc def
using System;
[VerifyType(typeof(int),typeof(System.Console))]
```

### つかいかた
導入
```sh
CSharpAnalyzer$dotnet pack
CSharpAnalyzer$dotnet tool install --add-source ./nupkg CSharpAnalyzer --version 1.0.0 -g
```
使う
```sh
$dotnet dependencies
```

### 依存しているもの
- [Roslyn](https://github.com/dotnet/roslyn)
    - C# コンパイラ
- [Xunit](https://github.com/xunit/xunit)
    - テストライブラリ(現在は 1 つの project にくっついてしまっているが、そのうち切り離す)
