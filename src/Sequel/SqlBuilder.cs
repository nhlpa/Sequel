using System;
using System.Collections.Generic;
using System.Linq;

namespace Sequel
{
    public class SqlBuilder
    {
        private const string _defaultTemplate = "select";

        private static readonly IDictionary<string, string[]> _templates = new Dictionary<string, string[]>()
        {
          { "select", new [] { "select", "top", "fields", "from", "join", "where", "groupby", "having", "orderby", "limit", "offset" } },
          { "insert", new [] { "insert", "columns", "values" } },
          { "update", new [] { "update", "set", "where" } },
          { "delete", new [] { "delete", "from", "join", "where" } }
        };

        private string[] _template;
        private readonly IDictionary<string, SqlClauseSet> _clauses = new Dictionary<string, SqlClauseSet>();

        /// <summary>
        /// Using default templates
        /// </summary>
        public SqlBuilder()
        {
            SetTemplate(_defaultTemplate);
        }

        /// <summary>
        /// Using custom template
        /// </summary>
        /// <param name="template"></param>
        public SqlBuilder(string template)
        {
            SetTemplate(template);
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
            return string.Join(" ", _template.Where(key => _clauses.ContainsKey(key)).Select(key => _clauses[key].ToSql()));
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