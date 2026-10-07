using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PnpUtilGui.Models;
using PnpUtilGui.Properties;

namespace PnpUtilGui.Utils
{
    internal class PnpUtilHelper
    {
        private static readonly PnpUtilOutputParser Parser = new PnpUtilOutputParser();

        public static Task<List<Driver>> EnumDrivers()
        {
            return Task.Run(() =>
            {
                var output = ExecutePnpUtil(new[] { "/enum-drivers" });

                return Parser.ParseEnumDriverOutput(output.StandardOutput);
            });
        }

        public static Task<string> DeleteDriver(string fileName, bool force = false)
        {
            return Task.Run(() =>
            {
                var output = ExecutePnpUtil(new[] { "/delete-driver", fileName, force ? "/force" : null });

                return FormatPnpUtilOutput(output);
            });
        }

        public static Task<string> ExportDriver(string fileName, string targetDir)
        {
            return Task.Run(() =>
            {
                var output = ExecutePnpUtil(new[] { "/export-driver", fileName, targetDir });

                return FormatPnpUtilOutput(output);
            });
        }

        private static string FormatPnpUtilOutput(PnpUtilResult output)
        {
            // The first two lines of pnputil output are the "Microsoft PnP Utility"
            // title and an empty separator line.
            var lines = output.StandardOutput.Skip(2).ToList();

            if (!string.IsNullOrWhiteSpace(output.StandardError))
            {
                lines.Add(output.StandardError.TrimEnd());
            }

            if (output.ExitCode != 0)
            {
                lines.Add(string.Format(Resources.PnpUtilHelper_ExitedWithCode, output.ExitCode));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static PnpUtilResult ExecutePnpUtil(string[] args)
        {
            var pnputilPath = Path.Combine(Environment.SystemDirectory, "pnputil.exe");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = File.Exists(pnputilPath) ? pnputilPath : "pnputil.exe",
                    Arguments = BuildArguments(args),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            using (proc)
            {
                // Drain the error stream in the background to avoid pipe deadlocks.
                var errorTask = proc.StandardError.ReadToEndAsync();

                var list = new List<string>();
                string line;
                while ((line = proc.StandardOutput.ReadLine()) != null)
                {
                    list.Add(line);
                }

                proc.WaitForExit();

                return new PnpUtilResult
                {
                    StandardOutput = list,
                    StandardError = errorTask.Result,
                    ExitCode = proc.ExitCode
                };
            }
        }

        private static string BuildArguments(IEnumerable<string> args)
        {
            return string.Join(
                " ",
                args.Where(arg => !string.IsNullOrWhiteSpace(arg)).Select(QuoteArgument));
        }

        // pnputil splits its command line on spaces, so every argument that contains
        // spaces (e.g. a OneDrive desktop like "C:\Users\<user>\OneDrive\Área de Trabalho\ddd")
        // must be quoted. Without quotes pnputil receives several broken arguments,
        // prints its usage screen and exits with code 1 without exporting anything.
        private static string QuoteArgument(string arg)
        {
            if (arg.Contains(' ') || arg.Contains('\t') || arg.Contains('"'))
            {
                return "\"" + arg.Replace("\"", "\\\"") + "\"";
            }

            return arg;
        }

        private sealed class PnpUtilResult
        {
            public List<string> StandardOutput { get; set; }

            public string StandardError { get; set; }

            public int ExitCode { get; set; }
        }
    }
}
