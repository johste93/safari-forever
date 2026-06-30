using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NameGenerator
{
    private static List<string> _nouns;
    private static List<string> nouns
    {
        get{
            if(_nouns == null)
                LoadNouns();

            return _nouns;
        }
    }

    private static List<string> _adjectives;
    private static List<string> adjectives
    {
        get{
            if(_adjectives == null)
                LoadAdjectives();
                
            return _adjectives;
        }
    }

    private static string[] name = new string[2];

    public static string[] RerollAll()
    {
        for(int i = 0; i < 2; i++)
            RerollIndex(i);
        
        return name;
    }

    public static string GenerateName()
    {
        for(int i = 0; i < 2; i++)
            RerollIndex(i);
        
        return name[0] + " " + name[1];
    }

    public static string RerollIndex(int index)
    {
        if(index < 1)
            name[index] = adjectives[Random.Range(0, adjectives.Count)].FirstLetterToUpper();
        else
            name[index] = nouns[Random.Range(0, nouns.Count)].FirstLetterToUpper();

        return name[index];
    }

    public static List<string> GetAdjectives()
    {
        return adjectives;
    }

    public static List<string> GetNouns()
    {
        return nouns;
    }

    private static void LoadNouns()
    {
        string longestWord = "";

        _nouns = new List<string>();
        TextAsset ta = Resources.Load<TextAsset>("Nouns");
        using (StringReader reader = new StringReader(ta.text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _nouns.Add(line);

                if(line.Length > longestWord.Length)
                    longestWord = line;
            }
        }
    }

    private static void LoadAdjectives()
    {
        string longestWord = "";

        _adjectives = new List<string>();
        TextAsset ta = Resources.Load<TextAsset>("Adjectives");
        using (StringReader reader = new StringReader(ta.text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _adjectives.Add(line);

                if(line.Length > longestWord.Length)
                    longestWord = line;
            }
        }
    }
}
