using UnityEngine;

namespace GMSoft.Game
{
    public class GMGameModule : GameModuleBase
    {
        public override void GamePlayAgain() 
        {
            Debug.Log("[GMSoft] ===> Gameplay Again");
        }

        public override void GameplayPause() 
        {
            Debug.Log("[GMSoft] ===> Gameplay Pause");
        }

        public override void GameplayResume() 
        {
            Debug.Log("[GMSoft] ===> Gameplay Resume");
        }

        public override void GameplayStart() 
        {
            Debug.Log("[GMSoft] ===> Gameplay Start");
        }

        public override void GameplayStop() 
        {
            Debug.Log("[GMSoft] ===> Gameplay Stop");
        }
    }
}
