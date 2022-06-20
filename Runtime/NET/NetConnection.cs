//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using System;
using System.Net.Sockets;
using EP.U3D.LIBRARY.BASE;

namespace EP.U3D.RUNTIME.LUA.NET
{
    public class NetConnection : LIBRARY.NET.NetConnection
    {
        public NetConnection(string host, int port, StatusDelegate onConnected, StatusDelegate onDisconnected, StatusDelegate onReconnected, StatusDelegate onErrorOccurred)
            : base(host, port, onConnected, onDisconnected, onReconnected, onErrorOccurred)
        {
        }

        protected override void RecvCallback(IAsyncResult ret)
        {
            if (sigSocketReleased == false)
            {
                if (socket != null)
                {
                    try
                    {
                        int bytesRead = socket.EndReceive(ret);
                        while (bytesRead < LIBRARY.NET.NetPacket.HEAD_LENGTH)
                        {
                            bytesRead += socket.Receive(receiveHeader, bytesRead, LIBRARY.NET.NetPacket.HEAD_LENGTH - bytesRead, SocketFlags.None);
                        }
                        if (bytesRead == LIBRARY.NET.NetPacket.HEAD_LENGTH)
                        {
                            if (LIBRARY.NET.NetPacket.Validate(receiveHeader))
                            {
                                int id = BitConverter.ToInt32(receiveHeader, LIBRARY.NET.NetPacket.ID_OFFSET);
                                int packetLength = BitConverter.ToInt32(receiveHeader, LIBRARY.NET.NetPacket.LENGTH_OFFSET);
                                int bodyLength = packetLength - LIBRARY.NET.NetPacket.HEAD_LENGTH;
                                LIBRARY.NET.NetPacket packet = new LIBRARY.NET.NetPacket(id, bodyLength);
                                packet.Head = receiveHeader;
                                if (bodyLength > 0)
                                {
                                    bytesRead = socket.Receive(packet.Body, 0, packet.BodyLength, SocketFlags.None);
                                    while (bytesRead < packet.BodyLength)
                                    {
                                        bytesRead += socket.Receive(packet.Body, bytesRead, packet.BodyLength - bytesRead, SocketFlags.None);
                                    }
                                }
                                Loom.QueueInMainThread(() =>
                                {
                                    LIBRARY.NET.NetManager.NotifyMsg(new EVT.Evt() { ID = packet.ID, Param = packet.Body, LParam = packet.Body });
                                });
                                StartReceive();
                            }
                            else
                            {
                                error = "packet is not validity";
                                ErrorOccurred();
                            }
                        }
                        else
                        {
                            error = "packet header read error,size is " + bytesRead;
                            ErrorOccurred();
                        }
                    }
                    catch (Exception e)
                    {
                        error = e.Message;
                        ErrorOccurred();
                    }
                }
                else
                {
                    // socket has been released.
                }
            }
        }
    }
}