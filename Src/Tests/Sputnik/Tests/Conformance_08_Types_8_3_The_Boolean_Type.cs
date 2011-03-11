using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_3_The_Boolean_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.3_The_Boolean_Type"); }
    [TestMethod] public void S8_3_A1_T1_js() { RunFile(@"S8.3_A1_T1.js"); }
    [TestMethod] public void S8_3_A1_T2_js() { RunFile(@"S8.3_A1_T2.js"); }
    [TestMethod] public void S8_3_A2_1_js() { RunFile(@"S8.3_A2.1.js"); }
    [TestMethod] public void S8_3_A2_2_js() { RunFile(@"S8.3_A2.2.js"); }
    [TestMethod] public void S8_3_A3_js() { RunFile(@"S8.3_A3.js"); }
  }
}