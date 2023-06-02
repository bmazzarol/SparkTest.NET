# [Results](#tab/results)

|Array    |
|---------|
|[1, 2, 3]|
|[4, 5, 6]|

_(top = 20)_

# [Schema](#tab/schema)

```shell
root
 |-- Array: array (nullable = true)
 |    |-- element: integer (containsNull = true)

```

# [Plan](#tab/plan)

```shell
== Parsed Logical Plan ==
Relation [Array#698] json

== Analyzed Logical Plan ==
Array: array<int>
Relation [Array#698] json

== Optimized Logical Plan ==
InMemoryRelation [Array#698], StorageLevel(disk, memory, deserialized, 1 replicas)
   +- FileScan json [Array#698] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Array:array<int>>

== Physical Plan ==
InMemoryTableScan [Array#698]
   +- InMemoryRelation [Array#698], StorageLevel(disk, memory, deserialized, 1 replicas)
         +- FileScan json [Array#698] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Array:array<int>>

```
