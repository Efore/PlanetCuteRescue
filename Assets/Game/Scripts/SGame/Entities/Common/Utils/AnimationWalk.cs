using UnityEngine;
using System.Collections;

namespace SGame.Entities.Common.Utils
{
    /// <summary>
    /// Component that simulates walking oscilation. It inherits from EntityAnimation
    /// <seealso cref="EntityAnimation"/>
    /// </summary>
    public class AnimationWalk : EntityAnimation
    {
        [SerializeField]private float walkingYrange = 0.005f;
        [SerializeField]private float walkingStep = 15.0f;

        // Update is called once per frame
        void Update()
        {
            if(!_isPaused)
                transform.position += new Vector3(0, Mathf.Sin(Time.time * walkingStep) * walkingYrange, 0);
        }
    }
}
