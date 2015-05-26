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
    /// Available list of random choice names for stories
    /// </summary>
    [Serializable]
    [Table("StoryName")]
    public class StoryName : IComparable, IEquatable<StoryName>
    {
        private static DataContext db = new DataContext();

        /// <summary>
        /// The DB PrimaryKey
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; private set; }

        /// <summary>
        /// The storyName's text
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of storyName this is
        /// </summary>
        public Origin Type { get; set; }

        /// <summary>
        /// The name part this qualifies for
        /// </summary>
        public NamePart Part { get; set; }

        /// <summary>
        /// Who created this
        /// </summary>
        public UserProfile Creator { get; set; }

        /// <summary>
        /// When this was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// If and when this was archived
        /// </summary>
        public DateTime? Archived { get; set; }

        /// <summary>
        /// What storyName this translates to if it was collapsed due to mispell or something else
        /// </summary>
        public StoryName CollapsedTo { get; set; }

        /// <summary>
        /// Why this was collapsed
        /// </summary>
        public string CollapseReason { get; set; }

        /// <summary>
        /// Is this archived
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return Archived.HasValue;
            }
        }

        /// <summary>
        /// Add a new storyName to the system, defaults to Private Type
        /// </summary>
        /// <param name="creator">Who created the storyName</param>
        /// <param name="name">The storyName string</param>
        /// <returns>the new storyName</returns>
        public static StoryName Create(UserProfile creator, string name, NamePart part)
        {
            return Create(creator, name, part, Origin.Private);
        }

        /// <summary>
        /// Creates a new storyName with a specified type
        /// </summary>
        /// <param name="creator">Who created the storyName</param>
        /// <param name="name">The storyName string</param>
        /// <param name="type">The origin of the storyName</param>
        /// <returns>the new storyName</returns>
        public static StoryName Create(UserProfile creator, string name, NamePart part, Origin type)
        {
            //No null creators
            if (creator == null)
                return null;

            var returnValue = new StoryName
            {
                Type = type,
                Name = name,
                Part = part,
                Creator = creator
            };

            if (returnValue != null)
            {
                db.UserProfiles.Attach(creator);

                db.StoryNames.Add(returnValue);
                db.SaveChanges();
            }

            return returnValue;
        }

        /// <summary>
        /// Archive this data
        /// </summary>
        /// <returns>status of archiving</returns>
        public int Archive()
        {
            this.Archived = DateTime.Now;

            return Update(false, db);
        }

        /// <summary>
        /// Change the type, might TODO add biz logic later, would need user type for that
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int ChangeStatus(Origin type)
        {
            //Kind of a waste of time
            if (Type == type)
                return 0;

            this.Type = type;

            return Update(false, db);
        }

        /// <summary>
        /// Collapse this storyName into another one
        /// </summary>
        /// <param name="storyName">The storyName we're collapsing into</param>
        /// <returns>Status of the update</returns>
        public int Collapse(StoryName storyName)
        {
            //Kind of a waste of time, can't collapse into self
            if (storyName.Equals(this))
                return -1;

            db.StoryNames.Attach(storyName);

            this.CollapsedTo = storyName;

            //Archive will update for us.
            return Archive();
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

            chainedDb.StoryNames.Attach(this);
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

            chainedDb.StoryNames.Attach(this);
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
            if (obj == null || obj.GetType() != typeof(StoryName))
                return -1;

            var other = (StoryName)obj;
            return other.Equals(this) ? 0 : 1;
        }

        /// <summary>
        /// Equating by db id
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns>if they are the same db object</returns>
        public bool Equals(StoryName other)
        {
            return other != null && other.Id.Equals(this.Id);
        }
    }
}