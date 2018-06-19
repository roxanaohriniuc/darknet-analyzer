using darknet_analyzer.DataAccess;
using darknet_analyzer.Models;
using darknet_analyzer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace darknet_analyzer.Services
{
    public class PacketSummaryService
    {
        private readonly PacketSummaryRepository packetSummaryRepository;
        private readonly PcapFileRepository receivedFileRepository;

        public PacketSummaryService(string dbConnectionString)
        {
            this.packetSummaryRepository = new PacketSummaryRepository(dbConnectionString);
            this.receivedFileRepository = new PcapFileRepository(dbConnectionString);
        }

        public void AnalyzePcapFile(PcapFile file)
        {
            var stopwatch = Stopwatch.StartNew();
            int packetCount = 0;
            var errors = new List<Exception>();
            var packetSummaries = new List<PacketSummary>();

            var insertTasks = new List<Task>();
            PcapParser.Parse(file.FilePath, p =>
            {
                try
                {
                    // insert packet summaries in batches
                    if(packetSummaries.Count >= 50000)
                    {
                        this.packetSummaryRepository.Create(packetSummaries);
                        packetCount += packetSummaries.Count;
                        packetSummaries.Clear();
                    }

                    Console.Write($"\rPackets: {packetCount}\tElapsed Time: {(int)(stopwatch.ElapsedMilliseconds / 1000)} (s)");

                    packetSummaries.Add(PacketSummary.Parse(p).ForFile(file.Id));
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            });

            this.packetSummaryRepository.Create(packetSummaries);
            packetCount += packetSummaries.Count;
            packetSummaries.Clear();
            Console.Write($"\rPackets: {packetCount}/{packetCount + errors.Count}\tElapsed Time: {(int)(stopwatch.ElapsedMilliseconds / 1000)} (s)");
            Console.WriteLine(Environment.NewLine);

            stopwatch.Stop();
            //errors.ForEach(e => this.userInterface.Error(e.ToString()));
        }

        public List<ProbeInformation> GetProbeInformationBatch(int batchNum, int batchSize)
        {
            return this.packetSummaryRepository.GetProbeInformationBatch(batchNum, batchSize);
        }
    }
}
