<!-- markdownlint-disable MD033 MD041 -->
<div align="center">

<img src="test-tubes-icon.png" alt="SparkTest.NET" width="150px"/>

# SparkTest.NET

> Due to inactivity and lack of support for [spark.NET](https://github.com/dotnet/spark)
> this has been archived.
> I would recommend building Spark applications in a supported language, not
> in dotnet.

[:running: **_Getting Started_**](https://bmazzarol.github.io/SparkTest.NET/articles/getting-started.html)
[:books: **_Documentation_**](https://bmazzarol.github.io/SparkTest.NET)

[![Nuget](https://img.shields.io/nuget/v/SparkTest.NET)](https://www.nuget.org/packages/SparkTest.NET/)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=bmazzarol_SparkTest.NET&metric=coverage)](https://sonarcloud.io/summary/new_code?id=bmazzarol_SparkTest.NET)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=bmazzarol_SparkTest.NET&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=bmazzarol_SparkTest.NET)
[![CD Build](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/cd-build.yml/badge.svg)](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/cd-build.yml)
[![Check Markdown](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/check-markdown.yml/badge.svg)](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/check-markdown.yml)
[![CodeQL](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/codeql.yml/badge.svg)](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/codeql.yml)

Support for testing :test_tube: Spark dotnet applications

</div>

## Features

* Test framework agnostic
* Zero dependencies (except spark dotnet!)
* Easy to use
* Stable and fast spark debug usage tailored to testing

```c#
// import the factory
using static SparkTest.NET.SparkSessionFactory;

...

// Then you can use a spark session within a test
[Fact]
public static void SomeTest()
{
    var result = UseSession(session =>
    {
        // session is safe to use here
        ...
        // return something to assert against
        return ...
    });
    ...
}
```

## Getting Started

To use this library, simply include `SparkTest.NET.dll` in your project or grab
it from [NuGet](https://www.nuget.org/packages/SparkTest.NET/), and add this to
the top of each test `.cs` file
that needs it:

```C#
using static SparkTest.NET.SparkSessionFactory;
```

## Why?

There is no documented/supported way to write tests for Spark dotnet
applications.

There are a number of foot guns and this aims to disarm them.

* spark-debug needs to run from the executing location of the tests assembly,
  this starts spark-debug in the correct location, and ensures that it is
  stopped after all the tests are complete
  * If spark-debug is already running, it will not be started, good for CI
* spark-debug without any tuning is not optimized for short running tests, this,
  * Sets the log level to Error (default is super chatty)
  * Disables the Spark UI settings
  * Stops Spark from spreading out and localizes work
  * Disables shuffling for Spark SQL
  * Enables all cores on the spark submit job
* SparkSession is not thread safe, its backed by a single SparkContext, this
  enforces that all operations that need a SparkSession run sequentially

The following settings configure the spark-submit process,

They can be set via the environment or from the test csproj file,

<!-- markdownlint-disable MD013 -->

| AssemblyMetadata Key             | Environment Variable Name | Required? | Description                                                                                                                               |
|----------------------------------|---------------------------|-----------|-------------------------------------------------------------------------------------------------------------------------------------------|
| SparkTest.NET.SparkHome          | SPARK_HOME                | Yes       | Location of spark on the system, used to start spark-submit                                                                               |
| SparkTest.NET.SparkDotnetJarName | SPARK_DOTNET_JAR_NAME     | Yes       | Name of the microsoft-spark jar file to use for example 'microsoft-spark-3-2_2.12-2.1.1.jar'                                              |
| SparkTest.NET.ExtraJars          | SPARK_DEBUG_EXTRA_JARS    | No        | Optional extra jar files to include from the executing test location, for example 'azure-storage-3.1.0.jar,spark-xml-assembly-0.14.0.jar' |

<!-- markdownlint-enable MD013 -->

eg.

```xml

<ItemGroup>
    <AssemblyMetadata Include="SparkTest.NET.ExtraJars"
                      Value="azure-storage-3.1.0.jar,spark-xml-assembly-0.14.0.jar"/>
</ItemGroup>
```

or

```shell
export SPARK_DEBUG_EXTRA_JARS=azure-storage-3.1.0.jar,spark-xml-assembly-0.14.0.jar
```

For more details/information have a look the test projects or create an issue.

## Attributions

[icons created by juicy_fish](https://www.flaticon.com/free-icons/test-tubes)
