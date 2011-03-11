using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_4_Comments : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.4_Comments"); }
    [TestMethod] public void S7_4_A1_T1_js() { RunFile(@"S7.4_A1_T1.js"); }
    [TestMethod] public void S7_4_A1_T2_js() { RunFile(@"S7.4_A1_T2.js"); }
    [TestMethod] public void S7_4_A2_T1_js() { RunFile(@"S7.4_A2_T1.js"); }
    [TestMethod] public void S7_4_A2_T2_js() { RunFile(@"S7.4_A2_T2.js"); }
    [TestMethod] public void S7_4_A3_js() { RunFile(@"S7.4_A3.js"); }
    [TestMethod] public void S7_4_A4_T1_js() { RunFile(@"S7.4_A4_T1.js"); }
    [TestMethod] public void S7_4_A4_T2_js() { RunFile(@"S7.4_A4_T2.js"); }
    [TestMethod] public void S7_4_A4_T3_js() { RunFile(@"S7.4_A4_T3.js"); }
    [TestMethod] public void S7_4_A4_T4_js() { RunFile(@"S7.4_A4_T4.js"); }
    [TestMethod] public void S7_4_A4_T5_js() { RunFile(@"S7.4_A4_T5.js"); }
    [TestMethod] public void S7_4_A4_T6_js() { RunFile(@"S7.4_A4_T6.js"); }
    [TestMethod] public void S7_4_A4_T7_js() { RunFile(@"S7.4_A4_T7.js"); }
    [TestMethod] public void S7_4_A5_js() { RunFile(@"S7.4_A5.js"); }
    [TestMethod] public void S7_4_A6_js() { RunFile(@"S7.4_A6.js"); }
  }
}