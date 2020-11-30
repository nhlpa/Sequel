namespace Sequel
{
    internal class SqlClause
    {        
        internal SqlClause(string glue, string pre, string[] tokens)
        {
            Glue = glue ?? string.Empty;
            Pre = pre ?? string.Empty;
            Tokens = tokens ?? new string[] { };
        }

        internal SqlClause(string glue, string pre, string token) : this(glue, pre, new[] { token }) { }

        internal string Pre { get; }
        internal string Glue { get; }
        internal string[] Tokens { get; }
    }
}
