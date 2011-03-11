using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_3_Properties_of_the_Array_Constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.3_Properties_of_the_Array_Constructor"); }
    [TestMethod] public void S15_4_3_A1_1_T1_js() { RunFile(@"S15.4.3_A1.1_T1.js"); }
    [TestMethod] public void S15_4_3_A1_1_T2_js() { RunFile(@"S15.4.3_A1.1_T2.js"); }
    [TestMethod] public void S15_4_3_A1_1_T3_js() { RunFile(@"S15.4.3_A1.1_T3.js"); }
    [TestMethod] public void S15_4_3_A2_1_js() { RunFile(@"S15.4.3_A2.1.js"); }
    [TestMethod] public void S15_4_3_A2_2_js() { RunFile(@"S15.4.3_A2.2.js"); }
    [TestMethod] public void S15_4_3_A2_3_js() { RunFile(@"S15.4.3_A2.3.js"); }
    [TestMethod] public void S15_4_3_A2_4_js() { RunFile(@"S15.4.3_A2.4.js"); }
  }
}