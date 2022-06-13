using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

public class FireBaseInit : MonoBehaviour
{

	//Firebase variables
	[Header("Firebase")]
	public DependencyStatus dependencyStatus;
	public FirebaseAuth auth;
	public FirebaseUser user;
	
	
	//Login Variables
	[Space]
	[Header("Login")]
	public InputField emailLoginField;
	public InputField passwordlLoginField;
	
	//Registration Variables
	[Space]
	[Header("Registration")]
	public InputField nameRegisterField;
	public InputField emailRegisterField;
	public InputField passwordRegisterField;
	public InputField confirmPasswordRegisterField;
	
	//Instance the class
	public static FireBaseInit instance;
	
	
	private void CreateInstance()
	{
		if(instance == null)
		{
			instance = this;
		}
	}
	
	
	private void Start()
	{
		StartCoroutine(CheckAndFixDependenciesAsync());
	}
	
	private void Awake()
	{
		//Check that all of the necessary dependencies for firebase are present on the system
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>{
			
			dependencyStatus = task.Result;
			
			if(dependencyStatus == DependencyStatus.Available)
			{
				InitializeFirebase();
			}
			
			else
			{
				Debug.LogError("Could not resolve all firebase dependencies" + dependencyStatus);
			}
		});
	
		CreateInstance();
	}
	
	private IEnumerator CheckAndFixDependenciesAsync()
	{
		var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
		
		yield return new WaitUntil(() => dependencyTask.IsCompleted);
		dependencyStatus = dependencyTask.Result;
			
		if(dependencyStatus == DependencyStatus.Available)
		{
			InitializeFirebase();
			yield return new WaitForEndOfFrame();
			StartCoroutine(CheckFourAutoLogin());
		}
			
		else
		{
			Debug.LogError("Could not resolve all firebase dependencies" + dependencyStatus);
		}
	}
	
	
	void InitializeFirebase()
	{
		//Set the default instance object
		auth = FirebaseAuth.DefaultInstance;
		
		auth.StateChanged += AuthStateChanged;
		AuthStateChanged(this,null);
	}
	
	private IEnumerator CheckFourAutoLogin()
	{
		if(user != null)
		{
			var reloadUser = user.ReloadAsync();
			
			yield return new WaitUntil(() => reloadUser.IsCompleted);
			
			AutoLogin();
		}
		
		else
		{
			UIManager.Instance.OpenLoginPanel();
		}
	}
	
	private void AutoLogin()
	{
		if(user != null)
		{
			if(user.IsEmailVerified)
			{
				References.username = user.DisplayName;
				UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
			}
			
			else
			{
				SendEmailForVerification();
			}
		}
		else
		{
			UIManager.Instance.OpenLoginPanel();
		}
	}
	
	void AuthStateChanged(object sender, System.EventArgs eventArgs)
	{
		if (auth.CurrentUser != user)
		{
			bool signedIn =user != auth.CurrentUser && auth.CurrentUser != null;
			
			if(!signedIn && user != null)
			{
				Debug.Log("Signed out" + user.UserId);
			}
			
			user = auth.CurrentUser;
			
			if(signedIn)
			{
				Debug.Log("Signed in" + user.UserId);
			}
		}
	}
	
	public void Login()
	{
		StartCoroutine(LoginAsync(emailLoginField.text, passwordlLoginField.text));
	}
	
	private IEnumerator LoginAsync(string email, string password)
	{
		var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
		
		yield return new WaitUntil(() => loginTask.IsCompleted);
		
		if (loginTask.Exception != null)
		{
			Debug.LogError(loginTask.Exception);
			
			FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
			AuthError authError = (AuthError)firebaseException.ErrorCode;
			
			string failedMessage = "Login Failed! Because ";
			
			switch(authError)
			{
				case AuthError.InvalidEmail:
					failedMessage += "Email is invalid";
					break;
				
				case AuthError.WrongPassword:
					failedMessage += "Wrong Password";
					break;
					
				case AuthError.MissingEmail:
					failedMessage += "Email is missing";
					break;
				
				case AuthError.MissingPassword:
					failedMessage += "Password is missing";
					break;
					
				default:
					failedMessage += "Login Failed";
					break;
			}
			
			Debug.Log(failedMessage);
			
			//Muestro el error en la pantalla
			UIManager.Instance.ShowErrorMassage(UIManager.Instance.loginErrorMessage, failedMessage);			
		}
		
		else
		{
			user = loginTask.Result;
			
			Debug.LogFormat("{0} You Are Succesfully Logged In", user.DisplayName);
			
			if(user.IsEmailVerified)
			{
				References.username = user.DisplayName;
				UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
			}
			else
			{
				SendEmailForVerification();
			}
			
		}
	}
	
	public void Register()
	{
		StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
	}
	
	private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
	{
		string errorMessage = "";
		
		if(name == "")
		{
			Debug.LogError("User Name is Empty");
			errorMessage += "User Name is Empty";
			
			UIManager.Instance.ShowErrorMassage(UIManager.Instance.registerErrorMessage, errorMessage);	
		}
		
		else if (email == "")
		{
			Debug.LogError("email field is empty");
			errorMessage += "email field is empty";
			
			UIManager.Instance.ShowErrorMassage(UIManager.Instance.registerErrorMessage, errorMessage);	
		}
		
		else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
		{
			Debug.LogError("Password does not match");
			errorMessage += "Password does not match";
			
			UIManager.Instance.ShowErrorMassage(UIManager.Instance.registerErrorMessage, errorMessage);	
		}
		
				
		
		else
		{
			var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
			
			yield return new WaitUntil(() => registerTask.IsCompleted);
		
			if (registerTask.Exception != null)
			{
				Debug.LogError(registerTask.Exception);
			
				FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
				AuthError authError = (AuthError)firebaseException.ErrorCode;
			
				string failedMessage = "Register Failed! Because ";
			
				switch(authError)
				{
				case AuthError.InvalidEmail:
					failedMessage += "Email is invalid";
					break;
				
				case AuthError.WrongPassword:
					failedMessage += "Wrong Password";
					break;
					
				case AuthError.MissingEmail:
					failedMessage += "Email is missing";
					break;
				
				case AuthError.MissingPassword:
					failedMessage += "Password is missing";
					break;
					
				default:
					failedMessage += "Register Failed";
					break;
				}
			
				Debug.Log(failedMessage);
				
				//Muestro el error en la pantalla
				UIManager.Instance.ShowErrorMassage(UIManager.Instance.registerErrorMessage, failedMessage);			
			}
		
			else
			{
				// get the user After reigstration succes
				user = registerTask.Result;
			
				UserProfile userProfile= new UserProfile{DisplayName = name};
				
				var updateProfileTask = user.UpdateUserProfileAsync(userProfile);
				
				yield return new WaitUntil(() => registerTask.IsCompleted);
		
				if (updateProfileTask.Exception != null)
				{
					// Delete the user if user update failed
					user.DeleteAsync();
					
					Debug.LogError(updateProfileTask.Exception);
			
					FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
					AuthError authError = (AuthError)firebaseException.ErrorCode;
			
					string failedMessage = "Profile update Failed! Because";
					switch(authError)
					{
					case AuthError.InvalidEmail:
						failedMessage += "Email is invalid";
						break;
				
					case AuthError.WrongPassword:
						failedMessage += "Wrong Password";
						break;
					
					case AuthError.MissingEmail:
						failedMessage += "Email is missing";
						break;
				
					case AuthError.MissingPassword:
						failedMessage += "Password is missing";
						break;
					
					default:
						failedMessage += "Register Failed";
						break;
					}
			
					Debug.Log(failedMessage);
					
					//Muestro el error en la pantalla
					UIManager.Instance.ShowErrorMassage(UIManager.Instance.registerErrorMessage, failedMessage);			

				}
				else
				{
					Debug.Log("Registration Succesful Welcome" + user.DisplayName);
					if(user.IsEmailVerified)
					{
							    UIManager.Instance.OpenLoginPanel();
					}
					
					else
					{
						    SendEmailForVerification();
					}
				}
			}
		}
	}
	
	public void SendEmailForVerification()
	{
		StartCoroutine(SendEmailVerificationAsync());
	}
	
	private IEnumerator SendEmailVerificationAsync()
	{
		if(user != null)
		{
			var sendEmailTask = user.SendEmailVerificationAsync();
			
			yield return new WaitUntil(() => sendEmailTask.IsCompleted);
			
			if(sendEmailTask.Exception != null)
			{
				FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
				AuthError error = (AuthError)firebaseException.ErrorCode;
				
				string errorMessage = "Unknown Error: Please try again later";
				
				switch(error)
				{
					case AuthError.Cancelled:
									errorMessage = "Email verification was Cancelled";
												break;
											    
					case AuthError.TooManyRequests:
						errorMessage = "Too many Request";
						break;
							
					case AuthError.InvalidRecipientEmail:
						errorMessage = "Email Your Entered is Invalid";
						break;
				}
				
				UIManager.Instance.ShowVerificationResponse(false, user.Email, errorMessage);
			}
			else
			{
				Debug.Log("Email has succesfully sent");	
				UIManager.Instance.ShowVerificationResponse(true, user.Email, null);
			}
			
		}
	}
	
	public void SignOut()
	{
		auth.SignOut();
		UnityEngine.SceneManagement.SceneManager.LoadScene("FirebaseLogin");
	}
	
}
