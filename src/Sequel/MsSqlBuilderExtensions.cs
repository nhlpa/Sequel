namespace Sequel.MsSql
{
    /// <summary>
    /// SQL Builder extensions for MSSQL
    /// </summary>
    public static class MsSqlBuilderExtensions
    {
        /// <summary>
        /// Cross apply table valued function
        /// </summary>    
        public static SqlBuilder CrossApply(this SqlBuilder sql, string tvf, string alias) =>
          sql.AddClause(
              keyword: "join",
              token: string.Concat(tvf, " AS ", alias),
              glue: "CROSS APPLY ",
              pre: null,
              post: null,
              singular: false);

        /// <summary>
        /// Cross apply adhoc 
        /// </summary>    
        public static SqlBuilder CrossApply(this SqlBuilder sql, SqlBuilder sqlBuilder, string alias) =>
          sql.AddClause(
              keyword: "join",
              token: string.Concat("(", sqlBuilder.ToSql(), ") AS ", alias),
              glue: "CROSS APPLY ",
              pre: null,
              post: null,
              singular: false);

        /// <summary>
        /// TOP n rows
        /// </summary>    
        public static SqlBuilder Top(this SqlBuilder sql, int n) =>
          sql.AddClause(
              keyword: "top",
              token: string.Concat("(", n.ToString(), ")"),
              glue: null,
              pre: "TOP",
              post: null);

        /// <summary>
        /// OFFSET x ROWS FETCH NEXT y ROWS ONLY
        /// </summary>    
        public static SqlBuilder OffsetFetch(this SqlBuilder sql, int offset, int fetch) =>
          sql.AddClause(
              keyword: "offset",
              token: string.Concat(offset.ToString(), " ROWS ", "FETCH NEXT ", fetch.ToString(), " ROWS ONLY"),
              glue: null,
              pre: "OFFSET ",
              post: null);
    }
}
