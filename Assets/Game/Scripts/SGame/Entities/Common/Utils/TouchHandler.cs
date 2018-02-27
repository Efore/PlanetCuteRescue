using UnityEngine;
using System.Collections;

namespace SGame.Entities.Common.Utils
{
    /// <summary>
    /// Father class that is used by Entities to handle touch collisions. It also contains a definition for the Blink 
    /// method used by Enemies and Characters while dying.
    /// <seealso cref="Entity"/>
    /// </summary>
    public class TouchHandler : MonoBehaviour
    {
        [SerializeField]protected Entity owner;
        [SerializeField]protected float timeForDie;

        protected bool _isDying;
        protected float _secsAcum;

        public virtual void HandleOnTouchCollider(RaycastHit obj) { }

        protected void Blink()
        {
            owner.SpriteData.enabled = !owner.SpriteData.enabled;
        }
    }
}