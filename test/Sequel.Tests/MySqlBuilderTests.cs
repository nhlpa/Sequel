using Sequel.MySql;
using Xunit;

namespace Sequel.Tests
{
  public class MySqlBuilderTests
  {
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
  }
}
