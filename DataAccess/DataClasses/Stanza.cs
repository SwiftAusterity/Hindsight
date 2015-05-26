using DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Divergence.DataAccess.DataClasses
{
    /// <summary>
    /// A plot point that is chosen for a single pathway node, contains many choices
    /// </summary>
    [Serializable]
    [Table("Stanza")]
    public class Stanza : IComparable, IEquatable<Stanza>
    {
        private static DataContext db = new DataContext();

        /// <summary>
        /// The DB PrimaryKey
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; private set; }

        /// <summary>
        /// Minimum Age of pathway to make this valid in
        /// </summary>
        public int MinimumAge { get; set; }

        /// <summary>
        /// Maximum age of pathway to make this in
        /// </summary>
        public int MaximumAge { get; set; }

        /// <summary>
        /// Descriptive text (eg what the user sees as the story portion
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The logic statements that determine if this stanza qualifies for the current pathway plot pool
        /// </summary>
        public IEnumerable<StatusLogic> StatusLogic { get; set; }

        /// <summary>
        /// The choices attached to this stanza
        /// </summary>
        public IEnumerable<Choice> Choices { get; set; }

        /// <summary>
        /// The tags associated with this stanza
        /// </summary>
        public IEnumerable<Tag> Tags { get; set; }

        /// <summary>
        /// All the tags in this and all children
        /// </summary>
        public IEnumerable<Tag> TagFamily
        {
            get
            {
                var returnValue = Tags.ToList();

                returnValue.AddRange(Choices.SelectMany(choice => choice.Tags));

                return returnValue;
            }
        }

        /// <summary>
        /// The access type
        /// </summary>
        public Origin Type { get; set; }

        /// <summary>
        /// Who created this
        /// </summary>
        public UserProfile Creator { get; set; }

        /// <summary>
        /// Add a choice to the stanza
        /// </summary>
        /// <param name="choice">the new choice</param>
        /// <returns>the new list of choices</returns>
        public IEnumerable<Choice> AddChoice(Choice choice)
        {
            //Don't add stuff we already have
            if (Choices.Any(c => c.Equals(choice)))
                return Choices;

            Choices.ToList().Add(choice);

            db.Choices.Attach(choice);

            Update(false, null);

            return Choices;
        }

        /// <summary>
        /// Append a tag to this
        /// </summary>
        /// <param name="tag">the tag to append</param>
        /// <returns>All the tags associated with this</returns>
        public IEnumerable<Tag> AddTag(Tag tag)
        {
            //Don't add stuff we already have
            if (Tags.Any(t => t.Equals(tag)))
                return Tags;

            Tags.ToList().Add(tag);

            db.Tags.Attach(tag);

            Update(false, null);

            return Tags;
        }

        /// <summary>
        /// Removes a tag from this
        /// </summary>
        /// <param name="tag">the tag to remove</param>
        /// <returns>The new list of tags</returns>
        public IEnumerable<Tag> RemoveTag(Tag tag)
        {
            //Don't remove stuff we don't have
            if (!Tags.Any(t => t.Equals(tag)))
                return Tags;

            Tags.ToList().Remove(tag);

            db.Tags.Attach(tag);

            Update(false, null);

            return Tags;
        }

        /// <summary>
        /// Create a new Stanza with no tags and private origin - Admin function
        /// </summary>
        /// <param name="minAge">minimum age this qualifies for</param>
        /// <param name="maxAge">maximum age this qualifies for</param>
        /// <param name="description">text description of the scenario</param>
        /// <param name="logic">block of logic statements to validate grabbing one of these</param>
        /// <param name="creator">creator of this</param>
        /// <returns>the new data</returns>
        public static Stanza Create(int minAge, int maxAge, string description, IEnumerable<StatusLogic> logic, UserProfile creator)
        {
            return Create(minAge, maxAge, description, logic, creator, Enumerable.Empty<Tag>(), Origin.Private);
        }

        /// <summary>
        /// Create a new Stanza - Admin function
        /// </summary>
        /// <param name="minAge">minimum age this qualifies for</param>
        /// <param name="maxAge">maximum age this qualifies for</param>
        /// <param name="description">text description of the scenario</param>
        /// <param name="logic">block of logic statements to validate grabbing one of these</param>
        /// <param name="creator">creator of this</param>
        /// <param name="tags">the tags to associate with this</param>
        /// <returns>the new data</returns>
        public static Stanza Create(int minAge, int maxAge, string description, IEnumerable<StatusLogic> logic, UserProfile creator, IEnumerable<Tag> tags)
        {
            return Create(minAge, maxAge, description, logic, creator, tags, Origin.Private);
        }

        /// <summary>
        /// Create a new Stanza - Admin function
        /// </summary>
        /// <param name="minAge">minimum age this qualifies for</param>
        /// <param name="maxAge">maximum age this qualifies for</param>
        /// <param name="description">text description of the scenario</param>
        /// <param name="logic">block of logic statements to validate grabbing one of these</param>
        /// <param name="creator">creator of this</param>
        /// <param name="type">the origin type</param>
        /// <returns>the new data</returns>
        public static Stanza Create(int minAge, int maxAge, string description, IEnumerable<StatusLogic> logic, UserProfile creator,  Origin type)
        {
            return Create(minAge, maxAge, description, logic, creator, Enumerable.Empty<Tag>(), type);
        }

        /// <summary>
        /// Create a new Stanza - Admin function
        /// </summary>
        /// <param name="minAge">minimum age this qualifies for</param>
        /// <param name="maxAge">maximum age this qualifies for</param>
        /// <param name="description">text description of the scenario</param>
        /// <param name="logic">block of logic statements to validate grabbing one of these</param>
        /// <param name="creator">creator of this</param>
        /// <param name="tags">the tags to associate with this</param>
        /// <param name="type">the origin type</param>
        /// <returns>the new data</returns>
        public static Stanza Create(int minAge, int maxAge, string description, IEnumerable<StatusLogic> logic, UserProfile creator, IEnumerable<Tag> tags, Origin type)
        {
            //No null creators
            if (creator == null)
                return null;

            var returnValue = new Stanza
            {
                MinimumAge = minAge,
                MaximumAge = maxAge,
                Text = description,
                StatusLogic = logic,
                Creator = creator,
                Tags = tags,
                Type = type
            };

            if (returnValue != null)
            {
                foreach (var tag in tags)
                    db.Tags.Attach(tag);

                db.UserProfiles.Attach(creator);

                db.Stanzas.Add(returnValue);
                db.SaveChanges();
            }

            return returnValue;
        }

        /// <summary>
        /// Get a new stanza for a new pathway
        /// </summary>
        /// <param name="age">Age of the pathway</param>
        /// <param name="status">current plot status</param>
        /// <returns>the stanza (does not make new data, this is just a select)</returns>
        public static Stanza GetNew(int age, PlotStatus status)
        {
            var rand = new Random();
            Stanza returnValue = null;

            //Deny stanzas by age first of all
            var availableStanzas = db.Stanzas.Where(stz => stz.MinimumAge <= age && stz.MaximumAge >= age && status.Validate(stz.StatusLogic));

            if(availableStanzas.Count() > 1)
                returnValue = availableStanzas.OrderBy(stz => rand.Next(availableStanzas.Count() - 1)).FirstOrDefault();
            else if(availableStanzas.Count() == 1)
                returnValue = availableStanzas.First();

            return returnValue;
        }

        /// <summary>
        /// Save changes to this object
        /// </summary>
        /// <param name="chained">Whether this was called internally or not</param>
        /// <param name="chainedDb">The db context passed in, only used internally</param>
        /// <returns>Result of saving</returns>
        public int Update(bool chained, DataContext chainedDb)
        {
            if (chainedDb == null)
                chainedDb = db;

            chainedDb.Stanzas.Attach(this);
            chainedDb.Entry(this).State = EntityState.Modified;
            return chained ? -99 : chainedDb.SaveChanges();
        }

        /// <summary>
        /// Deletes the data, like really deletes it and all its children. Not available from the main UI
        /// </summary>
        /// <param name="chained">Whether this was called internally or not</param>
        /// <param name="chainedDb">The db context passed in, only used internally</param>
        /// <returns>status of deletions</returns>
        public int Delete(bool chained, DataContext chainedDb)
        {
            if (chainedDb == null)
                chainedDb = db;

            chainedDb.Stanzas.Attach(this);
            chainedDb.Entry(this).State = EntityState.Deleted;

            return chained ? -99 : chainedDb.SaveChanges();
        }

        /// <summary>
        /// Simple compare of equality
        /// </summary>
        /// <param name="obj">the other object</param>
        /// <returns>
        /// -1: Null or invalid type comparison
        /// 0: Equal (by db id)
        /// 1: Not the same
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Stanza))
                return -1;

            var other = (Stanza)obj;
            return other.Equals(this) ? 0 : 1;
        }

        /// <summary>
        /// Equating by db id
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns>if they are the same db object</returns>
        public bool Equals(Stanza other)
        {
            return other != null && other.Id.Equals(this.Id);
        }
    }
}