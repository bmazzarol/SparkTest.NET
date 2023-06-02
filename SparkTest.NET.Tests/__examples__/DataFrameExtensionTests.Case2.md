# [Results](#tab/results)

|Byte|Short|Int|Long|Float|Double|Decimal|String|Char|Bool |Date      |SparkDate |DateTimeOffset            |Timestamp                 |Binary|Enum    |
|----|-----|---|----|-----|------|-------|------|----|-----|----------|----------|--------------------------|--------------------------|------|--------|
|1   |1    |1  |1   |1.0  |1.0   |1      |a     |a   |true |0001-01-01|0001-01-01|0001-01-01 00:00:00       |0001-01-01 00:00:00       |[01]  |None    |
|2   |2    |2  |2   |2.0  |2.0   |2      |b     |b   |false|9999-12-31|9999-12-31|9999-12-31 23:59:59.999999|9999-12-31 23:59:59.999999|[01]  |Critical|

_(top = 20)_

# [Schema](#tab/schema)

```shell
root
 |-- Byte: byte (nullable = true)
 |-- Short: short (nullable = true)
 |-- Int: integer (nullable = true)
 |-- Long: long (nullable = true)
 |-- Float: float (nullable = true)
 |-- Double: double (nullable = true)
 |-- Decimal: decimal(10,0) (nullable = true)
 |-- String: string (nullable = true)
 |-- Char: string (nullable = true)
 |-- Bool: boolean (nullable = true)
 |-- Date: date (nullable = true)
 |-- SparkDate: date (nullable = true)
 |-- DateTimeOffset: timestamp (nullable = true)
 |-- Timestamp: timestamp (nullable = true)
 |-- Binary: binary (nullable = true)
 |-- Enum: string (nullable = true)

```

# [Plan](#tab/plan)

```shell
== Parsed Logical Plan ==
Relation [Byte#1,Short#2,Int#3,Long#4L,Float#5,Double#6,Decimal#7,String#8,Char#9,Bool#10,Date#11,SparkDate#12,DateTimeOffset#13,Timestamp#14,Binary#15,Enum#16] json

== Analyzed Logical Plan ==
Byte: tinyint, Short: smallint, Int: int, Long: bigint, Float: float, Double: double, Decimal: decimal(10,0), String: string, Char: string, Bool: boolean, Date: date, SparkDate: date, DateTimeOffset: timestamp, Timestamp: timestamp, Binary: binary, Enum: string
Relation [Byte#1,Short#2,Int#3,Long#4L,Float#5,Double#6,Decimal#7,String#8,Char#9,Bool#10,Date#11,SparkDate#12,DateTimeOffset#13,Timestamp#14,Binary#15,Enum#16] json

== Optimized Logical Plan ==
InMemoryRelation [Byte#1, Short#2, Int#3, Long#4L, Float#5, Double#6, Decimal#7, String#8, Char#9, Bool#10, Date#11, SparkDate#12, DateTimeOffset#13, Timestamp#14, Binary#15, Enum#16], StorageLevel(disk, memory, deserialized, 1 replicas)
   +- FileScan json [Byte#1,Short#2,Int#3,Long#4L,Float#5,Double#6,Decimal#7,String#8,Char#9,Bool#10,Date#11,SparkDate#12,DateTimeOffset#13,Timestamp#14,Binary#15,Enum#16] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Byte:tinyint,Short:smallint,Int:int,Long:bigint,Float:float,Double:double,Decimal:decimal(...

== Physical Plan ==
InMemoryTableScan [Byte#1, Short#2, Int#3, Long#4L, Float#5, Double#6, Decimal#7, String#8, Char#9, Bool#10, Date#11, SparkDate#12, DateTimeOffset#13, Timestamp#14, Binary#15, Enum#16]
   +- InMemoryRelation [Byte#1, Short#2, Int#3, Long#4L, Float#5, Double#6, Decimal#7, String#8, Char#9, Bool#10, Date#11, SparkDate#12, DateTimeOffset#13, Timestamp#14, Binary#15, Enum#16], StorageLevel(disk, memory, deserialized, 1 replicas)
         +- FileScan json [Byte#1,Short#2,Int#3,Long#4L,Float#5,Double#6,Decimal#7,String#8,Char#9,Bool#10,Date#11,SparkDate#12,DateTimeOffset#13,Timestamp#14,Binary#15,Enum#16] Batched: false, DataFilters: [], Format: JSON, Location: InMemoryFileIndex(1 paths)[file:/SparkTest.NE..., PartitionFilters: [], PushedFilters: [], ReadSchema: struct<Byte:tinyint,Short:smallint,Int:int,Long:bigint,Float:float,Double:double,Decimal:decimal(...

```
