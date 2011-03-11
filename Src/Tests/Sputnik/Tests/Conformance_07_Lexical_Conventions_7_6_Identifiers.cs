using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_6_Identifiers : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.6_Identifiers"); }
    [TestMethod] public void S7_6_A1_2_T1_js() { RunFile(@"S7.6_A1.2_T1.js"); }
    [TestMethod] public void S7_6_A1_2_T2_js() { RunFile(@"S7.6_A1.2_T2.js"); }
    [TestMethod] public void S7_6_A1_2_T3_js() { RunFile(@"S7.6_A1.2_T3.js"); }
    [TestMethod] public void S7_6_A1_3_T1_js() { RunFile(@"S7.6_A1.3_T1.js"); }
    [TestMethod] public void S7_6_A1_3_T2_js() { RunFile(@"S7.6_A1.3_T2.js"); }
    [TestMethod] public void S7_6_A1_3_T3_js() { RunFile(@"S7.6_A1.3_T3.js"); }
    [TestMethod] public void S7_6_A2_1_T1_js() { RunFile(@"S7.6_A2.1_T1.js"); }
    [TestMethod] public void S7_6_A2_1_T2_js() { RunFile(@"S7.6_A2.1_T2.js"); }
    [TestMethod] public void S7_6_A2_1_T3_js() { RunFile(@"S7.6_A2.1_T3.js"); }
    [TestMethod] public void S7_6_A2_1_T4_js() { RunFile(@"S7.6_A2.1_T4.js"); }
    [TestMethod] public void S7_6_A4_1_T1_js() { RunFile(@"S7.6_A4.1_T1.js"); }
    [TestMethod] public void S7_6_A4_1_T2_js() { RunFile(@"S7.6_A4.1_T2.js"); }
    [TestMethod] public void S7_6_A4_2_T1_js() { RunFile(@"S7.6_A4.2_T1.js"); }
    [TestMethod] public void S7_6_A4_2_T2_js() { RunFile(@"S7.6_A4.2_T2.js"); }
    [TestMethod] public void S7_6_A4_3_T1_js() { RunFile(@"S7.6_A4.3_T1.js"); }
  }
}