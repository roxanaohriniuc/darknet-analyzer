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

        public ProbeInformation()
        {
            this.SourceIp = string.Empty;
        }
    }
}
