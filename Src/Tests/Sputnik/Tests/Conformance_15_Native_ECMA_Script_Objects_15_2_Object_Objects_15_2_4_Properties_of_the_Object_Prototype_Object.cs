using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_2_Object_Objects_15_2_4_Properties_of_the_Object_Prototype_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.4_Properties_of_the_Object_Prototype_Object"); }
    [TestMethod] public void S15_2_4_1_A1_T1_js() { RunFile(@"S15.2.4.1_A1_T1.js"); }
    [TestMethod] public void S15_2_4_1_A1_T2_js() { RunFile(@"S15.2.4.1_A1_T2.js"); }
    [TestMethod] public void S15_2_4_A1_T1_js() { RunFile(@"S15.2.4_A1_T1.js"); }
    [TestMethod] public void S15_2_4_A1_T2_js() { RunFile(@"S15.2.4_A1_T2.js"); }
    [TestMethod] public void S15_2_4_A2_js() { RunFile(@"S15.2.4_A2.js"); }
    [TestMethod] public void S15_2_4_A3_js() { RunFile(@"S15.2.4_A3.js"); }
    [TestMethod] public void S15_2_4_A4_js() { RunFile(@"S15.2.4_A4.js"); }
  }
}