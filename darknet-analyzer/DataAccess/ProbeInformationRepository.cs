using darknet_analyzer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace darknet_analyzer.DataAccess
{
    public class ProbeInformationRepository : BaseRepository
    {
        public ProbeInformationRepository(string connectionString) : base (connectionString) { }

        public void Insert(IEnumerable<ProbeInformation> probes)
        {
            var insertSql =
                "INSERT INTO dbo.ProbeInformation (SourceIp, NumTargetIps, NumTargetPorts, TotalBytes, TotalPackets, StartDateTime, EndDateTime) " +
                "SELECT SourceIp, NumTargetIps, NumTargetPorts, TotalBytes, TotalPackets, StartDateTime, EndDateTime " +
                "FROM @InsertTable";

            var insertTableParameter = new SqlParameter("@InsertTable", this.GetProbeInformationDataTable(probes));
            insertTableParameter.TypeName = "ProbeInformationInsert";

            this.NonQuery(insertSql, insertTableParameter);
        }

        private DataTable GetProbeInformationDataTable(IEnumerable<ProbeInformation> probes)
        {
            // Initialize the DataTable
            var dt = new DataTable("ProbeInformationInsert", "dbo");
            dt.Columns.Add("SourceIp", typeof(string));
            dt.Columns.Add("NumTargetIps", typeof(long));
            dt.Columns.Add("NumTargetPorts", typeof(long));
            dt.Columns.Add("TotalBytes", typeof(long));
            dt.Columns.Add("TotalPackets", typeof(long));
            dt.Columns.Add("StartDateTime", typeof(DateTime));
            dt.Columns.Add("EndDateTime", typeof(DateTime));

            // Populate DataTable with data
            foreach (var probeInformation in probes)
            {
                var row = dt.NewRow();
                row["SourceIp"] = probeInformation.SourceIp;
                row["NumTargetIps"] = probeInformation.NumTargetIps;
                row["NumTargetPorts"] = probeInformation.NumTargetPorts;
                row["TotalBytes"] = probeInformation.TotalBytes;
                row["TotalPackets"] = probeInformation.TotalPackets;
                row["StartDateTime"] = probeInformation.StartDateTime;
                row["EndDateTime"] = probeInformation.EndDateTime;
                dt.Rows.Add(row);
            }

            return dt;
        }

    }
}
