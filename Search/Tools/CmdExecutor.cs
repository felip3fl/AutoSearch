using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Search.Tools;

public class CmdExecutor
{
    private static void ExecuteCmdCommand(string command)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true; // Hides the CMD window

            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            process.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
            process.ErrorDataReceived += (sender, args) => error.AppendLine(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit(); // Wait for the process to exit

            Console.WriteLine("Output: " + output.ToString());
            Console.WriteLine("Error: " + error.ToString());
        }
    }

    public void ShutDownComputer(int timeInSeconds)
    {
        var timeInSecondsString = timeInSeconds;
        ExecuteCmdCommand($"shutdown -s -f -t {timeInSecondsString}");
    }

    public void StopShutDownComputer()
    {
        ExecuteCmdCommand($"shutdown -a");
    }
}
