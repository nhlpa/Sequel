# Cinch.SqlBuilder
Fluent SQL Server Query Builder

## `SELECT` (uses default template "||select|| ||from|| ||join|| ||where|| ||groupby|| ||having||")

```c#
var sqlBuilder = new SqlBuilder()
					.Select("Id")
					.Select("Salary")
					.From("dbo.Test");

var sql = sqlBuilder.ToSql();

/*
	SELECT Id, Salary FROM dbo.Test
*/
```

## `INSERT` (requires use of custom template constructor)

```c#
var sqlBuilder = new SqlBuilder("||insert|| ||columns|| ||values||")
                    .Insert("dbo.Test")
                    .Columns("Name")
                    .Columns("Salary")
                    .Values("'John', 50")
                    .Values("'Jane', 100");

var sql = sqlBuilder.ToSql();

/*
INSERT INTO dbo.Test (Name, Salary) VALUES ('John', 50), ('Jane', 100)
*/
```

## `UPDATE` (requires use of custom template constructor)

```c#
var sqlBuilder = new SqlBuilder("||update|| ||set|| ||where||")
                                .Update("dbo.Test")
                                .Set("Salary = 100")
                                .Where("EmployeeId = 1");

var sql = sqlBuilder.ToSql();
/*
UPDATE dbo.Test SET Salary = 100 WHERE EmployeeId = 1
*/
```