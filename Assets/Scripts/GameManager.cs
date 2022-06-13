using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
	[SerializeField]
	private TMP_Text welcomeText;
	
    // Start is called before the first frame update
    void Start()
    {
	    ShowWelcomeText();
    }

	private void ShowWelcomeText()
	{
		welcomeText.text = $"Welcome {References.username} to our Game Scene";
	}
	
	public void CerrarSesion()
	{
		FireBaseInit.instance.SignOut();
	}
}
