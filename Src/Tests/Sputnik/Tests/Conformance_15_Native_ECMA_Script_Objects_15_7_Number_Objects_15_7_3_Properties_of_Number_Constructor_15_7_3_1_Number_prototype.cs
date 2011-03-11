using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_3_Properties_of_Number_Constructor_15_7_3_1_Number_prototype : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.3_Properties_of_Number_Constructor\15.7.3.1_Number.prototype"); }
    [TestMethod] public void S15_7_3_1_A1_T1_js() { RunFile(@"S15.7.3.1_A1_T1.js"); }
    [TestMethod] public void S15_7_3_1_A1_T2_js() { RunFile(@"S15.7.3.1_A1_T2.js"); }
    [TestMethod] public void S15_7_3_1_A1_T3_js() { RunFile(@"S15.7.3.1_A1_T3.js"); }
    [TestMethod] public void S15_7_3_1_A2_T1_js() { RunFile(@"S15.7.3.1_A2_T1.js"); }
    [TestMethod] public void S15_7_3_1_A2_T2_js() { RunFile(@"S15.7.3.1_A2_T2.js"); }
    [TestMethod] public void S15_7_3_1_A3_js() { RunFile(@"S15.7.3.1_A3.js"); }
  }
}