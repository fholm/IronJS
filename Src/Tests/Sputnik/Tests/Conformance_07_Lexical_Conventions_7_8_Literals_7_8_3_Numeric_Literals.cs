using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_8_Literals_7_8_3_Numeric_Literals : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.8_Literals\7.8.3_Numeric_Literals"); }
    [TestMethod] public void S7_8_3_A1_1_T1_js() { RunFile(@"S7.8.3_A1.1_T1.js"); }
    [TestMethod] public void S7_8_3_A1_1_T2_js() { RunFile(@"S7.8.3_A1.1_T2.js"); }
    [TestMethod] public void S7_8_3_A1_2_T1_js() { RunFile(@"S7.8.3_A1.2_T1.js"); }
    [TestMethod] public void S7_8_3_A1_2_T2_js() { RunFile(@"S7.8.3_A1.2_T2.js"); }
    [TestMethod] public void S7_8_3_A1_2_T3_js() { RunFile(@"S7.8.3_A1.2_T3.js"); }
    [TestMethod] public void S7_8_3_A1_2_T4_js() { RunFile(@"S7.8.3_A1.2_T4.js"); }
    [TestMethod] public void S7_8_3_A1_2_T5_js() { RunFile(@"S7.8.3_A1.2_T5.js"); }
    [TestMethod] public void S7_8_3_A1_2_T6_js() { RunFile(@"S7.8.3_A1.2_T6.js"); }
    [TestMethod] public void S7_8_3_A1_2_T7_js() { RunFile(@"S7.8.3_A1.2_T7.js"); }
    [TestMethod] public void S7_8_3_A1_2_T8_js() { RunFile(@"S7.8.3_A1.2_T8.js"); }
    [TestMethod] public void S7_8_3_A2_1_T1_js() { RunFile(@"S7.8.3_A2.1_T1.js"); }
    [TestMethod] public void S7_8_3_A2_1_T2_js() { RunFile(@"S7.8.3_A2.1_T2.js"); }
    [TestMethod] public void S7_8_3_A2_1_T3_js() { RunFile(@"S7.8.3_A2.1_T3.js"); }
    [TestMethod] public void S7_8_3_A2_2_T1_js() { RunFile(@"S7.8.3_A2.2_T1.js"); }
    [TestMethod] public void S7_8_3_A2_2_T2_js() { RunFile(@"S7.8.3_A2.2_T2.js"); }
    [TestMethod] public void S7_8_3_A2_2_T3_js() { RunFile(@"S7.8.3_A2.2_T3.js"); }
    [TestMethod] public void S7_8_3_A2_2_T4_js() { RunFile(@"S7.8.3_A2.2_T4.js"); }
    [TestMethod] public void S7_8_3_A2_2_T5_js() { RunFile(@"S7.8.3_A2.2_T5.js"); }
    [TestMethod] public void S7_8_3_A2_2_T6_js() { RunFile(@"S7.8.3_A2.2_T6.js"); }
    [TestMethod] public void S7_8_3_A2_2_T7_js() { RunFile(@"S7.8.3_A2.2_T7.js"); }
    [TestMethod] public void S7_8_3_A2_2_T8_js() { RunFile(@"S7.8.3_A2.2_T8.js"); }
    [TestMethod] public void S7_8_3_A3_1_T1_js() { RunFile(@"S7.8.3_A3.1_T1.js"); }
    [TestMethod] public void S7_8_3_A3_1_T2_js() { RunFile(@"S7.8.3_A3.1_T2.js"); }
    [TestMethod] public void S7_8_3_A3_2_T1_js() { RunFile(@"S7.8.3_A3.2_T1.js"); }
    [TestMethod] public void S7_8_3_A3_2_T2_js() { RunFile(@"S7.8.3_A3.2_T2.js"); }
    [TestMethod] public void S7_8_3_A3_2_T3_js() { RunFile(@"S7.8.3_A3.2_T3.js"); }
    [TestMethod] public void S7_8_3_A3_3_T1_js() { RunFile(@"S7.8.3_A3.3_T1.js"); }
    [TestMethod] public void S7_8_3_A3_3_T2_js() { RunFile(@"S7.8.3_A3.3_T2.js"); }
    [TestMethod] public void S7_8_3_A3_3_T3_js() { RunFile(@"S7.8.3_A3.3_T3.js"); }
    [TestMethod] public void S7_8_3_A3_3_T4_js() { RunFile(@"S7.8.3_A3.3_T4.js"); }
    [TestMethod] public void S7_8_3_A3_3_T5_js() { RunFile(@"S7.8.3_A3.3_T5.js"); }
    [TestMethod] public void S7_8_3_A3_3_T6_js() { RunFile(@"S7.8.3_A3.3_T6.js"); }
    [TestMethod] public void S7_8_3_A3_3_T7_js() { RunFile(@"S7.8.3_A3.3_T7.js"); }
    [TestMethod] public void S7_8_3_A3_3_T8_js() { RunFile(@"S7.8.3_A3.3_T8.js"); }
    [TestMethod] public void S7_8_3_A3_4_T1_js() { RunFile(@"S7.8.3_A3.4_T1.js"); }
    [TestMethod] public void S7_8_3_A3_4_T2_js() { RunFile(@"S7.8.3_A3.4_T2.js"); }
    [TestMethod] public void S7_8_3_A3_4_T3_js() { RunFile(@"S7.8.3_A3.4_T3.js"); }
    [TestMethod] public void S7_8_3_A3_4_T4_js() { RunFile(@"S7.8.3_A3.4_T4.js"); }
    [TestMethod] public void S7_8_3_A3_4_T5_js() { RunFile(@"S7.8.3_A3.4_T5.js"); }
    [TestMethod] public void S7_8_3_A3_4_T6_js() { RunFile(@"S7.8.3_A3.4_T6.js"); }
    [TestMethod] public void S7_8_3_A3_4_T7_js() { RunFile(@"S7.8.3_A3.4_T7.js"); }
    [TestMethod] public void S7_8_3_A3_4_T8_js() { RunFile(@"S7.8.3_A3.4_T8.js"); }
    [TestMethod] public void S7_8_3_A4_1_T1_js() { RunFile(@"S7.8.3_A4.1_T1.js"); }
    [TestMethod] public void S7_8_3_A4_1_T2_js() { RunFile(@"S7.8.3_A4.1_T2.js"); }
    [TestMethod] public void S7_8_3_A4_1_T3_js() { RunFile(@"S7.8.3_A4.1_T3.js"); }
    [TestMethod] public void S7_8_3_A4_1_T4_js() { RunFile(@"S7.8.3_A4.1_T4.js"); }
    [TestMethod] public void S7_8_3_A4_1_T5_js() { RunFile(@"S7.8.3_A4.1_T5.js"); }
    [TestMethod] public void S7_8_3_A4_1_T6_js() { RunFile(@"S7.8.3_A4.1_T6.js"); }
    [TestMethod] public void S7_8_3_A4_1_T7_js() { RunFile(@"S7.8.3_A4.1_T7.js"); }
    [TestMethod] public void S7_8_3_A4_1_T8_js() { RunFile(@"S7.8.3_A4.1_T8.js"); }
    [TestMethod] public void S7_8_3_A4_2_T1_js() { RunFile(@"S7.8.3_A4.2_T1.js"); }
    [TestMethod] public void S7_8_3_A4_2_T2_js() { RunFile(@"S7.8.3_A4.2_T2.js"); }
    [TestMethod] public void S7_8_3_A4_2_T3_js() { RunFile(@"S7.8.3_A4.2_T3.js"); }
    [TestMethod] public void S7_8_3_A4_2_T4_js() { RunFile(@"S7.8.3_A4.2_T4.js"); }
    [TestMethod] public void S7_8_3_A4_2_T5_js() { RunFile(@"S7.8.3_A4.2_T5.js"); }
    [TestMethod] public void S7_8_3_A4_2_T6_js() { RunFile(@"S7.8.3_A4.2_T6.js"); }
    [TestMethod] public void S7_8_3_A4_2_T7_js() { RunFile(@"S7.8.3_A4.2_T7.js"); }
    [TestMethod] public void S7_8_3_A4_2_T8_js() { RunFile(@"S7.8.3_A4.2_T8.js"); }
    [TestMethod] public void S7_8_3_A5_1_T1_js() { RunFile(@"S7.8.3_A5.1_T1.js"); }
    [TestMethod] public void S7_8_3_A5_1_T2_js() { RunFile(@"S7.8.3_A5.1_T2.js"); }
    [TestMethod] public void S7_8_3_A5_1_T3_js() { RunFile(@"S7.8.3_A5.1_T3.js"); }
    [TestMethod] public void S7_8_3_A5_1_T4_js() { RunFile(@"S7.8.3_A5.1_T4.js"); }
    [TestMethod] public void S7_8_3_A5_1_T5_js() { RunFile(@"S7.8.3_A5.1_T5.js"); }
    [TestMethod] public void S7_8_3_A5_1_T6_js() { RunFile(@"S7.8.3_A5.1_T6.js"); }
    [TestMethod] public void S7_8_3_A5_1_T7_js() { RunFile(@"S7.8.3_A5.1_T7.js"); }
    [TestMethod] public void S7_8_3_A5_1_T8_js() { RunFile(@"S7.8.3_A5.1_T8.js"); }
    [TestMethod] public void S7_8_3_A6_1_T1_js() { RunFile(@"S7.8.3_A6.1_T1.js"); }
    [TestMethod] public void S7_8_3_A6_1_T2_js() { RunFile(@"S7.8.3_A6.1_T2.js"); }
    [TestMethod] public void S7_8_3_A6_2_T1_js() { RunFile(@"S7.8.3_A6.2_T1.js"); }
    [TestMethod] public void S7_8_3_A6_2_T2_js() { RunFile(@"S7.8.3_A6.2_T2.js"); }
  }
}