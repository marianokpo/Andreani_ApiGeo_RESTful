using Microsoft.EntityFrameworkCore;
using API_GEO.Librery;
using System.Linq;
using System.Collections.Generic;

namespace API_GEO.Models
{
    public class ApiContext : DbContext
    {
        
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<LocalizadorData> LocalizadorData { get; set; }

        public List<LocalizadorData> GetAll() => this.LocalizadorData.ToList<LocalizadorData>();

        public LocalizadorData GetID(long id)
        {
            List<LocalizadorData> LLD = GetAll();
            LocalizadorData LD = LLD.FirstOrDefault(SLD => SLD.id == id);

            return LD;
        }

        public void SetLocalizadorData(LocalizadorData LD)
        {
            this.LocalizadorData.Add(LD);
            this.SaveChanges();
        }
    }
}
