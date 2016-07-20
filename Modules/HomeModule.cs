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
      Get["/books/{id}"] = parameters =>
      {
        Book selectedBook = Book.Find(parameters.id);
        return View["book.cshtml", selectedBook];
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

    }
  }

}
