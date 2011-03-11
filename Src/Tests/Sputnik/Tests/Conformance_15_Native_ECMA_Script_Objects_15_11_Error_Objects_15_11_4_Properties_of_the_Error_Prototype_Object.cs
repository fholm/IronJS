using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_11_Error_Objects_15_11_4_Properties_of_the_Error_Prototype_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.11_Error_Objects\15.11.4_Properties_of_the_Error_Prototype_Object"); }
    [TestMethod] public void S15_11_4_1_A1_T1_js() { RunFile(@"S15.11.4.1_A1_T1.js"); }
    [TestMethod] public void S15_11_4_1_A1_T2_js() { RunFile(@"S15.11.4.1_A1_T2.js"); }
    [TestMethod] public void S15_11_4_2_A1_js() { RunFile(@"S15.11.4.2_A1.js"); }
    [TestMethod] public void S15_11_4_2_A2_js() { RunFile(@"S15.11.4.2_A2.js"); }
    [TestMethod] public void S15_11_4_3_A1_js() { RunFile(@"S15.11.4.3_A1.js"); }
    [TestMethod] public void S15_11_4_3_A2_js() { RunFile(@"S15.11.4.3_A2.js"); }
    [TestMethod] public void S15_11_4_4_A1_js() { RunFile(@"S15.11.4.4_A1.js"); }
    [TestMethod] public void S15_11_4_4_A2_js() { RunFile(@"S15.11.4.4_A2.js"); }
    [TestMethod] public void S15_11_4_A1_js() { RunFile(@"S15.11.4_A1.js"); }
    [TestMethod] public void S15_11_4_A2_js() { RunFile(@"S15.11.4_A2.js"); }
    [TestMethod] public void S15_11_4_A3_js() { RunFile(@"S15.11.4_A3.js"); }
    [TestMethod] public void S15_11_4_A4_js() { RunFile(@"S15.11.4_A4.js"); }
  }
}