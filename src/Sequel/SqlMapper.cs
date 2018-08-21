using FastMember;
using Sequel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sequel
{
  public class SqlMapper<TEntity> : ISqlMapper<TEntity>
  {
    private Type entityType;
    private TypeAccessor entityTypeAccessor;
    private IEnumerable<Member> members;
    private string table;
    private string key;
    private string[] fields;
    private string[] nonKeyFields;

    public SqlMapper()
    {
    }

    public SqlMapper(string table = null, string key = null, string[] fields = null)
    {
      if (!string.IsNullOrWhiteSpace(table))
      {
        this.table = table;
      }

      if (!string.IsNullOrWhiteSpace(key))
      {
        this.key = key;
      }

      if (fields != null && fields.Any())
      {
        this.fields = fields;
      }
    }

    public virtual string Table =>
      table ?? (table = EntityType.Name);

    public virtual string Key =>
      key ?? (key = "Id");

    public virtual string KeyQualified =>
      $"{Table}.{Key}";

    public virtual string[] Fields =>
      fields ?? (fields = Members.Select(p => p.Name).ToArray());

    public virtual string[] FieldsQualified =>
      Fields.Select(f => $"{Table}.{f}").ToArray();

    public virtual string[] NonKeyFields =>
      nonKeyFields ?? (nonKeyFields = Fields.Where(f => !string.Equals(f, $"{Key}", StringComparison.OrdinalIgnoreCase)).ToArray());

    public virtual string[] NonKeyFieldsQualified =>
      NonKeyFields.Select(f => $"{Table}.{f}").ToArray();

    public virtual SqlBuilder CreateSql
    {
      get
      {
        return new SqlBuilder()
        .Insert(Table)
        .Columns(NonKeyFields)
        .Values(string.Join(", ", NonKeyFields.Select(f => $"@{f}")));
      }
    }

    public virtual SqlBuilder ReadSql
    {
      get
      {
        return new SqlBuilder()
        .Select(FieldsQualified)
        .From(Table);
      }
    }

    public virtual SqlBuilder UpdateSql
    {
      get
      {
        return new SqlBuilder()
        .Update(Table)
        .Set(NonKeyFields.Select(f => $"{f} = @{f}").ToArray())
        .Where($"{Key} = @{Key}");
      }
    }

    public virtual SqlBuilder DeleteSql
    {
      get
      {
        return new SqlBuilder()
        .Delete()
        .From(Table)
        .Where($"{Key} = @{Key}");
      }
    }

    private Type EntityType =>
      entityType ?? (entityType = typeof(TEntity));

    private TypeAccessor EntityTypeAccessor =>
      entityTypeAccessor ?? (entityTypeAccessor = TypeAccessor.Create(EntityType));

    private IEnumerable<Member> Members
    {
      get
      {
        if (members == null)
        {
          members = EntityTypeAccessor
            .GetMembers()
            .Where(m =>
              m.CanWrite
              && m.Type.IsPublic
              && string.Equals(m.Type.Namespace, "system", StringComparison.OrdinalIgnoreCase)
              && !typeof(ICollection<>).IsAssignableFrom(m.Type)
            );
        }

        return members;
      }
    }
  }
}