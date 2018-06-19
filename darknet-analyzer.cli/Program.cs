using darknet_analyzer.Services;
using System;
using System.Configuration;

namespace darknet_analyzer.cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("File or directory to load:");
                Console.Write(">");
                var path = Console.ReadLine();

                var dbConnectionString = ConfigurationManager.ConnectionStrings["darknet-analyzer"].ConnectionString;
                new PcapFileService(dbConnectionString).ProcessFileOrDirectory(path);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            Console.ReadLine();
        }
    }
}
