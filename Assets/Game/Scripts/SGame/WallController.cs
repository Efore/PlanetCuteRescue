using UnityEngine;
using SGame.Entities.Common;


/// <summary>
/// Component for lateral walls to manage the behaviour when are collided by Enemies or Characters.
/// <seealso cref="GameManager.RearrangeEnemy"/>
/// <seealso cref="GameManager.AddToAvailableCharacters"/>
/// </summary>
public class WallController : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            GameManager.SINGLETON.RearrangeEnemy(other.gameObject);
        else if(other.tag == "Character")
        {
            other.gameObject.SetActive(false);
            GameManager.SINGLETON.AddToAvailableCharacters(other.gameObject);
        }
    }
    
}
