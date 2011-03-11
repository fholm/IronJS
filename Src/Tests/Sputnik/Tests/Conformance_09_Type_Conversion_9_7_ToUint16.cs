using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_7_ToUint16 : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.7_ToUint16"); }
    [TestMethod] public void S9_7_A1_js() { RunFile(@"S9.7_A1.js"); }
    [TestMethod] public void S9_7_A2_1_js() { RunFile(@"S9.7_A2.1.js"); }
    [TestMethod] public void S9_7_A2_2_js() { RunFile(@"S9.7_A2.2.js"); }
    [TestMethod] public void S9_7_A3_1_T1_js() { RunFile(@"S9.7_A3.1_T1.js"); }
    [TestMethod] public void S9_7_A3_1_T2_js() { RunFile(@"S9.7_A3.1_T2.js"); }
    [TestMethod] public void S9_7_A3_1_T3_js() { RunFile(@"S9.7_A3.1_T3.js"); }
    [TestMethod] public void S9_7_A3_1_T4_js() { RunFile(@"S9.7_A3.1_T4.js"); }
    [TestMethod] public void S9_7_A3_2_T1_js() { RunFile(@"S9.7_A3.2_T1.js"); }
  }
}