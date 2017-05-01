using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PSDT.TfsProvider.Base;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess;
using PSDT.TfsProvider.TeamFoundationServer.DataAccess.Fakes;

namespace PSDT.TfsProvider.Tests.TeamFoundationServer {
  [TestClass]
  public class TfsNodeTests {
    private const string TfsDriveName = "TfsOnline";

    public PowerShell PowerShell { get; set; }
    public ITfsDataService TfsDataService { get; set; }

    [TestInitialize]
    public void TestInitialize() {
      JObject projectcCollection1 = JObject.Parse("{ \"name\": \"collection1\" }");
      JObject projectcCollection2 = JObject.Parse("{ \"name\": \"collection2\" }");

      Dictionary<string, JArray> projectsLookup = new Dictionary<string, JArray> {
        {"collection1", JArray.Parse("[{ \"name\": \"Playground1\" },{ \"name\": \"Playground11\" }]")},
        {"collection2", JArray.Parse("[{ \"name\": \"Playground2\" },{ \"name\": \"Playground22\" }]")}
      };

      TfsDataService = new StubITfsDataService {
        RetrieveProjectCollections = () => new JArray(projectcCollection1, projectcCollection2),
        RetrieveProjectsTfsProjectCollection = projectCollection => projectsLookup[projectCollection.Name]
      };

      PowerShell = PowerShell.Create(RunspaceMode.NewRunspace);
      PowerShell.NewDrive(TfsDriveName, TfsDataService);
    }

    [TestMethod]
    public void RootContainerHasTfsProjectCollectionNodeChildren() {
      JArray expectedProjects = TfsDataService.RetrieveProjectCollections();

      Collection<IDriveItem> actualProjectNodes = PowerShell.AddScript($"Get-ChildItem").Invoke<IDriveItem>();

      Assert.IsFalse(PowerShell.HadErrors);
      Assert.IsTrue(expectedProjects.All(expectedProject => actualProjectNodes.Any(projectNode => projectNode.Name == expectedProject.Value<string>("name"))));
    }
  }
}