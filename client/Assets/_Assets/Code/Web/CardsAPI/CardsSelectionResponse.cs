using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsSelectionResponse
{
    public CardError Error { get; set; }
    public bool[] Cards { get; set; }
    public int CardPrice { get; set; }
}
