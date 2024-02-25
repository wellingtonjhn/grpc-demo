namespace GrpcDemo.Infrastructure.Repositories
{
    using System.Threading.Tasks;
    using Application.Contexts.Accounts.Contracts;
    using Application.Contexts.Accounts.Models;
    using GrpcDemo.Infrastructure.Database;

    public sealed class AccountsRepository : IAccountsRepository
    {
        private readonly IRavenContext _context;

        public AccountsRepository(IRavenContext context) => _context = context;

        public async Task<Account> GetByEmail(string email) 
            => await _context.GetAsync<Account>(account => account.Email == email);

        public async Task Save(Account account)
        {
            await _context.StoreAsync(account);
            await _context.SaveAsync();
        }
    }
}