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

    protected IronJS.Hosting.Context Ctx {
      get {
        var ctx = IronJS.Hosting.Context.Create();
        var error = new Action<string>(Error);
        var errorFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, error);
        ctx.PutGlobal("ERROR", errorFunc);
        ctx.PutGlobal("$ERROR", errorFunc);
        return ctx;
      }
    }

    protected void RunFile(string file) {
      var _ = Ctx.ExecuteFile(file);
    }

    protected void RunFile_ExpectException(string file) {
      try {
        var _ = Ctx.ExecuteFile(file);
        Assert.Fail("Expected exception from file " + file);
      } catch {

      }
    }
  }
}

