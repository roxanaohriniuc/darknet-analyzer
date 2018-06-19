using darknet_analyzer.DataAccess;
using darknet_analyzer.Models;
using System;
using System.IO;

namespace darknet_analyzer.Services
{
    public class PcapFileService
    {
        private readonly PcapFileRepository pcapFileRepository;
        private readonly PacketSummaryService packetSummaryService;
        private readonly ProbeInformationService probeInformationService;

        public PcapFileService(string dbConnectionString)
        {
            this.pcapFileRepository = new PcapFileRepository(dbConnectionString);
            this.packetSummaryService = new PacketSummaryService(dbConnectionString);
            this.probeInformationService = new ProbeInformationService(dbConnectionString);
        }

        public void ProcessFileOrDirectory(string path)
        {
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                foreach(var filePath in Directory.GetFiles(path, "*.pcap"))
                {
                    this.ProcessFile(path);
                }
            }
            else
            {
                this.ProcessFile(path);
            }

            // TODO: only analyze files that are not analyzed yet
            //var toAnalyze = this.pcapFileRepository.GetNotAnalyzedIds();
            this.probeInformationService.AnalyzeProbeInformation();
        }

        private void ProcessFile(string filePath)
        {
            var insertResult = this.pcapFileRepository.Insert(filePath);
            if (insertResult.AlreadyExists)
            {
                Console.WriteLine($"File already processed: {filePath}{Environment.NewLine}");
                return;
            }

            var file = new PcapFile
            {
                Id = insertResult.Id,
                FilePath = filePath
            };

            Console.WriteLine($"Parsing file: {filePath}");
            this.packetSummaryService.AnalyzePcapFile(file);
        }
    }
}
