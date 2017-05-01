using System;
using System.Collections.Generic;
using System.Diagnostics;
using PSDT.TfsProvider.Base;

namespace PSDT.TfsProvider.TeamFoundationServer {
  public class TfsBuild : Container {
    public BuildsContainer BuildsContainer { get; }
    public string Id { get; internal set; }
    public string Status { get; internal set; }
    public string Result { get; internal set; }
    public DateTime CompletedOn { get; internal set; }
    public string RequestedFor { get; internal set; }
    public Links Links { get; internal set; }

    public TfsBuild(string name, BuildsContainer parent) : base(name, parent) {
      BuildsContainer = parent;
    }

    public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      return new DriveItem[0];
    }

    public override void InvokeDefaultAction(object dynamicParameters) {
      Process.Start(Links.Web);
    }
  }
}