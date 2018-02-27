using SGame.Entities.Common;
using UnityEngine;


/// <summary>
/// Class in charge of the GUI management. It has functionality for every GUI button and only has communication with GameManager 
/// <seealso cref="GameManager"/>
/// </summary>
public class GUIController : MonoBehaviour {

    #region Private variables

    private bool _gamePaused = false;
    private int _assignedCharacters = 0;

    #endregion

    #region Serialize Fields
    
    [SerializeField]private bool playableScene;

    [SerializeField]private UnityEngine.UI.Image[] characterSlots;    
    [SerializeField]private UnityEngine.UI.Text ammo;
    [SerializeField]private UnityEngine.UI.Text score;
    [SerializeField]private UnityEngine.UI.Image pauseButton;

    [SerializeField]private GameObject pauseScreen = null;
    [SerializeField]private GameObject characterSlotsFolder;
    [SerializeField]private GameObject gameScreen;
    [SerializeField]private GameObject defeatScreen;
    [SerializeField]private GameObject victoryScreen;
    [SerializeField]private GameObject perfectVictoryScreen;

    #endregion

    #region Event Functions

    /// <summary>
    /// Called once per frame it just will check if Escape key has been pressed and will enter in Pause state if so.
    /// </summary>
    void Update () {
        if (playableScene && Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Method for Play button, used only in the Menu scene. It launches the Game scene.
    /// </summary>
    public void Play()
    {
        Application.LoadLevel("Game");
    }

    /// <summary>
    /// Method for Next Level button used only in the Game scene after a victory. Calls GameManager.NextLevel() .
    /// <seealso cref="GameManager.NextLevel"/>
    /// </summary>
    public void NextLevel()
    {
        GameManager.SINGLETON.NextLevel();
    }

    /// <summary>
    /// Method for Retry button used only in the Game scene after a defeat. Calls GameManager.Restart() .
    /// <seealso cref="GameManager.Restart"/>
    /// </summary>
    public void Restart()
    {
        GameManager.SINGLETON.Restart();
    }

    /// <summary>
    /// Method for Exit button used in both Game and Menu scenes. It stops the application.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Method for Pause button used only in Game Scene. It actives the Pause Scren GUI elements, as well as calls GameManager.Pause()
    /// <seealso cref="GameManager.Pause"/>
    /// </summary>
    public void Pause()
    {
        _gamePaused = !_gamePaused;
        GameManager.SINGLETON.Pause(_gamePaused);
        pauseScreen.SetActive(_gamePaused);

        if (_gamePaused)
            pauseButton.enabled = false;      
        else
            pauseButton.enabled = true;
    }


    /// <summary>
    /// Method called by GameManager when any character is either free or killed. It assigns that character one of the 
    /// top-right slots reserved for showing the player how many characters are out of the game at the moment .
    /// Depending on the param "alive", it will show an "Alive sprite" or a "Dead Sprite"  
    /// </summary>
    /// <param name="character">Character to assign</param>
    /// <param name="alive">Whether it is alive or not</param>
    public void AssignCharacter(MovableEntity character,bool alive)
    {
        characterSlots[_assignedCharacters].sprite = alive ? character.SpriteData.sprite : character.DeadSprite;
        characterSlots[_assignedCharacters].gameObject.SetActive(true);
        _assignedCharacters++;        
    }

    /// <summary>
    /// Method called by GameManager to modify the Ammo GUI text.
    /// </summary>
    /// <param name="ammunition">Ammunition to show</param>
    public void SetAmmo(int ammunition)
    {
        ammo.text = "x " + ammunition.ToString() ;
    }

    /// <summary>
    /// Method called by GameManager to modify the Score GUI text.
    /// </summary>
    /// <param name="num">Score to show</param>
    public void SetScore(int num)
    {
        score.text = "Score  " + num.ToString();
    }

    /// <summary>
    /// Method called by GameManager when a perfect victory is achieved to activate every PerfectVictoryScreen elements.
    /// </summary>
    public void ActivePerfectVictoryScreen()
    {
        gameScreen.SetActive(false);
        perfectVictoryScreen.SetActive(true);
        characterSlotsFolder.GetComponent<RectTransform>().localPosition = new Vector3(0, -95, 0);
    }

    /// <summary>
    /// Method called by GameManager when a victory is achieved to activate every VictoryScreen elements.
    /// </summary>
    public void ActiveVictoryScreen()
    {
        gameScreen.SetActive(false);
        victoryScreen.SetActive(true);
        characterSlotsFolder.GetComponent<RectTransform>().localPosition = new Vector3(0, -95, 0);
    }

    /// <summary>
    /// Method called by GameManager when the player is defeated to activate every PerfectVictoryScreen elements.
    /// </summary>
    public void ActiveDefeatScreen()
    {
        gameScreen.SetActive(false);
        defeatScreen.SetActive(true);
        characterSlotsFolder.GetComponent<RectTransform>().localPosition = new Vector3(0, -95, 0);
    }

    #endregion
}
