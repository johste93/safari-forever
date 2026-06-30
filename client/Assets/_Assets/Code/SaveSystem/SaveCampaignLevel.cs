using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class SaveCampaignLevel : MonoBehaviour
{
    public void OnClick()
    {
        if(!Application.isEditor)
            return;

        ThumbnailCamera.instance.Snap();
        string json = LevelSerializer.Save(true);
        JObject jObj = JObject.Parse(json);

        string name = NameGenerator.GenerateName();
        string path = Path.Combine(Application.persistentDataPath, $"{name}.json");
        File.WriteAllText(path, jObj.ToString());
    }
}
