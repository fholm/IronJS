using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_2_White_Space : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.2_White_Space"); }
    [TestMethod] public void S7_2_A1_1_T1_js() { RunFile(@"S7.2_A1.1_T1.js"); }
    [TestMethod] public void S7_2_A1_1_T2_js() { RunFile(@"S7.2_A1.1_T2.js"); }
    [TestMethod] public void S7_2_A1_2_T1_js() { RunFile(@"S7.2_A1.2_T1.js"); }
    [TestMethod] public void S7_2_A1_2_T2_js() { RunFile(@"S7.2_A1.2_T2.js"); }
    [TestMethod] public void S7_2_A1_3_T1_js() { RunFile(@"S7.2_A1.3_T1.js"); }
    [TestMethod] public void S7_2_A1_3_T2_js() { RunFile(@"S7.2_A1.3_T2.js"); }
    [TestMethod] public void S7_2_A1_4_T1_js() { RunFile(@"S7.2_A1.4_T1.js"); }
    [TestMethod] public void S7_2_A1_4_T2_js() { RunFile(@"S7.2_A1.4_T2.js"); }
    [TestMethod] public void S7_2_A1_5_T1_js() { RunFile(@"S7.2_A1.5_T1.js"); }
    [TestMethod] public void S7_2_A1_5_T2_js() { RunFile(@"S7.2_A1.5_T2.js"); }
    [TestMethod] public void S7_2_A2_1_T1_js() { RunFile(@"S7.2_A2.1_T1.js"); }
    [TestMethod] public void S7_2_A2_1_T2_js() { RunFile(@"S7.2_A2.1_T2.js"); }
    [TestMethod] public void S7_2_A2_2_T1_js() { RunFile(@"S7.2_A2.2_T1.js"); }
    [TestMethod] public void S7_2_A2_2_T2_js() { RunFile(@"S7.2_A2.2_T2.js"); }
    [TestMethod] public void S7_2_A2_3_T1_js() { RunFile(@"S7.2_A2.3_T1.js"); }
    [TestMethod] public void S7_2_A2_3_T2_js() { RunFile(@"S7.2_A2.3_T2.js"); }
    [TestMethod] public void S7_2_A2_4_T1_js() { RunFile(@"S7.2_A2.4_T1.js"); }
    [TestMethod] public void S7_2_A2_4_T2_js() { RunFile(@"S7.2_A2.4_T2.js"); }
    [TestMethod] public void S7_2_A2_5_T1_js() { RunFile(@"S7.2_A2.5_T1.js"); }
    [TestMethod] public void S7_2_A2_5_T2_js() { RunFile(@"S7.2_A2.5_T2.js"); }
    [TestMethod] public void S7_2_A3_1_T1_js() { RunFile(@"S7.2_A3.1_T1.js"); }
    [TestMethod] public void S7_2_A3_1_T2_js() { RunFile(@"S7.2_A3.1_T2.js"); }
    [TestMethod] public void S7_2_A3_2_T1_js() { RunFile(@"S7.2_A3.2_T1.js"); }
    [TestMethod] public void S7_2_A3_2_T2_js() { RunFile(@"S7.2_A3.2_T2.js"); }
    [TestMethod] public void S7_2_A3_3_T1_js() { RunFile(@"S7.2_A3.3_T1.js"); }
    [TestMethod] public void S7_2_A3_3_T2_js() { RunFile(@"S7.2_A3.3_T2.js"); }
    [TestMethod] public void S7_2_A3_4_T1_js() { RunFile(@"S7.2_A3.4_T1.js"); }
    [TestMethod] public void S7_2_A3_4_T2_js() { RunFile(@"S7.2_A3.4_T2.js"); }
    [TestMethod] public void S7_2_A3_5_T1_js() { RunFile(@"S7.2_A3.5_T1.js"); }
    [TestMethod] public void S7_2_A3_5_T2_js() { RunFile(@"S7.2_A3.5_T2.js"); }
    [TestMethod] public void S7_2_A4_1_T1_js() { RunFile(@"S7.2_A4.1_T1.js"); }
    [TestMethod] public void S7_2_A4_1_T2_js() { RunFile(@"S7.2_A4.1_T2.js"); }
    [TestMethod] public void S7_2_A4_2_T1_js() { RunFile(@"S7.2_A4.2_T1.js"); }
    [TestMethod] public void S7_2_A4_2_T2_js() { RunFile(@"S7.2_A4.2_T2.js"); }
    [TestMethod] public void S7_2_A4_3_T1_js() { RunFile(@"S7.2_A4.3_T1.js"); }
    [TestMethod] public void S7_2_A4_3_T2_js() { RunFile(@"S7.2_A4.3_T2.js"); }
    [TestMethod] public void S7_2_A4_4_T1_js() { RunFile(@"S7.2_A4.4_T1.js"); }
    [TestMethod] public void S7_2_A4_4_T2_js() { RunFile(@"S7.2_A4.4_T2.js"); }
    [TestMethod] public void S7_2_A4_5_T1_js() { RunFile(@"S7.2_A4.5_T1.js"); }
    [TestMethod] public void S7_2_A4_5_T2_js() { RunFile(@"S7.2_A4.5_T2.js"); }
    [TestMethod] public void S7_2_A5_T1_js() { RunFile(@"S7.2_A5_T1.js"); }
    [TestMethod] public void S7_2_A5_T2_js() { RunFile(@"S7.2_A5_T2.js"); }
    [TestMethod] public void S7_2_A5_T3_js() { RunFile(@"S7.2_A5_T3.js"); }
    [TestMethod] public void S7_2_A5_T4_js() { RunFile(@"S7.2_A5_T4.js"); }
    [TestMethod] public void S7_2_A5_T5_js() { RunFile(@"S7.2_A5_T5.js"); }
  }
}