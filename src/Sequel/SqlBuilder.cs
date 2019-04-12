using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sequel
  {
  public class SqlBuilder
    {
    private string _tmpl;
    private readonly IDictionary<string, SqlClauseSet> _clauses = new Dictionary<string, SqlClauseSet>();

    private static readonly IDictionary<string, string> _templates = new Dictionary<string, string>()
    {
      { "select", "||select|| ||top|| ||fields|| ||from|| ||join|| ||where|| ||groupby|| ||having|| ||orderby|| ||limit||" },
      { "insert", "||insert|| ||columns|| ||values||" },
      { "update", "||update|| ||set|| ||where||" },
      { "delete", "||delete|| ||from|| ||join|| ||where||" }
    };

    /// <summary>
    /// Using default templates
    /// </summary>
    public SqlBuilder()
      {
      _tmpl = _templates["select"];
      }

    /// <summary>
    /// Using custom template
    /// </summary>
    /// <param name="template"></param>
    public SqlBuilder(string template)
      {
      _tmpl = template;
      }

    /// <summary>
    /// Render SQL statement as string
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
      ToSql();

    /// <summary>
    /// Render SQL statement as string
    /// </summary>
    /// <returns></returns>
    public string ToSql()
      {
      foreach (var clauseSet in _clauses)
        {
        _tmpl = Regex.Replace(_tmpl, Concat("\\|\\|", clauseSet.Key, "\\|\\|"), clauseSet.Value.ToSql(), RegexOptions.IgnoreCase);
        }

      _tmpl = Regex.Replace(_tmpl, @"\|\|[a-z]+\|\|\s{0,1}", "").Trim();

      return _tmpl;
      }

    public SqlBuilder CrossApply(string tvf, string alias)
      => AddClause("join", Concat(tvf, " AS ", alias), " CROSS APPLY ", null, null, false);

    public SqlBuilder CrossApply(SqlBuilder sqlBuilder, string alias)
      => AddClause("join", Concat(sqlBuilder.ToSql(), " AS ", alias), " CROSS APPLY ", null, null, false);

    /// <summary>
    /// Register columns for INSERT
    /// </summary>
    /// <param name="into"></param>
    /// <returns></returns>
    public SqlBuilder Into(params string[] into) =>
      AddClause("columns", into, ", ", "(", ")");

    /// <summary>
    /// EXISTS predicate subquery
    /// </summary>
    /// <param name="sqlBuilder"></param>
    /// <returns></returns>
    public SqlBuilder Exists(SqlBuilder sqlBuilder) =>
      Where(Concat("EXISTS (", sqlBuilder.ToSql(), ")"));

    /// <summary>
    /// EXISTS predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder Exists(string predicate) =>
      Where(Concat("EXISTS (", predicate, ")"));

    /// <summary>
    /// FROM table
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public SqlBuilder From(string table) =>
      AddClause("from", table, null, "FROM ", null);

    /// <summary>
    /// FROM table with alias
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public SqlBuilder From(string table, string alias) =>
      From(Concat(table, " AS ", alias));

    /// <summary>
    /// FROM derived table
    /// </summary>
    /// <param name="derivedTable"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    public SqlBuilder From(SqlBuilder derivedTable, string alias) =>
      AddClause("from", derivedTable.ToSql(), null, "FROM (", ") as " + alias);

    /// <summary>
    /// DELETE clause
    /// </summary>
    /// <returns></returns>
    public SqlBuilder Delete()
      {
      _tmpl = _templates["delete"];
      return AddClause("delete", "", null, "DELETE ", null);
      }

    /// <summary>
    /// DELETE from alias
    /// </summary>
    /// <param name="alias"></param>
    /// <returns></returns>
    public SqlBuilder Delete(string alias)
      {
      _tmpl = _templates["delete"];
      return AddClause("delete", alias, null, "DELETE ", null);
      }

    /// <summary>
    /// GROUP BY columns
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public SqlBuilder GroupBy(params string[] columns) =>
      AddClause("groupby", columns, ", ", "GROUP BY ", null);

    /// <summary>
    /// HAVING predicates
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder Having(params string[] predicate) =>
      AddClause("having", predicate, ", ", "HAVING ", null);

    /// <summary>
    /// INSERT INTO table
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public SqlBuilder Insert(string table)
      {
      _tmpl = _templates["insert"];
      return AddClause("insert", table, null, "INSERT INTO ", null);
      }

    /// <summary>
    /// [INNER] JOIN table and predicate
    /// </summary>
    /// <param name="tableAndPredicate"></param>
    /// <returns></returns>
    public SqlBuilder Join(string tableAndPredicate) =>
      AddClause("join", tableAndPredicate, " INNER JOIN ", null, null, false);

    /// <summary>
    /// [INNER] JOIN table with predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder Join(string table, string predicate) =>
      Join(Concat(table, " ON ", predicate));

    /// <summary>
    /// [INNER] JOIN table with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder Join(string table, string alias, string predicate) =>
      Join(Concat(table, " AS ", alias, " ON ", predicate));

    /// <summary>
    /// [INNER] JOIN table from SqlBuilder with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder Join(SqlBuilder derivedTable, string alias, string predicate) =>
      Join(Concat("FROM (", derivedTable.ToSql(), ")"), alias, predicate);

    /// <summary>
    /// LEFT JOIN table and predicate
    /// </summary>
    /// <param name="tableAndPredicate"></param>
    /// <returns></returns>
    public SqlBuilder LeftJoin(string tableAndPredicate) =>
      AddClause("join", tableAndPredicate, " LEFT JOIN ", null, null, false);

    /// <summary>
    /// LEFT JOIN table with predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder LeftJoin(string table, string predicate) =>
      LeftJoin(Concat(table, " ON ", predicate));

    /// <summary>
    /// LEFT JOIN table with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder LeftJoin(string table, string alias, string predicate) =>
      LeftJoin(Concat(table, " AS ", alias, " ON ", predicate));

    /// <summary>
    /// LEFT JOIN table from SqlBuilder with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder LeftJoin(SqlBuilder derivedTable, string alias, string predicate) =>
      LeftJoin(Concat("FROM (", derivedTable.ToSql(), ")"), alias, predicate);

    /// <summary>
    /// LIMIT by n rows
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public SqlBuilder Limit(int n) =>
      AddClause("limit", n.ToString(), null, "LIMIT ", null, true);

    /// <summary>
    /// ORDER BY columns
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public SqlBuilder OrderBy(params string[] columns) =>
      AddClause("orderby", columns, ", ", "ORDER BY ", null);

    /// <summary>
    /// ORDER BY DESC columns (i.e. col desc, col2 desc)
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public SqlBuilder OrderByDesc(params string[] columns)
      {
      string[] columnsDesc = new string[columns.Length];
      for (var i = 0; i < columns.Length; i++)
        {
        columnsDesc[i] = columns[i] + " DESC";
        }

      return AddClause("orderby", columnsDesc, ", ", "ORDER BY ", null);
      }

    /// <summary>
    /// RIGHT JOIN table and predicate
    /// </summary>
    /// <param name="tableAndPredicate"></param>
    /// <returns></returns>
    public SqlBuilder RightJoin(string tableAndPredicate) =>
      AddClause("join", tableAndPredicate, " RIGHT JOIN ", null, null, false);

    /// <summary>
    /// RIGHT JOIN table with predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder RightJoin(string table, string predicate) =>
      RightJoin(Concat(table, " ON ", predicate));

    /// <summary>
    /// RIGHT JOIN table with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder RightJoin(string table, string alias, string predicate) =>
      RightJoin(Concat(table, " AS ", alias, " ON ", predicate));

    /// <summary>
    /// RIGHT JOIN table from SqlBuilder with alias and predicate
    /// </summary>
    /// <param name="table"></param>
    /// <param name="alias"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public SqlBuilder RightJoin(SqlBuilder derivedTable, string alias, string predicate) =>
      RightJoin(Concat("FROM (", derivedTable.ToSql(), ")"), alias, predicate);

    /// <summary>
    /// SELECT columns
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    public SqlBuilder Select(params string[] columns)
      {
      AddClause("select", string.Empty, string.Empty, "SELECT ", null);
      return AddClause("fields", columns, ", ", null, null);
      }

    /// <summary>
    /// SELECT columns and apply provided alias
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public SqlBuilder SelectWithAlias(string alias, params string[] columns)
      {
      string[] columnsAliased = new string[columns.Length];
      string aliasProper = (alias[alias.Length - 1] == '.') ? alias : alias + ".";

      for (var i = 0; i < columns.Length; i++)
        {
        columnsAliased[i] = aliasProper + columns[i];
        }

      AddClause("select", string.Empty, string.Empty, "SELECT ", null);
      return AddClause("fields", columnsAliased, ", ", null, null);
      }

    /// <summary>
    /// UPDATE &gt; SET column/value pairs
    /// </summary>
    /// <param name="columnAndValuePairs"></param>
    /// <returns></returns>
    public SqlBuilder Set(params string[] columnAndValuePairs) =>
      AddClause("set", columnAndValuePairs, ", ", "SET ", null);

    /// <summary>
    /// TOP n rows
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public SqlBuilder Top(int n) =>
      AddClause("top", Concat("(", n.ToString(), ")"), null, "TOP ", null, true);

    /// <summary>
    /// UPDATE table
    /// </summary>
    /// <param name="tableOrAlias"></param>
    /// <returns></returns>
    public SqlBuilder Update(string tableOrAlias)
      {
      _tmpl = _templates["update"];
      return AddClause("update", tableOrAlias, null, "UPDATE ", null);
      }

    /// <summary>
    /// INSERT single record
    /// </summary>
    /// <param name="columnAndValuePairs"></param>
    /// <returns></returns>
    public SqlBuilder Value(params string[] columnAndValuePairs) =>
      AddClause("values", columnAndValuePairs, ", ", "VALUES (", ")");

    /// <summary>
    /// INSERT multiple records
    /// </summary>
    /// <param name="columnAndValuePairs"></param>
    /// <returns></returns>
    public SqlBuilder Values(params string[] columnAndValuePairs) =>
      AddClause("values", columnAndValuePairs, "), (", "VALUES (", ")");

    /// <summary>
    /// WHERE [AND] predicates
    /// </summary>
    /// <param name="predicates"></param>
    /// <returns></returns>
    public SqlBuilder Where(params string[] predicates) =>
      AddClause("where", string.Join(" AND ", predicates), " AND ", "WHERE ", null);

    /// <summary>
    /// WHERE [OR] predicates
    /// </summary>
    /// <param name="predicates"></param>
    /// <returns></returns>
    public SqlBuilder WhereOr(params string[] predicates) =>
      AddClause("where", string.Join(" OR ", predicates), " OR ", "WHERE ", null);

    private SqlBuilder AddClause(string keyword, string[] sql, string glue, string pre, string post, bool singular = true) =>
      AddClause(keyword, string.Join(", ", sql), glue, pre, post, singular);

    private SqlBuilder AddClause(string keyword, string sql, string glue, string pre, string post, bool singular = true)
      {
      if (!_clauses.TryGetValue(keyword, out SqlClauseSet clauseSet))
        {
        clauseSet = new SqlClauseSet(glue, pre, post, singular);
        _clauses[keyword] = clauseSet;
        }

      clauseSet.Add(new SqlClause(sql, singular ? null : glue));

      return this;
      }

    private static string Concat(params string[] chunks)
      {
      return string.Join("", chunks);
      }
    }
  }