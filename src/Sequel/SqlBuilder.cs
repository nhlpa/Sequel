using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sequel
{
  public class SqlBuilder
  {
    private string _template;
    private readonly IDictionary<string, SqlClauseSet> _clauses = new Dictionary<string, SqlClauseSet>();

    private static readonly IDictionary<string, string> _templates = new Dictionary<string, string>()
    {
      { "select", "||select|| ||top|| ||fields|| ||from|| ||join|| ||where|| ||groupby|| ||having|| ||orderby|| ||limit|| ||offset||" },
      { "insert", "||insert|| ||columns|| ||values||" },
      { "update", "||update|| ||set|| ||where||" },
      { "delete", "||delete|| ||from|| ||join|| ||where||" }
    };

    /// <summary>
    /// Using default templates
    /// </summary>
    public SqlBuilder()
    {
      _template = _templates["select"];
    }

    /// <summary>
    /// Using custom template
    /// </summary>
    /// <param name="template"></param>
    public SqlBuilder(string template)
    {
      _template = template;
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
        _template = Regex.Replace(_template, Concat("\\|\\|", clauseSet.Key, "\\|\\|"), clauseSet.Value.ToSql(), RegexOptions.IgnoreCase);
      }

      _template = Regex.Replace(_template, @"\|\|[a-z]+\|\|\s{0,1}", "").Trim();

      return _template;
    }

    internal SqlBuilder AddClause(string keyword, string[] sql, string glue, string pre, string post, bool singular = true) =>
      AddClause(keyword, string.Join(", ", sql), glue, pre, post, singular);

    internal SqlBuilder AddClause(string keyword, string sql, string glue, string pre, string post, bool singular = true)
    {
      if (!_clauses.TryGetValue(keyword, out SqlClauseSet clauseSet))
      {
        clauseSet = new SqlClauseSet(glue, pre, post, singular);
        _clauses[keyword] = clauseSet;
      }

      clauseSet.Add(new SqlClause(sql, singular ? null : glue));

      return this;
    }

    internal void SetTemplate(string template)
      => _template = _templates.ContainsKey(template) ? _templates[template] : throw new ArgumentException("Invalid template");

    internal static string Concat(params string[] chunks)
    {

      return string.Join("", chunks);
    }
  }
}