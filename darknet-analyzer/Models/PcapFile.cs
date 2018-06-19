namespace darknet_analyzer.Models
{
    public class PcapFile
    {
        public int Id { get; set; }

        public string FilePath { get; set; }

        public bool Analyzed { get; set; }
    }
}
