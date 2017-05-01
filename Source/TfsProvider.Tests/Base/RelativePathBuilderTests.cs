using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSDT.TfsProvider.Base;

namespace PSDT.TfsProvider.Tests.Base {
  [TestClass]
  public class RelativePathBuilderTests {
    public RelativePathBuilder PathBuilder { get; set; }

    [TestInitialize]
    public void TestInitialize() {
      PathBuilder = new RelativePathBuilder(() => "TFS:\\");
    }

    [TestMethod]
    public void RelativeFromSourceToTemp() {
      PathBuilder.SetBasePath("TFS:\\Source");
      PathBuilder.SetPath("TFS:\\Temp");

      string actualPath = PathBuilder.ToString();

      Assert.AreEqual("..\\Temp", actualPath);
    }

    [TestMethod]
    public void RelativeTempToSource() {
      PathBuilder.SetBasePath("TFS:\\Temp");
      PathBuilder.SetPath("TFS:\\Source");

      string actualPath = PathBuilder.ToString();

      Assert.AreEqual("..\\Source", actualPath);
    }

    [TestMethod]
    public void StayAtCurrentLocation() {
      PathBuilder.SetBasePath("TFS:\\Temp");
      PathBuilder.SetPath("TFS:\\Temp");

      string actualPath = PathBuilder.ToString();

      Assert.AreEqual(string.Empty, actualPath);
    }

    [TestMethod]
    public void RelativeTempToTempChild() {
      PathBuilder.SetBasePath("TFS:\\Temp");
      PathBuilder.SetPath("TFS:\\Temp\\TempChild1");

      string actualPath = PathBuilder.ToString();

      Assert.AreEqual("TempChild1", actualPath);
    }

    [TestMethod]
    public void RelativeTempChildToTemp() {
      PathBuilder.SetBasePath("TFS:\\Temp\\TempChild1");
      PathBuilder.SetPath("TFS:\\Temp");

      string actualPath = PathBuilder.ToString();

      Assert.AreEqual("..\\", actualPath);
    }

    [TestMethod]
    public void RelativeTempToTempSubChild() {
      PathBuilder.SetBasePath("TFS:\\Temp");
      PathBuilder.SetPath("TFS:\\Temp\\TempChild1\\TempSubChild1");

      string actualPath = PathBuilder.ToString();

      Assert.AreEqual("TempChild1\\TempSubChild1", actualPath);
    }
  }
}