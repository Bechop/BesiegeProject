using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MainMenuManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        AsyncOperationHandle<StringList> handle;

        string path = Application.persistentDataPath;

        handle = Addressables.LoadAssetAsync<StringList>("SceneList");
        handle.Completed += _ => 
        {
            foreach (string sceneName in handle.Result.list)
            {
                GameObject go = Instantiate(buttonPrefab, parent);
                go.GetComponent<CustomButton>().onClick.AddListener(() => LoadLevel(sceneName));
                go.GetComponentInChildren<TextMeshProUGUI>().text = sceneName;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadLevel(string _name)
    {
        Addressables.LoadSceneAsync(_name);
    }
}
