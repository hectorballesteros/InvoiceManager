using InvoiceManager.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Infrastructure.Seeders
{
    public static class UserSeeder
    {
        public static void SeedUsers(this InvoiceManagerContext context)
        {
            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<User>();
                var user = new User
                {
                    Username = "hector",
                    Role = "Admin"
                };
                user.Password = hasher.HashPassword(user, "hectorfinix");

                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
}
