using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SearchView : PopupView
{
    public GameObject levelElementPrefab;
    public GameObject playerElementPrefab;
    public Transform container;
    public Transform radialLoading;

    public GameObject searchTips;
    public TMP_InputField inputField;

    private List<GameObject> listElements = new List<GameObject>();

    public void OnClickSearch()
    {
        ClearResults();
        searchTips.SetActive(listElements.Count == 0);

        string query = inputField.text;

        if(query.Length <= 3)
            return;

        if(query.Contains("#"))
        {
            //Search by nickname#0000
            SearchByPlayerIdentifier(query);
        }
        else if(query.Contains(" "))
        {
            //Search by levelname
            SearchByLevelName(query);
        }
        else
        {
            //Search by levelId
            SearchByLevelId(query);
        }
    }

    private void SearchByPlayerIdentifier(string query)
    {
        string[] segments = query.Split('#');

        if(segments.Length != 2)
            return;

        query = $"{segments[0]}/{segments[1]}";

        SearchAPI.SearchUser(query, (success, response)=>
        {
            if(!success)
                return;

            foreach(ProfileInfoDTO profileInfo in response.profiles)
            {
                SpawnPlayer(profileInfo);
            }

            searchTips.SetActive(listElements.Count == 0);
            radialLoading.SetAsLastSibling();
        }, true);
    }

    private void SearchByLevelName(string query)
    {
        string[] segments = query.Split(' ');

        if(segments.Length != 2)
            return;

        query = $"{segments[0].FirstLetterToUpper()}%20{segments[1].FirstLetterToUpper()}";

        SearchAPI.SearchLevel(query, (success, response)=>
        {
            if(!success)
                return;

            foreach(LevelInfoDTO levelInfo in response.levels)
            {
                SpawnLevel(levelInfo);
            }

            searchTips.SetActive(listElements.Count == 0);
            radialLoading.SetAsLastSibling();
        }, true);
    }

    private void SearchByLevelId(string query)
    {
        SearchAPI.SearchLevel(query, (success, response)=>
        {
            if(!success)
                return;

            foreach(LevelInfoDTO levelInfo in response.levels)
            {
                SpawnLevel(levelInfo);
            }

            searchTips.SetActive(listElements.Count == 0);
            radialLoading.SetAsLastSibling();
        }, true);
    }

    private void SpawnLevel(LevelInfoDTO levelInfo, MenuLocation exitLocation = MenuLocation.MainMenu)
    {
        GameObject last = Instantiate(levelElementPrefab, container);
		listElements.Add(last);
        LevelElement element = last.GetComponent<LevelElement>();
        element.Initalize(-1, levelInfo, exitLocation, true, false);
    }

    private void SpawnPlayer(ProfileInfoDTO profileDTO)
    {
        GameObject last = Instantiate(playerElementPrefab, container);
        listElements.Add(last);
        ProfileElement element = last.GetComponent<ProfileElement>();
        element.Initalize(profileDTO);
    }

    private void ClearResults()
    {
        List<GameObject> toDestroy = new List<GameObject>(listElements);

        for(int i = listElements.Count-1; i >= 0; i--)
        {
            Destroy(toDestroy[i]);
        }

        listElements = new List<GameObject>();
    }

    public void ClearSearch()
    {
        inputField.text = "";
    }

    public void OnClickClose()
    {
        base.Exit();
    }
}
