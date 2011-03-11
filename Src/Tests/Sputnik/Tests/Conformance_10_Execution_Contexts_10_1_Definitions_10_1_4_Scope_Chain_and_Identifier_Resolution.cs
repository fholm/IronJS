using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_10_Execution_Contexts_10_1_Definitions_10_1_4_Scope_Chain_and_Identifier_Resolution : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\10_Execution_Contexts\10.1_Definitions\10.1.4_Scope_Chain_and_Identifier_Resolution"); }
    [TestMethod] public void S10_1_4_A1_T1_js() { RunFile(@"S10.1.4_A1_T1.js"); }
    [TestMethod] public void S10_1_4_A1_T2_js() { RunFile(@"S10.1.4_A1_T2.js"); }
    [TestMethod] public void S10_1_4_A1_T3_js() { RunFile(@"S10.1.4_A1_T3.js"); }
    [TestMethod] public void S10_1_4_A1_T4_js() { RunFile(@"S10.1.4_A1_T4.js"); }
    [TestMethod] public void S10_1_4_A1_T5_js() { RunFile(@"S10.1.4_A1_T5.js"); }
    [TestMethod] public void S10_1_4_A1_T6_js() { RunFile(@"S10.1.4_A1_T6.js"); }
    [TestMethod] public void S10_1_4_A1_T7_js() { RunFile(@"S10.1.4_A1_T7.js"); }
    [TestMethod] public void S10_1_4_A1_T8_js() { RunFile(@"S10.1.4_A1_T8.js"); }
    [TestMethod] public void S10_1_4_A1_T9_js() { RunFile(@"S10.1.4_A1_T9.js"); }
  }
}