using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_1_The_Undefined_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.1_The_Undefined_Type"); }
    [TestMethod] public void S8_1_A1_T1_js() { RunFile(@"S8.1_A1_T1.js"); }
    [TestMethod] public void S8_1_A1_T2_js() { RunFile(@"S8.1_A1_T2.js"); }
    [TestMethod] public void S8_1_A2_T1_js() { RunFile(@"S8.1_A2_T1.js"); }
    [TestMethod] public void S8_1_A2_T2_js() { RunFile(@"S8.1_A2_T2.js"); }
    [TestMethod] public void S8_1_A3_js() { RunFile(@"S8.1_A3.js"); }
    [TestMethod] public void S8_1_A4_js() { RunFile(@"S8.1_A4.js"); }
    [TestMethod] public void S8_1_A5_js() { RunFile(@"S8.1_A5.js"); }
  }
}