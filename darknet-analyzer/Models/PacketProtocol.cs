namespace darknet_analyzer.Models
{
    public enum PacketProtocol : byte
    {
        Unknown = 0,
        Icmp = 20,
        Igmp = 25,
        Tcp = 50,
        Udp = 80
    }
}
