using System;

using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;

namespace ScriptBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var iDbContext = Starter.Create();

            IDataModel dm = iDbContext.LoadModel(null, "a2test.[SimpleModel.Load]");

            var scripter = new VueScriptBuilder();
            String script = dm.CreateScript(scripter);
            Console.WriteLine(script);
        }
    }
}
