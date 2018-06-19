using System.Data.SqlClient;

namespace darknet_analyzer.DataAccess
{
    public class PcapFileRepository : BaseRepository
    {
        public PcapFileRepository(string connectionString) : base (connectionString) { }

        public InsertPcapFileResult Create(string filePath)
        {
            var sql = "INSERT INTO PcapFile (FilePath) OUTPUT INSERTED.Id VALUES (@FilePath)";

            var result = new InsertPcapFileResult();
            try
            {
                result.Id = (int)this.ExecuteSql(c => c.ExecuteScalar(), sql, new SqlParameter("@FilePath", filePath));
            }
            catch (SqlException ex) when (ex.Number == DuplicateKeyExceptionNumber)
            {
                result.AlreadyExists = true;
                result.Id = this.GetId(filePath);
            }

            return result;
        }

        public void MarkFilesAsAnalyzed()
        {
            this.NonQuery("UPDATE PcapFile SET Analyzed = 1 WHERE Analyzed = 0");
        }

        public class InsertPcapFileResult
        {
            public int Id { get; set; }

            public bool AlreadyExists { get; set; }
        }

        public int GetId(string filePath)
        {
            var sql = "SELECT Id FROM PcapFile WHERE FilePath = @FilePath";

            return (int)this.ExecuteSql(c => c.ExecuteScalar(), sql, new SqlParameter("@FilePath", filePath));
        }
    }
}
