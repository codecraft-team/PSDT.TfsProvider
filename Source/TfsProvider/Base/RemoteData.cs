using System.Collections.Generic;

namespace PSDT.TfsProvider.Base {
  public class VirtualDriveSample : Container {
    public VirtualDriveSample() : base("TFS:\\") {
    }

    public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
      SampleFolder sourceDirectory = new SampleFolder("Source", this);
      sourceDirectory.AddContainer("Directory 1");
      sourceDirectory.AddContainerItem("File 1");
      sourceDirectory.AddContainerItem("File 2");

      SampleFolder ncrunchDirectory = new SampleFolder("NCrunch", this);
      ncrunchDirectory.AddContainer("1");
      ncrunchDirectory.AddContainer("2");
      ncrunchDirectory.AddContainer("3");

      SampleFolder tempDirectory = new SampleFolder("Temp", this);
      tempDirectory.AddContainer("T.Temp 1");
      tempDirectory.AddContainer("T.Temp 2");
      tempDirectory.AddContainerItem("T.File 1");
      tempDirectory.AddContainerItem("T.File 2");
      tempDirectory.AddContainerItem("T.File 3");

      return new[] {
        sourceDirectory,
        ncrunchDirectory,
        tempDirectory
      };
    }

    private class SampleFolder : Container {
      private readonly List<DriveItem> _childItems = new List<DriveItem>();

      public SampleFolder(string name, Container parent) : base(name, parent) {
      }

      public SampleFolder AddContainer(string directory) {
        SampleFolder sampleFolder = new SampleFolder(directory, this);
        _childItems.Add(sampleFolder);
        return sampleFolder;
      }

      public void AddContainerItem(string name) {
        SampleFile file = new SampleFile(name, this);
        _childItems.Add(file);
      }

      public override IEnumerable<IDriveItem> GetChildItems(object dynamicParameters) {
        return _childItems;
      }
    }

    private class SampleFile : ContainerItem {
      public SampleFile(string name, IContainer parent) : base(name, parent) {
      }
    }
  }
}