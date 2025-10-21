using UnityEngine;

namespace GMSoft.Game
{
    public class GDGameModule : GameModuleBase
    {
        public override void GamePlayAgain()
        {
            Debug.Log("[GameDistribution] ===> Gameplay Again");
        }

        public override void GameplayPause()
        {
            Debug.Log("[GameDistribution] ===> Gameplay Pause");
        }

        public override void GameplayResume()
        {
            Debug.Log("[GameDistribution] ===> Gameplay Resume");
        }

        public override void GameplayStart()
        {
            Debug.Log("[GameDistribution] ===> Gameplay Start");
        }

        public override void GameplayStop()
        {
            Debug.Log("[GameDistribution] ===> Gameplay Stop");
        }
    }
}
