using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryCatalog.Objects
{
  public class Author
  {
    private int _id;
    private string _name;

    public Author(string name, int Id=0)
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

    public static List<Author> GetAll()
    {
      List<Author> allAuthors = new List<Author>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM authors;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        string authorName = rdr.GetString(1);
        Author newAuthor = new Author(authorName, authorId);
        allAuthors.Add(newAuthor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allAuthors;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO authors (name) OUTPUT INSERTED.id VALUES (@AuthorName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@AuthorName";
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

      SqlCommand cmd = new SqlCommand("DELETE FROM authors; DELETE FROM books_authors;", conn);
      cmd.ExecuteNonQuery();
    }

    public override bool Equals(System.Object otherAuthor)
    {
      if(!(otherAuthor is Author))
      {
        return false;
      }
      else
      {
        Author newAuthor = (Author) otherAuthor;
        bool authorIdEquality = _id == newAuthor.GetId();
        bool authorNameEquality = _name == newAuthor.GetName();
        return (authorIdEquality && authorNameEquality);
      }
    }

    public static Author Find(int authorId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand("SELECT * FROM authors WHERE id = @AuthorId;", conn);

      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = authorId;
      cmd.Parameters.Add(authorIdParameter);

      int foundAuthorId = 0;
      string foundAuthorName = null;

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        foundAuthorId = rdr.GetInt32(0);
        foundAuthorName = rdr.GetString(1);
      }
      Author foundAuthor = new Author(foundAuthorName, foundAuthorId);

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundAuthor;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM authors WHERE id = @AuthorId;", conn);

      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = this.GetId();
      cmd.Parameters.Add(authorIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

  }
}
