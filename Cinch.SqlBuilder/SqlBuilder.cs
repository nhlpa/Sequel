using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cinch.SqlBuilder
{
	public class SqlBuilder : ISqlBuilder
	{
		string template;
		readonly IDictionary<string, string> templates = new Dictionary<string, string>()
		{
			{ "default", "||select|| ||from|| ||join|| ||where|| ||groupby|| ||having|| ||orderby||" },
			{ "insert", "||insert|| ||columns|| ||values||" },
			{ "update", "||update|| ||set|| ||where||" },
		};
		readonly IDictionary<string, SqlClauseSet> clauses = new Dictionary<string, SqlClauseSet>();

		public SqlBuilder()
		{
			template = templates["default"];
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

		public ISqlBuilder Columns(params string[] sql) =>
			AddClause("columns", string.Join(", ", sql), ", ", "(", ")");

		public ISqlBuilder Exists(ISqlBuilder sqlBuilder) =>
			Where($"EXISTS ({sqlBuilder.ToSql()})");

		public ISqlBuilder Exists(string sql) =>
			Where($"EXISTS ({sql})");

		public ISqlBuilder From(string sql) =>
			AddClause("from", sql, null, "FROM ", null);

		public ISqlBuilder From(ISqlBuilder sqlBuilder, string alias) =>
			AddClause("from", sqlBuilder.ToSql(), null, "FROM (", $") as {alias}");

		public ISqlBuilder GroupBy(string sql) =>
			AddClause("groupby", sql, ", ", "GROUP BY ", null);

		public ISqlBuilder Having(string sql) =>
			AddClause("having", sql, ", ", "HAVING ", null);

		public ISqlBuilder Insert(string sql)
		{
			template = templates["insert"];
			return AddClause("insert", sql, null, "INSERT INTO ", null);
		}
		
		public ISqlBuilder Join(string sql) =>
			AddClause("join", sql, " INNER JOIN ", null, null, false);

		public ISqlBuilder LeftJoin(string sql) =>
			AddClause("join", sql, " LEFT JOIN ", null, null, false);

		public ISqlBuilder OrderBy(string sql) =>
			AddClause("orderby", sql, ", ", "ORDER BY ", null);

		public ISqlBuilder OrderByDesc(string sql) =>
			AddClause("orderby", sql.IndexOf("desc", StringComparison.OrdinalIgnoreCase) > -1 ? sql : $"{sql} DESC", ", ", "ORDER BY ", null);

		public ISqlBuilder Select(params string[] sql) =>
			AddClause("select", string.Join(", ", sql), ", ", "SELECT ", null);

		public ISqlBuilder Set(params string[] sql) =>
			AddClause("set", string.Join(", ", sql), ", ", "SET ", null);

		public ISqlBuilder Update(string sql)
		{
			template = templates["update"];
			return AddClause("update", sql, null, "UPDATE ", null);
		}

		public ISqlBuilder Value(params string[] sql) =>
			AddClause("values", string.Join(", ", sql), ", ", "VALUES (", ")");

		public ISqlBuilder Values(params string[] sql) =>
			AddClause("values", string.Join(", ", sql), "), (", "VALUES (", ")");

		public ISqlBuilder Where(string sql) =>
			AddClause("where", sql, " AND ", "WHERE ", null);

		public ISqlBuilder WhereOr(string sql) =>
			AddClause("where", sql, " OR ", "WHERE ", null);

		ISqlBuilder AddClause(string keyword, string sql, string glue, string pre, string post, bool singular = true)
		{
			SqlClauseSet _clauses = null;

			if (!clauses.TryGetValue(keyword, out _clauses))
			{
				_clauses = new SqlClauseSet(glue, pre, post, singular);
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
