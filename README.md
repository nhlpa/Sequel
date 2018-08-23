# SeQueL
![NuGet Version](https://img.shields.io/nuget/v/Sequel.svg)
[![Build Status](https://travis-ci.org/pimbrouwers/sequel.svg?branch=master)](https://travis-ci.org/pimbrouwers/sequel)

[SeQueL](https://github.com/pimbrouwers/sequel) is a **blazing** fast SQL builder with an interface that emulates writing actual SQL queries.

It comes bundled with an `ISqlMapper<TEntity>` which can be used to generate metadata for domain classes. Yielding:
- Table name
- Key column
- Fields 
- Fields excluding key
- Fields qualified by table name
- Non-key fields qualified by table name
- `SqlBuilder` representing `CREATE` / `UPDATE` / `READ` / `DELETE`  for domain class for straight-forward CRUD application

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

## Using `ISqlMapper<TEntity>`
The default implementation `SqlMapper<TEntity>` will extract it's metadata using `System.Type` and [fast-member](https://github.com/mgravell/fast-member/). The table name is derived from the `System.Type.Name` field, `Id` is assumed as the key (don't worry this can easily be overriden), and field data is drawn from a fast-member `MemberSet` which includes all public, writeable and primitive types.

The fields array always returns the `SqlMapper.Key` as the first item, to make integrations with things like dapper's [multi mapping](https://github.com/StackExchange/Dapper#multi-mapping) feature easier.

> The example below is taking directly from SqlMapperTests.cs. For more in-depth information, review that [file](https://github.com/pimbrouwers/sequel/blob/master/src/Sequel.Tests/SqlMapperTests.cs).

```c#
public class MockEntity
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int Cents { get; set; }

  public double Dollars => Cents * 100;

  public IEnumerable<MockChildEntity> Children { get; set; }
}

public class MockChildEntity
{
  public int Id { get; set; }
  public int ParentId { get; set; }
  public string Name { get; set; }
}

var sqlMapper = new SqlMapper<MockEntity>();

sqlMapper.Table; // "MockEntity"
sqlMapper.Key; // "Id"
sqlMapper.Fields // "Id", "Name", "Cents" (Key field is always first)
sqlMapper.NonKeyFields // "Name", "Cents"
sqlMapper.CreateSql.ToSql(); // INSERT INTO MockEntity (Cents, Name) VALUES (@Cents, @Name)
sqlMapper.ReadSql.ToSql(); // SELECT MockEntity.Id, MockEntity.Cents, MockEntity.Name FROM MockEntity
sqlMapper.UpdateSql.ToSql(); // UPDATE MockEntity SET Cents = @Cents, Name = @Name WHERE Id = @Id
sqlMapper.DeleteSql.ToSql(); // DELETE FROM MockEntity WHERE Id = @Id
```

### .NET Core Integration
Using the SQL Mapper directly out of the box with the .NET Core IoC is dead simple. Simply register it within the `ConfiureServices()` method of your `Startup.cs`.

```c#
services.AddSingleton(typeof(ISqlMapper<>), typeof(SqlMapper<>));
```

> Despite fast-member's far surperior performance to reflection, it is still recommended to impose the singleton pattern given that there is overhead to generating the required metadata.

### Runtime customization

`SqlMapper<TEntity>` can be customized at run. Configurable properties at runtime include:
- `string Table`
- `string Key`
- `string[] Fields` (NonKeyFields will be derived from this)

```c#
public class MockEntity
{
  public int UID { get; set; }
  public string Name { get; set; }
  public int Cents { get; set; }
}

var fields = new string[] { "UID", "Name", "Cents" };
var sqlMapper = new SqlMapper<MockEntity>("MockEntityTbl", "UID", fields);
```

### Complete Customization
For complete control you have two options:
1. Inherit from `SqlMapper<TEntity>` and override any of the available `virtual` fields.
2. Implement your own `ISqlMapper<TEntity>` from scratch.

## Custom Templating

[sequel](https://github.com/pimbrouwers/sequel) has built-in templates to support the actions listed above, but also supports custom templating for edge cases. To formulate the SQL string, [sequel](https://github.com/pimbrouwers/sequel) uses `||` delimiters surrounding the following keywords:

- `||select||`
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
`||select|| ||from|| ||join|| ||where|| ||groupby|| ||having|| ||orderby||`

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
