using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_1_Primary_Expressions_11_1_5_Object_Initializer : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.1_Primary_Expressions\11.1.5_Object_Initializer"); }
    [TestMethod] public void S11_1_5_A1_1_js() { RunFile(@"S11.1.5_A1.1.js"); }
    [TestMethod] public void S11_1_5_A1_2_js() { RunFile(@"S11.1.5_A1.2.js"); }
    [TestMethod] public void S11_1_5_A1_3_js() { RunFile(@"S11.1.5_A1.3.js"); }
    [TestMethod] public void S11_1_5_A1_4_js() { RunFile(@"S11.1.5_A1.4.js"); }
    [TestMethod] public void S11_1_5_A2_js() { RunFile(@"S11.1.5_A2.js"); }
    [TestMethod] public void S11_1_5_A3_js() { RunFile(@"S11.1.5_A3.js"); }
    [TestMethod] public void S11_1_5_A4_1_js() { RunFile(@"S11.1.5_A4.1.js"); }
    [TestMethod] public void S11_1_5_A4_2_js() { RunFile(@"S11.1.5_A4.2.js"); }
    [TestMethod] public void S11_1_5_A4_3_js() { RunFile(@"S11.1.5_A4.3.js"); }
  }
}