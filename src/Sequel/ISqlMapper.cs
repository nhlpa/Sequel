using System;

namespace Sequel
{
  public interface ISqlMapper<TEntity>
  {
    string Table { get; }
    object Key { get; }
    string[] Fields { get; }
    string[] NonKeyFields { get; }
    SqlBuilder CreateSql { get; }
    SqlBuilder ReadSql { get; }
    SqlBuilder UpdateSql { get; }
    SqlBuilder DeleteSql { get; }
  }
}