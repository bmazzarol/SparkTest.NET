using System;
using BunsenBurner;
using Microsoft.Spark.Sql;
using static SparkTest.NET.SparkSessionFactory;
using Scenario = BunsenBurner.Scenario<BunsenBurner.Syntax.Aaa>;

namespace SparkTest.NET.Tests
{
    internal static class Shared
    {
        internal static Scenario.Arranged<T> ArrangeUsingSpark<T>(Func<SparkSession, T> arrange) =>
            UseSession(arrange).ArrangeData();
    }
}
