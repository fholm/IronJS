using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_4_Expression_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.4_Expression_Statement"); }
    [TestMethod] public void S12_4_A1_js() { RunFile(@"S12.4_A1.js"); }
    [TestMethod] public void S12_4_A2_T1_js() { RunFile(@"S12.4_A2_T1.js"); }
    [TestMethod] public void S12_4_A2_T2_js() { RunFile(@"S12.4_A2_T2.js"); }
  }
}