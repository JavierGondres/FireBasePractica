using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	
	[SerializeField]
	private TMP_Text userName;
	
	[SerializeField]
	private TMP_Text userEmail;
	
	[SerializeField]
	public TMP_InputField passwordUpdateInput;
	
	[SerializeField]
	public TMP_Text passwordUpdateMsg;
	
	private void Awake()
	{
		CreateInstance();
	}

	private void CreateInstance()
	{
		if(instance == null)
		{
			instance = this;
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
	    ShowUserInfo();
    }

	private void ShowUserInfo()
	{
		userName.text = References.username;
		userEmail.text = References.email;
	}
	
	public void CerrarSesion()
	{
		FireBaseInit.instance.SignOut();
	}
	
	public void TryUpdatePassword()
	{
		FireBaseInit.instance.UpdatePassword();
	}
}
