using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Sputnik {
  public class BaseTest {
    static string baseDir;
    static BaseTest() {
      System.IO.Directory.SetCurrentDirectory(@"..\..\..\Tests\Sputnik\js");
      baseDir = System.IO.Directory.GetCurrentDirectory();
    }

    void Error(string error) {
      Assert.Fail(error);
    }

    protected void SetSputnikDir(string subDir) {
      System.IO.Directory.SetCurrentDirectory(baseDir + "\\" + subDir);
    }

    static string header = "\r\n#################################################################\r\n";

    protected IronJS.Hosting.Context Ctx {
      get {
        var ctx = IronJS.Hosting.Context.Create();
        var error = new Action<string>(Error);
        var errorFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, error);
        var astPrinter = new Action<string>(s => Console.WriteLine(header + "AST:\r\n\r\n" + s.Replace("\n", "\r\n")));
        var exprPrinter = new Action<string>(s => Console.WriteLine(header + "EXPR:\r\n\r\n" + s));

         IronJS.Support.Debug.registerAstPrinter(astPrinter);
        IronJS.Support.Debug.registerExprPrinter(exprPrinter);

        ctx.PutGlobal("ERROR", errorFunc);
        ctx.PutGlobal("$ERROR", errorFunc);
        return ctx;
      }
    }

    protected void RunFile(string file) {
      Console.WriteLine(System.IO.Directory.GetCurrentDirectory() + "\\" + file);
      Console.WriteLine(header + "JAVASCRIPT:\r\n\r\n" + System.IO.File.ReadAllText(file));
      var _ = Ctx.ExecuteFile(file);
    }

    protected void RunFile_ExpectException<T>(string file) where T : Exception {
      var thrown = false;
      try {
        RunFile(file);
      } catch (T) {
        thrown = true;
      } finally {
        if (!thrown) {
          Assert.Fail("Expected exception from file " + file);
        }
      }
    }

    protected void RunFile_ExpectException(string file) {
      RunFile_ExpectException<Exception>(file);
    }
  }
}

