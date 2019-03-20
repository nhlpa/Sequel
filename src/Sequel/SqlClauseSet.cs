using System.Collections.Generic;

namespace Sequel
{
  public class SqlClauseSet : List<SqlClause>
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
      var sql = string.Empty;

      if (string.IsNullOrWhiteSpace(Glue))
      {
        sql = this[Count - 1].Sql;
      }
      else if (!Singular)
      {
        for (var i = 0; i < Count; i++)
        {
          sql += this[i].Glue + this[i].Sql;
        }
      }
      else
      {
        for (var i = 0; i < Count; i++)
        {
          if (i == 0)
          {
            sql += this[i].Sql;
          }
          else
          {
            sql += Glue + this[i].Sql;
          }
        }
      }

      return string.Join("", Pre, sql, Post).Trim();
    }
  }
}
