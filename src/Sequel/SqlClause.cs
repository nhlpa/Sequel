namespace Sequel
{
  public class SqlClause
  {
    public SqlClause(string sql, string glue)
    {
      if (!string.IsNullOrWhiteSpace(glue))
      {
        Glue = glue;
      }

      Sql = sql;
    }

    public string Glue { get; }
    public string Sql { get; }
  }
}
