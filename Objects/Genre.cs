using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryCatalog.Objects
{
  public class Genre
  {
    private int _id;
    private string _name;

    public Genre(string name, int Id=0)
    {
      _name = name;
      _id = Id;
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }

    public static List<Genre> GetAll()
    {
      List<Genre> allGenres = new List<Genre>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM genres;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int genreId = rdr.GetInt32(0);
        string genreName = rdr.GetString(1);
        Genre newGenre = new Genre(genreName, genreId);
        allGenres.Add(newGenre);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allGenres;
    }

    public List<Book> GetBooks()
    {
      List<Book> allBooks = new List<Book>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE genre_id = @GenreId;", conn);

      SqlParameter idParameter = new SqlParameter();
      idParameter.ParameterName = "@GenreId";
      idParameter.Value = this.GetId();
      cmd.Parameters.Add(idParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        DateTime? bookPublicationDate = rdr.GetDateTime(2);
        int bookGenreId = rdr.GetInt32(3);
        Book newBook = new Book(bookTitle, bookPublicationDate, bookGenreId, bookId);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO genres (name) OUTPUT INSERTED.id VALUES (@GenreName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@GenreName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);

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

      SqlCommand cmd = new SqlCommand("DELETE FROM genres;", conn);
      cmd.ExecuteNonQuery();
    }

    public override bool Equals(System.Object otherGenre)
    {
      if(!(otherGenre is Genre))
      {
        return false;
      }
      else
      {
        Genre newGenre = (Genre) otherGenre;
        bool genreIdEquality = _id == newGenre.GetId();
        bool genreNameEquality = _name == newGenre.GetName();
        return (genreIdEquality && genreNameEquality);
      }
    }

    public static Genre Find(int genreId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand("SELECT * FROM genres WHERE id = @GenreId;", conn);

      SqlParameter genreIdParameter = new SqlParameter();
      genreIdParameter.ParameterName = "@GenreId";
      genreIdParameter.Value = genreId;
      cmd.Parameters.Add(genreIdParameter);

      int foundGenreId = 0;
      string foundGenreName = null;

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        foundGenreId = rdr.GetInt32(0);
        foundGenreName = rdr.GetString(1);
      }
      Genre foundGenre = new Genre(foundGenreName, foundGenreId);

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundGenre;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM genres WHERE id = @GenreId;", conn);

      SqlParameter genreIdParameter = new SqlParameter();
      genreIdParameter.ParameterName = "@GenreId";
      genreIdParameter.Value = this.GetId();
      cmd.Parameters.Add(genreIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

  }
}
