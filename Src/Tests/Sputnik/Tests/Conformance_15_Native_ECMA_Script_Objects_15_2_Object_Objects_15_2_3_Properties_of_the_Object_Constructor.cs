using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_2_Object_Objects_15_2_3_Properties_of_the_Object_Constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.3_Properties_of_the_Object_Constructor"); }
    [TestMethod] public void S15_2_3_A1_js() { RunFile(@"S15.2.3_A1.js"); }
    [TestMethod] public void S15_2_3_A2_js() { RunFile(@"S15.2.3_A2.js"); }
    [TestMethod] public void S15_2_3_A3_js() { RunFile(@"S15.2.3_A3.js"); }
  }
}