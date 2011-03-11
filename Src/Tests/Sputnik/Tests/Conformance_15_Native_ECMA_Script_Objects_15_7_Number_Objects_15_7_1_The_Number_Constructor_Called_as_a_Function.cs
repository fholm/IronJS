using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_1_The_Number_Constructor_Called_as_a_Function : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.1_The_Number_Constructor_Called_as_a_Function"); }
    [TestMethod] public void S15_7_1_1_A1_js() { RunFile(@"S15.7.1.1_A1.js"); }
    [TestMethod] public void S15_7_1_1_A2_js() { RunFile(@"S15.7.1.1_A2.js"); }
  }
}