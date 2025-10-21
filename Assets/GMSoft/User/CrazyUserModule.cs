using CrazyGames;

namespace GMSoft.User
{
    public class CrazyUserModule : UserModuleBase
    {
        public override bool CanSetUserName()
        {
            return false;
        }

        public override string GetUserName()
        {
            return UserName;
        }

        public override void Initialize()
        {
            CrazySDK.User.GetUser((user) =>
            {
                UserName = user.username;
            });
        }

        public override void SetUserName(string userName)
        {
            if (!CanSetUserName()) return;
            UserName = userName;
        }
    }
}
