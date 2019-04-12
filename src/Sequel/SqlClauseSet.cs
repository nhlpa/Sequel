using System.Collections.Generic;
using System.Text;

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
      var sb = new StringBuilder(Pre);
      
      if (string.IsNullOrWhiteSpace(Glue))
      {
        // No glue, likely raw SQL
        sb.Append(this[Count - 1].Sql);
      }
      else if (!Singular)
      {
        // Not singular, add each clause and prepend glue
        for (var i = 0; i < Count; i++)
        {
          sb.Append(this[i].Glue);
          sb.Append(this[i].Sql);
        }
      }
      else
      {
        // Singular, add each clause and prepend glue for all but first item
        for (var i = 0; i < Count; i++)
        {
          if (i == 0)
          {
            sb.Append(this[i].Sql);            
          }
          else
          {
            sb.Append(Glue);
            sb.Append(this[i].Sql);
          }
        }
      }

      sb.Append(Post);
      return sb.ToString().Trim();
    }
  }
}
