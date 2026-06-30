using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerRank : MonoBehaviour
{
    public TextMeshProUGUI rankTextMesh;
    public TextMeshProUGUI nicknameTextMesh;
    public TextMeshProUGUI scoreTextMesh;

    public void Initalize(string nickname, int identifier, int score, int rank)
    {
        rankTextMesh.text = $"{rank}.";
        nicknameTextMesh.text = $"{nickname}#{identifier.ToString().PadLeft(4,'0')}"; 
        scoreTextMesh.text = score.ToString();
    }
}
