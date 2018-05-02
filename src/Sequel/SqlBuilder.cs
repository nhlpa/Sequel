using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sequel
{
  public class SqlBuilder
  {
    private string template;

    private readonly IDictionary<string, string> templates = new Dictionary<string, string>()
    {
      { "select", "||select|| ||from|| ||join|| ||where|| ||groupby|| ||having|| ||orderby||" },
      { "insert", "||insert|| ||columns|| ||values||" },
      { "update", "||update|| ||set|| ||where||" },
      { "delete", "||delete|| ||from|| ||join|| ||where||" }
    };

    private readonly IDictionary<string, SqlClauseSet> clauses = new Dictionary<string, SqlClauseSet>();

    public SqlBuilder()
    {
      template = templates["select"];
    }

    public SqlBuilder(string template)
    {
      this.template = template;
    }

    public override string ToString()
    {
      return ToSql();
    }

    public string ToSql()
    {
      foreach (var clauseSet in clauses)
      {
        template = Regex.Replace(template, $"\\|\\|{clauseSet.Key}\\|\\|", clauseSet.Value.ToSql(), RegexOptions.IgnoreCase);
      }

      template = Regex.Replace(template, @"\|\|[a-z]+\|\|\s{0,1}", "").Trim();

      return template;
    }

    public SqlBuilder Columns(params string[] sql) =>
      AddClause("columns", sql, ", ", "(", ")");

    public SqlBuilder Exists(SqlBuilder sqlBuilder) =>
      Where($"EXISTS ({sqlBuilder.ToSql()})");

    public SqlBuilder Exists(string sql) =>
      Where($"EXISTS ({sql})");

    public SqlBuilder From(string sql) =>
      AddClause("from", sql, null, "FROM ", null);

    public SqlBuilder From(SqlBuilder sqlBuilder, string alias) =>
      AddClause("from", sqlBuilder.ToSql(), null, "FROM (", $") as {alias}");

    public SqlBuilder Delete()
    {
      template = templates["delete"];
      return AddClause("delete", "", null, "DELETE ", null);
    }

    public SqlBuilder Delete(string sql)
    {
      template = templates["delete"];
      return AddClause("delete", sql, null, "DELETE ", null);
    }

    public SqlBuilder GroupBy(params string[] sql) =>
      AddClause("groupby", sql, ", ", "GROUP BY ", null);

    public SqlBuilder Having(params string[] sql) =>
      AddClause("having", sql, ", ", "HAVING ", null);

    public SqlBuilder Insert(string sql)
    {
      template = templates["insert"];
      return AddClause("insert", sql, null, "INSERT INTO ", null);
    }

    public SqlBuilder Join(string sql) =>
      AddClause("join", sql, " INNER JOIN ", null, null, false);

    public SqlBuilder LeftJoin(string sql) =>
      AddClause("join", sql, " LEFT JOIN ", null, null, false);

    public SqlBuilder OrderBy(params string[] sql) =>
      AddClause("orderby", sql, ", ", "ORDER BY ", null);

    public SqlBuilder OrderByDesc(string sql) =>
      AddClause("orderby", sql.IndexOf("desc", StringComparison.OrdinalIgnoreCase) > -1 ? sql : $"{sql} DESC", ", ", "ORDER BY ", null);

    public SqlBuilder Select(params string[] sql) =>
      AddClause("select", sql, ", ", "SELECT ", null);

    public SqlBuilder SelectTop(int n, params string[] sql) =>
      AddClause("select", sql, ", ", $"SELECT TOP {n} ", null);

    public SqlBuilder Set(params string[] sql) =>
      AddClause("set", sql, ", ", "SET ", null);

    public SqlBuilder Update(string sql)
    {
      template = templates["update"];
      return AddClause("update", sql, null, "UPDATE ", null);
    }

    public SqlBuilder Value(params string[] sql) =>
      AddClause("values", sql, ", ", "VALUES (", ")");

    public SqlBuilder Values(params string[] sql) =>
      AddClause("values", sql, "), (", "VALUES (", ")");

    public SqlBuilder Where(params string[] sql) =>
      AddClause("where", string.Join(" AND ", sql), " AND ", "WHERE ", null);

    public SqlBuilder WhereOr(params string[] sql) =>
      AddClause("where", string.Join(" OR ", sql), " OR ", "WHERE ", null);

    private SqlBuilder AddClause(string keyword, string[] sql, string glue, string pre, string post, bool singular = true) =>
      AddClause(keyword, string.Join(", ", sql), glue, pre, post, singular);

    private SqlBuilder AddClause(string keyword, string sql, string glue, string pre, string post, bool singular = true)
    {
      if (!clauses.TryGetValue(keyword, out SqlClauseSet clauseSet))
      {
        clauseSet = new SqlClauseSet(glue, pre, post, singular);
        clauses[keyword] = clauseSet;
      }

      clauseSet.Add(new SqlClause(sql, singular ? null : glue));

      return this;
    }

    private class SqlClauseSet : HashSet<SqlClause>
    {
      public SqlClauseSet(string glue, string pre, string post, bool singular = true)
      {
        Glue = glue;
        Post = post;
        Pre = pre;
        Singular = singular;
      }

      public string Glue { get; }
      public string Post { get; }
      public string Pre { get; }
      public bool Singular { get; }

      public string ToSql()
      {
        string sql;

        if (string.IsNullOrWhiteSpace(Glue))
        {
          sql = this.LastOrDefault().Sql;
        }
        else if (!Singular)
        {
          sql = string.Join("", this.Select(clause => $"{clause.Glue}{clause.Sql}"));
        }
        else
        {
          sql = string.Join(Glue, this.Select(clause => clause.Sql));
        }

        return $"{Pre}{sql}{Post}".Trim();
      }
    }

    private class SqlClause
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
}