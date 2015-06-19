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
    /// A plot divergence in a story, contains pathways
    /// </summary>
    [Serializable]
    [Table("Arc")]
    public class Arc : IComparable, IEquatable<Arc>
    {
        private static DataContext db = new DataContext();

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
        /// Pathways under this arc
        /// </summary>
        public ICollection<Pathway> Pathways { get; set; }

        /// <summary>
        /// The arc this one was forked from
        /// </summary>
        public Arc AncestorArc { get; set; }

        /// <summary>
        /// The story this belongs to
        /// </summary>
        public Story Story { get; set; }

        /// <summary>
        /// When this Arc was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// If and when this Arc was concluded or abandoned
        /// </summary>
        public DateTime? Archived { get; set; }

        /// <summary>
        /// Who created this arc
        /// </summary>
        public UserProfile Creator { get; set; }

        /// <summary>
        /// All the tags in all children
        /// </summary>
        [NotMapped]
        public IEnumerable<Tag> TagFamily
        {
            get
            {
                return Pathways.SelectMany(path => path.TagFamily);
            }
        }

        /// <summary>
        /// Is this arc archived
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
        /// The current plot status
        /// </summary>
        [NotMapped]
        public PlotStatus Status 
        {
            get
            {
                return CurrentPathway.Status;
            }
        }

        /// <summary>
        /// The most current pathway in this Arc
        /// </summary>
        [NotMapped]
        public Pathway CurrentPathway
        {
            get
            {
                return Pathways.OrderByDescending(p => p.Order).FirstOrDefault();
            }
        }

        /// <summary>
        /// Add a pathway to the list of pathways
        /// </summary>
        /// <param name="pathway">the pathway to add</param>
        /// <returns>The new list</returns>
        public ICollection<Pathway> AddPathway(Pathway pathway)
        {
            //Don't add stuff we already have
            if (Pathways.Any(p => p.Order.Equals(pathway.Order) || p.Equals(pathway)))
                return Pathways;

            Pathways.ToList().Add(pathway);

            db.Pathways.Attach(pathway);

            Update(false, null);

            return Pathways;
        }

        /// <summary>
        /// Create a new first arc or fork a new one
        /// </summary>
        /// <param name="story">the story this belongs to, must not be archived</param>
        /// <param name="ancestor">Optional: The arc you're forking from</param>
        /// <param name="startingFork">Optional: The pathway you're forking off of</param>
        /// <param name="firstChoice">Optional: The choice that's the fork starter</param>
        /// <param name="creator">The user creating this</param>
        /// <returns>the new arc</returns>
        public static Arc Create(Story story, Arc ancestor, Pathway startingFork, Choice firstChoice, UserProfile creator)
        {
            //We can't make new content off of archived stories
            if (story == null || story.IsArchived)
                return null;

            //No null creators
            if (creator == null)
                return null;

            //Again, this is bad pool
            if (ancestor != null && ancestor.IsArchived)
                return null;

            //Generate the new key
            var key = Guid.NewGuid();

            Arc returnValue = null;
            Pathway newPathway = null;
            //if the Arc, starting fork or choice are null we're making a new arc on an empty story
            if (ancestor == null || startingFork == null || firstChoice == null)
            {
                //Create the new starting pathway
                returnValue = new Arc
                {
                    Key = key,
                    Story = story,
                    Creator = creator
                };

                newPathway = Pathway.Create(creator, returnValue);

                returnValue.AddPathway(newPathway);
            }
            else
            {
                returnValue = new Arc
                {
                    Key = key,
                    Story = story,
                    AncestorArc = ancestor,
                    Creator = creator
                };

                newPathway = Pathway.Create(ancestor.CurrentPathway.Age, ancestor.CurrentPathway.Order, ancestor.CurrentPathway.Stanza, firstChoice, ancestor.CurrentPathway.Status, creator, returnValue);

                returnValue.AddPathway(newPathway);
            }

            if (returnValue != null)
            {
                if (ancestor == null)
                    db.Arcs.Attach(ancestor);

                if (startingFork == null)
                    db.Pathways.Attach(startingFork);

                if (firstChoice == null)
                    db.Choices.Attach(firstChoice);

                db.UserProfiles.Attach(creator);
                db.Pathways.Attach(newPathway);

                db.Arcs.Add(returnValue);
                db.SaveChanges();
            }

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

            chainedDb.Arcs.Attach(this);
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

            chainedDb.Arcs.Attach(this);
            chainedDb.Entry(this).State = EntityState.Deleted;

            foreach (var path in chainedDb.Pathways.Where(p => p.Arc == this))
                path.Delete(true, chainedDb);

            return chained ? -99 : chainedDb.SaveChanges();
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

            return Update(chained, chainedDb);
        }

        /// <summary>
        /// Compare two arcs
        /// </summary>
        /// <param name="obj">Must be an Arc</param>
        /// <returns>
        /// -2: null or inequatable type
        /// -1: input is older than current in the story
        /// 0: Equal
        /// 1: input is newer than current in the story
        /// 2: Equatable type but completely foreign contexts
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Arc))
                return -2;

            var other = (Arc)obj;

            if (other.Equals(this))
                return 0;

            if (other.Story.Equals(this.Story))
                return other.Created > this.Created ? -1 : 1;

            return 2;
        }

        /// <summary>
        /// Equating by db id
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns>if they are the same db object</returns>
        public bool Equals(Arc other)
        {
            return other != null && other.Id.Equals(this.Id);
        }
    }
}