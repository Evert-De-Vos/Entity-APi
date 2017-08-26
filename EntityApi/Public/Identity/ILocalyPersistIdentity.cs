namespace EntityApi.Public.Identity
{
    public interface ILocalyPersistIdentity
    {
        void SaveTokens();
        void LoadTokens();
    }
}
