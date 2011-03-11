using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_4_Properties_of_the_Number_Prototype_Object_15_7_4_4_Number_prototype_valueOf : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.4_Properties_of_the_Number_Prototype_Object\15.7.4.4_Number.prototype.valueOf"); }
    [TestMethod] public void S15_7_4_4_A1_T01_js() { RunFile(@"S15.7.4.4_A1_T01.js"); }
    [TestMethod] public void S15_7_4_4_A1_T02_js() { RunFile(@"S15.7.4.4_A1_T02.js"); }
    [TestMethod] public void S15_7_4_4_A2_T01_js() { RunFile(@"S15.7.4.4_A2_T01.js"); }
    [TestMethod] public void S15_7_4_4_A2_T02_js() { RunFile(@"S15.7.4.4_A2_T02.js"); }
    [TestMethod] public void S15_7_4_4_A2_T03_js() { RunFile(@"S15.7.4.4_A2_T03.js"); }
    [TestMethod] public void S15_7_4_4_A2_T04_js() { RunFile(@"S15.7.4.4_A2_T04.js"); }
    [TestMethod] public void S15_7_4_4_A2_T05_js() { RunFile(@"S15.7.4.4_A2_T05.js"); }
  }
}