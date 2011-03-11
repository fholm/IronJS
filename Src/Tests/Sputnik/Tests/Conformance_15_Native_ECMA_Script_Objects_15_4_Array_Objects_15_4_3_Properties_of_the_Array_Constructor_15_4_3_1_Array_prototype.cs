using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_3_Properties_of_the_Array_Constructor_15_4_3_1_Array_prototype : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.3_Properties_of_the_Array_Constructor\15.4.3.1_Array_prototype"); }
    [TestMethod] public void S15_4_3_1_A1_js() { RunFile(@"S15.4.3.1_A1.js"); }
    [TestMethod] public void S15_4_3_1_A2_js() { RunFile(@"S15.4.3.1_A2.js"); }
    [TestMethod] public void S15_4_3_1_A3_js() { RunFile(@"S15.4.3.1_A3.js"); }
    [TestMethod] public void S15_4_3_1_A4_js() { RunFile(@"S15.4.3.1_A4.js"); }
    [TestMethod] public void S15_4_3_1_A5_js() { RunFile(@"S15.4.3.1_A5.js"); }
  }
}