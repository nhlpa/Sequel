using System.Text;
using Xunit;

namespace Sequel.Tests
{
    public class SqlBuilderTests
    {
        [Fact]
        public void ToSqlEqualsToString()
        {
            var sql = new SqlBuilder().Select("1");

            Assert.Equal(sql.ToString(), sql.ToSql());
        }

        [Fact]
        public void EmptyBuilderTest()
        {
            var sqlBuilder = new SqlBuilder();

            Assert.Equal("", sqlBuilder.ToSql());
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
                       .Where("Id = 1")
                       .Select("*")
                       .From("dbo.Test")
                       .Where("Num = 2");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test WHERE Id = 1 AND Num = 2", result);
        }

        [Fact]
        public void SelectStarWhereMultipleOrTest()
        {
            var sqlBuilder = new SqlBuilder()
                       .Select("*")
                       .From("dbo.Test")                       
                       .Where("Id = 1")
                       .Where("Num = 2")
                       .WhereOr("Col = 3");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test WHERE Id = 1 AND Num = 2 OR Col = 3", result);
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
            var alias = "t";
            var columns = new string[] { "Id", "Salary" };
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
                                 .Select("COUNT(Salary) as TotalSalary")
                                 .From("dbo.Test")
                                 .GroupBy("Id");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT Id, COUNT(Salary) as TotalSalary FROM dbo.Test GROUP BY Id", result);
        }

        [Fact]
        public void SelectGroupByHavingTest()
        {
            var sqlBuilder = new SqlBuilder()
                                 .Select("Id")
                                 .Select("COUNT(Salary) as TotalSalary")
                                 .From("dbo.Test")
                                 .GroupBy("Id")
                                 .Having("COUNT(Salary) > 100");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT Id, COUNT(Salary) as TotalSalary FROM dbo.Test GROUP BY Id HAVING COUNT(Salary) > 100", result);
        }

        [Fact]
        public void SelectGroupByHavingMultipleTest()
        {
            var sqlBuilder = new SqlBuilder()
                                 .Select("Id")
                                 .Select("COUNT(Salary) as TotalSalary")
                                 .From("dbo.Test")
                                 .GroupBy("Id")
                                 .Having("COUNT(Salary) > 100", "MAX(Age) > 3");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT Id, COUNT(Salary) as TotalSalary FROM dbo.Test GROUP BY Id HAVING COUNT(Salary) > 100 AND MAX(Age) > 3", result);
        }

        [Fact]
        public void SelectGroupByHavingMultipleOrTest()
        {
            var sqlBuilder = new SqlBuilder()
                                 .Select("Id")
                                 .Select("COUNT(Salary) as TotalSalary")
                                 .From("dbo.Test")
                                 .GroupBy("Id")
                                 .Having("COUNT(Salary) > 100", "MAX(Age) > 3")
                                 .HavingOr("SUM(Points) > 1000");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT Id, COUNT(Salary) as TotalSalary FROM dbo.Test GROUP BY Id HAVING COUNT(Salary) > 100 AND MAX(Age) > 3 OR SUM(Points) > 1000", result);
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
            var orderByDesc = "Id";
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
                                 .SelectWithAlias("t", "Id")
                                 .SelectWithAlias("t", "Salary")
                                 .From("dbo.Test", "t")
                                 .OrderByWithAlias("t", "Id")
                                 .OrderByDescWithAlias("t", "Salary");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT t.Id, t.Salary FROM dbo.Test AS t ORDER BY t.Id, t.Salary DESC", result);
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
        public void JoinMultipleTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .Join("dbo.Employee e on e.Id = t.EmployeeId")
                                .Join("dbo.Manager m on m.Id = e.ManagerId");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId INNER JOIN dbo.Manager m on m.Id = e.ManagerId", result);
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
        public void JoinSqlBuilderTest()
        {
            var emp = new SqlBuilder().Select("*").From("dbo.Employee");

            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .Join(emp, "e", "e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t INNER JOIN (SELECT * FROM dbo.Employee) AS e ON e.Id = t.EmployeeId", result);
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
        public void LeftJoinSqlBuilderTest()
        {
            var emp = new SqlBuilder().Select("*").From("dbo.Employee");

            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .LeftJoin(emp, "e", "e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t LEFT JOIN (SELECT * FROM dbo.Employee) AS e ON e.Id = t.EmployeeId", result);
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
        public void RightJoinTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .RightJoin("dbo.Employee e on e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t RIGHT JOIN dbo.Employee e on e.Id = t.EmployeeId", result);
        }

        [Fact]
        public void RightJoinPedicateTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .RightJoin("dbo.Employee e", "e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t RIGHT JOIN dbo.Employee e ON e.Id = t.EmployeeId", result);
        }

        [Fact]
        public void RightJoinAliasPredicateTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .RightJoin("dbo.Employee", "e", "e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t RIGHT JOIN dbo.Employee AS e ON e.Id = t.EmployeeId", result);
        }

