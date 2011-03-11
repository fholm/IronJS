using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_1_The_Global_Object_15_1_1_Value_Properties_of_the_Global_Object_15_1_1_2_Infinity : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.1_The_Global_Object\15.1.1_Value_Properties_of_the_Global_Object\15.1.1.2_Infinity"); }
    [TestMethod] public void S15_1_1_2_A1_js() { RunFile(@"S15.1.1.2_A1.js"); }
    [TestMethod] public void S15_1_1_2_A2_T1_js() { RunFile(@"S15.1.1.2_A2_T1.js"); }
    [TestMethod] public void S15_1_1_2_A2_T2_js() { RunFile(@"S15.1.1.2_A2_T2.js"); }
    [TestMethod] public void S15_1_1_2_A3_1_js() { RunFile(@"S15.1.1.2_A3.1.js"); }
    [TestMethod] public void S15_1_1_2_A3_2_js() { RunFile(@"S15.1.1.2_A3.2.js"); }
  }
}