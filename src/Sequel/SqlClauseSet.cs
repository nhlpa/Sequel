using System.Collections.Generic;

namespace Sequel
{
    internal class SqlClauseSet : List<SqlClause>
    {
        
        internal SqlClauseSet(string glue, string pre, string post, bool predicate)
        {
            Glue = glue ?? string.Empty;
            Post = post ?? string.Empty;
            Predicate = predicate;
            Pre = pre ?? string.Empty;            
        }

        internal string Glue { get; }
        internal string Post { get; }
        public bool Predicate { get; }
        internal string Pre { get; }
        
        public override string ToString() => ToSql();

        internal string ToSql()
        {
            if (Count == 0)
            {
                return string.Empty;
            }
            else if (Predicate)
            {
                var clauses = new string[this.Count];

                for (int i = 0; i < this.Count; i++)
                {
                    var x = this[i];
                    clauses[i] = string.Concat(i == 0 ? x.Pre : x.Glue, string.Join(x.Glue, x.Tokens));
                }

                return string.Concat(Pre, string.Concat(clauses), Post);
            }
            else
            {
                var clauses = new string[this.Count];

                for (int i = 0; i < this.Count; i++)
                {
                    var x = this[i];
                    clauses[i] = string.Concat(x.Pre, string.Join(x.Glue, x.Tokens));
                }

                return string.Concat(Pre, string.Concat(string.Join(Glue, clauses), Post));
            }
        }
    }
}
