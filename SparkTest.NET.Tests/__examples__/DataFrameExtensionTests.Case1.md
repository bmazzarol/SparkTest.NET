# [Results](#tab/results)

|Id |
|---|
|1  |
|2  |
|3  |
|4  |
|5  |
|6  |
|7  |
|8  |
|9  |
|10 |

_(top = 20)_

# [Schema](#tab/schema)

```shell
root
 |-- Id: integer (nullable = true)

```

# [Plan](#tab/plan)

```shell
== Parsed Logical Plan ==
Relation [Id#1] json

== Analyzed Logical Plan ==
Id: int
Relation [Id#1] json

== Optimized Logical Plan ==
InMemoryRelation [Id#1], StorageLevel(disk, memory, deserialized, 1 replicas)
   +- FileScan json [Id#1] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Id:int>

== Physical Plan ==
*(1) ColumnarToRow
+- InMemoryTableScan [Id#1]
      +- InMemoryRelation [Id#1], StorageLevel(disk, memory, deserialized, 1 replicas)
            +- FileScan json [Id#1] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Id:int>

```
