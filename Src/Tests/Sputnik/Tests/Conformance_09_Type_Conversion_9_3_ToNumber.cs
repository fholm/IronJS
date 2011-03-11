using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_3_ToNumber : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.3_ToNumber"); }
    [TestMethod] public void S9_3_A1_T1_js() { RunFile(@"S9.3_A1_T1.js"); }
    [TestMethod] public void S9_3_A1_T2_js() { RunFile(@"S9.3_A1_T2.js"); }
    [TestMethod] public void S9_3_A2_T1_js() { RunFile(@"S9.3_A2_T1.js"); }
    [TestMethod] public void S9_3_A2_T2_js() { RunFile(@"S9.3_A2_T2.js"); }
    [TestMethod] public void S9_3_A3_T1_js() { RunFile(@"S9.3_A3_T1.js"); }
    [TestMethod] public void S9_3_A3_T2_js() { RunFile(@"S9.3_A3_T2.js"); }
    [TestMethod] public void S9_3_A4_1_T1_js() { RunFile(@"S9.3_A4.1_T1.js"); }
    [TestMethod] public void S9_3_A4_1_T2_js() { RunFile(@"S9.3_A4.1_T2.js"); }
    [TestMethod] public void S9_3_A4_2_T1_js() { RunFile(@"S9.3_A4.2_T1.js"); }
    [TestMethod] public void S9_3_A4_2_T2_js() { RunFile(@"S9.3_A4.2_T2.js"); }
    [TestMethod] public void S9_3_A5_T1_js() { RunFile(@"S9.3_A5_T1.js"); }
    [TestMethod] public void S9_3_A5_T2_js() { RunFile(@"S9.3_A5_T2.js"); }
  }
}