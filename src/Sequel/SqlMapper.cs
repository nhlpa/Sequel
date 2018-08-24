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

      if (fields != null && fields.Length > 0)
      {
        this.fields = fields;
      }

      EntityType = typeof(TEntity);
      EntityTypeAccessor = TypeAccessor.Create(EntityType);
      ResolveMembers();
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
          fieldsQualified = new string[Fields.Length];

          for (int i = 0; i < Fields.Length; i++)
          {
            fieldsQualified[i] = Table + "." + Fields[i];
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
          nonKeyFields = new string[Fields.Length - 1];
          var nonKeyFieldIndex = 0;

          for (var i = 0; i < Fields.Length; i++)
          {
            if (!string.Equals(Key, Fields[i]))
            {
              nonKeyFields[nonKeyFieldIndex] = Fields[i];

              nonKeyFieldIndex++;
            }
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
          nonKeyFieldsQualified = new string[NonKeyFields.Length];

          for (var i = 0; i < NonKeyFields.Length; i++)
          {
            nonKeyFieldsQualified[i] = Table + "." + NonKeyFields[i];
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

    private string KeyPredicate =>
      keyPredicate ?? (keyPredicate = Key + " = @" + Key);

    private string[] CreateValues
    {
      get
      {
        if (createValues == null)
        {
          createValues = new string[NonKeyFields.Length];

          for (var i = 0; i < NonKeyFields.Length; i++)
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

          for (var i = 0; i < NonKeyFields.Length; i++)
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
      MemberSet memberSet = EntityTypeAccessor.GetMembers();
      var members = new Member[memberSet.Count]; //initialize to full member count, but track valid count
      var memberIndex = 1; //start at 1 because we reserve the fist slot for the Key field

      //determine valid members and nonkey members
      for (var i = 0; i < memberSet.Count; i++)
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
            memberIndex++;
          }
        }
      }

      Members = new Member[memberIndex];

      //set valid members
      for (var i = 0; i < memberIndex; i++)
      {
        Members[i] = members[i];
      }
    }
  }
}