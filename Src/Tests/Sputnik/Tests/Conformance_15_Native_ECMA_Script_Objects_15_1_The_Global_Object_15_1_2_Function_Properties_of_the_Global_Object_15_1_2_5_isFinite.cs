using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_1_The_Global_Object_15_1_2_Function_Properties_of_the_Global_Object_15_1_2_5_isFinite : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.1_The_Global_Object\15.1.2_Function_Properties_of_the_Global_Object\15.1.2.5_isFinite"); }
    [TestMethod] public void S15_1_2_5_A1_T1_js() { RunFile(@"S15.1.2.5_A1_T1.js"); }
    [TestMethod] public void S15_1_2_5_A1_T2_js() { RunFile(@"S15.1.2.5_A1_T2.js"); }
    [TestMethod] public void S15_1_2_5_A2_1_js() { RunFile(@"S15.1.2.5_A2.1.js"); }
    [TestMethod] public void S15_1_2_5_A2_2_js() { RunFile(@"S15.1.2.5_A2.2.js"); }
    [TestMethod] public void S15_1_2_5_A2_3_js() { RunFile(@"S15.1.2.5_A2.3.js"); }
    [TestMethod] public void S15_1_2_5_A2_4_js() { RunFile(@"S15.1.2.5_A2.4.js"); }
    [TestMethod] public void S15_1_2_5_A2_5_js() { RunFile(@"S15.1.2.5_A2.5.js"); }
    [TestMethod] public void S15_1_2_5_A2_6_js() { RunFile(@"S15.1.2.5_A2.6.js"); }
    [TestMethod] public void S15_1_2_5_A2_7_js() { RunFile(@"S15.1.2.5_A2.7.js"); }
  }
}