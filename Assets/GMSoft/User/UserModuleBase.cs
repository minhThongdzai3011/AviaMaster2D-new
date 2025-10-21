namespace GMSoft.User
{
    public abstract class UserModuleBase
    {
        public string UserName;
        public abstract void Initialize();
        public abstract bool CanSetUserName();
        public abstract void SetUserName(string userName);
        public abstract string GetUserName();
    }
}