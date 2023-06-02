# [Results](#tab/results)

|Name |
|-----|
|John |
|Steve|
|Jess |

_(top = 20)_

# [Schema](#tab/schema)

```shell
root
 |-- Name: string (nullable = true)

```

# [Plan](#tab/plan)

```shell
== Parsed Logical Plan ==
Relation [Name#804] json

== Analyzed Logical Plan ==
Name: string
Relation [Name#804] json

== Optimized Logical Plan ==
InMemoryRelation [Name#804], StorageLevel(disk, memory, deserialized, 1 replicas)
   +- FileScan json [Name#804] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Name:string>

== Physical Plan ==
InMemoryTableScan [Name#804]
   +- InMemoryRelation [Name#804], StorageLevel(disk, memory, deserialized, 1 replicas)
         +- FileScan json [Name#804] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Name:string>

```
