using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_3_Properties_of_Number_Constructor_15_7_3_3_Number_MIN_VALUE : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.3_Properties_of_Number_Constructor\15.7.3.3_Number.MIN_VALUE"); }
    [TestMethod] public void S15_7_3_3_A1_js() { RunFile(@"S15.7.3.3_A1.js"); }
    [TestMethod] public void S15_7_3_3_A2_js() { RunFile(@"S15.7.3.3_A2.js"); }
    [TestMethod] public void S15_7_3_3_A3_js() { RunFile(@"S15.7.3.3_A3.js"); }
    [TestMethod] public void S15_7_3_3_A4_js() { RunFile(@"S15.7.3.3_A4.js"); }
  }
}