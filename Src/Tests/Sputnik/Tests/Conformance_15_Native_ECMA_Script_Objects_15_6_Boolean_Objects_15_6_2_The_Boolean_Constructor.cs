using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_6_Boolean_Objects_15_6_2_The_Boolean_Constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.6_Boolean_Objects\15.6.2_The_Boolean_Constructor"); }
    [TestMethod] public void S15_6_2_1_A1_js() { RunFile(@"S15.6.2.1_A1.js"); }
    [TestMethod] public void S15_6_2_1_A2_js() { RunFile(@"S15.6.2.1_A2.js"); }
    [TestMethod] public void S15_6_2_1_A3_js() { RunFile(@"S15.6.2.1_A3.js"); }
    [TestMethod] public void S15_6_2_1_A4_js() { RunFile(@"S15.6.2.1_A4.js"); }
  }
}