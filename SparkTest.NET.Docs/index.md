<!-- markdownlint-disable MD033 MD041 -->
<div align="center">

<img src="images/test-tubes-icon.png" alt="SparkTest.NET" width="150px"/>

# SparkTest.NET

---

[![Nuget](https://img.shields.io/nuget/v/SparkTest.NET)](https://www.nuget.org/packages/SparkTest.NET/)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=bmazzarol_SparkTest.NET&metric=coverage)](https://sonarcloud.io/summary/new_code?id=bmazzarol_SparkTest.NET)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=bmazzarol_SparkTest.NET&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=bmazzarol_SparkTest.NET)
[![CD Build](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/cd-build.yml/badge.svg)](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/cd-build.yml)
[![Check Markdown](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/check-markdown.yml/badge.svg)](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/check-markdown.yml)
[![CodeQL](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/codeql.yml/badge.svg)](https://github.com/bmazzarol/SparkTest.NET/actions/workflows/codeql.yml)

Support for testing Spark .NET applications

---

</div>

## Why?

There is no documented/supported way to write tests for Spark .NET
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
* @Microsoft.Spark.Sql.SparkSession is not thread safe, its backed by a single
  [SparkContext](https://learn.microsoft.com/en-gb/dotnet/api/microsoft.spark.sparkcontext?view=spark-dotnet),
  this enforces that all operations that need a
  @Microsoft.Spark.Sql.SparkSession run sequentially

## How?

The
[`SparkSessionFactory.UseSession`](xref:SparkTest.NET.SparkSessionFactory.UseSession*)
function will ensure that a spark-debug process is running before returning a
session that will then have exclusive access to Spark to run the user provided
operation.

The following details the sequence of operations,

```mermaid
sequenceDiagram
    participant t as User Code
    participant a as UseSession
    participant b as Spark Debug
    t ->> a: Run code with SparkSession
    note over t, a: This is a singleton operation under a lock
    a ->> b: Create SparkSession
    alt Spark Debug not running
        b -->> a: No response
        a ->> b: Start spark-debug
    end
    b -->> a: Session is active and ready to use
    a -->> t: Run user code against SparkSession returning result 
```
