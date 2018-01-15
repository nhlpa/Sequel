# Cinch.SqlBuilder
Fluent SQL Server Query Builder. For in-depth use cases see the Cinch.SqlBuilder.Tests project.

## `SELECT` (uses default template)

```c#
var sqlBuilder = new SqlBuilder()
	.Select("Id", "Salary")
	.From("dbo.Test");

var sql = sqlBuilder.ToSql(); // .ToString() also works

/*
SELECT Id, Salary FROM dbo.Test
*/
```

## `INSERT`

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

## `UPDATE`

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

## `DELETE`

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

## Usage with [dapper.net](https://github.com/StackExchange/Dapper)

```c#
using(var conn = new SqlConnection("your connection string")
{
  var sqlParams = new DynamicParameters();
  sqlParams.Add("Id", 1);
    
  var sqlBuilder = new SqlBuilder()
		      .Select("Id", "Salary")
		      .From("dbo.Test")
		      .Where("Id", "@Id");

  var sql = sqlBuilder.ToSql(); // .ToString() also works 
  /*
  SELECT Id, Salary FROM dbo.Test WHERE Id = @Id
  */ 
    
  var result = conn.Query(sql, sqlParams);
}
```
