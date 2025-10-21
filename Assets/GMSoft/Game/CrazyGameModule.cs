using CrazyGames;
using UnityEngine;

namespace GMSoft.Game
{
    public class CrazyGameModule : GameModuleBase
    {
        public override void GamePlayAgain()
        {
            Debug.Log("[CrazyGames] ===> Gameplay Again");
            CrazySDK.Game.GameplayStart();
        }

        public override void GameplayPause()
        {
            Debug.Log("[CrazyGames] ===> Gameplay Pause");
            CrazySDK.Game.GameplayStop();
        }

        public override void GameplayResume()
        {
            Debug.Log("[CrazyGames] ===> Gameplay Resume");
            CrazySDK.Game.GameplayStart();
        }

        public override void GameplayStart()
        {
            Debug.Log("[CrazyGames] ===> Gameplay Start");
            CrazySDK.Game.GameplayStart();
        }

        public override void GameplayStop()
        {
            Debug.Log("[CrazyGames] ===> Gameplay Stop");
            CrazySDK.Game.GameplayStop();
        }
    }
}
