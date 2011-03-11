using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_8_Literals_7_8_5_Regular_Expression_Literals : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.8_Literals\7.8.5_Regular_Expression_Literals"); }
    [TestMethod] public void S7_8_5_A1_1_T1_js() { RunFile(@"S7.8.5_A1.1_T1.js"); }
    [TestMethod] public void S7_8_5_A1_1_T2_js() { RunFile(@"S7.8.5_A1.1_T2.js"); }
    [TestMethod] public void S7_8_5_A1_2_T1_js() { RunFile(@"S7.8.5_A1.2_T1.js"); }
    [TestMethod] public void S7_8_5_A1_2_T2_js() { RunFile(@"S7.8.5_A1.2_T2.js"); }
    [TestMethod] public void S7_8_5_A1_2_T3_js() { RunFile(@"S7.8.5_A1.2_T3.js"); }
    [TestMethod] public void S7_8_5_A1_2_T4_js() { RunFile(@"S7.8.5_A1.2_T4.js"); }
    [TestMethod] public void S7_8_5_A1_3_T1_js() { RunFile(@"S7.8.5_A1.3_T1.js"); }
    [TestMethod] public void S7_8_5_A1_3_T2_js() { RunFile(@"S7.8.5_A1.3_T2.js"); }
    [TestMethod] public void S7_8_5_A1_3_T3_js() { RunFile(@"S7.8.5_A1.3_T3.js"); }
    [TestMethod] public void S7_8_5_A1_3_T4_js() { RunFile(@"S7.8.5_A1.3_T4.js"); }
    [TestMethod] public void S7_8_5_A1_3_T5_js() { RunFile(@"S7.8.5_A1.3_T5.js"); }
    [TestMethod] public void S7_8_5_A1_3_T6_js() { RunFile(@"S7.8.5_A1.3_T6.js"); }
    [TestMethod] public void S7_8_5_A1_4_T1_js() { RunFile(@"S7.8.5_A1.4_T1.js"); }
    [TestMethod] public void S7_8_5_A1_4_T2_js() { RunFile(@"S7.8.5_A1.4_T2.js"); }
    [TestMethod] public void S7_8_5_A1_5_T1_js() { RunFile(@"S7.8.5_A1.5_T1.js"); }
    [TestMethod] public void S7_8_5_A1_5_T2_js() { RunFile(@"S7.8.5_A1.5_T2.js"); }
    [TestMethod] public void S7_8_5_A1_5_T3_js() { RunFile(@"S7.8.5_A1.5_T3.js"); }
    [TestMethod] public void S7_8_5_A1_5_T4_js() { RunFile(@"S7.8.5_A1.5_T4.js"); }
    [TestMethod] public void S7_8_5_A1_5_T5_js() { RunFile(@"S7.8.5_A1.5_T5.js"); }
    [TestMethod] public void S7_8_5_A1_5_T6_js() { RunFile(@"S7.8.5_A1.5_T6.js"); }
    [TestMethod] public void S7_8_5_A2_1_T1_js() { RunFile(@"S7.8.5_A2.1_T1.js"); }
    [TestMethod] public void S7_8_5_A2_1_T2_js() { RunFile(@"S7.8.5_A2.1_T2.js"); }
    [TestMethod] public void S7_8_5_A2_2_T1_js() { RunFile(@"S7.8.5_A2.2_T1.js"); }
    [TestMethod] public void S7_8_5_A2_2_T2_js() { RunFile(@"S7.8.5_A2.2_T2.js"); }
    [TestMethod] public void S7_8_5_A2_3_T1_js() { RunFile(@"S7.8.5_A2.3_T1.js"); }
    [TestMethod] public void S7_8_5_A2_3_T2_js() { RunFile(@"S7.8.5_A2.3_T2.js"); }
    [TestMethod] public void S7_8_5_A2_3_T3_js() { RunFile(@"S7.8.5_A2.3_T3.js"); }
    [TestMethod] public void S7_8_5_A2_3_T4_js() { RunFile(@"S7.8.5_A2.3_T4.js"); }
    [TestMethod] public void S7_8_5_A2_3_T5_js() { RunFile(@"S7.8.5_A2.3_T5.js"); }
    [TestMethod] public void S7_8_5_A2_3_T6_js() { RunFile(@"S7.8.5_A2.3_T6.js"); }
    [TestMethod] public void S7_8_5_A2_4_T1_js() { RunFile(@"S7.8.5_A2.4_T1.js"); }
    [TestMethod] public void S7_8_5_A2_4_T2_js() { RunFile(@"S7.8.5_A2.4_T2.js"); }
    [TestMethod] public void S7_8_5_A2_5_T1_js() { RunFile(@"S7.8.5_A2.5_T1.js"); }
    [TestMethod] public void S7_8_5_A2_5_T2_js() { RunFile(@"S7.8.5_A2.5_T2.js"); }
    [TestMethod] public void S7_8_5_A2_5_T3_js() { RunFile(@"S7.8.5_A2.5_T3.js"); }
    [TestMethod] public void S7_8_5_A2_5_T4_js() { RunFile(@"S7.8.5_A2.5_T4.js"); }
    [TestMethod] public void S7_8_5_A2_5_T5_js() { RunFile(@"S7.8.5_A2.5_T5.js"); }
    [TestMethod] public void S7_8_5_A2_5_T6_js() { RunFile(@"S7.8.5_A2.5_T6.js"); }
    [TestMethod] public void S7_8_5_A3_1_T1_js() { RunFile(@"S7.8.5_A3.1_T1.js"); }
    [TestMethod] public void S7_8_5_A3_1_T2_js() { RunFile(@"S7.8.5_A3.1_T2.js"); }
    [TestMethod] public void S7_8_5_A3_1_T3_js() { RunFile(@"S7.8.5_A3.1_T3.js"); }
    [TestMethod] public void S7_8_5_A3_1_T4_js() { RunFile(@"S7.8.5_A3.1_T4.js"); }
    [TestMethod] public void S7_8_5_A3_1_T5_js() { RunFile(@"S7.8.5_A3.1_T5.js"); }
    [TestMethod] public void S7_8_5_A3_1_T6_js() { RunFile(@"S7.8.5_A3.1_T6.js"); }
    [TestMethod] public void S7_8_5_A3_1_T7_js() { RunFile(@"S7.8.5_A3.1_T7.js"); }
    [TestMethod] public void S7_8_5_A3_1_T8_js() { RunFile(@"S7.8.5_A3.1_T8.js"); }
    [TestMethod] public void S7_8_5_A3_1_T9_js() { RunFile(@"S7.8.5_A3.1_T9.js"); }
    [TestMethod] public void S7_8_5_A4_1_js() { RunFile(@"S7.8.5_A4.1.js"); }
    [TestMethod] public void S7_8_5_A4_2_js() { RunFile(@"S7.8.5_A4.2.js"); }
  }
}