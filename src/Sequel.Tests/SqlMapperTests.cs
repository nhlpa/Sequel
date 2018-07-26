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
        //Assert
        Assert.Equal("Id", sqlMapper.Key);
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

    public class PageSql : SqlMapperTest
    {
      [Fact]
      public void Should_be_valid_top5_sql_keyset_pagination_statement()
      {
        //Arrange
        var expected = "SELECT TOP 5 MockEntity.Cents, MockEntity.Id, MockEntity.Name FROM MockEntity ORDER BY MockEntity.Id";

        //Act
        var readSql = sqlMapper.PageSql(5).ToSql();

        //Assert
        Assert.Equal(expected, readSql);
      }

      [Fact]
      public void Should_be_valid_top5_desc_sql_keyset_pagination_statement()
      {
        //Arrange
        var expected = "SELECT TOP 5 MockEntity.Cents, MockEntity.Id, MockEntity.Name FROM MockEntity ORDER BY MockEntity.Id DESC";

        //Act
        var readSql = sqlMapper.PageSql(5, order: "desc").ToSql();

        //Assert
        Assert.Equal(expected, readSql);
      }

      [Fact]
      public void Should_be_valid_top5_gt1_sql_keyset_pagination_statement()
      {
        //Arrange
        var expected = "SELECT TOP 5 MockEntity.Cents, MockEntity.Id, MockEntity.Name FROM MockEntity WHERE MockEntity.Id > @Id ORDER BY MockEntity.Id";

        //Act
        var readSql = sqlMapper.PageSql(5, 1).ToSql();

        //Assert
        Assert.Equal(expected, readSql);
      }

      [Fact]
      public void Should_be_valid_top5_gt1_desc_sql_keyset_pagination_statement()
      {
        //Arrange
        var expected = "SELECT TOP 5 MockEntity.Cents, MockEntity.Id, MockEntity.Name FROM MockEntity WHERE MockEntity.Id < @Id ORDER BY MockEntity.Id DESC";

        //Act
        var readSql = sqlMapper.PageSql(5, 1, "desc").ToSql();

        //Assert
        Assert.Equal(expected, readSql);
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