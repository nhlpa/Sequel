namespace Sequel
{
  public interface ISqlMapper<TEntity>
  {
    /// <summary>
    /// Table name, can include schema
    /// </summary>
    string Table { get; }

    /// <summary>
    /// Primary key column name
    /// </summary>
    object Key { get; }

    /// <summary>
    /// All column names
    /// </summary>
    string[] Fields { get; }

    /// <summary>
    /// All column names prefixed with Table
    /// </summary>
    string[] FieldsQualified { get; }

    /// <summary>
    /// Column names excluding Key
    /// </summary>
    string[] NonKeyFields { get; }

    /// <summary>
    /// Column names excluding Key prefixed with Table
    /// </summary>
    string[] NonKeyFieldsQualified { get; }

    /// <summary>
    /// INSERT statement
    /// </summary>
    SqlBuilder CreateSql { get; }

    /// <summary>
    /// SELECT statement
    /// Sets parameter @{Key}
    /// </summary>
    SqlBuilder ReadSql { get; }

    /// <summary>
    /// UPDATE statement
    /// Sets parameter @{Key}
    /// </summary>
    SqlBuilder UpdateSql { get; }

    /// <summary>
    /// DELETE statement
    /// Sets parameter @{Key}
    /// </summary>
    SqlBuilder DeleteSql { get; }
  }
}