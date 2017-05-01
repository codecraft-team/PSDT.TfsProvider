using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class Links {
    public string Collection { get; set; }
    public string Api { get; set; }
    public string Web { get; set; }

    public Links(JToken token) {
      JToken links = token.SelectToken("_links");
      Collection = links?.SelectToken("collection")?.Value<string>("href");
      Api = links?.SelectToken("self")?.Value<string>("href");
      Web = links?.SelectToken("web")?.Value<string>("href");
    }
  }
}