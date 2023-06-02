# Test DataFrames

To build a test @Microsoft.Spark.Sql.DataFrame do the following,

Import the extensions,

```csharp
using SparkTest.NET.Extensions;
```

## Simple Usage

The @SparkTest.NET.Extensions.DataFrameExtensions.CreateDataFrameFromData*
creates a @Microsoft.Spark.Sql.DataFrame from test data

For example a simple single column numeric data set,

[!code-csharp[](../../SparkTest.NET.Tests/DataFrameExtensionTests.cs#SimpleDataFrameUsage)]

[!INCLUDE [SimpleDataFrameUsage](../../SparkTest.NET.Tests/__examples__/DataFrameExtensionTests.Case1.md)]

## Primitive types

All the primitive .NET types that are supported by Spark can be used,

[!code-csharp[](../../SparkTest.NET.Tests/DataFrameExtensionTests.cs#CreateDataFrameAllPrimitive)]

[!INCLUDE [CreateDataFrameAllPrimitive](../../SparkTest.NET.Tests/__examples__/DataFrameExtensionTests.Case2.md)]

## Collections (Spark Array)

Anything that extends @System.Collections.Generic.IEnumerable`1 can be used to
create Spark Array data,

[!code-csharp[](../../SparkTest.NET.Tests/DataFrameExtensionTests.cs#CreateDataFrameArray)]

[!INCLUDE [CreateDataFrameArray](../../SparkTest.NET.Tests/__examples__/DataFrameExtensionTests.Case3.md)]

## Dictionary (Spark Map)

Anything that extends @System.Collections.Generic.IEnumerable`1
of [KeyValuePair](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2)
can be used to create Spark Map data,

[!code-csharp[](../../SparkTest.NET.Tests/DataFrameExtensionTests.cs#CreateDataFrameMap)]

[!INCLUDE [CreateDataFrameMap](../../SparkTest.NET.Tests/__examples__/DataFrameExtensionTests.Case4.md)]

## Classes (Spark Struct)

Any POCO can be used to create Spark Struct data,

[!code-csharp[](../../SparkTest.NET.Tests/DataFrameExtensionTests.cs#CreateDataFrameStruct)]

[!INCLUDE [CreateDataFrameStruct](../../SparkTest.NET.Tests/__examples__/DataFrameExtensionTests.Case6.md)]

## Empty Frame

You can also create an empty @Microsoft.Spark.Sql.DataFrame that can be used to
test columns out,

[!code-csharp[](../../SparkTest.NET.Tests/SparkSessionFactoryTests.cs#EmptyDataFrame)]
