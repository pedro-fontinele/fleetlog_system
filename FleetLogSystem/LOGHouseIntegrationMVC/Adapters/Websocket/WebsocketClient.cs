using LOGHouseSystem.Models;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Text;
using WebSocketSharp;

namespace LOGHouseSystem.Adapters.Websocket
{
    public class WebsocketClient
    {
        private readonly string _url = "ws://socket.loghouse.com.br";

        public void SendMessage(string message)
        {
            using (var ws = new WebSocket(_url))
            {
                ws.Connect();
                ws.Send(message);
                ws.Close();
            }
        }
    }
}
