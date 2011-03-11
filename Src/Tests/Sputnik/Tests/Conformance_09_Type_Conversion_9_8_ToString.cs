using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_8_ToString : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.8_ToString"); }
    [TestMethod] public void S9_8_A1_T1_js() { RunFile(@"S9.8_A1_T1.js"); }
    [TestMethod] public void S9_8_A1_T2_js() { RunFile(@"S9.8_A1_T2.js"); }
    [TestMethod] public void S9_8_A2_T1_js() { RunFile(@"S9.8_A2_T1.js"); }
    [TestMethod] public void S9_8_A2_T2_js() { RunFile(@"S9.8_A2_T2.js"); }
    [TestMethod] public void S9_8_A3_T1_js() { RunFile(@"S9.8_A3_T1.js"); }
    [TestMethod] public void S9_8_A3_T2_js() { RunFile(@"S9.8_A3_T2.js"); }
    [TestMethod] public void S9_8_A4_T1_js() { RunFile(@"S9.8_A4_T1.js"); }
    [TestMethod] public void S9_8_A4_T2_js() { RunFile(@"S9.8_A4_T2.js"); }
    [TestMethod] public void S9_8_A5_T1_js() { RunFile(@"S9.8_A5_T1.js"); }
    [TestMethod] public void S9_8_A5_T2_js() { RunFile(@"S9.8_A5_T2.js"); }
  }
}