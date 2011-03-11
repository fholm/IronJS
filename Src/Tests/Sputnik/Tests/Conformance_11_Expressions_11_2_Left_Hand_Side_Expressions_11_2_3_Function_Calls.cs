using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_2_Left_Hand_Side_Expressions_11_2_3_Function_Calls : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.2_Left_Hand_Side_Expressions\11.2.3_Function_Calls"); }
    [TestMethod] public void S11_2_3_A1_js() { RunFile(@"S11.2.3_A1.js"); }
    [TestMethod] public void S11_2_3_A2_js() { RunFile(@"S11.2.3_A2.js"); }
    [TestMethod] public void S11_2_3_A3_T1_js() { RunFile(@"S11.2.3_A3_T1.js"); }
    [TestMethod] public void S11_2_3_A3_T2_js() { RunFile(@"S11.2.3_A3_T2.js"); }
    [TestMethod] public void S11_2_3_A3_T3_js() { RunFile(@"S11.2.3_A3_T3.js"); }
    [TestMethod] public void S11_2_3_A3_T4_js() { RunFile(@"S11.2.3_A3_T4.js"); }
    [TestMethod] public void S11_2_3_A3_T5_js() { RunFile(@"S11.2.3_A3_T5.js"); }
    [TestMethod] public void S11_2_3_A4_T1_js() { RunFile(@"S11.2.3_A4_T1.js"); }
    [TestMethod] public void S11_2_3_A4_T2_js() { RunFile(@"S11.2.3_A4_T2.js"); }
    [TestMethod] public void S11_2_3_A4_T3_js() { RunFile(@"S11.2.3_A4_T3.js"); }
    [TestMethod] public void S11_2_3_A4_T4_js() { RunFile(@"S11.2.3_A4_T4.js"); }
    [TestMethod] public void S11_2_3_A4_T5_js() { RunFile(@"S11.2.3_A4_T5.js"); }
  }
}