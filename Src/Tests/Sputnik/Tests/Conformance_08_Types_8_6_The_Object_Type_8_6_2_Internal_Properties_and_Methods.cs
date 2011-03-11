using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods"); }
    [TestMethod] public void S8_6_2_1_A1_js() { RunFile(@"S8.6.2.1_A1.js"); }
    [TestMethod] public void S8_6_2_1_A2_js() { RunFile(@"S8.6.2.1_A2.js"); }
    [TestMethod] public void S8_6_2_1_A3_js() { RunFile(@"S8.6.2.1_A3.js"); }
    [TestMethod] public void S8_6_2_2_A1_js() { RunFile(@"S8.6.2.2_A1.js"); }
    [TestMethod] public void S8_6_2_2_A2_js() { RunFile(@"S8.6.2.2_A2.js"); }
    [TestMethod] public void S8_6_2_3_A1_js() { RunFile(@"S8.6.2.3_A1.js"); }
    [TestMethod] public void S8_6_2_4_A1_js() { RunFile(@"S8.6.2.4_A1.js"); }
    [TestMethod] public void S8_6_2_4_A2_T1_js() { RunFile(@"S8.6.2.4_A2_T1.js"); }
    [TestMethod] public void S8_6_2_4_A2_T2_js() { RunFile(@"S8.6.2.4_A2_T2.js"); }
    [TestMethod] public void S8_6_2_4_A3_js() { RunFile(@"S8.6.2.4_A3.js"); }
    [TestMethod] public void S8_6_2_5_A1_js() { RunFile(@"S8.6.2.5_A1.js"); }
    [TestMethod] public void S8_6_2_5_A2_T1_js() { RunFile(@"S8.6.2.5_A2_T1.js"); }
    [TestMethod] public void S8_6_2_5_A2_T2_js() { RunFile(@"S8.6.2.5_A2_T2.js"); }
    [TestMethod] public void S8_6_2_5_A3_js() { RunFile(@"S8.6.2.5_A3.js"); }
    [TestMethod] public void S8_6_2_6_A1_js() { RunFile(@"S8.6.2.6_A1.js"); }
    [TestMethod] public void S8_6_2_6_A2_js() { RunFile(@"S8.6.2.6_A2.js"); }
    [TestMethod] public void S8_6_2_6_A3_js() { RunFile(@"S8.6.2.6_A3.js"); }
    [TestMethod] public void S8_6_2_6_A4_js() { RunFile(@"S8.6.2.6_A4.js"); }
    [TestMethod] public void S8_6_2_A1_js() { RunFile(@"S8.6.2_A1.js"); }
    [TestMethod] public void S8_6_2_A2_js() { RunFile(@"S8.6.2_A2.js"); }
    [TestMethod] public void S8_6_2_A3_js() { RunFile(@"S8.6.2_A3.js"); }
    [TestMethod] public void S8_6_2_A4_js() { RunFile(@"S8.6.2_A4.js"); }
    [TestMethod] public void S8_6_2_A5_T1_js() { RunFile(@"S8.6.2_A5_T1.js"); }
    [TestMethod] public void S8_6_2_A5_T2_js() { RunFile(@"S8.6.2_A5_T2.js"); }
    [TestMethod] public void S8_6_2_A5_T3_js() { RunFile(@"S8.6.2_A5_T3.js"); }
    [TestMethod] public void S8_6_2_A5_T4_js() { RunFile(@"S8.6.2_A5_T4.js"); }
    [TestMethod] public void S8_6_2_A6_js() { RunFile(@"S8.6.2_A6.js"); }
    [TestMethod] public void S8_6_2_A7_js() { RunFile(@"S8.6.2_A7.js"); }
  }
}