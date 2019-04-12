namespace Sequel
  {
  public class MsSqlBuilder : SqlBuilder
    {
    /// <summary>
    /// Cross apply table valued function
    /// </summary>
    /// <param name="tvf"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public SqlBuilder CrossApply(string tvf, string alias)
      => AddClause("join", Concat(tvf, " AS ", alias), " CROSS APPLY ", null, null, false);

    /// <summary>
    /// Cross apply adhoc 
    /// </summary>
    /// <param name="sqlBuilder"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public SqlBuilder CrossApply(SqlBuilder sqlBuilder, string alias)
      => AddClause("join", Concat(sqlBuilder.ToSql(), " AS ", alias), " CROSS APPLY ", null, null, false);

    /// <summary>
    /// TOP n rows
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public SqlBuilder Top(int n) =>
      AddClause("top", Concat("(", n.ToString(), ")"), null, "TOP ", null, true);

    }
  }
