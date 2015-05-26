using Divergence.DataAccess.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DataInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            var systemUser = new UserProfile();
            systemUser.UserId = -1;
            systemUser.UserName = "TheGreatDigitalAuthorInTheSky";

            context.UserProfiles.Add(systemUser);
            context.SaveChanges();

            context.UserProfiles.Attach(systemUser);

            ProtagonistGender.Create(systemUser, "Male", Origin.System);
            ProtagonistGender.Create(systemUser, "Female", Origin.System);
            ProtagonistGender.Create(systemUser, "Male-Bisexual", Origin.System);
            ProtagonistGender.Create(systemUser, "Female-Bisexual", Origin.System);
            ProtagonistGender.Create(systemUser, "Male-Homosexual", Origin.System);
            ProtagonistGender.Create(systemUser, "Female-Homosexual", Origin.System);
            ProtagonistGender.Create(systemUser, "Male-Transvestite", Origin.System);
            ProtagonistGender.Create(systemUser, "Female-Transvestite", Origin.System);
            ProtagonistGender.Create(systemUser, "Ungendered", Origin.System);

            ProtagonistName.Create(systemUser, "Bob", NamePart.First, Origin.System);
            ProtagonistName.Create(systemUser, "John", NamePart.First, Origin.System);
            ProtagonistName.Create(systemUser, "Jane", NamePart.First, Origin.System);
            ProtagonistName.Create(systemUser, "Sally", NamePart.First, Origin.System);
            ProtagonistName.Create(systemUser, "Marcus", NamePart.Middle, Origin.System);
            ProtagonistName.Create(systemUser, "\"The Human\"", NamePart.Middle, Origin.System);
            ProtagonistName.Create(systemUser, "\"The Dog\"", NamePart.Middle, Origin.System);
            ProtagonistName.Create(systemUser, "Cranked", NamePart.Middle, Origin.System);
            ProtagonistName.Create(systemUser, "Fillion", NamePart.Last, Origin.System);
            ProtagonistName.Create(systemUser, "Smith", NamePart.Last, Origin.System);
            ProtagonistName.Create(systemUser, "Waterton", NamePart.Last, Origin.System);
            ProtagonistName.Create(systemUser, "Smithton", NamePart.Last, Origin.System);

            StoryName.Create(systemUser, "The", NamePart.First, Origin.System);
            StoryName.Create(systemUser, "A", NamePart.First, Origin.System);
            StoryName.Create(systemUser, "Unlikely Tale", NamePart.Middle, Origin.System);
            StoryName.Create(systemUser, "Story of", NamePart.Middle, Origin.System);
            StoryName.Create(systemUser, "Parable", NamePart.Middle, Origin.System);
            StoryName.Create(systemUser, "the End of Days", NamePart.Last, Origin.System);
            StoryName.Create(systemUser, "Susan", NamePart.Last, Origin.System);
            StoryName.Create(systemUser, "a Dog", NamePart.Last, Origin.System);
            StoryName.Create(systemUser, "Kismet", NamePart.Last, Origin.System);

            Tag.Create(systemUser, "NSFW", Origin.System);
            Tag.Create(systemUser, "Innuendo", Origin.System);
            Tag.Create(systemUser, "Violence", Origin.System);
            Tag.Create(systemUser, "Sex", Origin.System);
        }
    }
}
