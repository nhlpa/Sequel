using System;
using System.Collections.Generic;
using System.Linq;

namespace Sequel
{
    /// <summary>
    /// SQL builder with an interface that emulates writing actual SQL queries.
    /// </summary>
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
        private string _pre;
        private string _post;

        /// <summary>
        /// Construct a new instance of SqlBuilder with (optional) pre- and post-SQL
        /// </summary>
        public SqlBuilder(string pre = null, string post = null)
        {
            SetTemplate(_defaultTemplate);
            _pre = pre;
            _post = post;
        }

        /// <summary>
        /// Render SQL statement as string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToSql();

        /// <summary>
        /// Render SQL statement as string
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            var templateClauses = 
                _template
                    .Where(key => _clauses.ContainsKey(key))
                    .Select(key => _clauses[key].ToSql());

            var sql = string.Join(" ", templateClauses);

            if(!string.IsNullOrWhiteSpace(_pre) && !string.IsNullOrWhiteSpace(_post))
            {
                return string.Concat(string.Concat(_pre, sql), _post);
            }
            else if (!string.IsNullOrWhiteSpace(_pre))
            {
                return string.Concat(_pre, sql);
            }
            else if (!string.IsNullOrWhiteSpace(_post))
            {
                return string.Concat(sql, _post);
            }
            else {
                return sql;
            }
        }

        internal SqlBuilder AddClause(string keyword, string glue, string pre, string post, bool singular = true) =>
            AddClause(keyword, new string[] {}, glue, pre, post, singular);

        internal SqlBuilder AddClause(string keyword, string token, string glue, string pre, string post, bool singular = true) => 
            AddClause(keyword, new[] { token }, glue, pre, post, singular);

        internal SqlBuilder AddClause(string keyword, string[] tokens, string glue, string pre, string post, bool singular = true)
        {
            if (!_clauses.ContainsKey(keyword))
            {                
                _clauses[keyword] = new SqlClauseSet(singular ? glue : " ", pre, post);
            }

            _clauses[keyword].Add(new SqlClause(glue, singular ? null : glue, tokens));

            return this;
        }

        internal void SetTemplate(string template) => 
            _template = _templates.ContainsKey(template) ? 
                _templates[template] : 
                throw new ArgumentException("Invalid template");

        internal void SetTemplate(string[] template) =>
            _template = template.Length > 0 ?
                template :
                throw new ArgumentException("Custom template has no elements");
    }
}