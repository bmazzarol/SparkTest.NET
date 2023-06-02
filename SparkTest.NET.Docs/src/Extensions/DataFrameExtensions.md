---
uid: SparkTest.NET.Extensions.DataFrameExtensions.AsSparkType(System.Type)
example: 
  - Simple types,

    [!code-csharp[](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#SimpleType1)]
      
    [!code-csharp[](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#SimpleType2)]
  - Array like types,

    [!code-csharp[](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#ArrayType1)]
      
    [!code-csharp[](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#ArrayType2)]
  - Map like types,

    [!code-csharp[](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#MapType1)]

    [!code-csharp[](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#MapType2)]  
  - Struct like types,

    [!code-csharp[](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#StructType1)]
---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.AsStructType(System.Type)
example:
- Converting a simple POCO to a struct type

  [!code-csharp[ ](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#StructType2)]
---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.AsStructType``1(``0)
example:
- Converting a anonymous object to a struct type

  [!code-csharp[ ](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#StructType3)]
---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.CreateDataFrameFromData``1(Microsoft.Spark.Sql.SparkSession,``0,``0[])
example:
- Creating a @Microsoft.Spark.Sql.DataFrame from first and rest provided data,

  [!code-csharp[ ](../../../SparkTest.NET.Tests/SparkSessionFactoryTests.cs#CreateDataFrameFromDataFirstRestExample)]
---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.CreateDataFrameFromData``1(Microsoft.Spark.Sql.SparkSession,System.Collections.Generic.IEnumerable{``0})
example:
- Creating a @Microsoft.Spark.Sql.DataFrame from a provided enumerable,

  [!code-csharp[ ](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#CreateDataFrameFromDataEnumerableExample)]

  [!INCLUDE [CreateDataFrameFromDataEnumerableExample](../../../SparkTest.NET.Tests/__examples__/DataFrameExtensionExamples.Case10.md)]
---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.CreateEmptyFrame(Microsoft.Spark.Sql.SparkSession)
example:
- Creating an empty @Microsoft.Spark.Sql.DataFrame,

  [!code-csharp[ ](../../../SparkTest.NET.Tests/SparkSessionFactoryTests.cs?highlight=4#EmptyDataFrame)]
---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.CreateStructType``1
example:
- Converting a simple POCO to a struct type

  [!code-csharp[ ](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#StructType4)]
---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.Debug(Microsoft.Spark.Sql.DataFrame,System.Int32,System.Int32,System.Boolean)
example:
- Example Debug usage for a @Microsoft.Spark.Sql.DataFrame,

  [!code-csharp[ ](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#DebugExample)]

  Results in,

  [!INCLUDE [DebugExample](../../../SparkTest.NET.Tests/__examples__/DataFrameExtensionExamples.Case12.md)]
---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.PrintSchemaString(Microsoft.Spark.Sql.DataFrame)
example:
- Example PrintSchemaString usage for a @Microsoft.Spark.Sql.DataFrame,

  [!code-csharp[ ](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#PrintSchemaStringExample)]

  Results in,

  [!INCLUDE [PrintSchemaStringExample](../../../SparkTest.NET.Tests/__examples__/DataFrameExtensionExamples.Case13.md)]

---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.ShowMdString(Microsoft.Spark.Sql.DataFrame,System.Int32,System.Int32,System.Boolean)
example:
- Example ShowMdString usage for a @Microsoft.Spark.Sql.DataFrame,

  [!code-csharp[ ](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#ShowMdStringExample)]

  Results in,

  [!INCLUDE [ShowMdStringExample](../../../SparkTest.NET.Tests/__examples__/DataFrameExtensionExamples.Case14.md)]

---

---
uid: SparkTest.NET.Extensions.DataFrameExtensions.ShowString(Microsoft.Spark.Sql.DataFrame,System.Int32,System.Int32,System.Boolean)
example:
- Example ShowString usage for a @Microsoft.Spark.Sql.DataFrame,

  [!code-csharp[ ](../../../SparkTest.NET.Tests/DataFrameExtensionExamples.cs#ShowStringExample)]

  Results in,

  [!INCLUDE [ShowStringExample](../../../SparkTest.NET.Tests/__examples__/DataFrameExtensionExamples.Case15.md)]

---
