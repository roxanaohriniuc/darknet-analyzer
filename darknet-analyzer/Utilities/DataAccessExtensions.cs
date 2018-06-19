using System;
using System.Collections.Generic;
using System.Data;

namespace darknet_analyzer.Utilities
{
    public static class DataAccessExtensions
    {
        public static DataTable ToDataTable(this IDataReader reader)
        {
            using (reader)
            {
                var dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
        }
    }
}
