using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_3_Line_Terminators : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.3_Line_Terminators"); }
    [TestMethod] public void S7_3_A1_1_T1_js() { RunFile(@"S7.3_A1.1_T1.js"); }
    [TestMethod] public void S7_3_A1_1_T2_js() { RunFile(@"S7.3_A1.1_T2.js"); }
    [TestMethod] public void S7_3_A1_2_T1_js() { RunFile(@"S7.3_A1.2_T1.js"); }
    [TestMethod] public void S7_3_A1_2_T2_js() { RunFile(@"S7.3_A1.2_T2.js"); }
    [TestMethod] public void S7_3_A1_3_js() { RunFile(@"S7.3_A1.3.js"); }
    [TestMethod] public void S7_3_A1_4_js() { RunFile(@"S7.3_A1.4.js"); }
    [TestMethod] public void S7_3_A2_1_T1_js() { RunFile_ExpectException(@"S7.3_A2.1_T1.js"); }
    [TestMethod] public void S7_3_A2_1_T2_js() { RunFile(@"S7.3_A2.1_T2.js"); }
    [TestMethod] public void S7_3_A2_2_T1_js() { RunFile(@"S7.3_A2.2_T1.js"); }
    [TestMethod] public void S7_3_A2_2_T2_js() { RunFile(@"S7.3_A2.2_T2.js"); }
    [TestMethod] public void S7_3_A2_3_js() { RunFile(@"S7.3_A2.3.js"); }
    [TestMethod] public void S7_3_A2_4_js() { RunFile(@"S7.3_A2.4.js"); }
    [TestMethod] public void S7_3_A3_1_T1_js() { RunFile(@"S7.3_A3.1_T1.js"); }
    [TestMethod] public void S7_3_A3_1_T2_js() { RunFile(@"S7.3_A3.1_T2.js"); }
    [TestMethod] public void S7_3_A3_1_T3_js() { RunFile(@"S7.3_A3.1_T3.js"); }
    [TestMethod] public void S7_3_A3_2_T1_js() { RunFile(@"S7.3_A3.2_T1.js"); }
    [TestMethod] public void S7_3_A3_2_T2_js() { RunFile(@"S7.3_A3.2_T2.js"); }
    [TestMethod] public void S7_3_A3_2_T3_js() { RunFile(@"S7.3_A3.2_T3.js"); }
    [TestMethod] public void S7_3_A3_3_T1_js() { RunFile(@"S7.3_A3.3_T1.js"); }
    [TestMethod] public void S7_3_A3_3_T2_js() { RunFile(@"S7.3_A3.3_T2.js"); }
    [TestMethod] public void S7_3_A3_4_T1_js() { RunFile(@"S7.3_A3.4_T1.js"); }
    [TestMethod] public void S7_3_A3_4_T2_js() { RunFile(@"S7.3_A3.4_T2.js"); }
    [TestMethod] public void S7_3_A4_T1_js() { RunFile(@"S7.3_A4_T1.js"); }
    [TestMethod] public void S7_3_A4_T2_js() { RunFile(@"S7.3_A4_T2.js"); }
    [TestMethod] public void S7_3_A4_T3_js() { RunFile(@"S7.3_A4_T3.js"); }
    [TestMethod] public void S7_3_A4_T4_js() { RunFile(@"S7.3_A4_T4.js"); }
    [TestMethod] public void S7_3_A5_1_T1_js() { RunFile(@"S7.3_A5.1_T1.js"); }
    [TestMethod] public void S7_3_A5_1_T2_js() { RunFile(@"S7.3_A5.1_T2.js"); }
    [TestMethod] public void S7_3_A5_2_T1_js() { RunFile(@"S7.3_A5.2_T1.js"); }
    [TestMethod] public void S7_3_A5_2_T2_js() { RunFile(@"S7.3_A5.2_T2.js"); }
    [TestMethod] public void S7_3_A5_3_js() { RunFile(@"S7.3_A5.3.js"); }
    [TestMethod] public void S7_3_A5_4_js() { RunFile(@"S7.3_A5.4.js"); }
    [TestMethod] public void S7_3_A6_T1_js() { RunFile(@"S7.3_A6_T1.js"); }
    [TestMethod] public void S7_3_A6_T2_js() { RunFile(@"S7.3_A6_T2.js"); }
    [TestMethod] public void S7_3_A6_T3_js() { RunFile(@"S7.3_A6_T3.js"); }
    [TestMethod] public void S7_3_A6_T4_js() { RunFile(@"S7.3_A6_T4.js"); }
    [TestMethod] public void S7_3_A7_T1_js() { RunFile(@"S7.3_A7_T1.js"); }
    [TestMethod] public void S7_3_A7_T2_js() { RunFile(@"S7.3_A7_T2.js"); }
    [TestMethod] public void S7_3_A7_T3_js() { RunFile(@"S7.3_A7_T3.js"); }
    [TestMethod] public void S7_3_A7_T4_js() { RunFile(@"S7.3_A7_T4.js"); }
    [TestMethod] public void S7_3_A7_T5_js() { RunFile(@"S7.3_A7_T5.js"); }
    [TestMethod] public void S7_3_A7_T6_js() { RunFile(@"S7.3_A7_T6.js"); }
    [TestMethod] public void S7_3_A7_T7_js() { RunFile(@"S7.3_A7_T7.js"); }
    [TestMethod] public void S7_3_A7_T8_js() { RunFile(@"S7.3_A7_T8.js"); }
  }
}