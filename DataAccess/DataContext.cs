using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Divergence.DataAccess.DataClasses;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<Story> Stories { get; set; }
        public DbSet<Arc> Arcs { get; set; }
        public DbSet<Pathway> Pathways { get; set; }
        public DbSet<Stanza> Stanzas { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<StatusValue> StatusValues { get; set; }
        public DbSet<StoryName> StoryNames { get; set; }
        public DbSet<ProtagonistName> ProtagonistNames { get; set; }
        public DbSet<ProtagonistGender> ProtagonistGenders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<Stanza>()
            //    .HasMany(c => c.Choices).WithMany(i => i.Courses)
            //    .Map(t => t.MapLeftKey("CourseID")
            //        .MapRightKey("InstructorID")
            //        .ToTable("CourseInstructor"));
        }
    }
}
