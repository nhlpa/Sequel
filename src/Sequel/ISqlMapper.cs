namespace Sequel
{
  public interface ISqlMapper<TEntity>
  {
    string Table { get; }
    string Key { get; }
    string KeyQualified { get; }

    string[] Fields { get; }

    string[] FieldsQualified { get; }

    string[] NonKeyFields { get; }

    string[] NonKeyFieldsQualified { get; }

    SqlBuilder CreateSql { get; }

    SqlBuilder ReadSql { get; }

    SqlBuilder UpdateSql { get; }

    SqlBuilder DeleteSql { get; }
  }
}