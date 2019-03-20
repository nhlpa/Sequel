using Xunit;

namespace Sequel.Tests
{
  public class SqlBuilderTests
  {
    [Fact]
    public void EmptyBuilderTest()
    {
      var sqlBuilder = new SqlBuilder();

      Assert.Equal(sqlBuilder.ToSql(), "");
    }

    [Fact]
    public void SelectStarTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test", result);
    }

    [Fact]
    public void SelectStarTestFromAlias()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test", "t");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test AS t", result);
    }

    [Fact]
    public void SelectStarTopTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .Top(5)
                           .From("dbo.Test");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT TOP 5 * FROM dbo.Test", result);
    }

    [Fact]
    public void SelectStarLimitTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .Limit(5);

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test LIMIT 5", result);
    }

    [Fact]
    public void SelectStarWhereTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .Where("Id = 1");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test WHERE Id = 1", result);
    }

    [Fact]
    public void SelectStarWhereMultipleTest()
    {
      var sqlBuilder = new SqlBuilder()
                 .Select("*")
                 .From("dbo.Test")
                 .Where("Id = 1")
                 .Where("Num = 2");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test WHERE Id = 1 AND Num = 2", result);
    }

    [Fact]
    public void SelectStarWhereMultipleParamsTest()
    {
      var sqlBuilder = new SqlBuilder()
                 .Select("*")
                 .From("dbo.Test")
                 .Where("Id = 1", "Num = 2");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test WHERE Id = 1 AND Num = 2", result);
    }

    [Fact]
    public void SelectStarWhereOrTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .WhereOr("Id = 1")
                           .WhereOr("Salary = 50");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test WHERE Id = 1 OR Salary = 50", result);
    }

    [Fact]
    public void SelectStarWhereOrParamsTest()
    {
      var sqlBuilder = new SqlBuilder()
                 .Select("*")
                 .From("dbo.Test")
                 .WhereOr("Id = 1", "Salary = 50");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test WHERE Id = 1 OR Salary = 50", result);
    }

    [Fact]
    public void SelectStarWhereExistsTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test t")
                           .Exists("select null from dbo.Employee e where e.Id = t.EmployeeId");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test t WHERE EXISTS (select null from dbo.Employee e where e.Id = t.EmployeeId)", result);
    }

    [Fact]
    public void SelectMultipleTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("Id")
                           .Select("Salary")
                           .From("dbo.Test");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT Id, Salary FROM dbo.Test", result);
    }

    [Fact]
    public void SelectMultipleParamsTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("Id", "Salary")
                           .From("dbo.Test");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT Id, Salary FROM dbo.Test", result);
    }

    [Fact]
    public void SelectWithAliasTest()
    {
      string alias = "t";
      string[] columns = new string[] { "Id", "Salary" };
      var sqlBuilder = new SqlBuilder()
                           .SelectWithAlias(alias, columns)
                           .From("dbo.Test t");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT t.Id, t.Salary FROM dbo.Test t", result);

      //Ensure mutation hasn't occured
      Assert.Equal("t", alias);
      Assert.Equal("Id", columns[0]);
      Assert.Equal("Salary", columns[1]);
    }

    [Fact]
    public void SelectWithAliasMultipleTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .SelectWithAlias("t", "Id")
                           .SelectWithAlias("t", "Salary")
                           .From("dbo.Test t");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT t.Id, t.Salary FROM dbo.Test t", result);
    }

    [Fact]
    public void SelectGroupByTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("Id")
                           .Select("Count(Salary) as TotalSalary")
                           .From("dbo.Test")
                           .GroupBy("Id");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT Id, Count(Salary) as TotalSalary FROM dbo.Test GROUP BY Id", result);
    }

    [Fact]
    public void SelectGroupByHavingTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("Id")
                           .Select("Count(Salary) as TotalSalary")
                           .From("dbo.Test")
                           .GroupBy("Id")
                           .Having("Count(Salary) > 100");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT Id, Count(Salary) as TotalSalary FROM dbo.Test GROUP BY Id HAVING Count(Salary) > 100", result);
    }

    [Fact]
    public void SelectOrderByTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .OrderBy("Id");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test ORDER BY Id", result);
    }

    [Fact]
    public void SelectOrderByDescTest()
    {
      string orderByDesc = "Id";
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .OrderByDesc(orderByDesc);

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test ORDER BY Id DESC", result);

      Assert.Equal("Id", orderByDesc);
    }

    [Fact]
    public void SelectOrderByMultipleTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .OrderBy("Id")
                           .OrderBy("Salary");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test ORDER BY Id, Salary", result);
    }

    [Fact]
    public void SelectOrderByDescMultipleTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .OrderByDesc("Id")
                           .OrderByDesc("Salary");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test ORDER BY Id DESC, Salary DESC", result);
    }

    [Fact]
    public void SelectOrderByMultipleMixedTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .OrderBy("Id")
                           .OrderByDesc("Salary");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test ORDER BY Id, Salary DESC", result);
    }

    [Fact]
    public void FromSqlBuilderTest()
    {
      var sqlBuilder1 = new SqlBuilder()
                           .Select("*")
                           .From("dbo.Test")
                           .Where("Id = 1");

      var sqlBuilder2 = new SqlBuilder()
                            .Select("*")
                            .From(sqlBuilder1, "t1");

      var result = sqlBuilder2.ToSql();

      Assert.Equal("SELECT * FROM (SELECT * FROM dbo.Test WHERE Id = 1) as t1", result);
    }

    [Fact]
    public void WhereExistsSqlBuilderTest()
    {
      var sqlBuilder1 = new SqlBuilder()
                           .Select("null")
                           .From("dbo.Test")
                           .Where("Salary > 50");

      var sqlBuilder2 = new SqlBuilder()
                            .Select("*")
                            .From("dbo.Test")
                            .Exists(sqlBuilder1);

      var result = sqlBuilder2.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test WHERE EXISTS (SELECT null FROM dbo.Test WHERE Salary > 50)", result);
    }

    [Fact]
    public void JoinTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Select("*")
                          .From("dbo.Test t")
                          .Join("dbo.Employee e on e.Id = t.EmployeeId");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId", result);
    }

    [Fact]
    public void JoinPedicateTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Select("*")
                          .From("dbo.Test t")
                          .Join("dbo.Employee e", "e.Id = t.EmployeeId");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test t INNER JOIN dbo.Employee e ON e.Id = t.EmployeeId", result);
    }

    [Fact]
    public void JoinAliasPredicateTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Select("*")
                          .From("dbo.Test t")
                          .Join("dbo.Employee", "e", "e.Id = t.EmployeeId");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test t INNER JOIN dbo.Employee AS e ON e.Id = t.EmployeeId", result);
    }

    [Fact]
    public void LeftJoinTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Select("*")
                          .From("dbo.Test t")
                          .LeftJoin("dbo.Employee e on e.Id = t.EmployeeId");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test t LEFT JOIN dbo.Employee e on e.Id = t.EmployeeId", result);
    }

    [Fact]
    public void LeftJoinPedicateTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Select("*")
                          .From("dbo.Test t")
                          .LeftJoin("dbo.Employee e", "e.Id = t.EmployeeId");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test t LEFT JOIN dbo.Employee e ON e.Id = t.EmployeeId", result);
    }

    [Fact]
    public void LeftJoinAliasPredicateTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Select("*")
                          .From("dbo.Test t")
                          .LeftJoin("dbo.Employee", "e", "e.Id = t.EmployeeId");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test t LEFT JOIN dbo.Employee AS e ON e.Id = t.EmployeeId", result);
    }

    [Fact]
    public void JoinLeftJoinTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Select("*")
                          .From("dbo.Test t")
                          .Join("dbo.Employee e on e.Id = t.EmployeeId")
                          .LeftJoin("dbo.Manager m on m.Id = e.ManagerId");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT * FROM dbo.Test t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId LEFT JOIN dbo.Manager m on m.Id = e.ManagerId", result);
    }

    [Fact]
    public void UpdateTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Update("dbo.Test")
                          .Set("Salary = 100")
                          .Where("EmployeeId = 1");

      var result = sqlBuilder.ToSql();

      Assert.Equal("UPDATE dbo.Test SET Salary = 100 WHERE EmployeeId = 1", result);
    }

    [Fact]
    public void UpdateMultipleTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Update("dbo.Test")
                          .Set("Salary = 100", "ManagerId = 2")
                          .Where("EmployeeId = 1");

      var result = sqlBuilder.ToSql();

      Assert.Equal("UPDATE dbo.Test SET Salary = 100, ManagerId = 2 WHERE EmployeeId = 1", result);
    }

    [Fact]
    public void InsertValueTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Insert("dbo.Test")
                          .Into("Name", "Salary")
                          .Value("'Pim'", "50");

      var result = sqlBuilder.ToSql();

      Assert.Equal("INSERT INTO dbo.Test (Name, Salary) VALUES ('Pim', 50)", result);
    }

    [Fact]
    public void InsertMultipleTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Insert("dbo.Test")
                          .Into("Name")
                          .Into("Salary")
                          .Values("'Pim'", "50")
                          .Values("'Lindsey'", "100");

      var result = sqlBuilder.ToSql();

      Assert.Equal("INSERT INTO dbo.Test (Name, Salary) VALUES ('Pim', 50), ('Lindsey', 100)", result);
    }

    [Fact]
    public void DeleteTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Delete()
                           .From("dbo.Test")
                           .Where("Id = 1");

      var result = sqlBuilder.ToSql();

      Assert.Equal("DELETE FROM dbo.Test WHERE Id = 1", result);
    }

    [Fact]
    public void DeleteJoinTest()
    {
      var sqlBuilder = new SqlBuilder()
                          .Delete("t")
                          .From("dbo.Test t")
                          .Join("dbo.Employee e on e.Id = t.EmployeeId")
                          .Where("e.Id = 1");

      var result = sqlBuilder.ToSql();

      Assert.Equal("DELETE t FROM dbo.Test t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId WHERE e.Id = 1", result);
    }
  }
}