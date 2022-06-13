using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
	[SerializeField]
	private Text welcomeText;
	
    // Start is called before the first frame update
    void Start()
    {
	    ShowWelcomeText();
    }

	private void ShowWelcomeText()
	{
		welcomeText.text = $"Welcome {References.username} to our Game Scene";
	}
}
