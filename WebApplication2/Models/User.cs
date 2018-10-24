using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class User
    {
        public int Id { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Год")]
        public int Year { get; set; }
        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "О себе")]
        public string About { get; set; }
        [Display(Name = "Адрес")]
        public string Adress { get; set; }

    }


    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class UsersDbContext : DbContext
    {
        public UsersDbContext() : base("conn")
        { }

        public DbSet<User> Users { get; set; }
    }
}