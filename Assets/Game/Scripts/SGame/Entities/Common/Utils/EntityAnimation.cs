using UnityEngine;
using System.Collections.Generic;

namespace SGame.Entities.Common.Utils
{   
    /// <summary>
    /// Father class for animations. It is used by entities.
    /// </summary>
    public class EntityAnimation : MonoBehaviour
    {
        protected bool _isPaused = false;

        public virtual void Start()
        {
            GameManager.SINGLETON.PauseEvent += Pause;
        }

        protected void Pause(bool pause)
        {
            _isPaused = pause;
        }

    }
}
