using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class Team {
    public string Id { get; set; }
    public string Name { get; set; }
    public string ApiUrl { get; set; }

    public Team(JToken teamToken) {
      Id = teamToken.Value<string>("id");
      Name = teamToken.Value<string>("name");
      ApiUrl = teamToken.Value<string>("url");
    }
  }
}