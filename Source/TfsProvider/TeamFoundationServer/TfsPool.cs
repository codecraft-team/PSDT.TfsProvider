using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsPool {
    public string Id { get; set; }
    public string Name { get; set; }

    public TfsPool(JToken token) {
      Id = token.Value<string>("id");
      Name = token.Value<string>("name");
    }
  }
}