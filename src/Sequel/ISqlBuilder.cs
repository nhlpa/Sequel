using System;

namespace Sequel
{
  public interface ISqlBuilder
  {
    string ToString();

    string ToSql();

    SqlBuilder Columns(params string[] columns);

    SqlBuilder Exists(SqlBuilder subquerySqlBuilder);

    SqlBuilder Exists(string predicate);

    SqlBuilder From(string table);

    SqlBuilder From(SqlBuilder derviedTable, string alias);

    SqlBuilder Delete();

    SqlBuilder Delete(string alias);

    SqlBuilder GroupBy(params string[] columns);

    SqlBuilder Having(params string[] predicates);

    SqlBuilder Insert(string table);

    SqlBuilder Join(string tableAndPredicate);

    SqlBuilder LeftJoin(string tableAndPredicate);

    SqlBuilder Limit(int n);

    SqlBuilder OrderBy(params string[] columns);

    SqlBuilder OrderByDesc(params string[] columns);

    SqlBuilder Select(params string[] columns);

    SqlBuilder SelectWithAlias(string alias, params string[] columns);

    SqlBuilder Set(params string[] columnAndValuePairs);

    SqlBuilder Top(int n);

    SqlBuilder Update(string tableOrAlias);

    SqlBuilder Value(params string[] columnAndValuePairs);

    SqlBuilder Values(params string[] columnAndValuePairs);

    SqlBuilder Where(params string[] predicates);

    SqlBuilder WhereOr(params string[] predicates);
  }
}