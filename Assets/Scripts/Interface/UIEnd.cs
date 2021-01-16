using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIEnd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDialog()
    {
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PlayerManager.Instance.SetFlag(PlayerController.Option.MOVE, false);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ROTATION, false);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ACTION, false);
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}
