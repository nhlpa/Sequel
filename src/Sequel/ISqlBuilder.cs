using System;

namespace Sequel
{
  public interface ISqlBuilder
  {
    string ToString();

    string ToSql();

    SqlBuilder Columns(params string[] sql);

    SqlBuilder Exists(SqlBuilder sqlBuilder);

    SqlBuilder Exists(string sql);

    SqlBuilder From(string sql);

    SqlBuilder From(SqlBuilder sqlBuilder, string alias);

    SqlBuilder Delete();

    SqlBuilder Delete(string sql);

    SqlBuilder GroupBy(params string[] sql);

    SqlBuilder Having(params string[] sql);

    SqlBuilder Insert(string sql);

    SqlBuilder Join(string sql);

    SqlBuilder LeftJoin(string sql);

    SqlBuilder Limit(int n);

    SqlBuilder OrderBy(params string[] sql);

    SqlBuilder OrderByDesc(params string[] sql);

    SqlBuilder Select(params string[] sql);

    SqlBuilder Set(params string[] sql);

    SqlBuilder Top(int n);

    SqlBuilder Update(string sql);

    SqlBuilder Value(params string[] sql);

    SqlBuilder Values(params string[] sql);

    SqlBuilder Where(params string[] sql);

    SqlBuilder WhereOr(params string[] sql);
  }
}