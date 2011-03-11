using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_1_Primary_Expressions_11_1_6_The_Grouping_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.1_Primary_Expressions\11.1.6_The_Grouping_Operator"); }
    [TestMethod] public void S11_1_6_A1_js() { RunFile(@"S11.1.6_A1.js"); }
    [TestMethod] public void S11_1_6_A2_js() { RunFile(@"S11.1.6_A2.js"); }
    [TestMethod] public void S11_1_6_A3_T1_js() { RunFile(@"S11.1.6_A3_T1.js"); }
    [TestMethod] public void S11_1_6_A3_T2_js() { RunFile(@"S11.1.6_A3_T2.js"); }
    [TestMethod] public void S11_1_6_A3_T3_js() { RunFile(@"S11.1.6_A3_T3.js"); }
    [TestMethod] public void S11_1_6_A3_T4_js() { RunFile(@"S11.1.6_A3_T4.js"); }
    [TestMethod] public void S11_1_6_A3_T5_js() { RunFile(@"S11.1.6_A3_T5.js"); }
    [TestMethod] public void S11_1_6_A3_T6_js() { RunFile(@"S11.1.6_A3_T6.js"); }
  }
}