        [Fact]
        public void RightJoinSqlBuilderTest()
        {
            var emp = new SqlBuilder().Select("*").From("dbo.Employee");

            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .RightJoin(emp, "e", "e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t RIGHT JOIN (SELECT * FROM dbo.Employee) AS e ON e.Id = t.EmployeeId", result);
        }

        [Fact]
        public void JoinRightJoinTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .Join("dbo.Employee e on e.Id = t.EmployeeId")
                                .RightJoin("dbo.Manager m on m.Id = e.ManagerId");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId RIGHT JOIN dbo.Manager m on m.Id = e.ManagerId", result);
        }

        [Fact]
        public void CrossJoinTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .CrossJoin("dbo.Employee");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t CROSS JOIN dbo.Employee", result);
        }

        [Fact]
        public void CrossJoinMultipleTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .CrossJoin("dbo.Employee")
                                .CrossJoin("dbo.Manager");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t CROSS JOIN dbo.Employee CROSS JOIN dbo.Manager", result);
        }

        [Fact]
        public void CrossJoinAliasTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .CrossJoin("dbo.Employee", "e");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t CROSS JOIN dbo.Employee AS e", result);
        }

        [Fact]
        public void CrossJoinSqlBuilderTest()
        {
            var emp = new SqlBuilder().Select("*").From("dbo.Employee");

            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .CrossJoin(emp, "e");

            var result = sqlBuilder.ToSql();

            Assert.Equal("SELECT * FROM dbo.Test t CROSS JOIN (SELECT * FROM dbo.Employee) AS e", result);
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
                                .Values("'Pim'", "50");

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
                                .From("dbo.Test", "t")
                                .Join("dbo.Employee e on e.Id = t.EmployeeId")
                                .Where("e.Id = 1");

            var result = sqlBuilder.ToSql();

            Assert.Equal("DELETE t FROM dbo.Test AS t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId WHERE e.Id = 1", result);
        }

        [Fact]
        public void CustomPreTest() {            
            var sqlBuilder = new SqlBuilder(pre: "WITH cte AS (SELECT 1) ")
                                 .Select("*")
                                 .From("cte");

            var result = sqlBuilder.ToSql();

            Assert.Equal("WITH cte AS (SELECT 1) SELECT * FROM cte", result);
        }

        [Fact]
        public void CustomPostTest()
        {
            var sqlBuilder = new SqlBuilder(post: "; SELECT last_insert_rowid();")
                                .Insert("dbo.Test")
                                .Into("Name", "Salary")
                                .Values("'Pim'", "50");

            var result = sqlBuilder.ToSql();

            Assert.Equal("INSERT INTO dbo.Test (Name, Salary) VALUES ('Pim', 50); SELECT last_insert_rowid();", result);
        }

        [Fact]
        public void CustomPrePostTest()
        {
            var sqlBuilder = new SqlBuilder(pre: "WITH cte AS (SELECT 1) ", post: " UNION SELECT 2")
                                 .Select("*")
                                 .From("cte");

            var result = sqlBuilder.ToSql();

            Assert.Equal("WITH cte AS (SELECT 1) SELECT * FROM cte UNION SELECT 2", result);
        }
    }
}