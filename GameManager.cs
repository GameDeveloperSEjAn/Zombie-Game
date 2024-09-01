using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public StarterAssetsInputs _inputs;
    private void Start()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
        {
            _inputs = _player.GetComponent<StarterAssetsInputs>();
        }
    }
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        UnlockCursor();
    }
    public void Restart()
    {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex );
        _inputs.cursorLocked = true;
        _inputs.SetCursorState(true);
    }
    public void Quit()
    {
        Application.Quit ();
    }
    private void UnlockCursor()
    {
        if (_inputs  != null)
        {
            _inputs .cursorLocked = false;
            _inputs.SetCursorState(false);
        }
    }
}
