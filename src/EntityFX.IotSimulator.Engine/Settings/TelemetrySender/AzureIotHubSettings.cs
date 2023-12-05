﻿using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine
{
    public class AzureIotHubSettings
    {
        public Uri HostName { get; set; }

        public Dictionary<string, string> SymmetricKeys { get; set; }
    }
}