using Xunit;
using System.Collections.Generic;
using System;
using LibraryCatalog.Objects;
using System.Data;
using System.Data.SqlClient;

namespace LibraryCatalog.Tests
{
  public class LibraryCatalogTests : IDisposable
  {
    DateTime? testDate = new DateTime(1990, 09, 05);
    DateTime? testDate2 = new DateTime(2012, 12, 21);
    public LibraryCatalogTests()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_catalog_test;Integrated Security=SSPI;";
    }

    public void Dispose()
    {
      Book.DeleteAll();
      Author.DeleteAll();
    }
    [Fact]
    public void Book_DatabaseEmpty()
    {
      //Arrange, Act
      int result = Book.GetAll().Count;
      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Book_Save_SavesBookToDatabase()
    {
      Book newBook = new Book("Cats", testDate, 2);

      newBook.Save();
      List<Book> expectedResult = new List<Book>{newBook};
      List<Book> actualResult = Book.GetAll();

      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Book_FindsBookInDatabase()
    {
      //Arrange
      Book expectedResult = new Book("Crime & Punishment", testDate, 3);
      expectedResult.Save();
      //Act
      Book result = Book.Find(expectedResult.GetId());
      //Assert
      Assert.Equal(expectedResult, result);
    }
    [Fact]
    public void Book_Delete_DeletesBookById()
    {
      Book firstBook = new Book("Cats", testDate, 2);
      firstBook.Save();
      Book secondBook = new Book("Crime & Punishment", testDate, 3);
      secondBook.Save();

      firstBook.Delete();

      List<Book> expectedResult = new List<Book>{secondBook};
      List<Book> actualResult = Book.GetAll();
      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Book_AddAuthors()
    {
      //Arrange
      Book newBook = new Book("Cats", testDate, 2);
      newBook.Save();
      Author newAuthor = new Author("Chad");
      newAuthor.Save();
      //Act
      newBook.AddAuthor(newAuthor.GetId());
      List<Author> result = newBook.GetAuthors();
      List<Author> expectedResult = new List<Author>{newAuthor};
      //Assert
      Assert.Equal(expectedResult, result);
    }
    [Fact]
    public void Book_DeleteAuthor()
    {
      //Arrange
      Book newBook = new Book("Cats", testDate, 2);
      newBook.Save();
      Author firstAuthor = new Author("Chad");
      firstAuthor.Save();
      Author secondAuthor = new Author("Todd");
      secondAuthor.Save();
      newBook.AddAuthor(firstAuthor.GetId());
      newBook.AddAuthor(secondAuthor.GetId());
      //Act
      newBook.DeleteAuthor(firstAuthor.GetId());
      List<Author> result = newBook.GetAuthors();
      List<Author> expectedResult = new List<Author>{secondAuthor};
      //Assert
      Assert.Equal(expectedResult, result);
    }
    [Fact]
    public void Book_Update_UpdatesBook()
    {
      Book firstBook = new Book("Cats", testDate, 2);
      firstBook.Save();

      firstBook.Update("Crime & Punishment", testDate2, 3);

      Book resultBook = Book.Find(firstBook.GetId());

      Assert.Equal("Crime & Punishment", resultBook.GetTitle());
      Assert.Equal(testDate2, resultBook.GetDatePublished());
      Assert.Equal(3, resultBook.GetGenreId());
    }
  }
}
