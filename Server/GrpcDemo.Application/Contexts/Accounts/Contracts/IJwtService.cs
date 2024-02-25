namespace GrpcDemo.Application.Contexts.Accounts.Contracts
{
    using Core;
    using Models;
    using Services;

    public interface IJwtService : IService
    {
        JsonWebToken CreateJsonWebToken(Account user);
    }
}