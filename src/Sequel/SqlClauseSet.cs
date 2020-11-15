using System.Collections.Generic;
using System.Linq;

namespace Sequel
{
    internal class SqlClauseSet : List<SqlClause>
    {
        
        internal SqlClauseSet(string glue, string pre, string post)
        {
            Glue = glue ?? string.Empty;
            Post = post ?? string.Empty;
            Pre = pre ?? string.Empty;
            //Singular = singular;
        }

        internal string Glue { get; }
        internal string Post { get; }
        internal string Pre { get; }
        //internal bool Singular { get; }

        public override string ToString() => ToSql();

        internal string ToSql()
        {
            if (Count == 0)
            {
                return string.Empty;
            }
            //else if (!Singular) {
            //    return string.Concat(
            //        Pre,
            //        string.Concat(
            //            string.Concat(                            
            //                string.Join(
            //                    " ",
            //                    Flatten())),
            //            Post));
            //}
            //else
            //{                
                return string.Concat(
                    Pre,
                    string.Concat(
                        string.Join(
                            Glue,
                            Flatten()),
                        Post));
            //}
        }

        private string[] Flatten() => this.Select(x => x.ToSql()).ToArray();
    }
}
