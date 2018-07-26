using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sequel.Tests
{
  public class SqlMapperTest
  {
    public SqlMapper<MockEntity> sqlMapper;

    public SqlMapperTest()
    {
      sqlMapper = new SqlMapper<MockEntity>();
    }

    public class Table : SqlMapperTest
    {
      [Fact]
      public void Table_should_equal_MockEntity()
      {
        //Assert
        Assert.Equal("MockEntity", sqlMapper.Table);
      }
    }

    public class Key : SqlMapperTest
    {
      [Fact]
      public void Key_should_equal_Id()
      {
        //Arrange
        var expected = "Id";

        //Act
        var key = sqlMapper.Key;

        //Assert
        Assert.Equal(expected, key);
      }

      [Fact]
      public void Key_qualified_should_equal_Id_MockEntity_prefix()
      {
        //Arrange
        var expected = "MockEntity.Id";

        //Act
        var key = sqlMapper.KeyQualified;

        //Assert
        Assert.Equal(expected, key);
      }
    }

    public class Fields : SqlMapperTest
    {
      [Fact]
      public void Fields_should_equal_Id_Name_Cents()
      {
        //Arrange
        var expected = new string[] { "Id", "Name", "Cents" };

        //Act
        var fields = sqlMapper.Fields;

        //Assert
        Assert.Equal(expected.OrderBy(e => e), fields.OrderBy(f => f));
      }

      [Fact]
      public void Fields_qualified_should_equal_Id_Name_Cents_MockEntity_prefix()
      {
        //Arrange
        var expected = new string[] { "MockEntity.Id", "MockEntity.Name", "MockEntity.Cents" };

        //Act
        var fields = sqlMapper.FieldsQualified;

        //Assert
        Assert.Equal(expected.OrderBy(e => e), fields.OrderBy(f => f));
      }
    }

    public class NonKeyFields : SqlMapperTest
    {
      [Fact]
      public void Nonkeyfields_should_equal_Name_Cents()
      {
        //Arrange
        var expected = new string[] { "Name", "Cents" };

        //Act
        var nonKeyFields = sqlMapper.NonKeyFields;

        //Assert
        Assert.Equal(expected.OrderBy(e => e), nonKeyFields.OrderBy(n => n));
      }

      [Fact]
      public void Nonkeyfields_qualified_should_equal_Name_Cents_MockEntity_prefix()
      {
        //Arrange
        var expected = new string[] { "MockEntity.Name", "MockEntity.Cents" };

        //Act
        var nonKeyFields = sqlMapper.NonKeyFieldsQualified;

        //Assert
        Assert.Equal(expected.OrderBy(e => e), nonKeyFields.OrderBy(n => n));
      }
    }

    public class CreateSql : SqlMapperTest
    {
      [Fact]
      public void Should_be_valid_sql_insert_statement()
      {
        //Arrange
        var expected = "INSERT INTO MockEntity (Cents, Name) VALUES (@Cents, @Name)";

        //Act
        var createSql = sqlMapper.CreateSql.ToSql();

        //Assert
        Assert.Equal(expected, createSql);
      }
    }

    public class ReadSql : SqlMapperTest
    {
      [Fact]
      public void Should_be_valid_sql_select_statement()
      {
        //Arrange
        var expected = "SELECT MockEntity.Cents, MockEntity.Id, MockEntity.Name FROM MockEntity";

        //Act
        var readSql = sqlMapper.ReadSql.ToSql();

        //Assert
        Assert.Equal(expected, readSql);
      }
    }

    public class UpdateSql : SqlMapperTest
    {
      [Fact]
      public void Should_be_valid_sql_update_statement()
      {
        //Arrange
        var expected = "UPDATE MockEntity SET Cents = @Cents, Name = @Name WHERE Id = @Id";

        //Act
        var updateSql = sqlMapper.UpdateSql.ToSql();

        //Assert
        Assert.Equal(expected, updateSql);
      }
    }

    public class DeleteSql : SqlMapperTest
    {
      [Fact]
      public void Should_be_valid_sql_delete_statement()
      {
        //Arrange
        var expected = "DELETE FROM MockEntity WHERE Id = @Id";

        //Act
        var deleteSql = sqlMapper.DeleteSql.ToSql();

        //Assert
        Assert.Equal(expected, deleteSql);
      }
    }

    public class MockEntity
    {
      public int Id { get; set; }
      public string Name { get; set; }
      public int Cents { get; set; }

      public double Dollars => Cents * 100;

      public IEnumerable<MockChildEntity> Children { get; set; }
    }

    public class MockChildEntity
    {
      public int Id { get; set; }
      public int ParentId { get; set; }
      public string Name { get; set; }
    }
  }
}