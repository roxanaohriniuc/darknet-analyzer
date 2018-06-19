using darknet_analyzer.DataAccess;
using System;
using System.Diagnostics;
using System.Linq;

namespace darknet_analyzer.Services
{
    public class ProbeInformationService
    {
        private readonly PacketSummaryService packetSummaryService;
        private readonly ProbeInformationRepository probeInformationRepository;

        public ProbeInformationService(string dbConnectionString)
        {
            this.packetSummaryService = new PacketSummaryService(dbConnectionString);
            this.probeInformationRepository = new ProbeInformationRepository(dbConnectionString);
        }

        public void AnalyzeProbeInformation()
        {
            // loop through all SourceIps and generate probe information
            const int batchSize = 1000;
            var batchNum = 0;
            var lastBatchSize = 0;

            var stopwatch = Stopwatch.StartNew();
            var totalProbes = 0;
            do
            {
                var probes = this.packetSummaryService.GetProbeInformationBatch(batchNum, batchSize);
                if(probes.Any())
                {
                    this.probeInformationRepository.Insert(probes);
                }

                totalProbes += probes.Count;
                lastBatchSize = probes.Count;
                batchNum++;

                Console.Write($"\rProbes: {totalProbes}\tElapsed Time: {(int)(stopwatch.ElapsedMilliseconds / 1000)} (s)");
            }
            while (lastBatchSize == batchSize);

            stopwatch.Stop();
            Console.WriteLine();
        }
    }
}
