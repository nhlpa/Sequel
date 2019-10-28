namespace Sequel
{
  public static class SqlBuilderExtensions
  {
    /// <summary>
    /// Register columns for INSERT
    /// </summary>
    /// <param name="into"></param>
    /// <returns></returns>
    public static SqlBuilder Into(this SqlBuilder sql, params string[] into) =>
      sql.AddClause("columns", into, ", ", "(", ")");

    /// <summary>
    /// EXISTS predicate subquery
    /// </summary>
    /// <param name="sqlBuilder"></param>
    /// <returns></returns>
    public static SqlBuilder Exists(this SqlBuilder sql, SqlBuilder sqlBuilder) =>
      sql.Where(string.Concat("EXISTS (", sqlBuilder.ToSql(), ")"));

    /// <summary>
    /// EXISTS predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder Exists(this SqlBuilder sql, string predicate) =>
      sql.Where(string.Concat("EXISTS (", predicate, ")"));

    /// <summary>
    /// FROM table
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public static SqlBuilder From(this SqlBuilder sql, string table) =>
      sql.AddClause("from", table, null, "FROM ", null);

    /// <summary>
    /// FROM table with alias
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static SqlBuilder From(this SqlBuilder sql, string table, string alias) =>
      sql.From(string.Concat(table, " AS ", alias));

    /// <summary>
    /// FROM derived table
    /// </summary>
    /// <param name="derivedTable"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static SqlBuilder From(this SqlBuilder sql, SqlBuilder derivedTable, string alias) =>
      sql.AddClause("from", derivedTable.ToSql(), null, "FROM (", ") as " + alias);

    /// <summary>
    /// DELETE clause
    /// </summary>
    /// <returns></returns>
    public static SqlBuilder Delete(this SqlBuilder sql)
    {
      sql.SetTemplate("delete");
      return sql.AddClause("delete", "", null, "DELETE ", null);
    }

    /// <summary>
    /// DELETE from alias
    /// </summary>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static SqlBuilder Delete(this SqlBuilder sql, string alias)
    {
      sql.SetTemplate("delete");
      return sql.AddClause("delete", alias, null, "DELETE ", null);
    }

    /// <summary>
    /// GROUP BY columns
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static SqlBuilder GroupBy(this SqlBuilder sql, params string[] columns) =>
      sql.AddClause("groupby", columns, ", ", "GROUP BY ", null);

    /// <summary>
    /// HAVING predicates
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder Having(this SqlBuilder sql, params string[] predicate) =>
      sql.AddClause("having", predicate, ", ", "HAVING ", null);

    /// <summary>
    /// INSERT INTO table
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public static SqlBuilder Insert(this SqlBuilder sql, string table)
    {
      sql.SetTemplate("insert");
      return sql.AddClause("insert", table, null, "INSERT INTO ", null);
    }

    /// <summary>
    /// [INNER] JOIN table and predicate
    /// </summary>
    /// <param name="tableAndPredicate"></param>
    /// <returns></returns>
    public static SqlBuilder Join(this SqlBuilder sql, string tableAndPredicate) =>
      sql.AddClause("join", tableAndPredicate, " INNER JOIN ", null, null, false);

    /// <summary>
    /// [INNER] JOIN table with predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder Join(this SqlBuilder sql, string table, string predicate) =>
      sql.Join(string.Concat(table, " ON ", predicate));

    /// <summary>
    /// [INNER] JOIN table with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder Join(this SqlBuilder sql, string table, string alias, string predicate) =>
      sql.Join(string.Concat(table, " AS ", alias, " ON ", predicate));

    /// <summary>
    /// [INNER] JOIN table from SqlBuilder with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder Join(this SqlBuilder sql, SqlBuilder derivedTable, string alias, string predicate) =>
      sql.Join(string.Concat("(", derivedTable.ToSql(), ")"), alias, predicate);
    //  sql.AddClause("join", string.Concat("(", sqlBuilder.ToSql(), ") AS ", alias), " CROSS APPLY ", null, null, false);

    /// <summary>
    /// LEFT JOIN table and predicate
    /// </summary>
    /// <param name="tableAndPredicate"></param>
    /// <returns></returns>
    public static SqlBuilder LeftJoin(this SqlBuilder sql, string tableAndPredicate) =>
      sql.AddClause("join", tableAndPredicate, " LEFT JOIN ", null, null, false);

    /// <summary>
    /// LEFT JOIN table with predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder LeftJoin(this SqlBuilder sql, string table, string predicate) =>
      sql.LeftJoin(string.Concat(table, " ON ", predicate));

    /// <summary>
    /// LEFT JOIN table with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder LeftJoin(this SqlBuilder sql, string table, string alias, string predicate) =>
      sql.LeftJoin(string.Concat(table, " AS ", alias, " ON ", predicate));

    /// <summary>
    /// LEFT JOIN table from SqlBuilder with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder LeftJoin(this SqlBuilder sql, SqlBuilder derivedTable, string alias, string predicate) =>
      sql.LeftJoin(string.Concat("(", derivedTable.ToSql(), ")"), alias, predicate);

    /// <summary>
    /// ORDER BY columns
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static SqlBuilder OrderBy(this SqlBuilder sql, params string[] columns) =>
      sql.AddClause("orderby", columns, ", ", "ORDER BY ", null);

    /// <summary>
    /// ORDER BY DESC columns (i.e. col desc, col2 desc)
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static SqlBuilder OrderByDesc(this SqlBuilder sql, params string[] columns)
    {
      string[] columnsDesc = new string[columns.Length];
      for (var i = 0; i < columns.Length; i++)
      {
        columnsDesc[i] = columns[i] + " DESC";
      }

      return sql.AddClause("orderby", columnsDesc, ", ", "ORDER BY ", null);
    }

    /// <summary>
    /// RIGHT JOIN table and predicate
    /// </summary>
    /// <param name="tableAndPredicate"></param>
    /// <returns></returns>
    public static SqlBuilder RightJoin(this SqlBuilder sql, string tableAndPredicate) =>
      sql.AddClause("join", tableAndPredicate, " RIGHT JOIN ", null, null, false);

    /// <summary>
    /// RIGHT JOIN table with predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder RightJoin(this SqlBuilder sql, string table, string predicate) =>
      sql.RightJoin(string.Concat(table, " ON ", predicate));

    /// <summary>
    /// RIGHT JOIN table with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder RightJoin(this SqlBuilder sql, string table, string alias, string predicate) =>
      sql.RightJoin(string.Concat(table, " AS ", alias, " ON ", predicate));

    /// <summary>
    /// RIGHT JOIN table from SqlBuilder with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static SqlBuilder RightJoin(this SqlBuilder sql, SqlBuilder derivedTable, string alias, string predicate) =>
      sql.RightJoin(string.Concat("FROM (", derivedTable.ToSql(), ")"), alias, predicate);

    /// <summary>
    /// SELECT columns
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static SqlBuilder Select(this SqlBuilder sql, params string[] columns)
    {
      sql.AddClause("select", string.Empty, string.Empty, "SELECT ", null);
      return sql.AddClause("fields", columns, ", ", null, null);
    }

    /// <summary>
    /// SELECT columns and apply provided alias
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static SqlBuilder SelectWithAlias(this SqlBuilder sql, string alias, params string[] columns)
    {
      string[] columnsAliased = new string[columns.Length];
      string aliasProper = (alias[alias.Length - 1] == '.') ? alias : alias + ".";

      for (var i = 0; i < columns.Length; i++)
      {
        columnsAliased[i] = aliasProper + columns[i];
      }

      sql.AddClause("select", string.Empty, string.Empty, "SELECT ", null);
      return sql.AddClause("fields", columnsAliased, ", ", null, null);
    }

    /// <summary>
    /// UPDATE &gt; SET column/value pairs
    /// </summary>
    /// <param name="columnAndValuePairs"></param>
    /// <returns></returns>
    public static SqlBuilder Set(this SqlBuilder sql, params string[] columnAndValuePairs) =>
      sql.AddClause("set", columnAndValuePairs, ", ", "SET ", null);

    /// <summary>
    /// UPDATE table
    /// </summary>
    /// <param name="tableOrAlias"></param>
    /// <returns></returns>
    public static SqlBuilder Update(this SqlBuilder sql, string tableOrAlias)
    {
      sql.SetTemplate("update");
      return sql.AddClause("update", tableOrAlias, null, "UPDATE ", null);
    }

    /// <summary>
    /// INSERT single record
    /// </summary>
    /// <param name="columnAndValuePairs"></param>
    /// <returns></returns>
    public static SqlBuilder Value(this SqlBuilder sql, params string[] columnAndValuePairs) =>
      sql.AddClause("values", columnAndValuePairs, ", ", "VALUES (", ")");

    /// <summary>
    /// INSERT multiple records
    /// </summary>
    /// <param name="columnAndValuePairs"></param>
    /// <returns></returns>
    public static SqlBuilder Values(this SqlBuilder sql, params string[] columnAndValuePairs) =>
      sql.AddClause("values", columnAndValuePairs, "), (", "VALUES (", ")");

    /// <summary>
    /// WHERE [AND] predicates
    /// </summary>
    /// <param name="predicates"></param>
    /// <returns></returns>
    public static SqlBuilder Where(this SqlBuilder sql, params string[] predicates) =>
      sql.AddClause("where", string.Join(" AND ", predicates), " AND ", "WHERE ", null);

    /// <summary>
    /// WHERE [OR] predicates
    /// </summary>
    /// <param name="predicates"></param>
    /// <returns></returns>
    public static SqlBuilder WhereOr(this SqlBuilder sql, params string[] predicates) =>
      sql.AddClause("where", string.Join(" OR ", predicates), " OR ", "WHERE ", null);
  }
}
