using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Controllers
{
    [Route("ws/[controller]/[action]")]
    public class WebSocketController : Controller
    {

        [HttpGet]
        /*	public string Get()
            {
                Players tmpPlayer;
                Startup.players[Startup.PlayerIndex] = new Players { id = Startup.PlayerIndex, status = true, opponentId = -1 };

                if (Startup.WaitingPlayers.Count == 0)
                {
                    Startup.WaitingPlayers.Enqueue(Startup.players[Startup.PlayerIndex]);
                    //str = "Добавлено в очередь " + Startup.WaitingPlayers.Count.ToString();
                }
                else
                {
                    tmpPlayer = Startup.WaitingPlayers.Dequeue();
                    Startup.players[Startup.PlayerIndex].opponentId = tmpPlayer.id;
                    Startup.players[tmpPlayer.id].opponentId = Startup.players[Startup.PlayerIndex].id;
                }
                Startup.PlayerIndex++;
                return (Startup.PlayerIndex - 1).ToString();
            }*/
        /*private readonly ApplicationDbContext _context;

		public ChopController(ApplicationDbContext context)
		{
			_context = context;

		}*/

        /// <summary>
        /// Нужно для коннекта клиента
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                
                Startup.Users.Add(webSocket, Startup.PlayerIndex);
                
                if (Startup.Users.Count % 2 == 0)
                {
                    await Wait(webSocket, 2);
                    /*WebSocket webSocketFirst = Startup.Users.First(pair => pair.Value == Startup.PlayerIndex).Key;
                    Startup.PlayerIndex++;*/
                    

                }
                await Wait(webSocket, 1);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
        private async Task Wait(WebSocket webSocket, int x)
        {
            
            string str = "0";
            int Index = Startup.PlayerIndex;
            if (x % 2 == 0)
            {
                Startup.PlayerIndex++;
            }
            byte[] message = System.Text.Encoding.UTF8.GetBytes(str);
            await webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
            WebSocket webSocketFirst;
            do
            {
                await Task.Delay(2000);
                webSocketFirst = Startup.Users.LastOrDefault(pair => pair.Value == Index && pair.Key != webSocket).Key;

            } while (webSocketFirst == null);
            await Echo(webSocket, webSocketFirst);

        }
        // Тест возврата полученных сообщений
        // message:
        // 0 - connected
        // 1 - open
        private async Task Echo(WebSocket webSocket, WebSocket webSocketSecond)
        {

            string str = "1";
            byte[] message = System.Text.Encoding.UTF8.GetBytes(str);
            await webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
            /*await webSocketSecond.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);*/

            var buffer = new byte[1024 * 4];

            /*Task SendMessage = new Task(()=>)*/

            while (true)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                    CancellationToken.None);
                await webSocketSecond.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count),
                   result.MessageType,
                   result.EndOfMessage,
                   CancellationToken.None);
            }


            /*while (true)
			{
                string str = "тест";
                int countbytes = 0;
				byte[] test;
				
				test = System.Text.Encoding.UTF8.GetBytes(str); 
				await webSocket.SendAsync(new ArraySegment<byte>(test), WebSocketMessageType.Text, true, CancellationToken.None);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count),
                    result.MessageType,
                    result.EndOfMessage,
                    CancellationToken.None);
                buffer = new byte[1024 * 4];
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
					CancellationToken.None);
			}
			await webSocket.CloseAsync(result.CloseStatus.Value,
				result.CloseStatusDescription,
				CancellationToken.None);*/
        }
    }
}

