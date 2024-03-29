﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using Google;

public class AuthManager : MonoBehaviour
{
    private void Awake() 
    {
        CheckFirebaseDependencies();    
    }

    void InitializeFirebase()
    {
        FD.auth = FirebaseAuth.DefaultInstance;
        FD.auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (FD.auth.CurrentUser != FD.user)
        {
            bool signedIn = 
            FD.user != FD.auth.CurrentUser && FD.auth.CurrentUser != null;
            
            if(!signedIn && FD.user != null)
            {
                Debug.Log("Signed out "+FD.user.UserId);
            }

            FD.user = FD.auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in "+FD.user.UserId);
            }
        }
    }

    

    private void CheckFirebaseDependencies()
    {
        Firebase.FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp
                    FD.app = Firebase.FirebaseApp.DefaultInstance;
                    Debug.Log("Firebase ready to use.");
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Dependencies not found, " +dependencyStatus);
                }
            });
    }

    private static void SignInWithCredential(Credential credential, Action onSignedIn)
    {
        if (FD.auth == null)
        {
            Debug.Log("Auth not initialized yet. Cancelling login.");
            return;
        }

        FD.auth
            .SignInWithCredentialAsync(credential)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Sign in with credential async cancelled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.Log("Sign in with credential sync ecnountered an error : " + task.Exception);
                    return;
                }

                // To do when user successfully signed up
                FD.user = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                FD.user.DisplayName, FD.user.UserId);
                FD.credential = credential;
                onSignedIn.Invoke();
            });
    }

    public static void LinkWithBaseCredential(Credential credential, Action onCredentialLinked)
    {
        FD.auth.CurrentUser
            .LinkWithCredentialAsync(credential)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("Linking credential canceled.");
                }
                if (task.IsFaulted)
                {
                    Debug.Log("Linking credential faulted. " + task.Exception);
                }

                FD.user = task.Result;
                Debug.LogFormat("CustomRenderTextureInitializationSource successfully linke to firebase user {0} {1}", FD.user.DisplayName, FD.user.UserId);
                onCredentialLinked.Invoke();
            });
    }

    public static void SignUpWithEmailAndPassword(string email, string password, Action onSignedUp)
    {
        FD.auth
            .CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task => {
                if (task.IsCanceled) 
                {
                    Debug.LogError("Sign up with email and password canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("Sign up with email and password encountered an error: "+task.Exception);
                    return;
                }

                // To do when user successfully signed up
                FD.user = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                FD.user.DisplayName, FD.user.UserId);

                Credential credential = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);

                if(FD.credential == null)
                {
                    FD.credential = credential;    
                }
                else
                {
                    LinkWithBaseCredential(credential, () =>{
                        onSignedUp.Invoke();
                    });
                }
                onSignedUp.Invoke();
            });
    }

    public static void SignUpWithGoogleWithCredential(Action onSignedIn)
    {

        if (GoogleSignIn.Configuration == null)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = "66234377928-p475e3snu11tn0iiprtnrrbkosh74p3g.apps.googleusercontent.com"
            };
        }

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
                return;
            }
            if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
                return;
            }
            // Get credential here
            Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
            Debug.Log("Google credential retrieved.");

            if(FD.credential == null)
            {
                AuthManager.SignInWithCredential(credential, () =>
                {
                    onSignedIn.Invoke();
                });
            }
            else
            {
                LinkWithBaseCredential(credential, () => {
                    onSignedIn.Invoke();
                });
            }
        });
    }

    public static void SignInWithEmailAndPasswordWithCredential(string email, string password, Action onSignedIn)
    {
        Credential credential = 
        Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
        if(FD.credential == null)
        {
            AuthManager.SignInWithCredential(credential, () =>
            {
                onSignedIn.Invoke();
            });
        }
        else
        {
            LinkWithBaseCredential(credential, () => {
                onSignedIn.Invoke();
            });
        }
        
    }

    public static void SignInWithGoogleWithCredential(Action onSignedIn)
    {
        if (GoogleSignIn.Configuration == null)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = "66234377928-p475e3snu11tn0iiprtnrrbkosh74p3g.apps.googleusercontent.com"
            };
        }

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
                return;
            }
            if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
                return;
            }
            // Get credential here
            Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);

            if(FD.credential != null)
            {
                AuthManager.SignInWithCredential(credential, () =>
                {
                    onSignedIn.Invoke();
                });
            }
            else
            {
                LinkWithBaseCredential(credential, ()=>{
                    onSignedIn.Invoke();
                });
            }
        });
    }

    public static void SignOut()
    {
        if(FD.auth == null)
        {
            Debug.Log("Already signed out.");
            return;
        }
        FD.auth.SignOut();
        FirebaseAuth.DefaultInstance.SignOut();
    }
}
