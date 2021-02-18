using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JuanMartin.FileSystemBackup
{
    public class BackupSettings
    {
        [JsonProperty]
        public string BaseFolder { get; private set; }
        [JsonProperty]
        public List<string> Folders { get; private set; }
    }
}
