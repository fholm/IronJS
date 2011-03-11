using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_7_Punctuators : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.7_Punctuators"); }
    [TestMethod] public void S7_7_A1_js() { RunFile(@"S7.7_A1.js"); }
    [TestMethod] public void S7_7_A2_T1_js() { RunFile(@"S7.7_A2_T1.js"); }
    [TestMethod] public void S7_7_A2_T10_js() { RunFile(@"S7.7_A2_T10.js"); }
    [TestMethod] public void S7_7_A2_T2_js() { RunFile(@"S7.7_A2_T2.js"); }
    [TestMethod] public void S7_7_A2_T3_js() { RunFile(@"S7.7_A2_T3.js"); }
    [TestMethod] public void S7_7_A2_T4_js() { RunFile(@"S7.7_A2_T4.js"); }
    [TestMethod] public void S7_7_A2_T5_js() { RunFile(@"S7.7_A2_T5.js"); }
    [TestMethod] public void S7_7_A2_T6_js() { RunFile(@"S7.7_A2_T6.js"); }
    [TestMethod] public void S7_7_A2_T7_js() { RunFile(@"S7.7_A2_T7.js"); }
    [TestMethod] public void S7_7_A2_T8_js() { RunFile(@"S7.7_A2_T8.js"); }
    [TestMethod] public void S7_7_A2_T9_js() { RunFile(@"S7.7_A2_T9.js"); }
  }
}