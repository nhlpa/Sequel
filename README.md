# Sequel

[![NuGet Version](https://img.shields.io/nuget/v/Sequel.svg)](https://www.nuget.org/packages/Sequel)
[![build](https://github.com/nhlpa/Sequel/actions/workflows/build.yml/badge.svg)](https://github.com/nhlpa/Sequel/actions/workflows/build.yml)

An efficient SQL builder with an interface that emulates writing actual SQL queries.

## Getting Started
### `SELECT`

```c#
var sqlBuilder = new SqlBuilder()
  .Select("Id", "Salary")
  .From("dbo.Test");

var sql = sqlBuilder.ToSql(); // .ToString() also works

/*
SELECT Id, Salary FROM dbo.Test
*/

// SELECT with INNER & LEFT JOIN
var sqlBuilder = new SqlBuilder()
  .Select("*")
  .From("dbo.Test t")
  .Join("dbo.Employee e on e.Id = t.EmployeeId")
  .LeftJoin("dbo.Manager m on m.Id = e.ManagerId");

var sql = sqlBuilder.ToSql();

/*
SELECT * FROM dbo.Test t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId LEFT JOIN dbo.Manager m on m.Id = e.ManagerId
*/
```

### `INSERT`

```c#
var sqlBuilder = new SqlBuilder()
  .Insert("dbo.Test")
  .Columns("Name", "Salary")
  .Values("'John'", "50")
  .Values("'Jane'", "100");

var sql = sqlBuilder.ToSql(); // .ToString() also works

/*
INSERT INTO dbo.Test (Name, Salary) VALUES ('John', 50), ('Jane', 100)
*/
```

### `UPDATE`

```c#
var sqlBuilder = new SqlBuilder()
  .Update("dbo.Test")
  .Set("Salary = 100", "ManagerId = 2")
  .Where("EmployeeId = 1");

var sql = sqlBuilder.ToSql(); // .ToString() also works

/*
UPDATE dbo.Test SET Salary = 100, ManagerId = 2 WHERE EmployeeId = 1
*/
```

### `DELETE`

```c#
var sqlBuilder = new SqlBuilder()
  .Delete()
  .From("dbo.Test")
  .Where("EmployeeId = 1");

var sql = sqlBuilder.ToSql(); // .ToString() also works

/*
DELETE FROM dbo.Test WHERE EmployeeId = 1
*/
```

## Injecting custom SQL

You are granted pre- & post-hooks into the final SQL string literaly, for the purpose of injecting custom SQL.

The pre-hook is useful in the case of CTE's or inline declarations.

```c#
var sqlBuilder = new SqlBuilder(pre: "WITH cte AS (SELECT 1) ")
  .Select("*")
  .From("cte");

var sql = sqlBuilder.ToSql();

/*
WITH cte AS (SELECT 1) SELECT * FROM cte"
*/
```

The post-hook is useful for situations like obtaining the last inserted row identifier.

```c#
var sqlBuilder = new SqlBuilder(post: "; SELECT last_insert_rowid();")
  .Insert("dbo.Test")
  .Into("Name", "Salary")
  .Value("'Pim'", "50");

var sql = sqlBuilder.ToSql();

/*
INSERT INTO dbo.Test (Name, Salary) VALUES ('Pim', 50); SELECT last_insert_rowid();
*/
```

## An example using [Dapper](https://github.com/StackExchange/Dapper)

```c#
using(var conn = new SqlConnection("your connection string")
{
  var sqlBuilder = new SqlBuilder()
    .Select("Id", "Salary")
    .From("dbo.Test")
    .Where("Id", "@Id");

  var sql = sqlBuilder.ToSql(); // .ToString() also works
  /*
  SELECT Id, Salary FROM dbo.Test WHERE Id = @Id
  */

  var result = conn.Query(sql, new { Id = 1 });
}
```

## Find a bug?

There's an [issue](https://github.com/nhlpa/Sequel/issues) for that.

## License

Built with ♥ by [NHLPA Engineering](https://github.com/nhlpa) in Toronto, ON. Licensed under [MIT](https://github.com/nhlpa/Sequel/blob/master/LICENSE).