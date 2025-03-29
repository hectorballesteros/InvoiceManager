using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceManager.Domain.Entities;
using System.Linq; 


public interface IAuthService
{
    Task<User?> Authenticate(string username, string password);
    string GenerateToken(User user);
}
