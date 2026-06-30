using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedOption
{
    public string label;
    public Language language;

    public LocalizedOption(string label, Language language)
    {
        this.label = label;
        this.language = language;
    }
}
