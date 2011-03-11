using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_11_Error_Objects_15_11_1_The_Error_Constructor_Called_as_a_Function : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.11_Error_Objects\15.11.1_The_Error_Constructor_Called_as_a_Function"); }
    [TestMethod] public void S15_11_1_1_A1_T1_js() { RunFile(@"S15.11.1.1_A1_T1.js"); }
    [TestMethod] public void S15_11_1_1_A2_T1_js() { RunFile(@"S15.11.1.1_A2_T1.js"); }
    [TestMethod] public void S15_11_1_1_A3_T1_js() { RunFile(@"S15.11.1.1_A3_T1.js"); }
    [TestMethod] public void S15_11_1_A1_T1_js() { RunFile(@"S15.11.1_A1_T1.js"); }
  }
}