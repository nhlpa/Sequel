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

    private string[] fields;
    private string[] nonKeyFields;

    public virtual string Table
      => EntityType.Name;

    public virtual object Key =>
      "Id";

    public virtual string[] Fields =>
      fields ?? (fields = Members.Select(p => p.Name).ToArray());

    public virtual string[] NonKeyFields =>
      nonKeyFields ?? (nonKeyFields = Fields.Where(f => !string.Equals(f, $"{Key}", StringComparison.OrdinalIgnoreCase)).ToArray());

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
        .Select(Fields)
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