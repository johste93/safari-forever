using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LevelUploadDTO
{
    public JObject SerializedLevel;
    public string Name;
    public string GameVersion {get{return Application.version;}}
}
