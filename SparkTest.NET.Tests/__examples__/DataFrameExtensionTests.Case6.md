# [Results](#tab/results)

|CreatedDate        |Person                                  |
|-------------------|----------------------------------------|
|0001-01-01 00:00:00|{Billy, 12, [{1, Football}, {3, Cars}]} |
|0001-01-01 00:00:00|{James, 11, [{1, Video Games}, {2, TV}]}|

_(top = 20)_

# [Schema](#tab/schema)

```shell
root
 |-- CreatedDate: timestamp (nullable = true)
 |-- Person: struct (nullable = true)
 |    |-- Name: string (nullable = true)
 |    |-- Age: integer (nullable = true)
 |    |-- Interests: array (nullable = true)
 |    |    |-- element: struct (containsNull = true)
 |    |    |    |-- Priority: integer (nullable = true)
 |    |    |    |-- Name: string (nullable = true)

```

# [Plan](#tab/plan)

```shell
== Parsed Logical Plan ==
Relation [CreatedDate#0,Person#1] json

== Analyzed Logical Plan ==
CreatedDate: timestamp, Person: struct<Name:string,Age:int,Interests:array<struct<Priority:int,Name:string>>>
Relation [CreatedDate#0,Person#1] json

== Optimized Logical Plan ==
InMemoryRelation [CreatedDate#0, Person#1], StorageLevel(disk, memory, deserialized, 1 replicas)
   +- FileScan json [CreatedDate#0,Person#1] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<CreatedDate:timestamp,Person:struct<Name:string,Age:int,Interests:array<struct<Priority:in...

== Physical Plan ==
InMemoryTableScan [CreatedDate#0, Person#1]
   +- InMemoryRelation [CreatedDate#0, Person#1], StorageLevel(disk, memory, deserialized, 1 replicas)
         +- FileScan json [CreatedDate#0,Person#1] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<CreatedDate:timestamp,Person:struct<Name:string,Age:int,Interests:array<struct<Priority:in...

```
