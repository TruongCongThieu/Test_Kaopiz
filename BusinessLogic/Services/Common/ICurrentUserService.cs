namespace BusinessLogic.Services.Common
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
        string Email { get; }
        string FullName { get; }
    }
}