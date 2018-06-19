using System;

namespace darknet_analyzer.Models
{
    public partial class PacketSummary
    {
        public DateTime ReceivedDateTime { get; set; }

        public int Bytes { get; set; }

        public PacketProtocol Protocol { get; set; }

        public string SourceIp { get; set; }

        public string SourcePort { get; set; }

        public string DestinationIp { get; set; }

        public string DestinationPort { get; set; }

        public int FileId { get; set; }

        public PacketSummary()
        {
            this.Protocol = PacketProtocol.Unknown;
            this.SourceIp = string.Empty;
            this.SourcePort = string.Empty;
            this.DestinationIp = string.Empty;
            this.DestinationPort = string.Empty;
        }

        public string ToDisplayString()
        {
            return string.Join(
                Environment.NewLine,
                $"Timestamp: {this.ReceivedDateTime}",
                $"Bytes: {this.Bytes}",
                $"Protocol: {this.Protocol}",
                $"Source IP: {this.SourceIp}",
                $"Source Port: {this.SourcePort}",
                $"Destination IP: {this.DestinationIp}",
                $"Destination Port:{this.DestinationPort}",
                Environment.NewLine);
        }

        public PacketSummary ForFile(int fileId)
        {
            this.FileId = fileId;
            return this;
        }
    }
}
