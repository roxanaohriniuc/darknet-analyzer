using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.Transport;
using System;

namespace darknet_analyzer.Models
{
    public partial class PacketSummary
    {
        public static PacketSummary Parse(Packet packet)
        {
            var summary = new PacketSummary();

            summary.MapPacket(packet);

            return summary;
        }

        private void MapPacket(Packet packet)
        {
            this.ReceivedDateTime = packet.Timestamp;
            this.Bytes = packet.Length;

            switch (packet.DataLink.Kind)
            {
                case DataLinkKind.Ethernet:
                    this.MapEthernetData(packet.Ethernet);
                    break;
                case DataLinkKind.IpV4:
                    this.MapIpV4Data(packet.IpV4);
                    break;
                default:
                    throw new Exception($"Unsupported DataLink. ({packet.DataLink.Kind})");
            }
        }

        private void MapEthernetData(EthernetDatagram datagram)
        {
            this.SourceIp = datagram.Source.ToString();
            this.DestinationIp = datagram.Destination.ToString();

            switch (datagram.EtherType)
            {
                case EthernetType.IpV4:
                    this.MapIpV4Data(datagram.IpV4);
                    break;
                default:
                    throw new Exception($"Unsupported EtherType. ({datagram.EtherType})");
            }
        }

        private void MapIpV4Data(IpV4Datagram datagram)
        {
            this.SourceIp = datagram.Source.ToString();
            this.DestinationIp = datagram.Destination.ToString();

            switch (datagram.Protocol)
            {
                case IpV4Protocol.IpV6:
                    this.MapIpV6Data(datagram.IpV6);
                    break;
                case IpV4Protocol.Tcp:
                    this.MapTcpData(datagram.Tcp);
                    break;
                case IpV4Protocol.Udp:
                    this.MapUdpData(datagram.Udp);
                    break;
                case IpV4Protocol.InternetControlMessageProtocol:
                    this.MapIcmpData(datagram.Icmp);
                    break;
                case IpV4Protocol.InternetGroupManagementProtocol:
                    this.MapIgmpData(datagram.Igmp);
                    break;
                default:
                    throw new Exception($"Unsupported Protocol. ({datagram.Protocol})");
            }
        }

        private void MapIpV6Data(IpV6Datagram datagram)
        {
            this.SourceIp = datagram.Source.ToString();

            if(string.IsNullOrEmpty(this.DestinationIp))
            {
                // only set if not already set for ipv6, may be routing address
                this.DestinationIp = datagram.CurrentDestination.ToString();
            }

            var payloadProtocol = datagram.ExtensionHeaders.NextHeader ?? datagram.NextHeader;
            switch (payloadProtocol)
            {
                case IpV4Protocol.Tcp:
                    this.MapTcpData(datagram.Tcp);
                    break;
                case IpV4Protocol.Udp:
                    this.MapUdpData(datagram.Udp);
                    break;
                case IpV4Protocol.InternetControlMessageProtocolForIpV6:
                    this.MapIcmpData(datagram.Icmp);
                    break;
                default:
                    throw new Exception($"Unsupported IPv6 Protocol. ({payloadProtocol})");
            }
        }

        private void MapTcpData(TcpDatagram datagram)
        {
            this.Protocol = PacketProtocol.Tcp;
            this.SourcePort = datagram.SourcePort.ToString();
            this.DestinationPort = datagram.DestinationPort.ToString();
        }

        private void MapUdpData(UdpDatagram datagram)
        {
            this.Protocol = PacketProtocol.Udp;
            this.SourcePort = datagram.SourcePort.ToString();
            this.DestinationPort = datagram.DestinationPort.ToString();
        }

        private void MapIcmpData(IcmpDatagram datagram)
        {
            this.Protocol = PacketProtocol.Icmp;
            throw new NotImplementedException("Icmp");
        }

        private void MapIgmpData(IgmpDatagram datagram)
        {
            this.Protocol = PacketProtocol.Igmp;
            throw new NotImplementedException("Igmp");
        }
    }
}
