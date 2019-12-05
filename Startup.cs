using aplication20_07_2019.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Security.Cryptography;

[assembly: OwinStartupAttribute(typeof(aplication20_07_2019.Startup))]
namespace aplication20_07_2019
{
    public partial class Startup
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
            CreateUsers();
        }
        public void CreateUsers()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = new ApplicationUser();
            user.Email = "lmsiehachraf@gmail.com";
            user.UserName = "a1";
            user.PasswordHash ="MD5";
            var check = userManager.Create(user, "kjkszpj");
            if(check.Succeeded)
            {
                userManager.AddToRole(user.Id, "Admin"); 
            }
        }
        public void CreateRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            IdentityRole role;
            if (!roleManager.RoleExists("admin"))
            {
                role = new IdentityRole();
                role.Name = "admin";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("prestataire"))
            {
                role = new IdentityRole();
                role.Name = "prestataire";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("responsable_marche"))
            {
                role = new IdentityRole();
                role.Name = "responsable_marche";
                roleManager.Create(role);
            }
        }
    }
}

