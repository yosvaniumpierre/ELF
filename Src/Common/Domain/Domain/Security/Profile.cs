namespace Avanade.Domain.Security
{
    using System;

    public class Profile
    {
        #region Constructors

        public Profile()
        {
            LastActivityDate = MinDate();
            LastUpdatedDate = MinDate();
            BirthDate = MinDate();
        }

        #endregion Constructors

        #region Properties

        public virtual string ApplicationName
        {
            get; set;
        }

        public virtual DateTime BirthDate
        {
            get; set;
        }

        public virtual string City
        {
            get; set;
        }

        public virtual string Country
        {
            get; set;
        }

        public virtual string FirstName
        {
            get; set;
        }

        public virtual string Gender
        {
            get; set;
        }

        public virtual long Id
        {
            get; private set;
        }

        public virtual bool IsAnonymous
        {
            get; set;
        }

        public virtual string Language
        {
            get; set;
        }

        public virtual DateTime LastActivityDate
        {
            get; set;
        }

        public virtual string LastName
        {
            get; set;
        }

        public virtual DateTime LastUpdatedDate
        {
            get; set;
        }

        public virtual string Occupation
        {
            get; set;
        }

        public virtual string State
        {
            get; set;
        }

        public virtual string Street
        {
            get; set;
        }

        public virtual string Subscription
        {
            get; set;
        }

        public virtual long UserId
        {
            get; set;
        }

        public virtual string Website
        {
            get; set;
        }

        public virtual string Zip
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        private static DateTime MinDate()
        {
            return Convert.ToDateTime("01/01/1753");
        }

        #endregion Methods
    }
}