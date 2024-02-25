namespace GrpcDemo.Application.Contexts.Accounts.Contracts
{
    using System.Threading.Tasks;
    using Core;
    using Models;

    public interface IAccountsRepository : IRepository
    {
        Task<Account> GetByEmail(string email);
        Task Save(Account account);
    }
}