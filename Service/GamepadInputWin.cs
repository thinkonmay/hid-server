﻿using DevSim.Interfaces;
using DevSim.Enums;
using DevSim.Utilities;
using System;
using System.Collections.Concurrent;
using System.Threading;
using static DevSim.Win32.User32;
using DevSim.Win32;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DevSim.Services
{
    public class GamepadInput : IGamepadInput
    {
        private readonly ViGEmClient vigem;
        private ConcurrentDictionary<int,IXbox360Controller> xboxs;
        private ConcurrentDictionary<int,Xbox360FeedbackReceivedEventHandler> feedbacks;
        public GamepadInput()
        {
            // initializes the SDK instance
            vigem = new ViGEmClient();
            xboxs = new ConcurrentDictionary<int, IXbox360Controller>();
            feedbacks = new ConcurrentDictionary<int, Xbox360FeedbackReceivedEventHandler>();

            // recommended: run this in its own thread
            // Task.Run(() => {
            //     while (true)
            //         try
            //         {
            //             // blocks for 250ms to not burn CPU cycles if no report is available
            //             // an overload is available that blocks indefinitely until the device is disposed, your choice!
            //             if (xbox == null) {
            //                 Thread.Sleep(TimeSpan.FromSeconds(5));
            //                 continue;
            //             }

            //             xbox.FeedbackReceived += (object sender, Xbox360FeedbackReceivedEventArgs e) => {
            //                 feedback = e;
            //             };


            //         }
            //         catch (Exception ex)
            //         {
            //             Console.WriteLine(ex.ToString());
            //             Thread.Sleep(1000);
            //         }
            // });
        }

        public bool Status(){
            return xboxs != null;
        }

        public IXbox360Controller Connect(int id)
        {
            var xbox = vigem.CreateXbox360Controller();
            xbox.AutoSubmitReport = true;
            xbox.Connect();
            xboxs.TryAdd(id,xbox);
            return xbox;
            // feedbacks.TryAdd(id,feedback);
        }

        public void DisConnect(int id)
        {
            xboxs.Remove(id,out var xbox);
            if (xbox != null)
                xbox.Disconnect();
            // feedbacks.Remove(id,out var feedback);
        }

        public async Task pressButton(int gamepad, int index, bool pressed)
        {
            Xbox360Button button;
            switch (index)
            {
                case 0:
                    button = Xbox360Button.A;
                    break;
                case 1:
                    button = Xbox360Button.B;
                    break;
                case 2:
                    button = Xbox360Button.X;
                    break;
                case 3:
                    button = Xbox360Button.Y;
                    break;
                case 4:
                    button = Xbox360Button.LeftShoulder;
                    break;
                case 5:
                    button = Xbox360Button.RightShoulder;
                    break;

                case 8:
                    button = Xbox360Button.Back;
                    break;
                case 9:
                    button = Xbox360Button.Start;
                    break;
                case 10:
                    button = Xbox360Button.LeftThumb;
                    break;
                case 11:
                    button = Xbox360Button.RightThumb;
                    break;
                case 12:
                    button = Xbox360Button.Up;
                    break;
                case 13:
                    button = Xbox360Button.Down;
                    break;
                case 14:
                    button = Xbox360Button.Left;
                    break;
                case 15:
                    button = Xbox360Button.Right;
                    break;
                case 16:
                    button = Xbox360Button.Guide;
                    break;
                default:
                    Console.WriteLine($"unknown button {index}");
                    return;
            }

            if (xboxs == null) {
                Console.WriteLine("Gamepad is not connected");
                return;
            }

            xboxs.TryGetValue(gamepad,out var xbox);
            if(xbox == null) {
                xbox = Connect(gamepad);
            }
            xbox.SetButtonState(button,pressed);
        }


        public async Task pressSlider(int gamepad, int index, float val)
        {
            Xbox360Slider slider;
            switch (index)
            {
                case 6:
                    slider = Xbox360Slider.LeftTrigger;
                    break;
                case 7:
                    slider = Xbox360Slider.RightTrigger;
                    break;
                default:
                    Console.WriteLine($"unknown slider {index}");
                    return;
            }

            if (xboxs == null) {
                Console.WriteLine("Gamepad is not connected");
                return;
            }

            xboxs.TryGetValue(gamepad,out var xbox);
            if(xbox == null) {
                xbox = Connect(gamepad);
            }
            xbox.SetSliderValue(slider,(byte)( val * Byte.MaxValue));
        }

        public async Task pressAxis(int gamepad, int index, float val)
        {
            Xbox360Axis slider;
            switch (index)
            {
                case 0:
                    slider = Xbox360Axis.LeftThumbX;
                    break;
                case 1:
                    slider = Xbox360Axis.LeftThumbY;
                    val = -val;
                    break;
                case 2:
                    slider = Xbox360Axis.RightThumbX;
                    break;
                case 3:
                    slider = Xbox360Axis.RightThumbY;
                    val = -val;
                    break;
                default:
                    Console.WriteLine($"unknown axis {index}");
                    return;
            }

            if (xboxs == null) {
                Console.WriteLine("Gamepad is not connected");
                return;
            }

            xboxs.TryGetValue(gamepad,out var xbox);
            if(xbox == null) {
                xbox = Connect(gamepad);
            }
            xbox.SetAxisValue(slider,(short) (val * 32767));
        }

    }

}