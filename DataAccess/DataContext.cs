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

            //Story
            modelBuilder.Entity<Story>()
                        .HasOptional(f => f.StartingArc)
                        .WithRequired(s => s.Story);

            modelBuilder.Entity<Story>()
                        .HasMany<Arc>(f => f.Arcs)
                        .WithRequired(s => s.Story);

            modelBuilder.Entity<Story>()
                        .HasMany<Tag>(f => f.Tags);

            //User Profile
            modelBuilder.Entity<UserProfile>()
                        .HasMany<Arc>(f => f.Arcs)
                        .WithRequired(s => s.Creator);

            modelBuilder.Entity<UserProfile>()
                        .HasMany<Choice>(f => f.Choices)
                        .WithRequired(s => s.Creator);

            modelBuilder.Entity<UserProfile>()
                        .HasMany<Stanza>(f => f.Stanzas)
                        .WithRequired(s => s.Creator);

            modelBuilder.Entity<UserProfile>()
                        .HasMany<Tag>(f => f.Tags)
                        .WithRequired(s => s.Creator);

            modelBuilder.Entity<UserProfile>()
                        .HasMany<Pathway>(f => f.Pathways)
                        .WithRequired(s => s.Creator);

            modelBuilder.Entity<UserProfile>()
                        .HasMany<ProtagonistGender>(f => f.ProtagonistGenders)
                        .WithRequired(s => s.Creator);

            modelBuilder.Entity<UserProfile>()
                        .HasMany<ProtagonistName>(f => f.ProtagonistNames)
                        .WithRequired(s => s.Creator);

            modelBuilder.Entity<UserProfile>()
                        .HasMany<StatusValue>(f => f.StatusValues)
                        .WithRequired(s => s.Creator);

            modelBuilder.Entity<UserProfile>()
                       .HasMany<StoryName>(f => f.StoryNames)
                       .WithRequired(s => s.Creator);
            
            //Arc
            modelBuilder.Entity<Arc>()
                        .HasMany<Pathway>(f => f.Pathways)
                        .WithRequired(s => s.Arc);

            modelBuilder.Entity<Arc>()
                        .HasOptional(f => f.AncestorArc);

            //Stanza
            modelBuilder.Entity<Stanza>()
                        .HasMany<StatusLogic>(f => f.StatusLogics);

            modelBuilder.Entity<Stanza>()
                        .HasMany<Choice>(f => f.Choices)
                        .WithMany(s => s.Stanzas);

            modelBuilder.Entity<Stanza>()
                        .HasMany<Tag>(f => f.Tags);

            //Choice
            modelBuilder.Entity<Choice>()
                        .HasMany<StatusChanges>(f => f.StatusChangeses);

            modelBuilder.Entity<Choice>()
                        .HasMany<Tag>(f => f.Tags);

            //Pathway
            modelBuilder.Entity<Pathway>()
                        .HasRequired<Choice>(f => f.Choice);

            modelBuilder.Entity<Pathway>()
                        .HasRequired<Stanza>(f => f.Stanza);

            //ProtagonistGender
            modelBuilder.Entity<ProtagonistGender>()
                        .HasOptional<ProtagonistGender>(f => f.CollapsedTo);

            //ProtagonistName
            modelBuilder.Entity<ProtagonistName>()
                        .HasOptional<ProtagonistName>(f => f.CollapsedTo);

            //StatusChanges
            modelBuilder.Entity<StatusChanges>()
                        .HasRequired<StatusModification>(f => f.StatusModifier);

            modelBuilder.Entity<StatusChanges>()
                        .HasRequired<StatusValue>(f => f.StatusType);

            //StatusLogic
            modelBuilder.Entity<StatusLogic>()
                        .HasMany<StatusComparison>(f => f.StatusComparisons);

            //StatusValue
            modelBuilder.Entity<StatusValue>()
                        .HasMany<StatusComparison>(f => f.StatusComparisons)
                        .WithRequired(s => s.StatusType);

            modelBuilder.Entity<StatusValue>()
                        .HasOptional<StatusValue>(f => f.CollapsedTo);

            //StoryName
            modelBuilder.Entity<StoryName>()
                       .HasOptional<StoryName>(f => f.CollapsedTo);

            //Tag
            modelBuilder.Entity<Tag>()
                       .HasOptional<Tag>(f => f.CollapsedTo);

        }
    }
}
