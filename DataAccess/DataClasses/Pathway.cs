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
    /// A single plot point node on a given Arc, contains one stanza 
    /// </summary>
    [Serializable]
    [Table("Pathway")]
    public class Pathway : IComparable, IEquatable<Pathway>
    {
        private static DataContext db = new DataContext();

        /// <summary>
        /// The DB PrimaryKey
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; private set; }

        /// <summary>
        /// Guid "friendly key"
        /// </summary>
        [Key]
        public Guid Key { get; private set; }

        /// <summary>
        /// Numerical order this pathway occupies within its Arc
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// What story age this pathway came under. Not unlike the protagonists physical age
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// The arc this pathway is on
        /// </summary>
        public Arc Arc { get; set; }

        /// <summary>
        /// The stanza chosen for this pathway
        /// </summary>
        public Stanza Stanza { get; set; }

        /// <summary>
        /// The choice chosen on this path
        /// </summary>
        public Choice Choice { get; set; }

        /// <summary>
        /// The current plot status
        /// </summary>
        public PlotStatus Status { get; set; }

        /// <summary>
        /// When this Arc was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Who created this
        /// </summary>
        public UserProfile Creator { get; set; }

        /// <summary>
        /// All the tags in all children
        /// </summary>
        public IEnumerable<Tag> TagFamily
        {
            get
            {
                var returnValue = new List<Tag>();

                //Only add the stanza's tags and the choice's tags
                returnValue.AddRange(Stanza.Tags);
                returnValue.AddRange(Choice.Tags);

                return returnValue;
            }
        }

        /// <summary>
        /// Remaining choices available to be chosen
        /// </summary>
        public IEnumerable<Choice> LockedChoices
        {
            get
            {
                return Arc.Pathways.Where(p => p.Order.Equals(Order)).Select(path => path.Choice);
            }
        }

        /// <summary>
        /// Remaining choices available to be chosen
        /// </summary>
        public IEnumerable<Choice> AvailableChoices
        {
            get
            {
                return Stanza.Choices.Where(c => !LockedChoices.Any(lc => lc.Equals(c)));
            }
        }


        /// <summary>
        /// Create a new Pathway for a new Arc
        /// </summary>
        /// <param name="creator">The user creating this</param>
        /// <param name="arc">The arc this belongs to</param>
        /// <returns>the new pathway object</returns>
        public static Pathway Create(UserProfile creator, Arc arc)
        {
            return Create(0, 0, null, null, null, creator, arc);
        }

        /// <summary>
        /// Create a new pathway
        /// </summary>
        /// <param name="currentAge">The last pathway's age</param>
        /// <param name="currentOrder">the last pathway's order</param>
        /// <param name="startingStanza">the stanza (if it exists) that was used to fork this from</param>
        /// <param name="forcedChoice">The choice already made by a forking</param>
        /// <param name="existingStatus">The plot status coming in from the existing pathway</param>
        /// <param name="creator">The user creating this</param>
        /// <param name="arc">The arc this belongs to</param>
        /// <returns>The new pathway</returns>
        public static Pathway Create(int currentAge, int currentOrder, Stanza startingStanza, Choice forcedChoice, PlotStatus existingStatus, UserProfile creator, Arc arc)
        {
            //No null creators
            if (creator == null)
                return null;

            //Generate the new key
            var key = Guid.NewGuid();

            //If the status is empty we just need a 0 status
            if (existingStatus == null)
                existingStatus = new PlotStatus();

            Pathway returnValue = null;
            //if the stanza is empty we're making a new pathway for a new arc
            if (startingStanza == null)
            {
                //Create the new starting pathway
                startingStanza = Stanza.GetNew(1, existingStatus);
                returnValue = new Pathway
                {
                    Key = key,
                    Age = 1,
                    Stanza = startingStanza,
                    Order = 1,
                    Status = existingStatus,
                    Creator = creator,
                    Arc = arc
                };
            }
            else
            {
                var newStatus = forcedChoice == null ? existingStatus : forcedChoice.AppendStatus(existingStatus);

                returnValue = new Pathway
                {
                    Key = key,
                    Stanza = startingStanza,
                    Choice = forcedChoice,
                    Status = newStatus,
                    Age = currentAge++,
                    Order = currentOrder++,
                    Creator = creator,
                    Arc = arc
                };
            }

            if (returnValue != null)
            {
                if(forcedChoice != null)
                    db.Choices.Attach(forcedChoice);

                if (arc != null)
                    db.Arcs.Attach(arc);

                db.UserProfiles.Attach(creator);

                db.Stanzas.Attach(startingStanza);
                db.Pathways.Add(returnValue);
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

            chainedDb.Pathways.Attach(this);
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

            chainedDb.Pathways.Attach(this);
            chainedDb.Entry(this).State = EntityState.Deleted;

            return chained ? -99 : chainedDb.SaveChanges();
        }

        /// <summary>
        /// Compare two pathways
        /// </summary>
        /// <param name="obj">Must be a pathway</param>
        /// <returns>
        /// -2: null or inequatable type
        /// -1: input is older than current in the story's arc
        /// 0: Equal
        /// 1: input is newer than current in the story's arc
        /// 2: Equatable type but completely foreign contexts
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Pathway))
                return -2;

            var other = (Pathway)obj;

            if (other.Equals(this))
                return 0;

            if (other.Arc.Equals(this.Arc))
                return other.Order > this.Order ? -1 : 1;
           
            return 2;
        }

        /// <summary>
        /// Equating by db id
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns>if they are the same db object</returns>
        public bool Equals(Pathway other)
        {
            return other != null && other.Id.Equals(this.Id);
        }
    }
}