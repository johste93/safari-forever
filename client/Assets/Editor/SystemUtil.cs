using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chumpware
{
	public static class SystemUtil
	{
		public static void Execute(string file, string[] args, string workingDirectory = null)
		{
			string AllArgs = string.Join(" ", args.Select(arg => ShellEscape(arg)).ToArray());
			string workingDirectoryDescription = workingDirectory == null ? "Current directory" : workingDirectory;
			Debug.Log("executing: " + file + " " + AllArgs + " from " + workingDirectoryDescription);

			using (var process = new System.Diagnostics.Process())
			{
				process.StartInfo.FileName = file;
				process.StartInfo.Arguments = AllArgs;
				if (workingDirectory != null)
					process.StartInfo.WorkingDirectory = workingDirectory;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.UseShellExecute = false;

				StringBuilder stdout = new StringBuilder();
				StringBuilder stderr = new StringBuilder();

				process.OutputDataReceived += (sender, e) =>
				{
					if (e.Data == null) return;
					stdout.Append(e.Data).Append("\n");
				};
				process.ErrorDataReceived += (sender, e) =>
				{
					if (e.Data == null) return;
					stderr.Append(e.Data).Append("\n");
				};
				process.EnableRaisingEvents = true;

				process.Start();

				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				process.WaitForExit();

				string output = stdout.ToString();
				if (output.Length > 0)
					Debug.Log("** output: \n" + output);
				string error = stderr.ToString();
				if (error.Length > 0)
					Debug.Log("** error: \n" + error);

				if (process.ExitCode != 0)
				{
					throw new Exception("Failed running " + file + " error: " + process.ExitCode + " : " + error);
				}
			}
		}

		private static string ShellEscape(string value)
		{
			if (value.Contains(' '))
				return "\"" + value + "\"";
			else
				return value;
		}
	}
}