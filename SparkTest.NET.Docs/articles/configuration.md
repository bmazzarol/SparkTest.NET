# Configuration

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
