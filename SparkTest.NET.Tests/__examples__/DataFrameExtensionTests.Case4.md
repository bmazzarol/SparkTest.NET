# [Results](#tab/results)

|Map                     |
|------------------------|
|{A -> 1, B -> 2, C -> 3}|
|{D -> 4, E -> 5, F -> 6}|

_(top = 20)_

# [Schema](#tab/schema)

```shell
root
 |-- Map: map (nullable = true)
 |    |-- key: string
 |    |-- value: integer (valueContainsNull = true)

```

# [Plan](#tab/plan)

```shell
== Parsed Logical Plan ==
Relation [Map#592] json

== Analyzed Logical Plan ==
Map: map<string,int>
Relation [Map#592] json

== Optimized Logical Plan ==
InMemoryRelation [Map#592], StorageLevel(disk, memory, deserialized, 1 replicas)
   +- FileScan json [Map#592] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Map:map<string,int>>

== Physical Plan ==
InMemoryTableScan [Map#592]
   +- InMemoryRelation [Map#592], StorageLevel(disk, memory, deserialized, 1 replicas)
         +- FileScan json [Map#592] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Map:map<string,int>>

```
