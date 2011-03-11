using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_7_Bitwise_Shift_Operators_11_7_3_The_Unsigned_Right_Shift_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.7_Bitwise_Shift_Operators\11.7.3_The_Unsigned_Right_Shift_Operator"); }
    [TestMethod] public void S11_7_3_A1_js() { RunFile(@"S11.7.3_A1.js"); }
    [TestMethod] public void S11_7_3_A2_1_T1_js() { RunFile(@"S11.7.3_A2.1_T1.js"); }
    [TestMethod] public void S11_7_3_A2_1_T2_js() { RunFile(@"S11.7.3_A2.1_T2.js"); }
    [TestMethod] public void S11_7_3_A2_1_T3_js() { RunFile(@"S11.7.3_A2.1_T3.js"); }
    [TestMethod] public void S11_7_3_A2_2_T1_js() { RunFile(@"S11.7.3_A2.2_T1.js"); }
    [TestMethod] public void S11_7_3_A2_3_T1_js() { RunFile(@"S11.7.3_A2.3_T1.js"); }
    [TestMethod] public void S11_7_3_A2_4_T1_js() { RunFile(@"S11.7.3_A2.4_T1.js"); }
    [TestMethod] public void S11_7_3_A2_4_T2_js() { RunFile(@"S11.7.3_A2.4_T2.js"); }
    [TestMethod] public void S11_7_3_A2_4_T3_js() { RunFile(@"S11.7.3_A2.4_T3.js"); }
    [TestMethod] public void S11_7_3_A3_T1_1_js() { RunFile(@"S11.7.3_A3_T1.1.js"); }
    [TestMethod] public void S11_7_3_A3_T1_2_js() { RunFile(@"S11.7.3_A3_T1.2.js"); }
    [TestMethod] public void S11_7_3_A3_T1_3_js() { RunFile(@"S11.7.3_A3_T1.3.js"); }
    [TestMethod] public void S11_7_3_A3_T1_4_js() { RunFile(@"S11.7.3_A3_T1.4.js"); }
    [TestMethod] public void S11_7_3_A3_T1_5_js() { RunFile(@"S11.7.3_A3_T1.5.js"); }
    [TestMethod] public void S11_7_3_A3_T2_1_js() { RunFile(@"S11.7.3_A3_T2.1.js"); }
    [TestMethod] public void S11_7_3_A3_T2_2_js() { RunFile(@"S11.7.3_A3_T2.2.js"); }
    [TestMethod] public void S11_7_3_A3_T2_3_js() { RunFile(@"S11.7.3_A3_T2.3.js"); }
    [TestMethod] public void S11_7_3_A3_T2_4_js() { RunFile(@"S11.7.3_A3_T2.4.js"); }
    [TestMethod] public void S11_7_3_A3_T2_5_js() { RunFile(@"S11.7.3_A3_T2.5.js"); }
    [TestMethod] public void S11_7_3_A3_T2_6_js() { RunFile(@"S11.7.3_A3_T2.6.js"); }
    [TestMethod] public void S11_7_3_A3_T2_7_js() { RunFile(@"S11.7.3_A3_T2.7.js"); }
    [TestMethod] public void S11_7_3_A3_T2_8_js() { RunFile(@"S11.7.3_A3_T2.8.js"); }
    [TestMethod] public void S11_7_3_A3_T2_9_js() { RunFile(@"S11.7.3_A3_T2.9.js"); }
    [TestMethod] public void S11_7_3_A4_T1_js() { RunFile(@"S11.7.3_A4_T1.js"); }
    [TestMethod] public void S11_7_3_A4_T2_js() { RunFile(@"S11.7.3_A4_T2.js"); }
    [TestMethod] public void S11_7_3_A4_T3_js() { RunFile(@"S11.7.3_A4_T3.js"); }
    [TestMethod] public void S11_7_3_A4_T4_js() { RunFile(@"S11.7.3_A4_T4.js"); }
    [TestMethod] public void S11_7_3_A5_1_T1_js() { RunFile(@"S11.7.3_A5.1_T1.js"); }
    [TestMethod] public void S11_7_3_A5_2_T1_js() { RunFile(@"S11.7.3_A5.2_T1.js"); }
  }
}