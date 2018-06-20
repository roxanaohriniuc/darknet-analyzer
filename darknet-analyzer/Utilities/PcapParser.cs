using PcapDotNet.Core;
using PcapDotNet.Packets;
using System;

namespace darknet_analyzer.Utilities
{
    public class PcapParser
    {
        public static void Parse(string fileName, Action<Packet> handler)
        {
            // Create the offline device
            OfflinePacketDevice selectedDevice = new OfflinePacketDevice(fileName);

            // Open the capture file
            using (PacketCommunicator communicator = selectedDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
            {
                // Read and dispatch packets until EOF is reached
                communicator.ReceivePackets(0, new HandlePacket(handler));
            }
        }
    }
}
