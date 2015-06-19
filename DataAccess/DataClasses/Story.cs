using DataAccess;
using Divergence.DataAccess.Helpers;
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
    /// The supertype for an entire collection of arcs and pathways
    /// </summary>
    [Serializable]
    [Table("Story")]
    public class Story : IComparable, IEquatable<Story>
    {
        private DataContext db = new DataContext();

        /// <summary>
        /// The DB PrimaryKey
        /// </summary>
        [Key]
        [Column(Order = 1)] 
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; private set; }

        /// <summary>
        /// Guid "friendly key"
        /// </summary>
        [Key]
        [Column(Order = 2)] 
        public Guid Key { get; private set; }

        /// <summary>
        /// string "friendly name"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The main character's name
        /// </summary>
        public string ProtagonistName { get; set; }

        /// <summary>
        /// The main character's non-binary gender description
        /// </summary>
        public string ProtagonistGender { get; set; }

        /// <summary>
        /// The first arc of this story
        /// </summary>
        public Arc StartingArc { get; set; }

        /// <summary>
        /// All the arcs in this story
        /// </summary>
        public IEnumerable<Arc> Arcs { get; set; }

        /// <summary>
        /// Tags associated with this story
        /// </summary>
        public IEnumerable<Tag> Tags { get; set; }

        /// <summary>
        /// All the tags in this story and all children
        /// </summary>
        [NotMapped]
        public IEnumerable<Tag> TagFamily
        {
            get
            {
                var returnValue = Tags.ToList();

                returnValue.AddRange(Arcs.SelectMany(arc => arc.TagFamily));

                return returnValue;
            }
        }

        /// <summary>
        /// Is this story started yet?
        /// </summary>
        [NotMapped]
        public bool IsStarted
        {
            get
            {
                return StartingArc != null;
            }
        }

        /// <summary>
        /// When this Story was created
        /// </summary>
        public DateTime Created { get; private set; }

        /// <summary>
        /// If and when this Story was concluded or abandoned
        /// </summary>
        public DateTime? Archived { get; private set; }

        /// <summary>
        /// Is this story archived?
        /// </summary>
        [NotMapped]
        public bool IsArchived
        {
            get
            {
                return Archived.HasValue;
            }
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
        /// Create a new story without tags
        /// </summary>
        /// <param name="name">name of the story</param>
        /// <param name="protagonistName">The protagonist's name</param>
        /// <param name="protagonistGender">the protagonist's gender</param>
        /// <returns>the new story object</returns>
        public Story Create(string name, string protagonistName, string protagonistGender)
        {
            return Create(name, protagonistName, protagonistGender, Enumerable.Empty<Tag>());
        }

        /// <summary>
        /// Create a new story with tags
        /// </summary>
        /// <param name="name">name of the story</param>
        /// <param name="protagonistName">The protagonist's name</param>
        /// <param name="protagonistGender">the protagonist's gender</param>
        /// <param name="tags">The tags to apply to the story</param>
        /// <returns>the new story object</returns>
        public Story Create(string name, string protagonistName, string protagonistGender, IEnumerable<Tag> tags)
        {
            //Generate the new key
            var key = Guid.NewGuid();

            if (String.IsNullOrWhiteSpace(name))
                name = RandomGenerator.StoryName();

            if (String.IsNullOrWhiteSpace(protagonistName))
                protagonistName = RandomGenerator.ProtagonistName();

            if (String.IsNullOrWhiteSpace(protagonistGender))
                protagonistGender = RandomGenerator.ProtagonistGender();

            var returnValue = new Story
            {
                Key = key,
                Name = name,
                ProtagonistGender = protagonistGender,
                ProtagonistName = protagonistName,
                Tags = tags
            };

            foreach (var tag in tags)
                db.Tags.Attach(tag);

            db.Stories.Add(returnValue);
            db.SaveChanges();

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

            chainedDb.Stories.Attach(this);
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

            chainedDb.Stories.Attach(this);
            chainedDb.Entry(this).State = EntityState.Deleted;

            foreach (var arc in chainedDb.Arcs.Where(a => a.Story.Id == this.Id))
                arc.Delete(true, chainedDb);

            return chained ? -99 : db.SaveChanges();
        }

        /// <summary>
        /// Archive this data
        /// </summary>
        /// <param name="chainedDb">Passthru db context internal use only</param>
        /// <returns>status of archiving</returns>
        public int Archive(bool chained, DataContext chainedDb)
        {
            if (chainedDb == null)
                chainedDb = db;

            this.Archived = DateTime.Now;

            foreach (var arc in db.Arcs.Where(a => a.Story.Id == this.Id))
                arc.Archive(chained, chainedDb);

            return Update(chained, chainedDb);
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
            if (obj == null || obj.GetType() != typeof(Story))
                return -1;

            var other = (Story)obj;
            return other.Equals(this) ? 0 : 1;
        }

        /// <summary>
        /// Equating by db id
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns>if they are the same db object</returns>
        public bool Equals(Story other)
        {
            return other != null && other.Id.Equals(this.Id);
        }
    }
}