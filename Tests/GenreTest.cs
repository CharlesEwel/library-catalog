using Xunit;
using System.Collections.Generic;
using System;
using LibraryCatalog.Objects;
using System.Data;
using System.Data.SqlClient;

namespace LibraryCatalog.Tests
{
  public class GenreTests : IDisposable
  {
    DateTime? testDate = new DateTime(1990, 09, 05);
    public GenreTests()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_catalog_test;Integrated Security=SSPI;";
    }

    public void Dispose()
    {
      Genre.DeleteAll();
      Book.DeleteAll();
    }
    [Fact]
    public void Genre_DatabaseEmpty()
    {
      //Arrange, Act
      int result = Genre.GetAll().Count;
      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Genre_Save_SavesGenreToDatabase()
    {
      Genre newGenre = new Genre("Sci-Fi");

      newGenre.Save();
      List<Genre> expectedResult = new List<Genre>{newGenre};
      List<Genre> actualResult = Genre.GetAll();

      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Genre_FindsGenreInDatabase()
    {
      //Arrange
      Genre expectedResult = new Genre("Literature");
      expectedResult.Save();
      //Act
      Genre result = Genre.Find(expectedResult.GetId());
      //Assert
      Assert.Equal(expectedResult, result);
    }
    [Fact]
    public void Genre_Delete_DeletesGenreById()
    {
      Genre firstGenre = new Genre("Sci-Fi");
      firstGenre.Save();
      Genre secondGenre = new Genre("Literature");
      secondGenre.Save();

      firstGenre.Delete();

      List<Genre> expectedResult = new List<Genre>{secondGenre};
      List<Genre> actualResult = Genre.GetAll();
      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Genre_GetBooks_GetsAllBookInGenre()
    {
      Genre firstGenre = new Genre("Sci-Fi");
      firstGenre.Save();
      Book firstBook = new Book("Cats", testDate, 2);
      firstBook.Save();
      Book secondBook = new Book("Crime & Punishment", testDate, firstGenre.GetId());
      secondBook.Save();

      List<Book> expectedResult = new List<Book> {secondBook};
      List<Book> actualResult = firstGenre.GetBooks();

      Assert.Equal(expectedResult, actualResult);
    }
  }
}
