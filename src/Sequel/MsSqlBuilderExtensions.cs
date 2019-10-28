namespace Sequel.MsSql
{
  public static class MsSqlBuilderExtensions
  {
    /// <summary>
    /// Cross apply table valued function
    /// </summary>
    /// <param name="tvf"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static SqlBuilder CrossApply(this SqlBuilder sql, string tvf, string alias) =>
      sql.AddClause("join", string.Concat(tvf, " AS ", alias), " CROSS APPLY ", null, null, false);

    /// <summary>
    /// Cross apply adhoc 
    /// </summary>
    /// <param name="sqlBuilder"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static SqlBuilder CrossApply(this SqlBuilder sql, SqlBuilder sqlBuilder, string alias) =>
      sql.AddClause("join", string.Concat("(", sqlBuilder.ToSql(), ") AS ", alias), " CROSS APPLY ", null, null, false);

    /// <summary>
    /// TOP n rows
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static SqlBuilder Top(this SqlBuilder sql, int n) =>
      sql.AddClause("top", string.Concat("(", n.ToString(), ")"), null, "TOP", null, true);

    /// <summary>
    /// OFFSET x ROWS FETCH NEXT y ROWS ONLY
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="offset"></param>
    /// <param name="fetch"></param>
    /// <returns></returns>
    public static SqlBuilder OffsetFetch(this SqlBuilder sql, int offset, int fetch) =>
      sql.AddClause("offset", string.Concat(offset.ToString(), " ROWS ", "FETCH NEXT ", fetch.ToString(), " ROWS ONLY"), null, "OFFSET ", null);
  }
}
