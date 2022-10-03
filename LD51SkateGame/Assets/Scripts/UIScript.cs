using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIScript : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject comboList;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject moveTextPrefab;
    private int listLength;

    private void Start()
    {
        Time.timeScale = 0;
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

    public void StartGame()
    {
        Menu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
