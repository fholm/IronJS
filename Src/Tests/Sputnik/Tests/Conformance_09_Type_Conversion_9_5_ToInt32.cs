using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_5_ToInt32 : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.5_ToInt32"); }
    [TestMethod] public void S9_5_A1_T1_js() { RunFile(@"S9.5_A1_T1.js"); }
    [TestMethod] public void S9_5_A2_1_T1_js() { RunFile(@"S9.5_A2.1_T1.js"); }
    [TestMethod] public void S9_5_A2_1_T2_js() { RunFile(@"S9.5_A2.1_T2.js"); }
    [TestMethod] public void S9_5_A2_2_T1_js() { RunFile(@"S9.5_A2.2_T1.js"); }
    [TestMethod] public void S9_5_A2_2_T2_js() { RunFile(@"S9.5_A2.2_T2.js"); }
    [TestMethod] public void S9_5_A2_3_T1_js() { RunFile(@"S9.5_A2.3_T1.js"); }
    [TestMethod] public void S9_5_A2_3_T2_js() { RunFile(@"S9.5_A2.3_T2.js"); }
    [TestMethod] public void S9_5_A3_1_T1_js() { RunFile(@"S9.5_A3.1_T1.js"); }
    [TestMethod] public void S9_5_A3_1_T2_js() { RunFile(@"S9.5_A3.1_T2.js"); }
    [TestMethod] public void S9_5_A3_1_T3_js() { RunFile(@"S9.5_A3.1_T3.js"); }
    [TestMethod] public void S9_5_A3_1_T4_js() { RunFile(@"S9.5_A3.1_T4.js"); }
    [TestMethod] public void S9_5_A3_2_T1_js() { RunFile(@"S9.5_A3.2_T1.js"); }
    [TestMethod] public void S9_5_A3_2_T2_js() { RunFile(@"S9.5_A3.2_T2.js"); }
  }
}