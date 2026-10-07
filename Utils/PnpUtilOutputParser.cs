using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PnpUtilGui.Models;

namespace PnpUtilGui.Utils
{
    internal class PnpUtilOutputParser
    {
        // Driver version lines look like "Driver Version:  08/21/2025 25.30.0.1".
        private static readonly Regex DriverVersionRegex =
            new Regex(@"\d{1,2}/\d{1,2}/\d{4}\s+\d+(\.\d+)+");

        public List<Driver> ParseEnumDriverOutput(IEnumerable<string> enumerable)
        {
            var driverList = new List<Driver>();

            foreach (var block in SplitIntoBlocks(enumerable ?? Enumerable.Empty<string>()))
            {
                var driver = ParseDriverBlock(block);

                if (driver != null)
                {
                    driverList.Add(driver);
                }
            }

            return driverList;
        }

        // Splits the pnputil output into blocks of non-empty lines. Unlike a
        // positional loop this is safe when the output does not end with an
        // empty line (newer Windows 11 builds do not).
        private static IEnumerable<List<string>> SplitIntoBlocks(IEnumerable<string> lines)
        {
            var block = new List<string>();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    if (block.Count > 0)
                    {
                        yield return block;
                        block = new List<string>();
                    }

                    continue;
                }

                block.Add(line);
            }

            if (block.Count > 0)
            {
                yield return block;
            }
        }

        private static Driver ParseDriverBlock(List<string> block)
        {
            // A driver block needs the five fixed fields plus a version line.
            // Anything else (e.g. the localized "Microsoft PnP Utility" title
            // line) is not a driver block and is skipped.
            if (block.Count < 6 ||
                !GetLineValue(block[0]).EndsWith(".inf", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var driver = new Driver
            {
                FileName = GetLineValue(block[0]),
                SourceName = GetLineValue(block[1]),
                Publisher = GetLineValue(block[2]),
                DriverClass = GetLineValue(block[3]),
                ClassGuid = GetLineValue(block[4]),
            };

            var remaining = block.Skip(5).ToList();
            var versionIndex = remaining.FindIndex(line => DriverVersionRegex.IsMatch(GetLineValue(line)));

            if (versionIndex < 0)
            {
                // Unexpected layout: fall back to the last two lines.
                if (remaining.Count >= 2)
                {
                    driver.DateAndVersion = GetLineValue(remaining[remaining.Count - 2]);
                    driver.CertificateSignerName = GetLineValue(remaining[remaining.Count - 1]);
                }
                else if (remaining.Count == 1)
                {
                    driver.DateAndVersion = GetLineValue(remaining[0]);
                }

                return driver;
            }

            // Some pnputil versions emit a "Class Version" line right before
            // the driver version line.
            if (versionIndex > 0 && !DriverVersionRegex.IsMatch(GetLineValue(remaining[versionIndex - 1])))
            {
                driver.ClassVersion = GetLineValue(remaining[versionIndex - 1]);
            }

            driver.DateAndVersion = GetLineValue(remaining[versionIndex]);

            // Newer pnputil versions append extra lines ("Attributes",
            // "WHCP Version") after the signer line; they are ignored.
            if (versionIndex + 1 < remaining.Count)
            {
                driver.CertificateSignerName = GetLineValue(remaining[versionIndex + 1]);
            }

            return driver;
        }

        private static string GetLineValue(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            var separatorIndex = line.IndexOf(':');

            return separatorIndex < 0 ? string.Empty : line.Substring(separatorIndex + 1).Trim();
        }
    }
}