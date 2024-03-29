﻿/*
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution. 

The Eclipse Public License is available at 
   http://www.eclipse.org/legal/epl-v10.html
and the Eclipse Distribution License is available at 
   http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace uPLibrary.Networking.M2Mqtt
{
    public class MqttNetworkChannel : IMqttNetworkChannel
    {
        // stream socket for communication
        private StreamSocket socket;

        // remote host information
        private HostName remoteHostName;
        private int remotePort;

        // using SSL
        private bool secure;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket">Socket opened with the client</param>
        public MqttNetworkChannel(StreamSocket socket)
        {
            this.socket = socket;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="remoteHostName">Remote Host name</param>
        /// <param name="remotePort">Remote port</param>
        /// <param name="secure">Using SSL</param>
        public MqttNetworkChannel(string remoteHostName, int remotePort, bool secure)
        {
            this.remoteHostName = new HostName(remoteHostName);
            this.remotePort = remotePort;
            this.secure = secure;
        }

        public bool DataAvailable
        {
            get { return true; }
        }

        public int Receive(byte[] buffer)
        {
            IBuffer result;

            // read all data needed (until fill buffer)
            int idx = 0;
            while (idx < buffer.Length)
            {
                // read is executed synchronously
                result = this.socket.InputStream.ReadAsync(buffer.AsBuffer(), (uint)buffer.Length, InputStreamOptions.None).AsTask().Result;
                idx += (int)result.Length;
            }
            return buffer.Length;
        }

        public int Send(byte[] buffer)
        {
            // send is executed synchronously
            return (int)this.socket.OutputStream.WriteAsync(buffer.AsBuffer()).AsTask().Result;
        }

        public void Close()
        {
            this.socket.Dispose();
        }

        public void Connect()
        {
            this.socket = new StreamSocket();

            // connection is executed synchronously
            this.socket.ConnectAsync(this.remoteHostName,
                this.remotePort.ToString(),
                this.secure ? SocketProtectionLevel.Tls12 : SocketProtectionLevel.PlainSocket).AsTask().Wait();
        }

        
    }
}
