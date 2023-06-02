# [Results](#tab/results)

|Id |Name        |
|---|------------|
|1  |Some Name 1 |
|2  |Some Name 2 |
|3  |Some Name 3 |
|4  |Some Name 4 |
|5  |Some Name 5 |
|6  |Some Name 6 |
|7  |Some Name 7 |
|8  |Some Name 8 |
|9  |Some Name 9 |
|10 |Some Name 10|

_(top = 20)_

# [Schema](#tab/schema)

```shell
root
 |-- Id: integer (nullable = true)
 |-- Name: string (nullable = true)

```

# [Plan](#tab/plan)

```shell
== Parsed Logical Plan ==
Relation [Id#238,Name#239] json

== Analyzed Logical Plan ==
Id: int, Name: string
Relation [Id#238,Name#239] json

== Optimized Logical Plan ==
InMemoryRelation [Id#238, Name#239], StorageLevel(disk, memory, deserialized, 1 replicas)
   +- FileScan json [Id#238,Name#239] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Id:int,Name:string>

== Physical Plan ==
InMemoryTableScan [Id#238, Name#239]
   +- InMemoryRelation [Id#238, Name#239], StorageLevel(disk, memory, deserialized, 1 replicas)
         +- FileScan json [Id#238,Name#239] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Id:int,Name:string>

```
