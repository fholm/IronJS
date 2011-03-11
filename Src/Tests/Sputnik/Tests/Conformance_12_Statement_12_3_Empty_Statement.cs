using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_3_Empty_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.3_Empty_Statement"); }
    [TestMethod] public void S12_3_A1_js() { RunFile(@"S12.3_A1.js"); }
  }
}