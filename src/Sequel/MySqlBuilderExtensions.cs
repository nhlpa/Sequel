namespace Sequel.MySql
{
  public static class MySqlBuilderExtensions
  {
    /// <summary>
    /// LIMIT by n rows
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static SqlBuilder Limit(this SqlBuilder sql, int n) =>
      sql.AddClause("limit", n.ToString(), null, "LIMIT ", null, true);

  }
}
