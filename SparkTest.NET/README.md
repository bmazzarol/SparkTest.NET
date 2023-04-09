<!-- markdownlint-disable MD013 -->

# ![SparkTest.NET](https://raw.githubusercontent.com/bmazzarol/SparkTest.NET/main/test-tubes-icon-small.png) SparkTest.NET

<!-- markdownlint-enable MD013 -->

[![Nuget](https://img.shields.io/nuget/v/SparkTest.NET)](https://www.nuget.org/packages/SparkTest.NET/)

Support for testing Spark dotnet applications

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
