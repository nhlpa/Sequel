using Xunit;

namespace Sequel.Tests
{
  public class SqlClauseTests
  {

    [Fact]
    public void EnsureGlueIsNulll()
    {
      Assert.Null(new SqlClause("SELECT 1", "").Glue);
      Assert.Null(new SqlClause("SELECT 1", null).Glue);
    }
  }
}
