using System.Collections.Generic;
using System.Linq;
using Docfx.ResultSnippets;
using FluentAssertions;
using Microsoft.Spark.Sql.Types;
using SparkTest.NET.Extensions;
using Xunit;

namespace SparkTest.NET.Tests
{
    public static class DataFrameExtensionExamples
    {
        [Fact(DisplayName = "string can be converted to a Spark StringType")]
        public static void Case1() =>
            #region SimpleType1

            typeof(string).AsSparkType().Should().Be(new StringType());

            #endregion

        [Fact(DisplayName = "long can be converted to a Spark LongType")]
        public static void Case2() =>
            #region SimpleType2

            typeof(long).AsSparkType().Should().Be(new LongType());

            #endregion

        [Fact(DisplayName = "array of int can be converted to a Spark ArrayType of IntegerType")]
        public static void Case3() =>
            #region ArrayType1

            typeof(int[]).AsSparkType().Should().Be(new ArrayType(new IntegerType()));

            #endregion

        [Fact(
            DisplayName = "enumerable of float can be converted to a Spark ArrayType of FloatType"
        )]
        public static void Case4() =>
            #region ArrayType2

            typeof(IEnumerable<float>).AsSparkType().Should().Be(new ArrayType(new FloatType()));

            #endregion

        [Fact(
            DisplayName = "dictionary of int string can be converted to a Spark MapType of IntegerType, StringType"
        )]
        public static void Case5() =>
            #region MapType1

            typeof(Dictionary<int, string>)
                .AsSparkType()
                .Should()
                .Be(new MapType(new IntegerType(), new StringType()));

            #endregion

        [Fact(
            DisplayName = "Enumerable of KeyValuePair of long byte[] can be converted to a Spark MapType of LongType, BinaryType"
        )]
        public static void Case6() =>
            #region MapType2

            typeof(IEnumerable<KeyValuePair<long, byte[]>>)
                .AsSparkType()
                .Should()
                .Be(new MapType(new LongType(), new BinaryType()));

            #endregion

        #region StructType1

        enum Colour
        {
            Red,
            Green,
            Blue
        }

        class Car
        {
            public string? Name { get; set; }
            public int Age { get; set; }
            public Colour Colour { get; set; }
        }

        [Fact(DisplayName = "POCOs can be converted to Spark Struct types, Example 1")]
        public static void Case7() =>
            typeof(Car)
                .AsSparkType()
                .Should()
                .Be(
                    new StructType(
                        new[]
                        {
                            new StructField("Name", new StringType()),
                            new StructField("Age", new IntegerType()),
                            new StructField("Colour", new StringType()),
                        }
                    )
                );

        #endregion

        #region StructType2

        class Person
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }

        [Fact(DisplayName = "POCOs can be converted to Spark Struct types, Example 2")]
        public static void Case8() =>
            typeof(Person)
                .AsStructType()
                .Should()
                .Be(
                    new StructType(
                        new[]
                        {
                            new StructField("Name", new StringType()),
                            new StructField("Age", new IntegerType())
                        }
                    )
                );

        #endregion

        [Fact(DisplayName = "Anonymous objects can be converted to Spark Struct types")]
        public static void Case9()
        {
            #region StructType3

            var customPerson = new
            {
                FirstName = "James",
                LastName = "Stanley",
                Hobby = "Cycling"
            };
            customPerson
                .AsStructType()
                .Should()
                .Be(
                    new StructType(
                        new[]
                        {
                            new StructField("FirstName", new StringType()),
                            new StructField("LastName", new StringType()),
                            new StructField("Hobby", new StringType())
                        }
                    )
                );

            #endregion
        }

        [Fact(DisplayName = "Enumerable types can be used to create DataFrames")]
        public static void Case10() =>
            SparkSessionFactory
                .UseSession(s =>
                {
                    #region CreateDataFrameFromDataEnumerableExample

                    var list = Enumerable
                        .Range(1, 10)
                        .Select(i => new { Id = i, Name = $"Some Name {i}" });
                    var df = s.CreateDataFrameFromData(list);

                    #endregion

                    df.ShowMdString().SaveResults();
                    return df.Collect();
                })
                .Should()
                .HaveCount(10)
                .And.Subject.First()
                .Schema.Fields.Should()
                .Contain(x => x.Name == "Name");

        #region StructType4

        class Toy
        {
            public string? Name { get; set; }
            public string? Brand { get; set; }
            public decimal Cost { get; set; }
        }

        [Fact(DisplayName = "POCOs can be converted to Spark Struct types, Example 3")]
        public static void Case11() =>
            DataFrameExtensions
                .CreateStructType<Toy>()
                .Should()
                .Be(
                    new StructType(
                        new[]
                        {
                            new StructField("Name", new StringType()),
                            new StructField("Brand", new StringType()),
                            new StructField("Cost", new DecimalType())
                        }
                    )
                );

        #endregion

        [Fact(DisplayName = "Example Debug usage on a DataFrame")]
        public static void Case12()
        {
            var result = SparkSessionFactory.UseSession(s =>
            {
                #region DebugExample

                var df = s.CreateDataFrameFromData(
                    new { Name = "John" },
                    new { Name = "Steve" },
                    new { Name = "Jess" }
                );
                var debugResult = df.Debug();

                #endregion

                debugResult.AsFencedResult().SaveResults();

                return debugResult;
            });

            result.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Example PrintSchemaString usage on a DataFrame")]
        public static void Case13()
        {
            var result = SparkSessionFactory.UseSession(s =>
            {
                #region PrintSchemaStringExample

                var df = s.CreateDataFrameFromData(
                    new { Name = "John" },
                    new { Name = "Steve" },
                    new { Name = "Jess" }
                );
                var debugResult = df.PrintSchemaString();

                #endregion

                debugResult.AsFencedResult().SaveResults();

                return debugResult;
            });

            result.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Example ShowMdString usage on a DataFrame")]
        public static void Case14()
        {
            var result = SparkSessionFactory.UseSession(s =>
            {
                #region ShowMdStringExample

                var df = s.CreateDataFrameFromData(
                    new { Name = "John" },
                    new { Name = "Steve" },
                    new { Name = "Jess" }
                );
                var debugResult = df.ShowMdString();

                #endregion

                debugResult.SaveResults();

                return debugResult;
            });

            result.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Example ShowString usage on a DataFrame")]
        public static void Case15()
        {
            var result = SparkSessionFactory.UseSession(s =>
            {
                #region ShowStringExample

                var df = s.CreateDataFrameFromData(
                    new { Name = "John" },
                    new { Name = "Steve" },
                    new { Name = "Jess" }
                );
                var debugResult = df.ShowString();

                #endregion

                debugResult.AsFencedResult().SaveResults();

                return debugResult;
            });

            result.Should().NotBeNullOrEmpty();
        }
    }
}
