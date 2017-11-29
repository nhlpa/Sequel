using System;
using System.Collections.Generic;
using System.Text;

namespace Cinch.SqlBuilder
{
    public interface ISqlBuilder
    {
        string ToSql();
        
        ISqlBuilder Columns(params string[] sql);
        ISqlBuilder Exists(ISqlBuilder sqlBuilder);
        ISqlBuilder Exists(string sql);
        ISqlBuilder From(string sql);
        ISqlBuilder From(ISqlBuilder sqlBuilder, string alias);
        ISqlBuilder GroupBy(string sql);
        ISqlBuilder Having(string sql);
        ISqlBuilder Insert(string sql);
        ISqlBuilder Join(string sql);
        ISqlBuilder LeftJoin(string sql);
        ISqlBuilder OrderBy(string sql);
        ISqlBuilder OrderByDesc(string sql);
        ISqlBuilder Select(params string[] sql);
        ISqlBuilder Set(params string[] sql);
        ISqlBuilder Update(string sql);
        ISqlBuilder Value(params string[] sql)
        ISqlBuilder Values(params string[] sql);
        ISqlBuilder Where(string sql);
        ISqlBuilder WhereOr(string sql);
    }
}
