using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_6_Boolean_Objects_15_6_3_Properties_of_the_Boolean_Constructor_15_6_3_1_Boolean_prototype : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.6_Boolean_Objects\15.6.3_Properties_of_the_Boolean_Constructor\15.6.3.1_Boolean.prototype"); }
    [TestMethod] public void S15_6_3_1_A1_js() { RunFile(@"S15.6.3.1_A1.js"); }
    [TestMethod] public void S15_6_3_1_A2_js() { RunFile(@"S15.6.3.1_A2.js"); }
    [TestMethod] public void S15_6_3_1_A3_js() { RunFile(@"S15.6.3.1_A3.js"); }
    [TestMethod] public void S15_6_3_1_A4_js() { RunFile(@"S15.6.3.1_A4.js"); }
  }
}