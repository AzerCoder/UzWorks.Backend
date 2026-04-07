using Microsoft.AspNetCore.Identity;
using UzWorks.Identity.Models;
using UzWorks.Core.Constants;
using UzWorks.Core.Enums.GenderTypes;

namespace UzWorks.API.Middleware;

 public class RoleInitializer
{
    public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        string SuperAdminEmail = "goblindev02@gmail.com";
        string SuperAdminPhoneNumber = "998932505255";
        string SuperAdminPassword = "123456devA@8613759241";
        string ExtraAdminPhoneNumber = "998939989980";
        string ExtraAdminPassword = "Azamjon1220";

        var Roles = new Dictionary<string, string>()
        {
            { RoleNames.SuperAdmin, "SuperAdmin" },
            { RoleNames.Supervisor, "Supervisor" },
            { RoleNames.Employer, "Employer" },
            { RoleNames.Employee, "Employee" },
            { RoleNames.NewUser, "NewUser" },
        };

        foreach (var role in Roles)
            if (await roleManager.FindByNameAsync(role.Key) == null)
                await roleManager.CreateAsync(new Role(role.Key, role.Value));

        var adminRoles = new[]
        {
            RoleNames.Employee,
            RoleNames.Employer,
            RoleNames.SuperAdmin,
            RoleNames.Supervisor
        };

        User user = await userManager.FindByNameAsync(SuperAdminEmail);

        if (user != null) 
        {
            foreach (var roleName in adminRoles)
            {
                if (!await userManager.IsInRoleAsync(user, roleName))
                    await userManager.AddToRoleAsync(user, roleName);
            }
        }

        else
        {
            user = new User(
                "Abdulaziz", "Nabijonov", SuperAdminPhoneNumber, SuperAdminEmail, GenderEnum.Male, new DateTime(2002, 06, 17)
            );
            user.PhoneNumberConfirmed = true;
            user.EmailConfirmed = true;
            
            IdentityResult result = await userManager.CreateAsync(user, SuperAdminPassword);
            
            if (result.Succeeded)
            {
                foreach (var roleName in adminRoles)
                {
                    if (!await userManager.IsInRoleAsync(user, roleName))
                        await userManager.AddToRoleAsync(user, roleName);
                }
            }

        }

        // Ensure the requested phone account also has full admin capabilities.
        var extraAdminUser = await userManager.FindByNameAsync(ExtraAdminPhoneNumber);

        if (extraAdminUser != null)
        {
            foreach (var roleName in adminRoles)
            {
                if (!await userManager.IsInRoleAsync(extraAdminUser, roleName))
                    await userManager.AddToRoleAsync(extraAdminUser, roleName);
            }
        }
        else
        {
            extraAdminUser = new User("Admin", "User", ExtraAdminPhoneNumber)
            {
                PhoneNumberConfirmed = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(extraAdminUser, ExtraAdminPassword);

            if (result.Succeeded)
            {
                foreach (var roleName in adminRoles)
                {
                    if (!await userManager.IsInRoleAsync(extraAdminUser, roleName))
                        await userManager.AddToRoleAsync(extraAdminUser, roleName);
                }
            }
        }
    }
}