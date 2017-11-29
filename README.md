# Cinch.SqlBuilder
Fluent SQL Server Query Builder, excluding `DELETE`.

## `SELECT` (uses default template)

```c#
var sqlBuilder = 
	new SqlBuilder()
		.Select("Id", "Salary")
		.From("dbo.Test");

var sql = sqlBuilder.ToSql(); // .ToString() also works

/*
SELECT Id, Salary FROM dbo.Test
*/
```

## `INSERT` (requires use of custom template constructor)

```c#
var sqlBuilder = 
	new SqlBuilder()
		.Insert("dbo.Test")
		.Columns("Name", "Salary")
		.Values("'John'", "50")
		.Values("'Jane'", "100");

var sql = sqlBuilder.ToSql(); // .ToString() also works

/*
INSERT INTO dbo.Test (Name, Salary) VALUES ('John', 50), ('Jane', 100)
*/
```

## `UPDATE` (requires use of custom template constructor)

```c#
var sqlBuilder = 
	new SqlBuilder()
		.Update("dbo.Test")
		.Set("Salary = 100", "ManagerId = 2")
		.Where("EmployeeId = 1");

var sql = sqlBuilder.ToSql(); // .ToString() also works

/*
UPDATE dbo.Test SET Salary = 100, ManagerId = 2 WHERE EmployeeId = 1
*/
```
