using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using PSDT.TfsProvider.Base;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsBuildDefinition : Container {
    public BuildDefinitionsContainer BuildDefinitionsContainer { get; }
    public string Id { get; internal set; }
    public Links Links { get; internal set; }
    public TfsUser AuthoredBy { get; internal set; }
    public DateTime CreatedDate { get; internal set; }
    public TfsQueue Queue { get; internal set; }
    public string Revision { get; set; }

    public TfsBuildDefinition(string name, BuildDefinitionsContainer parent) : base(name, parent) {
      BuildDefinitionsContainer = parent;
    }

    public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      return new DriveItem[0];
    }

    public override void InvokeDefaultAction(object dynamicParameters) {
      TfsBuildDefinitionInvokeItemParameters parameter = dynamicParameters as TfsBuildDefinitionInvokeItemParameters;
      if (null == parameter || !parameter.Queue) {
        Process.Start(Links.Web);
      }
      else {
        BuildDefinitionsContainer.Project.ProjectCollection.Tfs.DataService.QueueBuild(this);
      }
    }

    public override object InvokeDefaultActionDynamicParameters() {
      return new TfsBuildDefinitionInvokeItemParameters();
    }
  }

  internal class TfsBuildDefinitionInvokeItemParameters {
    [Alias("q")]
    [Parameter]
    public SwitchParameter Queue { get; set; }
  }
}