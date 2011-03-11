using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_9_Date_Objects_15_9_2_The_Date_Constructor_Called_as_a_Function : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.9_Date_Objects\15.9.2_The_Date_Constructor_Called_as_a_Function"); }
    [TestMethod] public void S15_9_2_1_A1_js() { RunFile(@"S15.9.2.1_A1.js"); }
    [TestMethod] public void S15_9_2_1_A2_js() { RunFile(@"S15.9.2.1_A2.js"); }
  }
}