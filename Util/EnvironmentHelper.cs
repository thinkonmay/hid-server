﻿using DevSim.Enums;
using System;
using System.Diagnostics;

namespace DevSim.Utilities
{
    public static class EnvironmentHelper
    {
        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
    }
}


        public static bool IsLinux => OperatingSystem.IsLinux();

        public static bool IsMac => OperatingSystem.IsMacOS();

        public static bool IsWindows => OperatingSystem.IsWindows();

        public static Platform Platform
        {
            get
            {
                if (IsWindows)
                {
                    return Platform.Windows;
                }
                else if (IsLinux)
                {
                    return Platform.Linux;
                }
                else if (IsMac)
                {
                    return Platform.MacOS;
                }
                else
                {
                    return Platform.Unknown;
                }
            }
        }
    }
}
