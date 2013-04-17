using System;
using System.Text.RegularExpressions;
using nhsevidence.ScreeningScheduling.Core;
using System.Text;
using System.Reflection;
using System.IO;

namespace nhsevidence.ScreeningScheduling
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string arguments = string.Join (" ", args);

			if (arguments.Contains ("--help")) {
				PrintHelp ();
				Environment.Exit (0);
			}

			var container = new Container ();
			bool foundTasks = false;

			var tasksMatch = Regex.Match (arguments, @"--tasks ([a-zA-Z0-9,]+)", RegexOptions.IgnoreCase);
			if (tasksMatch.Success) {
				Console.WriteLine (string.Concat ("Tasks: ", tasksMatch.Groups[1].Value));
				var tasks = tasksMatch.Groups[1].Value.Split (',');
				foreach (var task in tasks) {
					container.AddTask (task.Trim ());
				}
				foundTasks = true;
			}

			if (!foundTasks) {
				PrintHelp();
				Environment.Exit (1);
			}

			var depsMatch = Regex.Match (arguments, @"--deps ([a-zA-Z0-9,=>]+)", RegexOptions.IgnoreCase);
			if (depsMatch.Success) {
				Console.WriteLine (string.Concat ("Dependencies: ", depsMatch.Groups[1].Value));
				var deps = depsMatch.Groups[1].Value.Split (',');
				foreach (var dep in deps) {
					var splitDepsMatch = Regex.Match (dep, @"([a-zA-Z0-9]+)=>([a-zA-Z0-9]+)", RegexOptions.IgnoreCase);
					if (splitDepsMatch.Groups.Count != 3)
						continue;
					container.AddDependencies (splitDepsMatch.Groups[1].Value, splitDepsMatch.Groups[2].Value);
				}
			}

			StringBuilder sb = new StringBuilder ();
			var resolved = container.Resolve ();
			for (int i = 0; i < resolved.Count; i++) {
				sb.Append (resolved[i].Name);
				if (i + 1 < resolved.Count)
					sb.Append (",");
			}

			Console.WriteLine (string.Concat ("Result: ", sb));
		}

		static void PrintHelp ()
		{
			using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("nhsevidence.ScreeningScheduling.help.txt")) {
				using (StreamReader reader = new StreamReader (stream)) {
					Console.Write (reader.ReadToEnd ());
				}
			}
		}
	}
}
