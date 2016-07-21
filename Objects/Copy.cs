using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryCatalog.Objects
{
  public class Copy
  {
    private int _id;
    private int _bookId;

    public Copy(int bookId, int Id=0)
    {
      _bookId = bookId;
      _id = Id;
    }
    public int GetId()
    {
      return _id;
    }
    public int GetBookId()
    {
      return _bookId;
    }
    public string GetBook()
    {
      int bookId = this.GetBookId();
      Book book = Book.Find(bookId);
      string currentBook = book.GetTitle();
      return currentBook;
    }

    public override bool Equals(System.Object otherCopy)
    {
      if(!(otherCopy is Copy))
      {
        return false;
      }
      else
      {
        Copy newCopy = (Copy) otherCopy;
        bool copyIdEquality = _id == newCopy.GetId();
        bool copyBookIdEquality = _bookId == newCopy.GetBookId();
        return (copyIdEquality && copyBookIdEquality);
      }
    }
  }
}
