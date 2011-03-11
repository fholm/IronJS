using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_1_The_Global_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.1_The_Global_Object"); }
    [TestMethod] public void S15_1_A1_T1_js() { RunFile(@"S15.1_A1_T1.js"); }
    [TestMethod] public void S15_1_A1_T2_js() { RunFile(@"S15.1_A1_T2.js"); }
    [TestMethod] public void S15_1_A2_T1_js() { RunFile(@"S15.1_A2_T1.js"); }
  }
}