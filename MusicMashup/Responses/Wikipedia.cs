using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace MusicMashup.Responses
{
    public class Wikipedia : IResponse
    {
        public Query Query { get; set; }

        public void EnsureValidResponse()
        {
            if (Query == null)
                throw new HttpParseException("Wikipedia response does not contain 'query'");

            if (Query.Pages == null)
                throw new HttpParseException("Wikipedia response does not contain 'query.pages'");

            var children = Query.Pages.Children();
            if (!children.Any() || children.First().First == null)
                throw new HttpParseException("Wikipedia response does not contain any children for 'query.pages'");

            if (children.First().First["extract"] == null)
                throw new HttpParseException("Wikipedia response does not contain 'extract'");
        }

        public string ParseExtract()
        {
            var page = Query.Pages.Children().First().First;
            return page["extract"].ToString();
        }
    }

    public class Query
    {
        public JObject Pages { get; set; }
    }
}