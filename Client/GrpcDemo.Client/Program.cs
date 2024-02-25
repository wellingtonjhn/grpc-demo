namespace GrpcDemo.Client
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var accountServiceGateway = new AccountServiceGateway("http://localhost:5000");

                const string email = "wellington@email.com";
                const string password = "pass";

                var accessToken = await accountServiceGateway.Login(email, password);

                await accountServiceGateway.ChangePassword(accessToken, email, "newpass", "wrong-pass");
            }
            catch (Exception ex)
            {
                ex.Message.Dump();
                ex.StackTrace.Dump();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
