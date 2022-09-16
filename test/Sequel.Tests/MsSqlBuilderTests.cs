using Sequel.MsSql;
using Xunit;

namespace Sequel.Tests
{
  public class MsSqlBuilderTests
  {
    [Fact]
    public void SelectStarTopTest()
    {
      var sqlBuilder = new SqlBuilder()
                           .Select("*")
                           .Top(5)
                           .From("dbo.Test");

      var result = sqlBuilder.ToSql();

      Assert.Equal("SELECT TOP(5) * FROM dbo.Test", result);
    }

    [Fact]
    public void CrossApplySqlBuilderTest()
    {
      var ca = new SqlBuilder()
        .Select("t.TestId")
        .Top(1)
        .From("Test", "t")
        .Where("t.TestId = ot.TestId")
        .OrderByDesc("Modified");

      var q = new SqlBuilder()
        .Select("ot.TestId")
        .From("OuterTest", "ot")
        .CrossApply(ca, "t");

      var result = q.ToSql();

      Assert.Equal("SELECT ot.TestId FROM OuterTest AS ot CROSS APPLY (SELECT TOP(1) t.TestId FROM Test AS t WHERE t.TestId = ot.TestId ORDER BY Modified DESC) AS t", result);
    }

    [Fact]
    public void OffsetFetchTest()
    {
      var sql = new SqlBuilder()
        .Select("*")
        .From("Test", "t")
        .OrderByDesc("Modified")
        .OffsetFetch(10, 50);

      var result = sql.ToSql();

      Assert.Equal("SELECT * FROM Test AS t ORDER BY Modified DESC OFFSET 10 ROWS FETCH NEXT 50 ROWS ONLY", result);
    }
  }
}
