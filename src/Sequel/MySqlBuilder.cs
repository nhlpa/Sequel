namespace Sequel
  {
  public class MySqlBuilder : SqlBuilder
    {
    /// <summary>
    /// LIMIT by n rows
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public SqlBuilder Limit(int n) =>
      AddClause("limit", n.ToString(), null, "LIMIT ", null, true);

    }
  }
