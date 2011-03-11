using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_6_The_Object_Type_8_6_1_Property_Attributes : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.6_The_Object_Type\8.6.1_Property_Attributes"); }
    [TestMethod] public void S8_6_1_A1_js() { RunFile(@"S8.6.1_A1.js"); }
    [TestMethod] public void S8_6_1_A2_js() { RunFile(@"S8.6.1_A2.js"); }
    [TestMethod] public void S8_6_1_A3_js() { RunFile(@"S8.6.1_A3.js"); }
  }
}