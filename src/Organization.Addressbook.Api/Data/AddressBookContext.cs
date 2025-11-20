using Microsoft.EntityFrameworkCore;
using Organization.Addressbook.Api.Models;
using Models = Organization.Addressbook.Api.Models;

namespace Organization.Addressbook.Api.Data
{
    public class AddressBookContext : DbContext
    {
        public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options) { }

        public DbSet<Models.Organization> Organizations => Set<Models.Organization>();
        public DbSet<Models.OrganizationBranch> OrganizationBranches => Set<Models.OrganizationBranch>();
        public DbSet<Models.Address> Addresses => Set<Models.Address>();
        public DbSet<Models.ContactDetail> ContactDetails => Set<Models.ContactDetail>();
        public DbSet<Models.Person> Persons => Set<Models.Person>();
        public DbSet<Models.PersonAddress> PersonAddresses => Set<Models.PersonAddress>();
        public DbSet<Models.PersonOrganization> PersonOrganizations => Set<Models.PersonOrganization>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.PersonOrganization>().HasKey(po => new { po.PersonId, po.OrganizationId });

            modelBuilder.Entity<Models.PersonOrganization>()
                .HasOne(po => po.Person)
                .WithMany(p => p.PersonOrganizations)
                .HasForeignKey(po => po.PersonId);

            modelBuilder.Entity<Models.PersonOrganization>()
                .HasOne(po => po.Organization)
                .WithMany(o => o.PersonOrganizations)
                .HasForeignKey(po => po.OrganizationId);

            modelBuilder.Entity<Models.OrganizationBranch>()
                .HasOne(b => b.Address)
                .WithMany()
                .HasForeignKey(b => b.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Models.PersonAddress>()
                .HasOne(pa => pa.Address)
                .WithMany()
                .HasForeignKey(pa => pa.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Models.ContactDetail>()
                .HasOne(cd => cd.OrganizationBranch)
                .WithMany(b => b.ContactDetails)
                .HasForeignKey(cd => cd.OrganizationBranchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.ContactDetail>()
                .HasOne(cd => cd.Person)
                .WithMany(p => p.ContactDetails)
                .HasForeignKey(cd => cd.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
