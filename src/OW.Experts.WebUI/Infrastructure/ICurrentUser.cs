namespace OW.Experts.WebUI.Infrastructure
{
    public interface ICurrentUser
    {
        string Name { get; }
        bool IsAdmin { get; }
        bool IsExpert { get; }
    }
}