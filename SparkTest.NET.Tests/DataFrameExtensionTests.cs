﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BunsenBurner;
using BunsenBurner.Utility;
using BunsenBurner.Verify.Xunit;
using Docfx.ResultSnippets;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Spark.Sql.Types;
using SparkTest.NET.Extensions;
using VerifyXunit;
using Xunit;
using static SparkTest.NET.Tests.Shared;

namespace SparkTest.NET.Tests
{
    [UsesVerify]
    public static class DataFrameExtensionTests
    {
        [Fact(DisplayName = "A data frame can be created from JSON data")]
        public static async Task Case1() =>
            await ArrangeUsingSpark(s =>
                {
                    #region SimpleDataFrameUsage

                    var df = s.CreateDataFrameFromData(
                        new { Id = 1 },
                        Enumerable.Range(2, 9).Select(i => new { Id = i }).ToArray()
                    );

                    #endregion

                    df.ShowMdString().SaveResults();
                    return df;
                })
                .Act(df => df.Debug())
                .AssertResultIsUnchanged();

        [Fact(DisplayName = "All primitive C# types can be loaded into spark")]
        public static async Task Case2() =>
            await ArrangeUsingSpark(s =>
                {
                    #region CreateDataFrameAllPrimitive

                    var df = s.CreateDataFrameFromData(
                        new
                        {
                            Byte = (byte)1,
                            Short = (short)1,
                            Int = 1,
                            Long = 1L,
                            Float = 1.0F,
                            Double = 1.0,
                            Decimal = (decimal)1.0,
                            String = "a",
                            Char = 'a',
                            Bool = true,
                            Date = DateTime.MinValue,
                            SparkDate = new Date(DateTime.MinValue),
                            DateTimeOffset = DateTimeOffset.MinValue,
                            Timestamp = new Timestamp(DateTime.MinValue),
                            Binary = new byte[] { 1 },
                            Enum = LogLevel.None
                        },
                        new
                        {
                            Byte = (byte)2,
                            Short = (short)2,
                            Int = 2,
                            Long = 2L,
                            Float = 2.0F,
                            Double = 2.0,
                            Decimal = (decimal)2.0,
                            String = "b",
                            Char = 'b',
                            Bool = false,
                            Date = DateTime.MaxValue,
                            SparkDate = new Date(DateTime.MaxValue),
                            DateTimeOffset = DateTimeOffset.MaxValue,
                            Timestamp = new Timestamp(DateTimeOffset.MaxValue.UtcDateTime),
                            Binary = new byte[] { 1 },
                            Enum = LogLevel.Critical
                        }
                    );

                    #endregion

                    df.ShowMdString().SaveResults();
                    return df;
                })
                .Act(df => df.Debug())
                .AssertResultIsUnchanged();

        [Fact(DisplayName = "Array columns can be loaded into spark")]
        public static async Task Case3() =>
            await ArrangeUsingSpark(s =>
                {
                    #region CreateDataFrameArray

                    var df = s.CreateDataFrameFromData(
                        new { Array = new[] { 1, 2, 3 } },
                        new { Array = new[] { 4, 5, 6 } }
                    );

                    #endregion

                    df.ShowMdString().SaveResults();
                    return df;
                })
                .Act(df => df.Debug())
                .AssertResultIsUnchanged();

        [Fact(DisplayName = "Map/Dictionary columns can be loaded into spark")]
        public static async Task Case4() =>
            await ArrangeUsingSpark(s =>
                {
                    #region CreateDataFrameMap

                    var df = s.CreateDataFrameFromData(
                        new
                        {
                            Map = new Dictionary<string, int>
                            {
                                ["A"] = 1,
                                ["B"] = 2,
                                ["C"] = 3
                            }
                        },
                        new
                        {
                            Map = new Dictionary<string, int>
                            {
                                ["D"] = 4,
                                ["E"] = 5,
                                ["F"] = 6
                            }
                        }
                    );

                    #endregion

                    df.ShowMdString().SaveResults();
                    return df;
                })
                .Act(df => df.Debug())
                .AssertResultIsUnchanged();

        [Fact(DisplayName = "An empty enumerable will throw")]
        public static async Task Case5() =>
            await ArrangeUsingSpark(ManualDisposal.New)
                .Act(s => s.Value.CreateDataFrameFromData(Enumerable.Empty<object>()))
                .AssertFailsWith(
                    (InvalidOperationException e) =>
                        e.Message.Should().Be("Enumerable must not be empty")
                );

        [Fact(DisplayName = "Custom record columns can be loaded into spark")]
        public static async Task Case6() =>
            await ArrangeUsingSpark(s =>
                {
                    #region CreateDataFrameStruct

                    var df = s.CreateDataFrameFromData(
                        new
                        {
                            CreatedDate = DateTimeOffset.MinValue,
                            Person = new
                            {
                                Name = "Billy",
                                Age = 12,
                                Interests = new[]
                                {
                                    new { Priority = 1, Name = "Football" },
                                    new { Priority = 3, Name = "Cars" }
                                }
                            }
                        },
                        new
                        {
                            CreatedDate = DateTimeOffset.MinValue,
                            Person = new
                            {
                                Name = "James",
                                Age = 11,
                                Interests = new[]
                                {
                                    new { Priority = 1, Name = "Video Games" },
                                    new { Priority = 2, Name = "TV" }
                                }
                            }
                        }
                    );

                    #endregion

                    df.ShowMdString().SaveResults();
                    return df;
                })
                .Act(df => df.Debug())
                .AssertResultIsUnchanged();

        [Fact(DisplayName = "Explain plan indexes can be removed")]
        public static async Task Case7() =>
            await ArrangeUsingSpark(
                    s =>
                        s.CreateDataFrameFromData(
                            new { Id = 1 },
                            Enumerable.Range(2, 9).Select(i => new { Id = i }).ToArray()
                        )
                )
                .Act(df => df.ExplainString().ReIndexExplainPlan(true))
                .Assert(ep => ep.Should().NotContain("#"));

        [Fact(DisplayName = "ShowMdString with no selected options is an empty string")]
        public static async Task Case8() =>
            await ArrangeUsingSpark(
                    s =>
                        s.CreateDataFrameFromData(
                            new { Id = 1 },
                            Enumerable.Range(2, 9).Select(i => new { Id = i }).ToArray()
                        )
                )
                .Act(df => df.ShowMdString(showResults: false, showPlan: false, showSchema: false))
                .Assert(md => md.Should().BeEmpty());

        [Fact(DisplayName = "ShowMdString with 1 selected option is only that option not a tab")]
        public static async Task Case9() =>
            await ArrangeUsingSpark(
                    s =>
                        s.CreateDataFrameFromData(
                            new { Id = 1 },
                            Enumerable.Range(2, 9).Select(i => new { Id = i }).ToArray()
                        )
                )
                .Act(df => df.ShowMdString(showResults: true, showPlan: false, showSchema: false))
                .AssertResultIsUnchanged();
    }
}
