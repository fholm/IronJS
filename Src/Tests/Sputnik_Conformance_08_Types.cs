using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
  [TestClass]
  public class Sputnik_Conformance_08_Types : Sputnik {
    [TestMethod]
    public void Conformance_08_Types_8_1_The_Undefined_Type_S8_1_A1_T1_js() {
      Test(@"Conformance\08_Types\8.1_The_Undefined_Type", () => { RunFile("S8.1_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_1_The_Undefined_Type_S8_1_A1_T2_js() {
      Test(@"Conformance\08_Types\8.1_The_Undefined_Type", () => { RunFile("S8.1_A1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_1_The_Undefined_Type_S8_1_A2_T1_js() {
      Test(@"Conformance\08_Types\8.1_The_Undefined_Type", () => { RunFile("S8.1_A2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_1_The_Undefined_Type_S8_1_A2_T2_js() {
      Test(@"Conformance\08_Types\8.1_The_Undefined_Type", () => { RunFile("S8.1_A2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_1_The_Undefined_Type_S8_1_A3_js() {
      Test(@"Conformance\08_Types\8.1_The_Undefined_Type", () => { RunFile("S8.1_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_1_The_Undefined_Type_S8_1_A4_js() {
      Test(@"Conformance\08_Types\8.1_The_Undefined_Type", () => { RunFile("S8.1_A4.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_1_The_Undefined_Type_S8_1_A5_js() {
      Test(@"Conformance\08_Types\8.1_The_Undefined_Type", () => { RunFile("S8.1_A5.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_2_The_Null_Type_S8_2_A1_T1_js() {
      Test(@"Conformance\08_Types\8.2_The_Null_Type", () => { RunFile("S8.2_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_2_The_Null_Type_S8_2_A1_T2_js() {
      Test(@"Conformance\08_Types\8.2_The_Null_Type", () => { RunFile("S8.2_A1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_2_The_Null_Type_S8_2_A2_js() {
      Test(@"Conformance\08_Types\8.2_The_Null_Type", () => { RunFile_ExpectException("S8.2_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_2_The_Null_Type_S8_2_A3_js() {
      Test(@"Conformance\08_Types\8.2_The_Null_Type", () => { RunFile("S8.2_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_3_The_Boolean_Type_S8_3_A1_T1_js() {
      Test(@"Conformance\08_Types\8.3_The_Boolean_Type", () => { RunFile("S8.3_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_3_The_Boolean_Type_S8_3_A1_T2_js() {
      Test(@"Conformance\08_Types\8.3_The_Boolean_Type", () => { RunFile("S8.3_A1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_3_The_Boolean_Type_S8_3_A2_1_js() {
      Test(@"Conformance\08_Types\8.3_The_Boolean_Type", () => { RunFile_ExpectException("S8.3_A2.1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_3_The_Boolean_Type_S8_3_A2_2_js() {
      Test(@"Conformance\08_Types\8.3_The_Boolean_Type", () => { RunFile_ExpectException("S8.3_A2.2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_3_The_Boolean_Type_S8_3_A3_js() {
      Test(@"Conformance\08_Types\8.3_The_Boolean_Type", () => { RunFile("S8.3_A3.js"); });
    }


    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A1_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A10_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A10.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A11_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A11.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A12_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A12.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A13_T1_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A13_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A13_T2_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A13_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A13_T3_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A13_T3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A14_T1_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A14_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A14_T2_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A14_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A14_T3_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A14_T3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A2_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A3_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A4_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A4.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A5_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A5.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A6_1_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A6.1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A6_2_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A6.2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A7_1_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A7.1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A7_2_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A7.2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A7_3_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A7.3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A7_4_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A7.4.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A8_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A8.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A9_T1_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A9_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A9_T2_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A9_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_4_The_String_Type_S8_4_A9_T3_js() {
      Test(@"Conformance\08_Types\8.4_The_String_Type", () => { RunFile("S8.4_A9_T3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A1_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A10_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A10.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A11_T1_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A11_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A11_T2_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A11_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A12_1_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A12.1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A12_2_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A12.2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A13_T1_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A13_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A13_T2_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A13_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A14_T1_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A14_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A14_T2_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A14_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A2_1_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A2.1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A2_2_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A2.2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A3_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A4_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A4.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A5_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A5.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A6_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A6.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A7_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A7.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A8_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A8.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_5_The_Number_Type_S8_5_A9_js() {
      Test(@"Conformance\08_Types\8.5_The_Number_Type", () => { RunFile("S8.5_A9.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_S8_6_A2_T1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type", () => { RunFile("S8.6_A2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_S8_6_A2_T2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type", () => { RunFile("S8.6_A2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_S8_6_A3_T1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type", () => { RunFile("S8.6_A3_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_S8_6_A3_T2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type", () => { RunFile("S8.6_A3_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_S8_6_A4_T1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type", () => { RunFile("S8.6_A4_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_1_Property_Attributes_S8_6_1_A1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.1_Property_Attributes", () => { RunFile("S8.6.1_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_1_Property_Attributes_S8_6_1_A2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.1_Property_Attributes", () => { RunFile("S8.6.1_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_1_Property_Attributes_S8_6_1_A3_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.1_Property_Attributes", () => { RunFile("S8.6.1_A3.js"); });
    }


    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_1_A1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.1_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_1_A2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.1_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_1_A3_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.1_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_2_A1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.2_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_2_A2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.2_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_3_A1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.3_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_4_A1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.4_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_4_A2_T1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.4_A2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_4_A2_T2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.4_A2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_4_A3_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.4_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_5_A1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.5_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_5_A2_T1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.5_A2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_5_A2_T2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.5_A2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_5_A3_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.5_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_6_A1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.6_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_6_A2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.6_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_6_A3_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.6_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_6_A4_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2.6_A4.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A3_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A4_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A4.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A5_T1_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A5_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A5_T2_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A5_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A5_T3_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A5_T3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A5_T4_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A5_T4.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A6_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A6.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_6_The_Object_Type_8_6_2_Internal_Properties_and_Methods_S8_6_2_A7_js() {
      Test(@"Conformance\08_Types\8.6_The_Object_Type\8.6.2_Internal_Properties_and_Methods", () => { RunFile("S8.6.2_A7.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_1_A1_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7.1_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_1_A2_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7.1_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_2_A1_T1_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7.2_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_2_A1_T2_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7.2_A1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_2_A2_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7.2_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_2_A3_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7.2_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_A1_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7_A1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_A2_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7_A2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_A3_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7_A3.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_A4_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7_A4.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_A5_T1_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7_A5_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_A5_T2_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7_A5_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_A6_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7_A6.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_7_The_Reference_Type_S8_7_A7_js() {
      Test(@"Conformance\08_Types\8.7_The_Reference_Type", () => { RunFile("S8.7_A7.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_8_The_List_Type_S8_8_A2_T1_js() {
      Test(@"Conformance\08_Types\8.8_The_List_Type", () => { RunFile("S8.8_A2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_8_The_List_Type_S8_8_A2_T2_js() {
      Test(@"Conformance\08_Types\8.8_The_List_Type", () => { RunFile("S8.8_A2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_08_Types_8_8_The_List_Type_S8_8_A2_T3_js() {
      Test(@"Conformance\08_Types\8.8_The_List_Type", () => { RunFile("S8.8_A2_T3.js"); });
    }
  }
}

