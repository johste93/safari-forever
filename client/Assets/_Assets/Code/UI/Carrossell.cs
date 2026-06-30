using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;
using TMPro;

public class Carrossell : MonoBehaviour
{
	public ShopCanvas shopCanvas;
    public Vector3 left;
    public Vector3 right;
    public float yOffset = -1f;
    public GameObject cagePrefab;
    public GameObject shadowPrefab;
    public TextMeshProUGUI textMeshPro;

    public GameObject currentCage;

    private int currentIndex;

    public GameObject[] characters;

    private float spinDuration = 0.3f;
    private float slowStartDuration = 0.5f;

    private GameObject currentCharacter;

    private float lastSpawnTime;

    public delegate void CarrossellEvent();
    public CarrossellEvent On_CharacterChanged;

    private List<Tween> tweens = new List<Tween>();

    private float cooldown;

    private void OnEnable()
    {
        Show();
    }

    public void Show()
    {
        currentIndex = SaveManager.currentSave.currentCharacter;
        
        if(!SaveManager.currentSave.unlockedHats[SaveManager.currentSave.currentHat])
            SaveManager.currentSave.currentHat = (int) Hat.None;

        SpawnCharacter(currentIndex, left, true);
    }

    public void Hide()
    {
        if(currentCharacter == null)
            return;

        KillAllTweens();

        Destroy(currentCharacter);
    }

    private void OnDisable()
    {
        if(currentCharacter != null)
            Destroy(currentCharacter);
    }

    public void Next()
    {
        if(cooldown > 0f)
            return;

        cooldown = spinDuration;

        DespawnCharacter(left);

        currentIndex++;
        if(currentIndex >= characters.Length)
            currentIndex = 0;

        SpawnCharacter(currentIndex, right);
    }

    public void Previous()
    {
        if(cooldown > 0f)
            return;

        cooldown = spinDuration;

        DespawnCharacter(right);

        currentIndex--;
        if(currentIndex < 0)
            currentIndex = characters.Length-1;

        SpawnCharacter(currentIndex, left);
    }

    public void NextHat()
    {
        do
        {
            SaveManager.currentSave.currentHat++;
            if(SaveManager.currentSave.currentHat >= System.Enum.GetNames(typeof(Hat)).Length)
                SaveManager.currentSave.currentHat = 0;

        } 
        while(!SaveManager.currentSave.unlockedHats[SaveManager.currentSave.currentHat]);

        SaveManager.Save();

        currentCharacter.GetComponentInChildren<HatController>().SetHat((Hat)SaveManager.currentSave.currentHat);
    }

    public void PreviousHat()
    {
        do
        {
            SaveManager.currentSave.currentHat--;
            if(SaveManager.currentSave.currentHat < 0)
                SaveManager.currentSave.currentHat = System.Enum.GetNames(typeof(Hat)).Length-1;
        }
        while(!SaveManager.currentSave.unlockedHats[SaveManager.currentSave.currentHat]);

        SaveManager.Save();

        currentCharacter.GetComponentInChildren<HatController>().SetHat((Hat)SaveManager.currentSave.currentHat);
    }

    public void SetIndex(int characterIndex)
    {
        this.currentIndex = characterIndex;
        DespawnCharacter(right);
        SpawnCharacter(characterIndex, left, false);
    }

    private void SpawnCharacter(int characterIndex, Vector3 start, bool slowSpawn = false)
    {
        textMeshPro.text = ((Animal)currentIndex).ToString();
        textMeshPro.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, textMeshPro.font, SaveManager.currentSave.language);
        
        if(On_CharacterChanged != null)
            On_CharacterChanged();
            
        lastSpawnTime = Time.time;

        GameObject newCharacter = Instantiate(characters[characterIndex], start, Quaternion.identity, transform);
        newCharacter.GetComponent<FSM_CharacterController>().enabled = false;

        if(!SaveManager.currentSave.unlockedCharacter[characterIndex])
        {
            currentCage = Instantiate(cagePrefab, newCharacter.transform);
        }

        GameObject shadow = Instantiate(shadowPrefab, newCharacter.transform.position + new Vector3(0f, 0.16f, 0f), Quaternion.identity, newCharacter.transform);
        newCharacter.transform.localScale = Vector3.zero;
        newCharacter.SetActive(true);

		shadow.GetComponentInChildren<ShopSpotlight>().Initalize(shopCanvas);

        tweens.Add(newCharacter.transform.DOMoveX(Vector3.Lerp(left, right, 0.5f).x, slowSpawn ? slowStartDuration : spinDuration).SetEase(Ease.InQuad));
        tweens.Add(newCharacter.transform.DOMoveY(Vector3.Lerp(left, right, 0.5f).y + yOffset, slowSpawn ? slowStartDuration : spinDuration).SetEase(Ease.OutQuad));
        tweens.Add(newCharacter.transform.DOScale(Vector3.one, slowSpawn ? slowStartDuration : spinDuration).SetEase(Ease.OutQuad));

        currentCharacter = newCharacter;
        currentCharacter.GetComponentInChildren<HatController>().SetHat((Hat)SaveManager.currentSave.currentHat);
    }

    private void DespawnCharacter(Vector3 end)
    {   
        float timeSinceLastTime = Time.time -lastSpawnTime;
        if(currentCharacter != null)
        {
            float timeLeftOnPreviousSpin = Mathf.Max(0, spinDuration-timeSinceLastTime);
            tweens.Add(currentCharacter.transform.DOMoveX(end.x, spinDuration).SetEase(Ease.OutQuad).SetDelay(timeLeftOnPreviousSpin));
            tweens.Add(currentCharacter.transform.DOMoveY(end.y, spinDuration).SetEase(Ease.InQuad).SetDelay(timeLeftOnPreviousSpin));
            tweens.Add(currentCharacter.transform.DOScale(Vector3.zero, spinDuration).SetEase(Ease.InQuad).SetDelay(timeLeftOnPreviousSpin));
            //currentCharacter.transform.localScale = new Vector3(Mathf.Abs(currentCharacter.transform.localScale.x) * Mathf.Sign(end.x), currentCharacter.transform.localScale.y, currentCharacter.transform.localScale.z);
            Destroy(currentCharacter, spinDuration + timeLeftOnPreviousSpin);
            
            currentCharacter = null;
        }
    }

    private void Update()
    {
        if(cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

	private void KillAllTweens()
	{
		foreach(Tween t in tweens)
        {
            if(t != null)
                t.Kill();
        }

        tweens = new List<Tween>();
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}
}
