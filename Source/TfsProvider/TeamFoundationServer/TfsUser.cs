using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsUser {
    public string DisplayName { get; set; }
    public string UniqueName { get; set; }
    public string Url { get; set; }

    public TfsUser(JToken token) {
      DisplayName = token.Value<string>("displayName");
      UniqueName = token.Value<string>("uniqueName");
      Url = token.Value<string>("url");
    }
  }
}