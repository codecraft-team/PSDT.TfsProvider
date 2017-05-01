using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PSDT.TfsProvider.TeamFoundationServer.DataAccess {
  public class QueueBuildResponse {
    private readonly string _agentName;
    private readonly HttpResponseMessage _response;

    public QueueBuildResponse(string agentName, HttpResponseMessage responseMessage) {
      _agentName = agentName;
      _response = responseMessage;
    }

    public void Validate() {
      if (_response.StatusCode == HttpStatusCode.OK) {
        return;
      }

      string responseContent = _response.Content.ReadAsStringAsync().Result;
      JObject responseJson = JObject.Parse(responseContent);
      StringBuilder message = new StringBuilder();
      message.Append(responseJson.Value<string>("Message") ?? "Unknown error, response does't contain Message.");

      JToken validationResults = responseJson.SelectToken("ValidationResults");
      JArray validationItems = validationResults as JArray;
      if (validationItems != null) {
        foreach (JToken validationItem in validationItems) {
          string validationMessage = validationItem.Value<string>("message");
          if (string.IsNullOrWhiteSpace(validationMessage)) {
            continue;
          }

          validationMessage = validationMessage.Replace("{agent name}", _agentName);
          string validationResult = validationItem.Value<string>("result");
          message.Append($" Severity: {validationResult}. {validationMessage}");
        }
      }

      throw new InvalidOperationException(message.ToString());
    }
  }
}