using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScript : MonoBehaviour
{
    [SerializeField] private GameObject comboList;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject moveTextPrefab;
    private int listLength;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AddToComboList(string str)
    {
        var text = Instantiate(textPrefab, comboList.transform);
        text.GetComponent<TextMeshProUGUI>().text = str;
    }

    public void AddMovesToComboList(List<string> strings)
    {
        foreach(string str in strings)
        {
            var text = Instantiate(moveTextPrefab, comboList.transform);
            text.GetComponent<TextMeshProUGUI>().text = str;
        }
    }

    public void ClearComboList()
    {
        foreach(Transform child in comboList.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
