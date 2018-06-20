using System;

namespace darknet_analyzer.Models
{
    public class ProbeInformation
    {
        public string SourceIp { get; set; }

        public long NumTargetIps { get; set; }

        public long NumTargetPorts { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public long TotalBytes { get; set; }

        public long TotalPackets { get; set; }

        public double Rate => (this.TotalPackets / Math.Max((this.EndDateTime - this.StartDateTime).TotalSeconds, 1));

        public ScanType ScanType => this.DetermineProbeType();

        public ProbeInformation()
        {
            this.SourceIp = string.Empty;
        }

        private ScanType DetermineProbeType()
        {
            const int verticalThreshold = 5;
            const int horizontalThreshold = 4;

            var probeType = ScanType.NotScan;
            if (this.NumTargetIps > horizontalThreshold && this.NumTargetPorts > verticalThreshold)
            {
                probeType = ScanType.Strobe;
            }
            else if (this.NumTargetIps > horizontalThreshold)
            {
                probeType = ScanType.Horizontal;
            }
            else if (this.NumTargetPorts > verticalThreshold)
            {
                probeType = ScanType.Vertical;
            }

            return probeType;
        }
    }

    public enum ScanType : byte
    {
        NotScan = 0,
        Horizontal = 1,
        Vertical = 2,
        Strobe = 3
    }
}
