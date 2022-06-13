using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	[SerializeField]
	private GameObject loginPanel;

	[SerializeField]
	private GameObject registrationPanel;
	
	[SerializeField]
	private GameObject emailVerificationPanel;
	
	[SerializeField]
	private TMP_Text emailVerificationText;
	
	[SerializeField]
	public TMP_Text loginErrorMessage;
	
	[SerializeField]
	public TMP_Text registerErrorMessage;

	private void Awake()
	{
		CreateInstance();
	}

	private void CreateInstance()
	{
		if(Instance == null)
		{
			Instance = this;
		}
	}

	public void OpenLoginPanel()
	{
		loginPanel.SetActive(true);
		registrationPanel.SetActive(false);
		emailVerificationPanel.SetActive(false);
	}

	public void OpenRegistrationPanel()
	{
		registrationPanel.SetActive(true);
		loginPanel.SetActive(false);
	}
	
	private void ClearUI()
	{
		registrationPanel.SetActive(false);
		loginPanel.SetActive(false);
		emailVerificationPanel.SetActive(false);
	}
	
	public void ShowVerificationResponse(bool isEmailSent, string emailId, string errorMessage)
	{
		ClearUI();
		emailVerificationPanel.SetActive(true);
		
		if(isEmailSent)
		{
			emailVerificationText.text = $"Please verify your email adress \n Verification email has been sent to {emailId}";
		}
		
		else
		{
			emailVerificationText.text = $"Couldn't sent email: {errorMessage}";
		}
	}
	
	public void ShowErrorMassage(TMP_Text massage, string errorMessage)
	{
		massage.text = $"Error: {errorMessage}";
	}
	
		
}

