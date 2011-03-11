using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_3_Function_Objects_15_3_4_Properties_of_the_Function_Prototype_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.3_Function_Objects\15.3.4_Properties_of_the_Function_Prototype_Object"); }
    [TestMethod] public void S15_3_4_1_A1_T1_js() { RunFile(@"S15.3.4.1_A1_T1.js"); }
    [TestMethod] public void S15_3_4_1_A1_T2_js() { RunFile(@"S15.3.4.1_A1_T2.js"); }
    [TestMethod] public void S15_3_4_A1_js() { RunFile(@"S15.3.4_A1.js"); }
    [TestMethod] public void S15_3_4_A2_T1_js() { RunFile(@"S15.3.4_A2_T1.js"); }
    [TestMethod] public void S15_3_4_A2_T2_js() { RunFile(@"S15.3.4_A2_T2.js"); }
    [TestMethod] public void S15_3_4_A2_T3_js() { RunFile(@"S15.3.4_A2_T3.js"); }
    [TestMethod] public void S15_3_4_A3_T1_js() { RunFile(@"S15.3.4_A3_T1.js"); }
    [TestMethod] public void S15_3_4_A3_T2_js() { RunFile(@"S15.3.4_A3_T2.js"); }
    [TestMethod] public void S15_3_4_A4_js() { RunFile(@"S15.3.4_A4.js"); }
    [TestMethod] public void S15_3_4_A5_js() { RunFile(@"S15.3.4_A5.js"); }
  }
}