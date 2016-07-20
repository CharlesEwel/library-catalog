using Xunit;
using System.Collections.Generic;
using System;
using LibraryCatalog.Objects;
using System.Data;
using System.Data.SqlClient;

namespace LibraryCatalog.Tests
{
  public class AuthorTests : IDisposable
  {
    DateTime? testDate = new DateTime(1990, 09, 05);
    public AuthorTests()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_catalog_test;Integrated Security=SSPI;";
    }

    public void Dispose()
    {
      Author.DeleteAll();
    }
    [Fact]
    public void Author_DatabaseEmpty()
    {
      //Arrange, Act
      int result = Author.GetAll().Count;
      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Author_Save_SavesAuthorToDatabase()
    {
      Author newAuthor = new Author("Chad");

      newAuthor.Save();
      List<Author> expectedResult = new List<Author>{newAuthor};
      List<Author> actualResult = Author.GetAll();

      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Author_FindsAuthorInDatabase()
    {
      //Arrange
      Author expectedResult = new Author("Todd");
      expectedResult.Save();
      //Act
      Author result = Author.Find(expectedResult.GetId());
      //Assert
      Assert.Equal(expectedResult, result);
    }
    [Fact]
    public void Author_Delete_DeletesAuthorById()
    {
      Author firstAuthor = new Author("Chad");
      firstAuthor.Save();
      Author secondAuthor = new Author("Todd");
      secondAuthor.Save();

      firstAuthor.Delete();

      List<Author> expectedResult = new List<Author>{secondAuthor};
      List<Author> actualResult = Author.GetAll();
      Assert.Equal(expectedResult, actualResult);
    }
  }
}
