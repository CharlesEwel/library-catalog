using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryCatalog.Objects
{
  public class Book
  {
    private int _id;
    private string _title;
    private DateTime? _datePublished;
    private int _genreId;

    public Book(string title, DateTime? datePublished, int genreId, int Id=0)
    {
      _title = title;
      _datePublished = datePublished;
      _genreId = genreId;
      _id = Id;
    }

    public int GetId()
    {
      return _id;
    }
    public string GetTitle()
    {
      return _title;
    }
    public DateTime? GetDatePublished()
    {
      return _datePublished;
    }
    public int GetGenreId()
    {
      return _genreId;
    }

    public static List<Book> GetAll()
    {
      List<Book> allBooks = new List<Book>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        DateTime? bookPublishDate = rdr.GetDateTime(2);
        int bookGenre = rdr.GetInt32(3);
        Book newBook = new Book(bookTitle, bookPublishDate, bookGenre, bookId);
        allBooks.Add(newBook);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allBooks;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books (title, year_published, genre_id) OUTPUT INSERTED.id VALUES (@BookTitle, @YearPublished, @GenreId);", conn);

      SqlParameter titleParameter = new SqlParameter();
      titleParameter.ParameterName = "@BookTitle";
      titleParameter.Value = this.GetTitle();
      cmd.Parameters.Add(titleParameter);

      SqlParameter yearParameter = new SqlParameter();
      yearParameter.ParameterName = "@YearPublished";
      yearParameter.Value = this.GetDatePublished();
      cmd.Parameters.Add(yearParameter);

      SqlParameter genreParameter = new SqlParameter();
      genreParameter.ParameterName = "@GenreId";
      genreParameter.Value = this.GetGenreId();
      cmd.Parameters.Add(genreParameter);

      rdr=cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM books; DELETE FROM books_authors; DELETE FROM copies", conn);
      cmd.ExecuteNonQuery();
    }

    public override bool Equals(System.Object otherBook)
    {
      if(!(otherBook is Book))
      {
        return false;
      }
      else
      {
        Book newBook = (Book) otherBook;
        bool bookIdEquality = _id == newBook.GetId();
        bool bookTitleEquality = _title == newBook.GetTitle();
        bool bookPublishDateEquality = _datePublished == newBook.GetDatePublished();
        bool bookGenreIdEquality = _genreId == newBook.GetGenreId();
        return (bookIdEquality && bookTitleEquality && bookPublishDateEquality && bookGenreIdEquality);
      }
    }

    public static Book Find(int bookId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = bookId;
      cmd.Parameters.Add(bookIdParameter);

      int foundBookId = 0;
      string foundBookTitle = null;
      DateTime? foundBookPublishedDate = null;
      int foundBookGenreId = 0;

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        foundBookId = rdr.GetInt32(0);
        foundBookTitle = rdr.GetString(1);
        foundBookPublishedDate = rdr.GetDateTime(2);
        foundBookGenreId = rdr.GetInt32(3);
      }
      Book foundBook = new Book(foundBookTitle, foundBookPublishedDate, foundBookGenreId, foundBookId);

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundBook;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public void AddAuthor(int authorId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (book_id, author_id) VALUES (@BookId, @AuthorId);", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = authorId;
      cmd.Parameters.Add(authorIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public void DeleteAuthor(int authorId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM books_authors WHERE book_id = @BookId AND author_id = @AuthorId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = authorId;
      cmd.Parameters.Add(authorIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public List<Author> GetAuthors()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT authors.* FROM books JOIN books_authors ON (books_authors.book_id = books.id) JOIN authors ON (books_authors.author_id = authors.id) WHERE book_id = @BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      rdr=cmd.ExecuteReader();

      List<Author> foundAuthors = new List<Author>{};

      while(rdr.Read())
      {
        int foundId = rdr.GetInt32(0);
        string foundName = rdr.GetString(1);
        Author foundAuthor = new Author(foundName, foundId);
        foundAuthors.Add(foundAuthor);
      }

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundAuthors;
    }

    public string GetGenre()
    {
      int genreId = this.GetGenreId();
      Genre genre = Genre.Find(genreId);
      string currentGenre = genre.GetName();
      return currentGenre;
    }

    public void Update(string newTitle, DateTime? newPublicationDate, int newGenreId)
    {
      _title = newTitle;
      _datePublished = newPublicationDate;
      _genreId = newGenreId;

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE books SET title=@NewTitle WHERE id=@BookId; UPDATE books SET year_published=@NewPublicationDate WHERE id=@BookId; UPDATE books SET genre_id=@NewGenreId WHERE id=@BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      SqlParameter titleParameter = new SqlParameter();
      titleParameter.ParameterName = "@NewTitle";
      titleParameter.Value = newTitle;
      cmd.Parameters.Add(titleParameter);

      SqlParameter yearParameter = new SqlParameter();
      yearParameter.ParameterName = "@NewPublicationDate";
      yearParameter.Value = newPublicationDate;
      cmd.Parameters.Add(yearParameter);

      SqlParameter genreParameter = new SqlParameter();
      genreParameter.ParameterName = "@NewGenreId";
      genreParameter.Value = newGenreId;
      cmd.Parameters.Add(genreParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public static List<Book> SearchForBookByTitle(string searchTerm, bool partialMatches)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE title LIKE @SearchTerm;", conn);

      SqlParameter searchTermParameter = new SqlParameter();
      searchTermParameter.ParameterName = "@SearchTerm";
      searchTermParameter.Value = searchTerm;
      if(partialMatches)
      {
        searchTermParameter.Value = "%"+searchTerm+"%";
      }
      cmd.Parameters.Add(searchTermParameter);

      int foundBookId = 0;
      string foundBookTitle = null;
      DateTime? foundBookPublishedDate = null;
      int foundBookGenreId = 0;
      List<Book> foundBooks = new List<Book>{};

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        foundBookId = rdr.GetInt32(0);
        foundBookTitle = rdr.GetString(1);
        foundBookPublishedDate = rdr.GetDateTime(2);
        foundBookGenreId = rdr.GetInt32(3);
        Book foundBook = new Book(foundBookTitle, foundBookPublishedDate, foundBookGenreId, foundBookId);
        foundBooks.Add(foundBook);
      }

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundBooks;
    }

    public static List<Book> SearchForBookByAuthor(string searchTerm, bool partialMatches)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand("SELECT books.* FROM authors JOIN books_authors ON (books_authors.author_id = authors.id) JOIN books ON (books_authors.book_id = books.id) WHERE authors.name LIKE @SearchTerm;", conn);

      SqlParameter searchTermParameter = new SqlParameter();
      searchTermParameter.ParameterName = "@SearchTerm";
      searchTermParameter.Value = searchTerm;
      if(partialMatches)
      {
        searchTermParameter.Value = "%"+searchTerm+"%";
      }
      cmd.Parameters.Add(searchTermParameter);

      int foundBookId = 0;
      string foundBookTitle = null;
      DateTime? foundBookPublishedDate = null;
      int foundBookGenreId = 0;
      List<Book> foundBooks = new List<Book>{};

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        foundBookId = rdr.GetInt32(0);
        foundBookTitle = rdr.GetString(1);
        foundBookPublishedDate = rdr.GetDateTime(2);
        foundBookGenreId = rdr.GetInt32(3);
        Book foundBook = new Book(foundBookTitle, foundBookPublishedDate, foundBookGenreId, foundBookId);
        if(!foundBooks.Contains(foundBook))
        {
          foundBooks.Add(foundBook);
        }
      }

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundBooks;
    }
    public void StockBook()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO copies (book_id) VALUES (@BookId);", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public List<Copy> GetCopies()
    {
      List<Copy> allCopies = new List<Copy>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM copies WHERE book_id = @BookId;", conn);

      SqlParameter idParameter = new SqlParameter();
      idParameter.ParameterName = "@BookId";
      idParameter.Value = this.GetId();
      cmd.Parameters.Add(idParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int Id = rdr.GetInt32(0);
        int bookId = rdr.GetInt32(1);
        Copy newCopy = new Copy(bookId, Id);
        allCopies.Add(newCopy);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allCopies;
    }

    public List<Copy> GetCheckedOutCopies()
    {
      List<Copy> allCheckedOutCopies = new List<Copy>{};

      Dictionary<string, object> allCheckouts = Copy.GetAllCheckouts();
      List<Copy> allCopies = (List<Copy>) allCheckouts["copies"];

      foreach(Copy copy in allCopies)
      {
        if(copy.GetBookId()==this.GetId())
        {
          allCheckedOutCopies.Add(copy);
        }
      }
      return allCheckedOutCopies;
    }
    public List<Copy> GetInStockCopies()
    {
      List<Copy> inStockCopies = new List<Copy>{};
      foreach(var copy in this.GetCopies())
      {
        if(!(this.GetCheckedOutCopies().Contains(copy)))
        {
          inStockCopies.Add(copy);
        }
      }
      return inStockCopies;
    }
    public List<DateTime?> GetDueDates()
    {

      List<DateTime?> allDueDates= new List<DateTime?>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT checkouts.* FROM books JOIN copies ON (copies.book_id = books.id) JOIN checkouts ON (copies.id = checkouts.copy_id) WHERE books.id=@BookId AND checkouts.returned=0;", conn);

      SqlParameter bookParameter = new SqlParameter();
      bookParameter.ParameterName = "@BookId";
      bookParameter.Value = this.GetId();
      cmd.Parameters.Add(bookParameter);

      rdr = cmd.ExecuteReader();
      DateTime? foundDueDate = null;
      while(rdr.Read())
      {
        foundDueDate = rdr.GetDateTime(3);
        allDueDates.Add(foundDueDate);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allDueDates;
    }
  }
}
