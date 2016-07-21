using Xunit;
using System.Collections.Generic;
using System;
using LibraryCatalog.Objects;
using System.Data;
using System.Data.SqlClient;

namespace LibraryCatalog.Tests
{
  public class PatronTests : IDisposable
  {
    DateTime? testDate = new DateTime(1990, 09, 05);
    DateTime? testDate2 = new DateTime(2012, 12, 21);
    public PatronTests()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_catalog_test;Integrated Security=SSPI;";
    }

    public void Dispose()
    {
      Patron.DeleteAll();
      Book.DeleteAll();
    }
    [Fact]
    public void Patron_DatabaseEmpty()
    {
      //Arrange, Act
      int result = Patron.GetAll().Count;
      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Patron_Save_SavesPatronToDatabase()
    {
      Patron newPatron = new Patron("Mayor McCheese");

      newPatron.Save();
      List<Patron> expectedResult = new List<Patron>{newPatron};
      List<Patron> actualResult = Patron.GetAll();

      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Patron_FindsPatronInDatabase()
    {
      //Arrange
      Patron expectedResult = new Patron("Mayor McCheese");
      expectedResult.Save();
      //Act
      Patron result = Patron.Find(expectedResult.GetId());
      //Assert
      Assert.Equal(expectedResult, result);
    }
    [Fact]
    public void Patron_Delete_DeletesPatronById()
    {
      Patron firstPatron = new Patron("Mayor McCheese");
      firstPatron.Save();
      Patron secondPatron = new Patron("Franklin Pierce");
      secondPatron.Save();

      firstPatron.Delete();

      List<Patron> expectedResult = new List<Patron>{secondPatron};
      List<Patron> actualResult = Patron.GetAll();
      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Patron_Checkout_ChecksoutABook()
    {
      Patron firstPatron = new Patron("Mayor McCheese");
      firstPatron.Save();
      Book newBook = new Book("Cats", testDate, 2);
      newBook.Save();
      newBook.StockBook();

      firstPatron.CheckoutBook(newBook.GetCopies()[0].GetId(), testDate2);

      List<Copy> result = firstPatron.GetCheckOutRecord(false);
      List<Copy> expectedResult = newBook.GetCopies();

      Assert.Equal(expectedResult, result);
    }
    [Fact]
    public void Patron_ReturnBook_ReturnsCheckedoutBook()
    {
      Patron firstPatron = new Patron("Mayor McCheese");
      firstPatron.Save();
      Book newBook = new Book("Cats", testDate, 2);
      newBook.Save();
      newBook.StockBook();

      firstPatron.CheckoutBook(newBook.GetCopies()[0].GetId(), testDate2);
      firstPatron.ReturnBook(newBook.GetCopies()[0].GetId());

      int result = firstPatron.GetCheckOutRecord(false).Count;

      Assert.Equal(0, result);
    }
    [Fact]
    public void Patron_GetCheckoutHistory_ReturnsCheckOutHistory()
    {
      Patron firstPatron = new Patron("Mayor McCheese");
      firstPatron.Save();
      Book firstBook = new Book("Cats", testDate, 2);
      firstBook.Save();
      firstBook.StockBook();
      Book secondBook = new Book("Dogs", testDate, 2);
      secondBook.Save();
      secondBook.StockBook();

      firstPatron.CheckoutBook(firstBook.GetCopies()[0].GetId(), testDate2);
      firstPatron.CheckoutBook(secondBook.GetCopies()[0].GetId(), testDate2);
      firstPatron.ReturnBook(firstBook.GetCopies()[0].GetId());

      Copy result = firstPatron.GetCheckOutRecord(true)[0];
      Copy expectedResult = firstBook.GetCopies()[0];
      Assert.Equal(expectedResult, result);
    }
  }
}
