using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class TestSQLite : MonoBehaviour
{

    public LevelPreview levelPreview;

    // Start is called before the first frame update
    void Start()
    {
        DataService ds = new DataService("database.db");
        var result = ds.connection.Query<Levels>("SELECT * FROM Levels WHERE LevelId = '33e9Wu'").First();

        levelPreview.FromBytes(result.Thumbnail);
        levelPreview.SetName(result.Name);
    }
}
