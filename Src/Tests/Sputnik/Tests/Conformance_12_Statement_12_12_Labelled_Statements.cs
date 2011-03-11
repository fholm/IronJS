using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_12_Labelled_Statements : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.12_Labelled_Statements"); }
    [TestMethod] public void S12_12_A1_T1_js() { RunFile(@"S12.12_A1_T1.js"); }
  }
}