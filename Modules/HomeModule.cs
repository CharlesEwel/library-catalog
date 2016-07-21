using Nancy;
using System.Collections.Generic;
using System.Data.SqlClient;
using LibraryCatalog.Objects;

namespace LibraryCatalog
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => View["index.cshtml"];

      Get["/books"] = _ =>
      {
        List<Book> allBooks = Book.GetAll();
        return View["books.cshtml", allBooks];
      };
      Post["/results"] = _ =>
      {
        string searchTerm = Request.Form["search-term"];
        bool searchType = Request.Form["search-type"];
        bool searchByTitle = Request.Form["title-or-author"];
        List<Book> searchResult = new List<Book>{};
        if(searchByTitle)
        {
          searchResult = Book.SearchForBookByTitle(searchTerm, searchType);
        }
        else
        {
          searchResult = Book.SearchForBookByAuthor(searchTerm, searchType);
        }
        Dictionary<string, object> model = new Dictionary<string, object>{};
        model.Add("results", searchResult);
        model.Add("priorSearchTerm", searchTerm);
        model.Add("priorSearchType", searchType);
        model.Add("priorSearchBy", searchByTitle);
        return View["results.cshtml", model];
      };
      Get["/books/{id}"] = parameters =>
      {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Author> allAuthors = Author.GetAll();
        Book selectedBook = Book.Find(parameters.id);
        List<Genre> allGenres = Genre.GetAll();
        List<Patron> allPatrons = Patron.GetAll();
        model.Add("patrons", allPatrons);
        model.Add("genres", allGenres);
        model.Add("book", selectedBook);
        model.Add("authors", allAuthors);
        return View["book.cshtml", model];
      };
      Get["/books/{id}/stock"]= parameters =>
      {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Author> allAuthors = Author.GetAll();
        Book selectedBook = Book.Find(parameters.id);
        selectedBook.StockBook();
        List<Genre> allGenres = Genre.GetAll();
        List<Patron> allPatrons = Patron.GetAll();
        model.Add("patrons", allPatrons);
        model.Add("genres", allGenres);
        model.Add("book", selectedBook);
        model.Add("authors", allAuthors);
        return View["book.cshtml", model];
      };
      Post["/books/{id}"] = parameters =>
      {
        Book selectedBook = Book.Find(parameters.id);
        int selectedAuthor = Request.Form["author-name"];
        Dictionary<string, object> model = new Dictionary<string, object>{};
        selectedBook.AddAuthor(selectedAuthor);
        List<Author> allAuthors = Author.GetAll();
        List<Genre> allGenres = Genre.GetAll();
        List<Patron> allPatrons = Patron.GetAll();
        model.Add("patrons", allPatrons);
        model.Add("genres", allGenres);
        model.Add("book", selectedBook);
        model.Add("authors", allAuthors);
        return View["book.cshtml", model];
      };
      Delete["/books/{id}"] = parameters =>
      {
        Book selectedBook = Book.Find(parameters.id);
        int authorToDelete = Request.Form["author-name"];
        selectedBook.DeleteAuthor(authorToDelete);
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Author> allAuthors = Author.GetAll();
        List<Genre> allGenres = Genre.GetAll();
        List<Patron> allPatrons = Patron.GetAll();
        model.Add("patrons", allPatrons);
        model.Add("genres", allGenres);
        model.Add("book", selectedBook);
        model.Add("authors", allAuthors);
        return View["book.cshtml", model];
      };
      Patch["/books/{id}"] = parameters =>
      {
        Book selectedBook = Book.Find(parameters.id);
        selectedBook.Update(Request.Form["book-title"], Request.Form["publication-date"], Request.Form["new-genre"]);
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Author> allAuthors = Author.GetAll();
        List<Genre> allGenres = Genre.GetAll();
        List<Patron> allPatrons = Patron.GetAll();
        model.Add("patrons", allPatrons);
        model.Add("genres", allGenres);
        model.Add("book", selectedBook);
        model.Add("authors", allAuthors);
        return View["book.cshtml", model];
      };
      Get["/books/add"] = _ =>
      {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Genre> allGenres = Genre.GetAll();
        List<Author> allAuthors = Author.GetAll();
        model.Add("genres", allGenres);
        model.Add("authors", allAuthors);
        return View["book_new.cshtml", model];
      };
      Post["/books/add"] = _ =>
      {
        Book newBook = new Book(Request.Form["book-title"], Request.Form["publication-date"], Request.Form["genre"]);
        newBook.Save();
        int authorId = Request.Form["author"];
        newBook.AddAuthor(authorId);
        List<Book> allBooks = Book.GetAll();
        return View["books.cshtml", allBooks];
      };

      Get["/authors"] = _ =>
      {
        List<Author> allAuthors = Author.GetAll();
        return View["authors.cshtml", allAuthors];
      };
      Get["/authors/{id}"] = parameters =>
      {
        Author selectedAuthor = Author.Find(parameters.id);
        return View["author.cshtml", selectedAuthor];
      };
      Get["/authors/add"] = _ =>View["author_new.cshtml"];
      Post["/authors/add"] = _ =>
      {
        Author newAuthor = new Author(Request.Form["author-name"]);
        newAuthor.Save();
        List<Author> allAuthors = Author.GetAll();
        return View["authors.cshtml", allAuthors];
      };

      Get["/genres"] = _ =>
      {
        List<Genre> allGenres = Genre.GetAll();
        return View["genres.cshtml", allGenres];
      };
      Get["/genres/{id}"] = parameters =>
      {
        Genre selectedGenre = Genre.Find(parameters.id);
        return View["genre.cshtml", selectedGenre];
      };
      Get["/genres/add"] = _ =>View["genre_new.cshtml"];
      Post["/genres/add"] = _ =>
      {
        Genre newGenre = new Genre(Request.Form["genre-name"]);
        newGenre.Save();
        List<Genre> allGenres = Genre.GetAll();
        return View["genres.cshtml", allGenres];
      };
      Get["/patrons"] = _ =>
      {
        List<Patron> allPatrons = Patron.GetAll();
        return View["patrons.cshtml", allPatrons];
      };
      Get["/patrons/{id}"] = parameters =>
      {
        Patron selectedPatron = Patron.Find(parameters.id);
        return View["patron.cshtml", selectedPatron];
      };
      Get["/patrons/new"] = _ =>  View["patron_new.cshtml"];

      Post["/patrons"] = _ =>
      {
        Patron newPatron = new Patron(Request.Form["name"]);
        newPatron.Save();
        List<Patron> allPatrons = Patron.GetAll();
        return View["patrons.cshtml", allPatrons];
      };
    }
  }

}
