using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_9_Date_Objects_15_9_4_Properties_of_the_Date_Constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.9_Date_Objects\15.9.4_Properties_of_the_Date_Constructor"); }
    [TestMethod] public void S15_9_4_A1_js() { RunFile(@"S15.9.4_A1.js"); }
    [TestMethod] public void S15_9_4_A2_js() { RunFile(@"S15.9.4_A2.js"); }
    [TestMethod] public void S15_9_4_A3_js() { RunFile(@"S15.9.4_A3.js"); }
    [TestMethod] public void S15_9_4_A4_js() { RunFile(@"S15.9.4_A4.js"); }
    [TestMethod] public void S15_9_4_A5_js() { RunFile(@"S15.9.4_A5.js"); }
  }
}