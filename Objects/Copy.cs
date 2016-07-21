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

    public static Copy Find(int copyId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand("SELECT * FROM copies WHERE id = @CopyId;", conn);

      SqlParameter copyIdParameter = new SqlParameter();
      copyIdParameter.ParameterName = "@CopyId";
      copyIdParameter.Value = copyId;
      cmd.Parameters.Add(copyIdParameter);

      int foundCopyId = 0;
      int foundCopyBookId = 0;

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        foundCopyId = rdr.GetInt32(0);
        foundCopyBookId = rdr.GetInt32(1);
      }
      Copy foundCopy = new Copy(foundCopyBookId, foundCopyId);

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundCopy;
    }

    public static Dictionary<string,object> GetAllCheckouts()
    {
      Dictionary<string,object> checkouts = new Dictionary<string,object>{};

      List<Patron> patrons = new List<Patron>{};
      List<Copy> copies = new List<Copy>{};
      List<DateTime?> dueDates = new List<DateTime?>{};
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM checkouts WHERE returned = 0;", conn);

      rdr = cmd.ExecuteReader();
      int foundPatronId=0;
      int foundCopyId=0;
      DateTime? foundDueDate = null;
      while(rdr.Read())
      {
        foundPatronId = rdr.GetInt32(1);
        foundCopyId = rdr.GetInt32(2);
        foundDueDate = rdr.GetDateTime(3);
        Patron foundPatron = Patron.Find(foundPatronId);
        Copy foundCopy = Copy.Find(foundCopyId);
        patrons.Add(foundPatron);
        copies.Add(foundCopy);
        dueDates.Add(foundDueDate);
      }

      checkouts.Add("patrons", patrons);
      checkouts.Add("copies", copies);
      checkouts.Add("dueDates", dueDates);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return checkouts;
    }
  }
}
