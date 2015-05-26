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
    /// A single choice for a single stanza
    /// </summary>
    [Serializable]
    [Table("Choice")]
    public class Choice : IComparable, IEquatable<Choice>
    {
        private static DataContext db = new DataContext();

        /// <summary>
        /// The DB PrimaryKey
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; private set; }

        /// <summary>
        /// The text of the choice
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The changes to status that this choice imposes
        /// </summary>
        public IEnumerable<StatusChanges> StatusChanges { get; set; }

        /// <summary>
        /// The stanzas this belongs to
        /// </summary>
        public IEnumerable<Stanza> Stanzas { get; set; }

        /// <summary>
        /// The tags associated with this stanza
        /// </summary>
        public IEnumerable<Tag> Tags { get; set; }

        /// <summary>
        /// Who created this
        /// </summary>
        public UserProfile Creator { get; set; }

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
        /// Associate a stanza with this
        /// </summary>
        /// <param name="stanza">the stanza to associate</param>
        /// <returns>all the associated stanzas</returns>
        public IEnumerable<Stanza> AddStanza(Stanza stanza)
        {
            //Don't add stuff we already have
            if (Stanzas.Any(s => s.Equals(stanza)))
                return Stanzas;

            Stanzas.ToList().Add(stanza);

            db.Stanzas.Attach(stanza);

            Update(false, null);

            return Stanzas;
        }

        /// <summary>
        /// Create a new Choice with no tags, admin function makes data
        /// </summary>
        /// <param name="statusChanges">Block of status changes this choice applies</param>
        /// <param name="text">The text of the choice</param>
        /// <param name="stanzas">Stanzas this belongs to</param>
        /// <param name="creator">The user creating this</param>
        /// <returns>the new data</returns>
        public static Choice Create(IEnumerable<StatusChanges> statusChanges, string text, Stanza stanza, UserProfile creator)
        {
            return Create(statusChanges, text, stanza, creator, Enumerable.Empty<Tag>());
        }

        /// <summary>
        /// Create a new Choice, admin function makes data
        /// </summary>
        /// <param name="statusChanges">Block of status changes this choice applies</param>
        /// <param name="text">The text of the choice</param>
        /// <param name="stanzas">Stanzas this belongs to</param>
        /// <param name="creator">The user creating this</param>
        /// <param name="tags">The tags to apply to this</param>
        /// <returns>the new data</returns>
        public static Choice Create(IEnumerable<StatusChanges> statusChanges, string text, Stanza stanza, UserProfile creator, IEnumerable<Tag> tags)
        {
            //No null creators
            if (creator == null)
                return null;

            var stanzas = new List<Stanza>();
            stanzas.Add(stanza);

            var returnValue = new Choice
            {
                StatusChanges = statusChanges,
                Text = text,
                Stanzas = stanzas,
                Creator = creator,
                Tags = tags
            };

            if (returnValue != null)
            {
                foreach (var tag in tags)
                    db.Tags.Attach(tag);

                db.Stanzas.Attach(stanza);

                db.UserProfiles.Attach(creator);

                db.Choices.Add(returnValue);
                db.SaveChanges();
            }

            return returnValue;
        }

        /// <summary>
        /// Append this choice's changes to the status block
        /// </summary>
        /// <param name="status">current status</param>
        /// <returns>Fluent design, the status results</returns>
        public PlotStatus AppendStatus(PlotStatus status)
        {
            foreach (var change in StatusChanges)
            {
                var key = change.StatusType;

                if(change.StatusModifier.Type == ModifierType.Remove)
                {
                    if(status.StatusValues.ContainsKey(key))
                        status.StatusValues.Remove(key);

                    continue;
                }
                
                if(!status.StatusValues.ContainsKey(key))
                {
                    status.StatusValues.Add(key, change.StatusModifier.Value);
                    continue;
                }

                var newValue = change.StatusModifier.Modify(status.StatusValues[key]);

                if (newValue != null)
                    status.StatusValues[key] = newValue.Value;
            }

            return status;
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

            chainedDb.Choices.Attach(this);
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

            chainedDb.Choices.Attach(this);
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
            if (obj == null || obj.GetType() != typeof(Choice))
                return -1;

            var other = (Choice)obj;
            return other.Equals(this) ? 0 : 1;
        }

        /// <summary>
        /// Equating by db id
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns>if they are the same db object</returns>
        public bool Equals(Choice other)
        {
            return other != null && other.Id.Equals(this.Id);
        }
    }
}