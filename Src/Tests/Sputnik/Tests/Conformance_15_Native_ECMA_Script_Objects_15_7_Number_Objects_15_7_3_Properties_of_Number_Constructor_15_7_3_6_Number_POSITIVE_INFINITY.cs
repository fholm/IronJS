using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_3_Properties_of_Number_Constructor_15_7_3_6_Number_POSITIVE_INFINITY : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.3_Properties_of_Number_Constructor\15.7.3.6_Number.POSITIVE_INFINITY"); }
    [TestMethod] public void S15_7_3_6_A1_js() { RunFile(@"S15.7.3.6_A1.js"); }
    [TestMethod] public void S15_7_3_6_A2_js() { RunFile(@"S15.7.3.6_A2.js"); }
    [TestMethod] public void S15_7_3_6_A3_js() { RunFile(@"S15.7.3.6_A3.js"); }
    [TestMethod] public void S15_7_3_6_A4_js() { RunFile(@"S15.7.3.6_A4.js"); }
  }
}