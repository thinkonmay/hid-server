using Microsoft.AspNetCore.Mvc;
using DevSim.Interfaces;
using DevSim.Enums;
using System.Text;
using System.Net.WebSockets;

namespace DevSim.Controllers
{
    [ApiController]
    [Route("Socket")]
    public class SocketController : ControllerBase
    {
        private readonly ILogger<KeyboardController> _log;
        private readonly IKeyboardMouseInput _key;
        private readonly IGamepadInput _gamepad;
        public SocketController(ILogger<KeyboardController> logger,
                                IGamepadInput gamepad,
                                IKeyboardMouseInput key) {
            _log = logger;
            _key = key;
            _gamepad = gamepad;
        }

        [HttpGet]
        public async Task Get(string? token)
        {
            var context = ControllerContext.HttpContext;
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await Handle(webSocket);
            }
        }

        private async Task Handle(WebSocket ws)
        {
            try
            {
                do
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var message = ReceiveMessage(ws, memoryStream).Result;
                        if (message.Count > 0) {
                            var receivedMessage = Encoding.UTF8.GetString(memoryStream.ToArray());

                            var arr = receivedMessage.Split("|");
                            switch (arr[0])
                            {
                                case "mmr":
                                    await _key.ToggleRelativeMouse(true);
                                    _key.SendMouseMove(Single.Parse(arr[1]),Single.Parse(arr[2]));
                                    break;
                                case "mma":
                                    await _key.ToggleRelativeMouse(false);
                                    _key.SendMouseMove(Single.Parse(arr[1]),Single.Parse(arr[2]));
                                    break;
                                case "mw":
                                    _key.SendMouseWheel(Int32.Parse(arr[1]));
                                    break;
                                case "mu":
                                    _key.SendMouseButtonAction((ButtonCode)Int32.Parse(arr[1]),ButtonAction.Up);
                                    break;
                                case "md":
                                    _key.SendMouseButtonAction((ButtonCode)Int32.Parse(arr[1]),ButtonAction.Down);
                                    break;

                                case "kd":
                                    _key.SendKeyDown(arr[1]);
                                    break;
                                case "ku":
                                    _key.SendKeyUp(arr[1]);
                                    break;
                                case "kr":
                                    _key.SetKeyStatesUp();
                                    break;

                                case "gs":
                                    _gamepad.pressSlider(Int32.Parse(arr[1]),Single.Parse(arr[2]));
                                    break;
                                case "ga":
                                    _gamepad.pressAxis(Int32.Parse(arr[1]),Single.Parse(arr[2]));
                                    break;
                                case "gb":
                                    _gamepad.pressButton(Int32.Parse(arr[1]),arr[2] == "1");
                                    break;

                                default:
                                break;
                            }
                        }
                    }
                } while (ws.State == WebSocketState.Open);
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
            }
            _log.LogInformation("Connection closed");
        }

        private async Task<WebSocketReceiveResult> ReceiveMessage(WebSocket ws, Stream memoryStream)
        {
            var readBuffer = new ArraySegment<byte>(new byte[4 * 1024]);
            WebSocketReceiveResult result;

            do
            {
                result = await ws.ReceiveAsync(readBuffer, CancellationToken.None);
                await memoryStream.WriteAsync(readBuffer.Array, readBuffer.Offset, result.Count,
                    CancellationToken.None);
            } while (!result.EndOfMessage);
            return result;
        }

        private async Task SendMessage(WebSocket ws, string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            var buffer = new ArraySegment<byte>(bytes);

            try
            {
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            } catch { _log.LogError("Fail to send websocket to client"); }
        }
    }
}