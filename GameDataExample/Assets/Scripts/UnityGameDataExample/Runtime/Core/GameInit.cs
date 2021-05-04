namespace UnityGameDataExample.Runtime.Core
{
    using Craiel.UnityEssentials.Runtime.Enums;
    using Craiel.UnityEssentials.Runtime.Event;
    using Craiel.UnityEssentials.Runtime.Scene;
    using Enums;
    using Events;
    using UnityEngine;

    public class GameInit : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameObject StaticContent;

        public virtual void Start()
        {
            // Bring up the main systems
            GameCore.InstantiateAndInitialize();

            if (this.StaticContent != null)
            {
                SceneObjectController.Instance.RegisterObjectAsRoot(SceneRootCategory.Static, this.StaticContent, true);
            }

            GameEvents.Send(new EventSwitchGameState(GameMasterState.MainMenu));
        }
    }
}
