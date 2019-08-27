using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    
    public InputField emailInput, passwordInput;
    public Button emailSignUp, emailSignIn, googleSignUp, googleSignIn, playButton, signOut, reset;
    public GameObject messagePanel;
    public Text messageDisplay;

    private const string EMAIL_KEY = "email_key";
    private const string PASSWORD_KEY = "password_key";

    private void Start()
    {
        Init();
    }

    private void Init() 
    {
        string savedEmail = PlayerPrefs.GetString(EMAIL_KEY, "");
        string savedPassword = PlayerPrefs.GetString(PASSWORD_KEY, "");

        emailInput.text = savedEmail;
        passwordInput.text = savedPassword;

        // add listener ke buttons
        emailSignUp.onClick.AddListener(()=> {
            AuthManager.SignUpWithEmailAndPassword(emailInput.text, passwordInput.text, ()=>{
                PlayerPrefs.SetString(EMAIL_KEY, emailInput.text);
                PlayerPrefs.SetString(PASSWORD_KEY, passwordInput.text);

                string m = string.Format("Sign up berhasil. User id : {0}", FD.user.UserId);
                ShowMessagePanel(m, () => {
                   // SceneManager.LoadScene(1);
                });
            });
        });

        emailSignIn.onClick.AddListener(() =>
        {
            AuthManager.SignInWithEmailAndPasswordWithCredential(emailInput.text, passwordInput.text, () =>
            {
                PlayerPrefs.SetString(EMAIL_KEY, emailInput.text);
                PlayerPrefs.SetString(PASSWORD_KEY, passwordInput.text);

                string m = string.Format("Sign in berhasil. User id : {0}", FD.user.UserId);
                ShowMessagePanel(m, () =>
                {
                   // SceneManager.LoadScene(1);
                });
            });
        });

        googleSignUp.onClick.AddListener(() =>
        {
            AuthManager.SignUpWithGoogleWithCredential(() =>
            {
                string m = string.Format("Sign up via Google berhasil. User id : {0}", FD.user.UserId);
                ShowMessagePanel(m, () =>
                {
                   // SceneManager.LoadScene(1);
                });
            });
        });

        googleSignIn.onClick.AddListener(()=> {
            AuthManager.SignInWithGoogleWithCredential(()=>{
                string m = string.Format("Sign in via Google berhasil. User id : {0}", FD.user.UserId);
                ShowMessagePanel(m, () =>
                {
                   // SceneManager.LoadScene(1);
                });
            });
        });


        signOut.onClick.AddListener(() => {
            AuthManager.SignOut();
        });

        reset.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    // fungsi buat ngebuka panel window
    public void ShowMessagePanel(string message, UnityAction playAction)
    {
        messagePanel.SetActive(true);
        messageDisplay.text = message;
        playButton.onClick.AddListener(playAction);
    }

}
