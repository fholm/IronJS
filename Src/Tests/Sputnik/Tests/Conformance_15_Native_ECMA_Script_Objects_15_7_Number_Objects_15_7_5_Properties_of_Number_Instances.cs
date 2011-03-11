using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_5_Properties_of_Number_Instances : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.5_Properties_of_Number_Instances"); }
    [TestMethod] public void S15_7_5_A1_T01_js() { RunFile(@"S15.7.5_A1_T01.js"); }
    [TestMethod] public void S15_7_5_A1_T02_js() { RunFile(@"S15.7.5_A1_T02.js"); }
    [TestMethod] public void S15_7_5_A1_T03_js() { RunFile(@"S15.7.5_A1_T03.js"); }
    [TestMethod] public void S15_7_5_A1_T04_js() { RunFile(@"S15.7.5_A1_T04.js"); }
    [TestMethod] public void S15_7_5_A1_T05_js() { RunFile(@"S15.7.5_A1_T05.js"); }
    [TestMethod] public void S15_7_5_A1_T06_js() { RunFile(@"S15.7.5_A1_T06.js"); }
    [TestMethod] public void S15_7_5_A1_T07_js() { RunFile(@"S15.7.5_A1_T07.js"); }
  }
}