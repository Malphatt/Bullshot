using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour {

    public GameObject ControlsBook;
    public GameObject CreditsBook;

    public void StartButton() {
        SceneManager.LoadScene("Game");
    }
    public void ControlsButton() {
        ControlsBook.SetActive(true);
        CreditsBook.SetActive(false);
    }
    public void CreditsButton() {
        ControlsBook.SetActive(false);
        CreditsBook.SetActive(true);
    }
    public void QuitButton() {
        Application.Quit();
    }
    public void BackButton() {
        ControlsBook.SetActive(false);
        CreditsBook.SetActive(false);
    }
}
