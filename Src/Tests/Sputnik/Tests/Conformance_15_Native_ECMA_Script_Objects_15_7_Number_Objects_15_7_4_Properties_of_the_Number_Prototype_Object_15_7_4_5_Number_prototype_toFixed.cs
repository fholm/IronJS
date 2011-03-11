using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_4_Properties_of_the_Number_Prototype_Object_15_7_4_5_Number_prototype_toFixed : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.4_Properties_of_the_Number_Prototype_Object\15.7.4.5_Number.prototype.toFixed"); }
    [TestMethod] public void S15_7_4_5_A1_1_T01_js() { RunFile(@"S15.7.4.5_A1.1_T01.js"); }
    [TestMethod] public void S15_7_4_5_A1_1_T02_js() { RunFile(@"S15.7.4.5_A1.1_T02.js"); }
    [TestMethod] public void S15_7_4_5_A1_3_T01_js() { RunFile(@"S15.7.4.5_A1.3_T01.js"); }
    [TestMethod] public void S15_7_4_5_A1_3_T02_js() { RunFile(@"S15.7.4.5_A1.3_T02.js"); }
    [TestMethod] public void S15_7_4_5_A1_4_T01_js() { RunFile(@"S15.7.4.5_A1.4_T01.js"); }
    [TestMethod] public void S15_7_4_5_A2_T01_js() { RunFile(@"S15.7.4.5_A2_T01.js"); }
  }
}