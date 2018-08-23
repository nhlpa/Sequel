using FastMember;
using System;
using System.Collections.Generic;

namespace Sequel
{
  public class SqlMapper<TEntity> : ISqlMapper<TEntity>
  {
    private const string DEFAULT_KEY = "Id";

    private string table;
    private string key;

    private string[] fields;
    private string[] fieldsQualified;
    private string[] nonKeyFields;
    private string[] nonKeyFieldsQualified;

    private string keyPredicate;
    private string[] createValues;
    private string[] updateSet;

    public SqlMapper()
    {
      EntityType = typeof(TEntity);
      EntityTypeAccessor = TypeAccessor.Create(EntityType);
      ResolveMembers();
    }

    public SqlMapper(string table = null, string key = null, string[] fields = null)
      : this()
    {
      if (!string.IsNullOrWhiteSpace(table))
      {
        this.table = table;
      }

      if (!string.IsNullOrWhiteSpace(key))
      {
        this.key = key;
      }

      if (fields != null && fields.Length > 0)
      {
        this.fields = fields;
      }
    }

    /// <summary>
    /// Table name, can include schema
    /// </summary>
    public virtual string Table =>
      table ?? (table = EntityType.Name);

    /// <summary>
    /// Primary key column name
    /// </summary>
    public virtual string Key =>
      key ?? (key = DEFAULT_KEY);

    /// <summary>
    /// Primary key column name prefixed with {Table}
    /// </summary>
    public virtual string KeyQualified =>
      Table + "." + Key;

    /// <summary>
    /// All column names
    /// </summary>
    public virtual string[] Fields
    {
      get
      {
        if (fields == null)
        {
          fields = new string[Members.Length];

          for (int i = 0; i < Members.Length; i++)
          {
            fields[i] = Members[i].Name;
          }
        }

        return fields;
      }
    }

    /// <summary>
    /// All column names prefixed with {Table}
    /// </summary>
    public virtual string[] FieldsQualified
    {
      get
      {
        if (fieldsQualified == null)
        {
          fieldsQualified = new string[Members.Length];

          for (int i = 0; i < Members.Length; i++)
          {
            fieldsQualified[i] = Table + "." + Members[i].Name;
          }
        }

        return fieldsQualified;
      }
    }

    /// <summary>
    /// Column names excluding Key
    /// </summary>
    public virtual string[] NonKeyFields
    {
      get
      {
        if (nonKeyFields == null)
        {
          nonKeyFields = new string[NonKeyMembers.Length];

          for (int i = 0; i < NonKeyMembers.Length; i++)
          {
            nonKeyFields[i] = NonKeyMembers[i].Name;
          }
        }

        return nonKeyFields;
      }
    }

    /// <summary>
    /// Column names excluding Key prefixed with {Table}
    /// </summary>
    public virtual string[] NonKeyFieldsQualified
    {
      get
      {
        if (nonKeyFieldsQualified == null)
        {
          nonKeyFieldsQualified = new string[NonKeyMembers.Length];

          for (int i = 0; i < NonKeyMembers.Length; i++)
          {
            nonKeyFieldsQualified[i] = Table + "." + NonKeyMembers[i].Name;
          }
        }

        return nonKeyFieldsQualified;
      }
    }

    /// <summary>
    /// INSERT statement
    /// </summary>
    public virtual SqlBuilder CreateSql =>
      new SqlBuilder()
        .Insert(Table)
        .Columns(NonKeyFields)
        //.Values(string.Join(", ", NonKeyFields.Select(f => $"@{f}")));
        .Values(CreateValues);

    /// <summary>
    /// SELECT statement
    /// Sets parameter @{Key}
    /// </summary>
    public virtual SqlBuilder ReadSql =>
      new SqlBuilder()
        .Select(FieldsQualified)
        .From(Table);

    /// <summary>
    /// UPDATE statement
    /// Sets parameter @{Key}
    /// </summary>
    public virtual SqlBuilder UpdateSql =>
      new SqlBuilder()
        .Update(Table)
        //.Set(NonKeyFields.Select(f => $"{f} = @{f}").ToArray())
        .Set(UpdateSet)
        .Where(KeyPredicate);

    /// <summary>
    /// DELETE statement
    /// Sets parameter @{Key}
    /// </summary>
    public virtual SqlBuilder DeleteSql =>
      new SqlBuilder()
        .Delete()
        .From(Table)
        .Where(KeyPredicate);

    private Type EntityType { get; }

    private TypeAccessor EntityTypeAccessor { get; }

    private Member[] Members { get; set; }

    private Member[] NonKeyMembers { get; set; }

    private string KeyPredicate =>
      keyPredicate ?? (keyPredicate = Key + " = @" + Key);

    private string[] CreateValues
    {
      get
      {
        if (createValues == null)
        {
          createValues = new string[NonKeyFields.Length];

          for (int i = 0; i < NonKeyFields.Length; i++)
          {
            createValues[i] += "@" + NonKeyFields[i];
          }
        }

        return createValues;
      }
    }

    private string[] UpdateSet
    {
      get
      {
        if (updateSet == null)
        {
          updateSet = new string[NonKeyFields.Length];

          for (int i = 0; i < NonKeyFields.Length; i++)
          {
            updateSet[i] = NonKeyFields[i] + " = @" + NonKeyFields[i];
          }
        }

        return updateSet;
      }
    }

    private bool IsValidMember(Member m)
    {
      return m.CanWrite
        && m.Type.IsPublic
        && string.Equals(m.Type.Namespace, "system", StringComparison.OrdinalIgnoreCase)
        && !typeof(ICollection<>).IsAssignableFrom(m.Type);
    }

    private void ResolveMembers()
    {
      var memberSet = EntityTypeAccessor.GetMembers();
      var members = new Member[memberSet.Count]; //initialize to full member count, but track valid count
      var validMembers = 0;

      var nonKeyMembers = new Member[memberSet.Count - 1]; //initialize to full member count -1 for key, but track valid count
      int validNonKeyMembers = 0;

      int memberIndex = 1; //start at 1 because we reserve the fist slot for the Key field
      int nonKeyMemberIndex = 0;

      //determine valid members and nonkey members
      for (int i = 0; i < memberSet.Count; i++)
      {
        var member = memberSet[i];

        if (IsValidMember(member))
        {
          if (string.Equals(Key, member.Name, StringComparison.OrdinalIgnoreCase))
          {
            members[0] = member;
          }
          else
          {
            members[memberIndex] = member;
            nonKeyMembers[nonKeyMemberIndex] = member;

            validNonKeyMembers++;
            memberIndex++;
            nonKeyMemberIndex++;
          }

          validMembers++;
        }
      }

      Members = new Member[validMembers];
      NonKeyMembers = new Member[validNonKeyMembers];

      //set valid members
      for (int i = 0; i < validMembers; i++)
      {
        Members[i] = members[i];
      }

      for (int i = 0; i < validNonKeyMembers; i++)
      {
        NonKeyMembers[i] = nonKeyMembers[i];
      }
    }
  }
}