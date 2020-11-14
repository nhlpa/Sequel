namespace Sequel
{
    internal class SqlClause
    {
        internal SqlClause(string[] tokens, string glue = null)
        {
            Glue = glue ?? string.Empty;
            Tokens = tokens;
        }

        internal SqlClause(string token, string glue) : this(new[] { token }, glue) { }

        internal string Glue { get; }
        internal string[] Tokens { get; }

        public override string ToString() => ToSql();

        internal string ToSql() => string.Concat(Glue, string.Join(Glue, Tokens));
    }
}
