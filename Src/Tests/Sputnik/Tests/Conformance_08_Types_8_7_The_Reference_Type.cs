using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_7_The_Reference_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.7_The_Reference_Type"); }
    [TestMethod] public void S8_7_1_A1_js() { RunFile(@"S8.7.1_A1.js"); }
    [TestMethod] public void S8_7_1_A2_js() { RunFile(@"S8.7.1_A2.js"); }
    [TestMethod] public void S8_7_2_A1_T1_js() { RunFile(@"S8.7.2_A1_T1.js"); }
    [TestMethod] public void S8_7_2_A1_T2_js() { RunFile(@"S8.7.2_A1_T2.js"); }
    [TestMethod] public void S8_7_2_A2_js() { RunFile(@"S8.7.2_A2.js"); }
    [TestMethod] public void S8_7_2_A3_js() { RunFile(@"S8.7.2_A3.js"); }
    [TestMethod] public void S8_7_A1_js() { RunFile(@"S8.7_A1.js"); }
    [TestMethod] public void S8_7_A2_js() { RunFile(@"S8.7_A2.js"); }
    [TestMethod] public void S8_7_A3_js() { RunFile(@"S8.7_A3.js"); }
    [TestMethod] public void S8_7_A4_js() { RunFile(@"S8.7_A4.js"); }
    [TestMethod] public void S8_7_A5_T1_js() { RunFile(@"S8.7_A5_T1.js"); }
    [TestMethod] public void S8_7_A5_T2_js() { RunFile(@"S8.7_A5_T2.js"); }
    [TestMethod] public void S8_7_A6_js() { RunFile(@"S8.7_A6.js"); }
    [TestMethod] public void S8_7_A7_js() { RunFile(@"S8.7_A7.js"); }
  }
}