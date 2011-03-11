using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_8_ToString_9_8_1_ToString_Applied_to_the_Number_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.8_ToString\9.8.1_ToString_Applied_to_the_Number_Type"); }
    [TestMethod] public void S9_8_1_A1_js() { RunFile(@"S9.8.1_A1.js"); }
    [TestMethod] public void S9_8_1_A10_js() { RunFile(@"S9.8.1_A10.js"); }
    [TestMethod] public void S9_8_1_A2_js() { RunFile(@"S9.8.1_A2.js"); }
    [TestMethod] public void S9_8_1_A3_js() { RunFile(@"S9.8.1_A3.js"); }
    [TestMethod] public void S9_8_1_A4_js() { RunFile(@"S9.8.1_A4.js"); }
    [TestMethod] public void S9_8_1_A6_js() { RunFile(@"S9.8.1_A6.js"); }
    [TestMethod] public void S9_8_1_A7_js() { RunFile(@"S9.8.1_A7.js"); }
    [TestMethod] public void S9_8_1_A8_js() { RunFile(@"S9.8.1_A8.js"); }
    [TestMethod] public void S9_8_1_A9_T1_js() { RunFile(@"S9.8.1_A9_T1.js"); }
    [TestMethod] public void S9_8_1_A9_T2_js() { RunFile(@"S9.8.1_A9_T2.js"); }
  }
}