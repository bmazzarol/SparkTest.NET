using System;
using System.Linq;
using System.Threading.Tasks;
using BunsenBurner;
using BunsenBurner.Verify.Xunit;
using FluentAssertions;
using Microsoft.Spark.Sql.Types;
using SparkTest.NET.Extensions;
using VerifyXunit;
using Xunit;
using static SparkTest.NET.Tests.Shared;

namespace SparkTest.NET.Tests
{
    [UsesVerify]
    public static class SparkSessionFactoryTests
    {
        [Fact(DisplayName = "A spark session can be returned and used to query")]
        public static async Task Case1() =>
            await ArrangeUsingSpark(
                    s =>
                        s.CreateDataFrameFromData(
                            new { Id = 1 },
                            Enumerable.Range(2, 9).Select(i => new { Id = i }).ToArray()
                        )
                )
                .Act(df => df.Collect())
                .Assert(c => c.Count() == 10);

        [Fact(DisplayName = "A spark data frame can be created from a custom type")]
        public static async Task Case2() =>
            await ArrangeUsingSpark(
                    session => session.CreateDataFrameFromData(new { A = 1, B = "2", C = 3.0 })
                )
                .Act(df => df.Collect())
                .Assert(rows => rows.Count() == 1)
                .And(
                    rows =>
                        rows.First().Values.Should().BeEquivalentTo(new object[] { 1, "2", 3.0 })
                );

        [Fact(DisplayName = "All the spark types are supported from dotnet custom types")]
        public static async Task Case3() =>
            await ArrangeUsingSpark(
                    s =>
                        s.CreateDataFrameFromData(
                            new
                            {
                                Bool = true,
                                Int = 1,
                                Long = (long)1,
                                Double = 0.0,
                                Date = new Date(DateTime.Now),
                                Timestamp = new Timestamp(DateTime.Now),
                                String = ""
                            }
                        )
                )
                .Act(df => df.Collect().First().Schema.Json)
                .AssertResultIsUnchanged();

        [Fact(DisplayName = "Nested objects are supported")]
        public static async Task Case4() =>
            await new
            {
                Id = 1,
                Details = new
                {
                    Name = "Ben",
                    Age = 20,
                    NumericTypes = new
                    {
                        Byte = (byte)0x0,
                        Short = (short)1,
                        Decimal = (decimal)0.0,
                        Float = (float)0.0
                    }
                }
            }
                .ArrangeData()
                .Act(data => data.AsStructType().Json)
                .AssertResultIsUnchanged();

        [Fact(DisplayName = "Nullable objects are supported")]
        public static async Task Case6() =>
            await new { NullableInt = (int?)null }
                .ArrangeData()
                .Act(data => data.AsStructType().Json)
                .AssertResultIsUnchanged();

        [Fact(DisplayName = "Objects that are not supported in Spark throw")]
        public static async Task Case7() =>
            await new { Guid = Guid.NewGuid() }
                .ArrangeData()
                .Act(data => data.AsStructType())
                .AssertFailsWith(
                    (NotSupportedException e) =>
                        e.Message.Should().Be("System.Guid is not supported in Spark")
                );

        [Fact(DisplayName = "Empty data frame returns a single empty row")]
        public static async Task Case8() =>
            await ArrangeUsingSpark(s => s.CreateEmptyFrame())
                .Act(df => df.Collect().ToList())
                .Assert(rows =>
                {
                    rows.Should().HaveCount(1);
                    rows.SelectMany(x => x.Values).Should().BeEmpty();
                });
    }
}
