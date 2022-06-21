//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using EP.U3D.LIBRARY.BASE;
using System.Collections;
using UnityEngine.Networking;

namespace EP.U3D.RUNTIME.LUA.NET
{
    public class NetManager : LIBRARY.NET.NetManager
    {
        public delegate void CgiDelegate(string err, byte[] data);

        public static new NetConnection ConnectTo(int type, string host, int post, LIBRARY.NET.NetConnection.StatusDelegate onConnected, LIBRARY.NET.NetConnection.StatusDelegate onDisconnected, LIBRARY.NET.NetConnection.StatusDelegate onReconnected, LIBRARY.NET.NetConnection.StatusDelegate onErrorOccurred)
        {
            if (connections.ContainsKey(type))
            {
                DisconnectFrom(type);
            }
            NetConnection connection = new NetConnection(host, post, onConnected, onDisconnected, onReconnected, onErrorOccurred);
            connection.Connect();
            connections.Add(type, connection);
            return connection;
        }

        public static void SendCgi(int id, byte[] body, CgiDelegate callback = null, int uid = -1, int rid = -1, string host = null)
        {
            Loom.StartCR(DoCgi(id, body, callback, uid, rid, host));
        }

        private static IEnumerator DoCgi(int id, byte[] body, CgiDelegate callback = null, int uid = -1, int rid = -1, string host = null)
        {
            if (string.IsNullOrEmpty(host)) host = Constants.CGI_SERVER_URL;
            using (UnityWebRequest request = new UnityWebRequest(host, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/octet-stream");
                request.SetRequestHeader("AccessToken", Constants.CGI_ACCESS_TOKEN);
                request.SetRequestHeader("RefreshToken", Constants.CGI_REFRESH_TOKEN);
                request.SetRequestHeader("CID", id.ToString());
                request.SetRequestHeader("UID", uid == -1 ? Constants.CGI_SERVER_UID.ToString() : uid.ToString());
                request.SetRequestHeader("RID", rid.ToString());
                yield return request.SendWebRequest();
                if (request.responseCode == 200)
                {
                    callback?.Invoke(null, request.downloadHandler.data);
                }
                else
                {
                    callback?.Invoke(request.error, request.downloadHandler.data);
                }
            }
        }
    }
}