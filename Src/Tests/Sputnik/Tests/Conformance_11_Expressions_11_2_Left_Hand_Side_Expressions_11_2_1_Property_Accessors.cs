using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_2_Left_Hand_Side_Expressions_11_2_1_Property_Accessors : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.2_Left_Hand_Side_Expressions\11.2.1_Property_Accessors"); }
    [TestMethod] public void S11_2_1_A1_1_js() { RunFile(@"S11.2.1_A1.1.js"); }
    [TestMethod] public void S11_2_1_A1_2_js() { RunFile(@"S11.2.1_A1.2.js"); }
    [TestMethod] public void S11_2_1_A2_js() { RunFile(@"S11.2.1_A2.js"); }
    [TestMethod] public void S11_2_1_A3_T1_js() { RunFile(@"S11.2.1_A3_T1.js"); }
    [TestMethod] public void S11_2_1_A3_T2_js() { RunFile(@"S11.2.1_A3_T2.js"); }
    [TestMethod] public void S11_2_1_A3_T3_js() { RunFile(@"S11.2.1_A3_T3.js"); }
    [TestMethod] public void S11_2_1_A3_T4_js() { RunFile(@"S11.2.1_A3_T4.js"); }
    [TestMethod] public void S11_2_1_A3_T5_js() { RunFile(@"S11.2.1_A3_T5.js"); }
    [TestMethod] public void S11_2_1_A4_T1_js() { RunFile(@"S11.2.1_A4_T1.js"); }
    [TestMethod] public void S11_2_1_A4_T2_js() { RunFile(@"S11.2.1_A4_T2.js"); }
    [TestMethod] public void S11_2_1_A4_T3_js() { RunFile(@"S11.2.1_A4_T3.js"); }
    [TestMethod] public void S11_2_1_A4_T4_js() { RunFile(@"S11.2.1_A4_T4.js"); }
    [TestMethod] public void S11_2_1_A4_T5_js() { RunFile(@"S11.2.1_A4_T5.js"); }
    [TestMethod] public void S11_2_1_A4_T6_js() { RunFile(@"S11.2.1_A4_T6.js"); }
    [TestMethod] public void S11_2_1_A4_T7_js() { RunFile(@"S11.2.1_A4_T7.js"); }
    [TestMethod] public void S11_2_1_A4_T8_js() { RunFile(@"S11.2.1_A4_T8.js"); }
    [TestMethod] public void S11_2_1_A4_T9_js() { RunFile(@"S11.2.1_A4_T9.js"); }
  }
}