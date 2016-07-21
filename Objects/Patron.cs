using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryCatalog.Objects
{
  public class Patron
  {
    private int _id;
    private string _name;

    public Patron(string name, int Id=0)
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

    public static List<Patron> GetAll()
    {
      List<Patron> allPatrons = new List<Patron>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patrons;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int patronId = rdr.GetInt32(0);
        string patronName = rdr.GetString(1);
        Patron newPatron = new Patron(patronName, patronId);
        allPatrons.Add(newPatron);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allPatrons;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO patrons (name) OUTPUT INSERTED.id VALUES (@PatronName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@PatronName";
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

      SqlCommand cmd = new SqlCommand("DELETE FROM patrons; DELETE FROM checkouts;", conn);
      cmd.ExecuteNonQuery();
    }

    public override bool Equals(System.Object otherPatron)
    {
      if(!(otherPatron is Patron))
      {
        return false;
      }
      else
      {
        Patron newPatron = (Patron) otherPatron;
        bool patronIdEquality = _id == newPatron.GetId();
        bool patronNameEquality = _name == newPatron.GetName();
        return (patronIdEquality && patronNameEquality);
      }
    }

    public static Patron Find(int patronId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand("SELECT * FROM patrons WHERE id = @PatronId;", conn);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = patronId;
      cmd.Parameters.Add(patronIdParameter);

      int foundPatronId = 0;
      string foundPatronName = null;

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        foundPatronId = rdr.GetInt32(0);
        foundPatronName = rdr.GetString(1);
      }
      Patron foundPatron = new Patron(foundPatronName, foundPatronId);

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundPatron;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM patrons WHERE id = @PatronId;", conn);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public void CheckoutBook(int copyId, DateTime? dueDate)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO checkouts (patron_id, copy_id, due_date, returned) VALUES (@PatronId, @CopyId, @DueDate, @Returned);", conn);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);

      SqlParameter copyIdParameter = new SqlParameter();
      copyIdParameter.ParameterName = "@CopyId";
      copyIdParameter.Value = copyId;
      cmd.Parameters.Add(copyIdParameter);

      SqlParameter dueDateParameter = new SqlParameter();
      dueDateParameter.ParameterName = "@DueDate";
      dueDateParameter.Value = dueDate;
      cmd.Parameters.Add(dueDateParameter);

      SqlParameter returnedParameter = new SqlParameter();
      returnedParameter.ParameterName = "@Returned";
      returnedParameter.Value = false;
      cmd.Parameters.Add(returnedParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public List<Copy> GetCheckOutRecord(bool history)
    {
      List<Copy> checkedOutCopies = new List<Copy> {};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT copies.* FROM patrons JOIN checkouts ON (checkouts.patron_id = patrons.id) JOIN copies ON (checkouts.copy_id = copies.id) WHERE patrons.id=@PatronId AND checkouts.returned=@Returned;", conn);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);

      SqlParameter returnedParameter = new SqlParameter();
      returnedParameter.ParameterName = "@Returned";
      returnedParameter.Value = history;
      cmd.Parameters.Add(returnedParameter);


      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int Id = rdr.GetInt32(0);
        int bookId = rdr.GetInt32(1);
        Copy newCopy = new Copy(bookId, Id);
        checkedOutCopies.Add(newCopy);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return checkedOutCopies;
    }
    public void ReturnBook(int copyId)
    {

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE checkouts SET returned = @Returned WHERE patron_id=@PatronId AND copy_id = @CopyId;", conn);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);

      SqlParameter copyIdParameter = new SqlParameter();
      copyIdParameter.ParameterName = "@CopyId";
      copyIdParameter.Value = copyId;
      cmd.Parameters.Add(copyIdParameter);

      SqlParameter returnedParameter = new SqlParameter();
      returnedParameter.ParameterName = "@Returned";
      returnedParameter.Value = true;
      cmd.Parameters.Add(returnedParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
