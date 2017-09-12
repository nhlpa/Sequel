using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cinch.SqlBuilder.Tests
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

            Assert.Equal(result, "SELECT * FROM dbo.Test");
        }

        [Fact]
        public void SelectStarWhereTest()
        {
            var sqlBuilder = new SqlBuilder()
                                 .Select("*")
                                 .From("dbo.Test")
                                 .Where("Id = 1");

            var result = sqlBuilder.ToSql();

            Assert.Equal(result, "SELECT * FROM dbo.Test WHERE Id = 1");
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

            Assert.Equal(result, "SELECT * FROM dbo.Test WHERE Id = 1 OR Salary = 50");
        }

        [Fact]
        public void SelectStarWhereExistsTest()
        {
            var sqlBuilder = new SqlBuilder()
                                 .Select("*")
                                 .From("dbo.Test t")
                                 .Exists("select null from dbo.Employee e where e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal(result, "SELECT * FROM dbo.Test t WHERE EXISTS (select null from dbo.Employee e where e.Id = t.EmployeeId)");
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

            Assert.Equal(result, "SELECT Id, Count(Salary) as TotalSalary FROM dbo.Test GROUP BY Id");
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

            Assert.Equal(result, "SELECT Id, Count(Salary) as TotalSalary FROM dbo.Test GROUP BY Id HAVING Count(Salary) > 100");
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

            Assert.Equal(result, "SELECT * FROM (SELECT * FROM dbo.Test WHERE Id = 1) as t1");
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

            Assert.Equal(result, "SELECT * FROM dbo.Test WHERE EXISTS (SELECT null FROM dbo.Test WHERE Salary > 50)");
        }

        [Fact]
        public void JoinTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .Join("dbo.Employee e on e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal(result, "SELECT * FROM dbo.Test t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId");
        }

        [Fact]
        public void LeftJoinTest()
        {
            var sqlBuilder = new SqlBuilder()
                                .Select("*")
                                .From("dbo.Test t")
                                .LeftJoin("dbo.Employee e on e.Id = t.EmployeeId");

            var result = sqlBuilder.ToSql();

            Assert.Equal(result, "SELECT * FROM dbo.Test t LEFT JOIN dbo.Employee e on e.Id = t.EmployeeId");
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

            Assert.Equal(result, "SELECT * FROM dbo.Test t INNER JOIN dbo.Employee e on e.Id = t.EmployeeId LEFT JOIN dbo.Manager m on m.Id = e.ManagerId");
        }

        [Fact]
        public void UpdateTest()
        {
            var sqlBuilder = new SqlBuilder("||update|| ||set|| ||where||")
                                .Update("dbo.Test")
                                .Set("Salary = 100")
                                .Where("EmployeeId = 1");

            var result = sqlBuilder.ToSql();

            Assert.Equal(result, "UPDATE dbo.Test SET Salary = 100 WHERE EmployeeId = 1");
        }
    }
}
