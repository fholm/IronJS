using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_4_Properties_of_the_Number_Prototype_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.4_Properties_of_the_Number_Prototype_Object"); }
    [TestMethod] public void S15_7_4_A1_js() { RunFile(@"S15.7.4_A1.js"); }
    [TestMethod] public void S15_7_4_A2_js() { RunFile(@"S15.7.4_A2.js"); }
    [TestMethod] public void S15_7_4_A3_1_js() { RunFile(@"S15.7.4_A3.1.js"); }
    [TestMethod] public void S15_7_4_A3_2_js() { RunFile(@"S15.7.4_A3.2.js"); }
    [TestMethod] public void S15_7_4_A3_3_js() { RunFile(@"S15.7.4_A3.3.js"); }
    [TestMethod] public void S15_7_4_A3_4_js() { RunFile(@"S15.7.4_A3.4.js"); }
    [TestMethod] public void S15_7_4_A3_5_js() { RunFile(@"S15.7.4_A3.5.js"); }
    [TestMethod] public void S15_7_4_A3_6_js() { RunFile(@"S15.7.4_A3.6.js"); }
    [TestMethod] public void S15_7_4_A3_7_js() { RunFile(@"S15.7.4_A3.7.js"); }
  }
}