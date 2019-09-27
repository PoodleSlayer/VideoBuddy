using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoBuddy.Utility
{
	public class ProcessRunner
	{
		/// <summary>
		/// Used for running a Process async
		/// </summary>
		/// <param name="processPath">The path to the command line process to run</param>
		/// <param name="processArguments">The arguments to pass to the command line process</param>
		/// <returns></returns>
		public static Task<int> RunProcessAsync(string processPath, string processArguments)
		{
			// Need to revisit this idea since I won't be able to get the output events in
			// my ViewModels for writing the log at the bottom...
			var tcs = new TaskCompletionSource<int>();

			var process = new Process
			{
				StartInfo =
				{
					FileName = processPath,
					Arguments = processArguments,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
				},
				EnableRaisingEvents = true
			};

			process.Exited += (sender, args) =>
			{
				tcs.SetResult(process.ExitCode);
				process.Dispose();
			};

			process.Start();

			return tcs.Task;
		}

		/// <summary>
		/// Returns a Process with the proper StartInfo settings to silently run the Command Prompt
		/// </summary>
		/// <param name="processPath">The path to the command line process to run</param>
		/// <param name="processArguments">The arguments to pass to the command line process</param>
		/// <returns></returns>
		public static Process CreateProcess(string processPath, string processArguments)
		{
			Process process = new Process
			{
				StartInfo =
				{
					FileName = processPath,
					Arguments = processArguments,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
				},
			};

			return process;
		}
	}
}
