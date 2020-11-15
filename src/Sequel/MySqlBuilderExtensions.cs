namespace Sequel.MySql
{
    /// <summary>
    /// SQL Builder extensions for MySQL/MariaDB
    /// </summary>
    public static class MySqlBuilderExtensions
    {
        /// <summary>
        /// LIMIT by n rows
        /// </summary>        
        public static SqlBuilder Limit(this SqlBuilder sql, int n) =>
          sql.AddClause(
              keyword: "limit",
              token: n.ToString(),
              glue: null,
              pre: "LIMIT ",
              post: null);

    }
}
