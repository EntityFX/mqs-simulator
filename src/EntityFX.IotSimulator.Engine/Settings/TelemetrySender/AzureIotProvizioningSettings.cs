﻿using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine
{
    public class AzureIotCenterSettings
    {
        public string ProvisioningHost { get; set; }

        public string IdScope { get; set; }

        public Dictionary<string, string> SymmetricKeys { get; set; }
    }
}