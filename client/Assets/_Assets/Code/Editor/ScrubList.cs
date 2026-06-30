using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class ScrubList : EditorWindow
{
    Dictionary<int, int> wordCount;

    private void OnGUI () 
    {
        if(GUILayout.Button("Scrub"))
        {
            wordCount = new Dictionary<int, int>();
            TextAsset ta = Resources.Load<TextAsset>("Adjectives");
            string unscrubbed = ta.text;
            int total = 0;

            using (StringReader reader = new StringReader(unscrubbed))
            {
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.persistentDataPath, "adjectives.txt")))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if(line.Contains("-"))
                            continue;

                        if(line.Length <= 12)
                        {
                            if(wordCount.ContainsKey(line.Length))
                            {
                                wordCount[line.Length]++;
                            }
                            else
                            {
                                wordCount.Add(line.Length, 1);
                            }
                            writer.WriteLine(line);
                            total++;
                        }
                            
                    }
                }
            }

            foreach(KeyValuePair<int,int> kVP in wordCount)
                Debug.Log(kVP.Key + " : " + kVP.Value);

            Debug.Log("Total: " + total);
        }
    }

    [MenuItem("Tools/Scruber")]
    static void Init()
    {
        ScrubList window = (ScrubList)EditorWindow.GetWindow(typeof(ScrubList));
        window.Show();
    }
}
