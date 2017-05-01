using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer.DataAccess {
  public class RetrieveBuildsResponse {
    public JArray Builds { get; set; }
  }
}