using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_4_The_String_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.4_The_String_Type"); }
    [TestMethod] public void S8_4_A1_js() { RunFile(@"S8.4_A1.js"); }
    [TestMethod] public void S8_4_A10_js() { RunFile(@"S8.4_A10.js"); }
    [TestMethod] public void S8_4_A11_js() { RunFile(@"S8.4_A11.js"); }
    [TestMethod] public void S8_4_A12_js() { RunFile(@"S8.4_A12.js"); }
    [TestMethod] public void S8_4_A13_T1_js() { RunFile(@"S8.4_A13_T1.js"); }
    [TestMethod] public void S8_4_A13_T2_js() { RunFile(@"S8.4_A13_T2.js"); }
    [TestMethod] public void S8_4_A13_T3_js() { RunFile(@"S8.4_A13_T3.js"); }
    [TestMethod] public void S8_4_A14_T1_js() { RunFile(@"S8.4_A14_T1.js"); }
    [TestMethod] public void S8_4_A14_T2_js() { RunFile(@"S8.4_A14_T2.js"); }
    [TestMethod] public void S8_4_A14_T3_js() { RunFile(@"S8.4_A14_T3.js"); }
    [TestMethod] public void S8_4_A2_js() { RunFile(@"S8.4_A2.js"); }
    [TestMethod] public void S8_4_A3_js() { RunFile(@"S8.4_A3.js"); }
    [TestMethod] public void S8_4_A4_js() { RunFile(@"S8.4_A4.js"); }
    [TestMethod] public void S8_4_A5_js() { RunFile(@"S8.4_A5.js"); }
    [TestMethod] public void S8_4_A6_1_js() { RunFile(@"S8.4_A6.1.js"); }
    [TestMethod] public void S8_4_A6_2_js() { RunFile(@"S8.4_A6.2.js"); }
    [TestMethod] public void S8_4_A7_1_js() { RunFile(@"S8.4_A7.1.js"); }
    [TestMethod] public void S8_4_A7_2_js() { RunFile(@"S8.4_A7.2.js"); }
    [TestMethod] public void S8_4_A7_3_js() { RunFile(@"S8.4_A7.3.js"); }
    [TestMethod] public void S8_4_A7_4_js() { RunFile(@"S8.4_A7.4.js"); }
    [TestMethod] public void S8_4_A8_js() { RunFile(@"S8.4_A8.js"); }
    [TestMethod] public void S8_4_A9_T1_js() { RunFile(@"S8.4_A9_T1.js"); }
    [TestMethod] public void S8_4_A9_T2_js() { RunFile(@"S8.4_A9_T2.js"); }
    [TestMethod] public void S8_4_A9_T3_js() { RunFile(@"S8.4_A9_T3.js"); }
  }
}