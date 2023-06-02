# Getting Started

To use this library, simply include `SparkTest.NET.dll` in your project or grab
it from [NuGet](https://www.nuget.org/packages/SparkTest.NET/), and add this to
the top of each test `.cs` file
that needs it:

```C#
using static SparkTest.NET.SparkSessionFactory;
```

Then you can use it in a test project like this,

[!code-csharp[](../../SparkTest.NET.Tests/SparkSessionFactoryTests.cs#ExampleUseSession)]
