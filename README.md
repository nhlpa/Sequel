# Sequel
![NuGet Version](https://img.shields.io/nuget/v/Sequel.svg)
[![Build Status](https://travis-ci.org/pimbrouwers/sequel.svg?branch=master)](https://travis-ci.org/pimbrouwers/sequel)

A SQL builder with an interface that emulates writing actual SQL queries.

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
```

### `INSERT`

```c#
var sqlBuilder = new SqlBuilder()
  .Insert("dbo.Test")
  .Columns("Name", "Salary")
  .Values("'John'", "50").Values("'Jane'", "100");

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


## Custom Templating

[sequel](https://github.com/pimbrouwers/sequel) has built-in templates to support the actions listed above, but also supports custom templating for edge cases. To formulate the SQL string, [sequel](https://github.com/pimbrouwers/sequel) uses `||` delimiters surrounding the following keywords:

- `||select||`
- `||fields||`
- `||from||`
- `||join||`
- `||where||`
- `||groupby||`
- `||having||`
- `||orderby|`
- `||insert||`
- `||columns||`
- `||values|`
- `||update||`
- `||set||`
- `||delete||`

To use `SqlBuilder` with a custom template, provide the template when constructing `new SqlBuilder("YOUR CUSTOM TEMPLATE")`.

### The default templates are as follows:

#### Select
`||select|| ||fields|| ||from|| ||join|| ||where|| ||groupby|| ||having|| ||orderby||`

#### Insert
`||insert|| ||columns|| ||values||`

#### Update
`||update|| ||set|| ||where||`

#### Delete
`||delete|| ||from|| ||join|| ||where||`

## Usage with [dapper.net](https://github.com/StackExchange/Dapper)

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

There's an [issue](https://github.com/pimbrouwers/sequel/issues) for that.

## License

Built with ♥ by [Pim Brouwers](https://github.com/pimbrouwers) in Toronto, ON. Licensed under [GNU](https://github.com/pimbrouwers/sequel/blob/master/LICENSE).
