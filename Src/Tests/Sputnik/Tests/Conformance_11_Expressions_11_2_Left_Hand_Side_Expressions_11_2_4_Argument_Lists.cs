using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_2_Left_Hand_Side_Expressions_11_2_4_Argument_Lists : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.2_Left_Hand_Side_Expressions\11.2.4_Argument_Lists"); }
    [TestMethod] public void S11_2_4_A1_1_T1_js() { RunFile(@"S11.2.4_A1.1_T1.js"); }
    [TestMethod] public void S11_2_4_A1_1_T2_js() { RunFile(@"S11.2.4_A1.1_T2.js"); }
    [TestMethod] public void S11_2_4_A1_2_T1_js() { RunFile(@"S11.2.4_A1.2_T1.js"); }
    [TestMethod] public void S11_2_4_A1_2_T2_js() { RunFile(@"S11.2.4_A1.2_T2.js"); }
    [TestMethod] public void S11_2_4_A1_3_T1_js() { RunFile(@"S11.2.4_A1.3_T1.js"); }
    [TestMethod] public void S11_2_4_A1_4_T1_js() { RunFile(@"S11.2.4_A1.4_T1.js"); }
    [TestMethod] public void S11_2_4_A1_4_T2_js() { RunFile(@"S11.2.4_A1.4_T2.js"); }
    [TestMethod] public void S11_2_4_A1_4_T3_js() { RunFile(@"S11.2.4_A1.4_T3.js"); }
    [TestMethod] public void S11_2_4_A1_4_T4_js() { RunFile(@"S11.2.4_A1.4_T4.js"); }
  }
}