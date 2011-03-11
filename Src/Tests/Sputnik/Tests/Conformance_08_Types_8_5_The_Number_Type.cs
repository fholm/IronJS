using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_5_The_Number_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.5_The_Number_Type"); }
    [TestMethod] public void S8_5_A1_js() { RunFile(@"S8.5_A1.js"); }
    [TestMethod] public void S8_5_A10_js() { RunFile(@"S8.5_A10.js"); }
    [TestMethod] public void S8_5_A11_T1_js() { RunFile(@"S8.5_A11_T1.js"); }
    [TestMethod] public void S8_5_A11_T2_js() { RunFile(@"S8.5_A11_T2.js"); }
    [TestMethod] public void S8_5_A12_1_js() { RunFile(@"S8.5_A12.1.js"); }
    [TestMethod] public void S8_5_A12_2_js() { RunFile(@"S8.5_A12.2.js"); }
    [TestMethod] public void S8_5_A13_T1_js() { RunFile(@"S8.5_A13_T1.js"); }
    [TestMethod] public void S8_5_A13_T2_js() { RunFile(@"S8.5_A13_T2.js"); }
    [TestMethod] public void S8_5_A14_T1_js() { RunFile(@"S8.5_A14_T1.js"); }
    [TestMethod] public void S8_5_A14_T2_js() { RunFile(@"S8.5_A14_T2.js"); }
    [TestMethod] public void S8_5_A2_1_js() { RunFile(@"S8.5_A2.1.js"); }
    [TestMethod] public void S8_5_A2_2_js() { RunFile(@"S8.5_A2.2.js"); }
    [TestMethod] public void S8_5_A3_js() { RunFile(@"S8.5_A3.js"); }
    [TestMethod] public void S8_5_A4_js() { RunFile(@"S8.5_A4.js"); }
    [TestMethod] public void S8_5_A5_js() { RunFile(@"S8.5_A5.js"); }
    [TestMethod] public void S8_5_A6_js() { RunFile(@"S8.5_A6.js"); }
    [TestMethod] public void S8_5_A7_js() { RunFile(@"S8.5_A7.js"); }
    [TestMethod] public void S8_5_A8_js() { RunFile(@"S8.5_A8.js"); }
    [TestMethod] public void S8_5_A9_js() { RunFile(@"S8.5_A9.js"); }
  }
}