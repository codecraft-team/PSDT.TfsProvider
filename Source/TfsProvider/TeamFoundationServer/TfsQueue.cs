using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsQueue {
    public string Id { get; set; }
    public string Name { get; set; }
    public TfsPool Pool { get; set; }

    public TfsQueue(JToken token) {
      Id = token.Value<string>("id");
      Name = token.Value<string>("name");
      Pool = new TfsPool(token.SelectToken("pool"));
    }
  }
}