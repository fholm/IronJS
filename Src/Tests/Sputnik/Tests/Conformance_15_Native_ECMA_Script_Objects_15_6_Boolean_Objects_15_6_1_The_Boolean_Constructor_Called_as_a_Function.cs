using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_6_Boolean_Objects_15_6_1_The_Boolean_Constructor_Called_as_a_Function : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.6_Boolean_Objects\15.6.1_The_Boolean_Constructor_Called_as_a_Function"); }
    [TestMethod] public void S15_6_1_1_A1_T1_js() { RunFile(@"S15.6.1.1_A1_T1.js"); }
    [TestMethod] public void S15_6_1_1_A1_T2_js() { RunFile(@"S15.6.1.1_A1_T2.js"); }
    [TestMethod] public void S15_6_1_1_A1_T3_js() { RunFile(@"S15.6.1.1_A1_T3.js"); }
    [TestMethod] public void S15_6_1_1_A1_T4_js() { RunFile(@"S15.6.1.1_A1_T4.js"); }
    [TestMethod] public void S15_6_1_1_A1_T5_js() { RunFile(@"S15.6.1.1_A1_T5.js"); }
    [TestMethod] public void S15_6_1_1_A2_js() { RunFile(@"S15.6.1.1_A2.js"); }
  }
}