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

        public void CreateOrUpdate(IEnumerable<ProbeInformation> probes)
        {
            var insertSql =
                // Update existing records
                "UPDATE e SET " +
                "e.NumTargetIps = u.NumTargetIps, e.NumTargetPorts = u.NumTargetPorts, " +
                "e.TotalBytes = u.TotalBytes, e.Totalpackets = u.TotalPackets, " +
                "e.StartDateTime = u.StartDateTime, e.EndDateTime = u.EndDateTime," +
                "e.Rate = u.Rate, e.ScanType = u.ScanType " +
                "FROM @InsertTable u " +
                "INNER JOIN dbo.ProbeInformation e ON u.SourceIp = e.SourceIp; " +
                // Insert new records
                "INSERT INTO dbo.ProbeInformation (SourceIp, NumTargetIps, NumTargetPorts, TotalBytes, TotalPackets, StartDateTime, EndDateTime, Rate, ScanType) " +
                "SELECT i.SourceIp, i.NumTargetIps, i.NumTargetPorts, i.TotalBytes, i.TotalPackets, i.StartDateTime, i.EndDateTime, i.Rate, i.ScanType " +
                "FROM @InsertTable i " +
                "LEFT JOIN dbo.ProbeInformation e on i.SourceIp = e.SourceIp " +
                "WHERE e.SourceIp IS NULL;";

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
            dt.Columns.Add("Rate", typeof(decimal));
            dt.Columns.Add("ScanType", typeof(byte));

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
                row["Rate"] = probeInformation.Rate;
                row["ScanType"] = (byte)probeInformation.ScanType;
                dt.Rows.Add(row);
            }

            return dt;
        }

    }
}
