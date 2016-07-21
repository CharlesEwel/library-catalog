using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryCatalog.Objects
{
  public class Copy
  {
    private int _bookId;

    public Copy(int bookId, int Id=0)
    {
      _bookId = bookId;
    }
  }
}
