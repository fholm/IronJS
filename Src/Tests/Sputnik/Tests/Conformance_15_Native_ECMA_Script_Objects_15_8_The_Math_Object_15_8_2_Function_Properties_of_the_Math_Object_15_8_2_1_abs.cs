using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_8_The_Math_Object_15_8_2_Function_Properties_of_the_Math_Object_15_8_2_1_abs : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.8_The_Math_Object\15.8.2_Function_Properties_of_the_Math_Object\15.8.2.1_abs"); }
    [TestMethod] public void S15_8_2_1_A1_js() { RunFile(@"S15.8.2.1_A1.js"); }
    [TestMethod] public void S15_8_2_1_A2_js() { RunFile(@"S15.8.2.1_A2.js"); }
    [TestMethod] public void S15_8_2_1_A3_js() { RunFile(@"S15.8.2.1_A3.js"); }
  }
}