using Nancy;
using LibraryCatalog.Objects;

namespace LibraryCatalog
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => View["index.cshtml"];
    }
  }

}
