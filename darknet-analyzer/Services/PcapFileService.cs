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

        public void LoadFileOrDirectory(string path)
        {
            // get the file attributes for file or directory
            var attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                foreach(var filePath in Directory.GetFiles(path, "*.pcap"))
                {
                    this.LoadFile(filePath);
                }
            }
            else
            {
                this.LoadFile(path);
            }
        }

        private void LoadFile(string filePath)
        {
            var insertResult = this.pcapFileRepository.Create(filePath);
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

            Console.WriteLine($"Loading file: {filePath}");
            this.packetSummaryService.LoadPcapFile(file);
        }

        public void AnalyzeNewFiles()
        {
            Console.WriteLine("Analyzing new source ip addresses.");
            this.probeInformationService.AnalyzeProbeInformation();
            this.pcapFileRepository.MarkFilesAsAnalyzed();
        }
    }
}
