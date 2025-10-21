namespace GMSoft.User
{
    public class GMUserModule : UserModuleBase
    {
        public override bool CanSetUserName()
        {
            return true;
        }

        public override string GetUserName()
        {
            return UserName;
        }

        public override void Initialize()
        {
        }

        public override void SetUserName(string userName)
        {
            if (!CanSetUserName()) return;
            UserName = userName;
        }
    }
}