using UnityEngine;
using System.Collections;

/// <summary>
/// Class responsible for loading the highest score from PlayerPrefs.
/// Used only in the Menu scene.
/// </summary>
public class LoadHighScore : MonoBehaviour {

	
	/// <summary>
    /// It is called just before the first frame. If exists, the highest score is assigned to the 
    /// corresponding High Score UI text.
    /// </summary>
    void Start () {
        string highScore = "HIGH SCORE  ";
        if (PlayerPrefs.HasKey("HighScore"))
            highScore += PlayerPrefs.GetInt("HighScore");
        else
            highScore += "0";

        gameObject.GetComponent<UnityEngine.UI.Text>().text = highScore;
	}
	
}
