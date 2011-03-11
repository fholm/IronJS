using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_3_Function_Objects_15_3_3_Properties_of_the_Function_Constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.3_Function_Objects\15.3.3_Properties_of_the_Function_Constructor"); }
    [TestMethod] public void S15_3_3_A1_js() { RunFile(@"S15.3.3_A1.js"); }
    [TestMethod] public void S15_3_3_A2_T1_js() { RunFile(@"S15.3.3_A2_T1.js"); }
    [TestMethod] public void S15_3_3_A2_T2_js() { RunFile(@"S15.3.3_A2_T2.js"); }
    [TestMethod] public void S15_3_3_A3_js() { RunFile(@"S15.3.3_A3.js"); }
  }
}