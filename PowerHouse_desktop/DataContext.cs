using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerHouse_desktop
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(ConfigurationManager.AppSettings["ConnStringDB"]);


        public DbSet<FormData> formDataDbSet { get; set; }
            

    }
}
