using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_2_ToBoolean : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.2_ToBoolean"); }
    [TestMethod] public void S9_2_A1_T1_js() { RunFile(@"S9.2_A1_T1.js"); }
    [TestMethod] public void S9_2_A1_T2_js() { RunFile(@"S9.2_A1_T2.js"); }
    [TestMethod] public void S9_2_A2_T1_js() { RunFile(@"S9.2_A2_T1.js"); }
    [TestMethod] public void S9_2_A2_T2_js() { RunFile(@"S9.2_A2_T2.js"); }
    [TestMethod] public void S9_2_A3_T1_js() { RunFile(@"S9.2_A3_T1.js"); }
    [TestMethod] public void S9_2_A3_T2_js() { RunFile(@"S9.2_A3_T2.js"); }
    [TestMethod] public void S9_2_A4_T1_js() { RunFile(@"S9.2_A4_T1.js"); }
    [TestMethod] public void S9_2_A4_T2_js() { RunFile(@"S9.2_A4_T2.js"); }
    [TestMethod] public void S9_2_A4_T3_js() { RunFile(@"S9.2_A4_T3.js"); }
    [TestMethod] public void S9_2_A4_T4_js() { RunFile(@"S9.2_A4_T4.js"); }
    [TestMethod] public void S9_2_A5_T1_js() { RunFile(@"S9.2_A5_T1.js"); }
    [TestMethod] public void S9_2_A5_T2_js() { RunFile(@"S9.2_A5_T2.js"); }
    [TestMethod] public void S9_2_A5_T3_js() { RunFile(@"S9.2_A5_T3.js"); }
    [TestMethod] public void S9_2_A5_T4_js() { RunFile(@"S9.2_A5_T4.js"); }
    [TestMethod] public void S9_2_A6_T1_js() { RunFile(@"S9.2_A6_T1.js"); }
    [TestMethod] public void S9_2_A6_T2_js() { RunFile(@"S9.2_A6_T2.js"); }
  }
}