using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cinch.SqlBuilder
{
    public class SqlBuilder : ISqlBuilder
    {
        string template;
        readonly IDictionary<string, SqlClauseSet> clauses = new Dictionary<string, SqlClauseSet>();

        public SqlBuilder()
        {
            this.template = "||select|| ||from|| ||join|| ||where|| ||groupby|| ||having||";
        }

        public SqlBuilder(string template)
        {
            this.template = template;
        }

        public string ToSql()
        {
            foreach (var clauseSet in clauses)
            {
                template = Regex.Replace(template, $"\\|\\|{clauseSet.Key}\\|\\|", clauseSet.Value.ToSql(), RegexOptions.IgnoreCase);
            }

            template = Regex.Replace(template, @"\|\|[a-z]+\|\|", "");

            return template;
        }

        public ISqlBuilder Exists(ISqlBuilder sqlBuilder) =>
            Where($"EXISTS ({sqlBuilder.ToSql()})");

        public ISqlBuilder Exists(string sql) =>
            Where($"EXISTS ({sql})");

        public ISqlBuilder From(string sql) =>
            AddClause("from", sql, null, " FROM ", null);

        public ISqlBuilder From(ISqlBuilder sqlBuilder, string alias) =>
            AddClause("from", sqlBuilder.ToSql(), null, " FROM ( ", $" ) as {alias}");

        public ISqlBuilder GroupBy(string sql) =>
            AddClause("groupby", sql, ", ", "GROUP BY ", null);

        public ISqlBuilder Having(string sql) =>
            AddClause("having", sql, ", ", "having ", null);

        public ISqlBuilder Join(string sql) =>
            AddClause("join", sql, " INNER JOIN ", null, null, false);

        public ISqlBuilder LeftJoin(string sql) =>
            AddClause("join", sql, " LEFT JOIN ", null, null, false);

        public ISqlBuilder Select(string sql) =>
            AddClause("select", sql, ", ", "SELECT ", null);

        public ISqlBuilder Set(string sql) =>
            AddClause("set", sql, ", ", "SET ", null);

        public ISqlBuilder Update(string sql) =>
            AddClause("update", sql, null, " UPDATE ", null);

        public ISqlBuilder Where(string sql) =>
            AddClause("where", sql, " AND ", "WHERE ", null);

        public ISqlBuilder WhereOr(string sql) =>
            AddClause("where", sql, " OR ", "WHERE ", null);

        ISqlBuilder AddClause(string keyword, string sql, string glue, string pre, string post, bool singular = true)
        {
            SqlClauseSet _clauses = null;

            if (!clauses.TryGetValue(keyword, out _clauses))
            {
                _clauses = new SqlClauseSet(keyword, glue, pre, post, singular);
                clauses[keyword] = _clauses;
            }

            SqlClause clause;

            if (singular)
            {
                clause = new SqlClause(sql);
            }
            else
            {
                clause = new SqlClause(sql, glue);
            }

            _clauses.Add(clause);

            return this;
        }

        class SqlClauseSet : HashSet<SqlClause>
        {
            public SqlClauseSet(string keyword, string glue, string pre, string post, bool singular = true)
            {
                Glue = glue;
                Keyword = keyword;
                Post = post;
                Pre = pre;
                Singular = singular;
            }

            public string Glue { get; }
            public string Keyword { get; }
            public string Post { get; }
            public string Pre { get; }
            public bool Singular { get; }

            public string ToSql()
            {
                string sql;

                if (string.IsNullOrWhiteSpace(Glue))
                {
                    sql = this.FirstOrDefault().Sql;
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

        class SqlClause
        {
            public SqlClause(string sql)
            {
                Sql = sql;
            }

            public SqlClause(string sql, string glue)
            {
                Glue = glue;
                Sql = sql;
            }

            public string Glue { get; }
            public string Sql { get; }
        }
    }
}
