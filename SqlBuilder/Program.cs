
using System;
using System.IO;

using A2v10.Data.Generator;

namespace SqlBuilder;

public class Program
{
	static void Main(String[] args)
	{
		if (args == null || args.Length == 0)
		{
			Console.WriteLine("usage: sqlbuilder <input file>");
			return;
		}
		String fileName = Path.GetFullPath(args[0]);
		if (!File.Exists(fileName))
		{
			Console.WriteLine($"file not found: {fileName}");
			return;
		}

		try
		{
			var sb = new SolutionBuilder();
			var mb = new ModelBuilder();
			sb.BuildSolution(fileName, mb);
			Console.WriteLine(mb.ToString());
		} catch (Exception ex)
		{
			if (ex.InnerException != null)
				ex = ex.InnerException;
			Console.WriteLine(ex.Message);
		}
		Console.ReadKey();
	}
}
