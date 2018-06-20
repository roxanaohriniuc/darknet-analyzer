using darknet_analyzer.Models;
using darknet_analyzer.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace darknet_analyzer.cli
{
    public class CliRunner
    {
        private readonly ProbeInformationService probeInformationService;
        private readonly PcapFileService pcapFileService;

        public CliRunner()
        {
            var dbConnectionString = ConfigurationManager.ConnectionStrings["darknet-analyzer"].ConnectionString;
            this.probeInformationService = new ProbeInformationService(dbConnectionString);
            this.pcapFileService = new PcapFileService(dbConnectionString);
        }

        public void Run()
        {
            Console.WriteLine("-- DarkNet-Analyzer --");
            do
            {
                Console.WriteLine("Enter Command. (h) Help");
                Console.Write(">");
            }
            while (this.HandleInput() || this.HandleInput());
        }

        private bool HandleInput()
        {
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.Write(">");
                return false;
            }

            var inputQueue = new Queue<string>(input.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            this.ProcessCommand(inputQueue.Dequeue(), inputQueue.ToArray());

            return true;
        }

        private void ProcessCommand(string command, params string[] parameters)
        {
            Console.WriteLine();
            switch(command.ToUpperInvariant())
            {
                case "H":
                case "HELP":
                    this.WriteMenu();
                    break;
                case "1":
                case "L":
                case "LOAD":
                    this.LoadPcapFile(parameters);
                    break;
                case "2":
                case "A":
                case "ANALYZE":
                    this.pcapFileService.AnalyzeNewFiles();
                    break;
                case "3":
                case "Q":
                case "QUERY":
                    this.QueryProbes(parameters);
                    break;
                default:
                    Console.WriteLine("Unsupported command.");
                    break;
            }

            Console.WriteLine();
        }

        private void WriteMenu()
        {
            Console.WriteLine(string.Join(
                Environment.NewLine,
                "Available Commands:",
                "  (h)\tHelp",
                "  (l)\tLoad PCAP File",
                "  (a)\tAnalyze Packets",
                "  (q)\tQuery Probes",
                "Press ENTER twice to exit program."
                ));
        }

        private void LoadPcapFile(string[] parameters)
        {
            if(parameters.Length < 1)
            {
                Console.WriteLine("Expected command syntax: l <filepath>");
                return;
            }

            var path = parameters[0];
            this.pcapFileService.LoadFileOrDirectory(path);
        }

        private void QueryProbes(string[] parameters)
        {
            var scanType = ScanType.NotScan;
            var comparison = "<>";
            var top = 100;

            if (parameters.Length > 0)
            {
                // 1st parameter is scan type
                if (!this.TryParseScanType(parameters[0], ref scanType))
                {
                    return;
                }

                // if scan type provided, default comparison to equal
                comparison = "=";
            }

            if(parameters.Length > 1)
            {
                // 2nd parameter is comparison (= or <>)
                if(!this.TryParseComparison(parameters[1], ref comparison))
                {
                    return;
                }
            }

            if(parameters.Length > 2)
            {
                if(!int.TryParse(parameters[2], out top))
                {
                    Console.WriteLine("Invalid top. Use an integer.");
                    return;
                }
            }

            var probes = this.probeInformationService.GetProbes(scanType, comparison, top);
            Console.WriteLine(string.Join(Environment.NewLine, probes.Select(p => p.ToDescription())));
        }

        private bool TryParseScanType(string input, ref ScanType scanType)
        {
            switch(input.ToUpperInvariant())
            {
                case "V":
                case "VERTICAL":
                    scanType = ScanType.Vertical;
                    break;
                case "H":
                case "HORIZONTAL":
                    scanType = ScanType.Horizontal;
                    break;
                case "S":
                case "STROBE":
                    scanType = ScanType.Strobe;
                    break;
                case "N":
                case "NOTSCAN":
                    scanType = ScanType.NotScan;
                    break;
                default:
                    Console.WriteLine("Invalid scan type. Use (V, H, S, or N)");
                    return false;
            }

            return true;
        }

        private bool TryParseComparison(string input, ref string comparison)
        {
            switch(input.ToUpperInvariant())
            {
                case "<>":
                case "!=":
                case "NOT EQUAL":
                    comparison = "<>";
                    break;
                case "=":
                case "==":
                case "EQUAL":
                    comparison = "=";
                    break;
                default:
                    Console.WriteLine("Invalid comparison. Use (<> or =)");
                    return false;
            }

            return true;
        }
    }
}
