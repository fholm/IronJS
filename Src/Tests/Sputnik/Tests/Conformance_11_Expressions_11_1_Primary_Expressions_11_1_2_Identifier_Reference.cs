using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_1_Primary_Expressions_11_1_2_Identifier_Reference : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.1_Primary_Expressions\11.1.2_Identifier_Reference"); }
    [TestMethod] public void S11_1_2_A1_T1_js() { RunFile(@"S11.1.2_A1_T1.js"); }
    [TestMethod] public void S11_1_2_A1_T2_js() { RunFile(@"S11.1.2_A1_T2.js"); }
  }
}