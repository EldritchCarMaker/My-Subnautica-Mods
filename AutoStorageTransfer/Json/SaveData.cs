using System.Collections.Generic;
#if SN1
using SMLHelper.V2.Json;
using SMLHelper.V2.Json.Attributes;
#else
using Nautilus.Json.Attributes;
using Nautilus.Json;
#endif

namespace AutoStorageTransfer.Json
{
    [FileName("AutoStorageTransfer")]
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, SaveInfo> SavedStorages = new Dictionary<string, SaveInfo>();
    }
    public class SaveInfo
    {
        public string StorageID;
        public bool IsReciever;
    }
}
