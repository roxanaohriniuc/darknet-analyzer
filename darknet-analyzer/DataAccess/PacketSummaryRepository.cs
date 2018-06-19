using darknet_analyzer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace darknet_analyzer.DataAccess
{
    public class PacketSummaryRepository : BaseRepository
    {
        public PacketSummaryRepository(string connectionString) : base(connectionString) { }

        public void Insert(IEnumerable<PacketSummary> packetSummaries)
        {
            var insertSql =
                "INSERT INTO dbo.PacketSummary (ReceivedDateTime, Bytes, Protocol, SourceIp, SourcePort, DestinationIp, DestinationPort, FileId) " +
                "SELECT ReceivedDateTime, Bytes, Protocol, SourceIp, SourcePort, DestinationIp, DestinationPort, FileId " +
                "FROM @InsertTable";

            var insertTableParameter = new SqlParameter("@InsertTable", this.GetPacketSummaryDataTable(packetSummaries));
            insertTableParameter.TypeName = "PacketSummaryInsert";

            this.NonQuery(insertSql, insertTableParameter);
        }

        private DataTable GetPacketSummaryDataTable(IEnumerable<PacketSummary> packetSummaries)
        {
            // Initialize the DataTable
            var dt = new DataTable("PacketSummaryInsert", "dbo");
            dt.Columns.Add("ReceivedDateTime", typeof(DateTime));
            dt.Columns.Add("Bytes", typeof(int));
            dt.Columns.Add("Protocol", typeof(int));
            dt.Columns.Add("SourceIp", typeof(string));
            dt.Columns.Add("SourcePort", typeof(string));
            dt.Columns.Add("DestinationIp", typeof(string));
            dt.Columns.Add("DestinationPort", typeof(string));
            dt.Columns.Add("FileId", typeof(int));

            // Populate DataTable with data
            foreach (var packetSummary in packetSummaries)
            {
                var row = dt.NewRow();
                row["ReceivedDateTime"] = packetSummary.ReceivedDateTime;
                row["Bytes"] = packetSummary.Bytes;
                row["Protocol"] = (int)packetSummary.Protocol;
                row["SourceIp"] = packetSummary.SourceIp;
                row["SourcePort"] = packetSummary.SourcePort;
                row["DestinationIp"] = packetSummary.DestinationIp;
                row["DestinationPort"] = packetSummary.DestinationPort;
                row["FileId"] = packetSummary.FileId;
                dt.Rows.Add(row);
            }

            return dt;
        }

        public List<ProbeInformation> GetProbeInformationBatch(int batchNum, int batchSize)
        {
            var dt = this.ExecuteStoredProcedure("GetProbeInformationBatch", new SqlParameter("@BatchNum", batchNum), new SqlParameter("@BatchSize", batchSize));

            return dt.Select().Select(r => new ProbeInformation
            {
                SourceIp = (string)r["SourceIp"],
                NumTargetIps = (int)r["NumTargetIps"],
                NumTargetPorts = (int)r["NumTargetPorts"],
                TotalBytes = (int)r["TotalBytes"],
                TotalPackets = (int)r["TotalPackets"],
                StartDateTime = (DateTime)r["StartDateTime"],
                EndDateTime = (DateTime)r["EndDateTime"]
            }).ToList();
        }
    }
}
