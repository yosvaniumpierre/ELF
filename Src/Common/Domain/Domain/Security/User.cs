namespace Avanade.Domain.Security
{
    using System;
    using System.Collections.Generic;

    public class User
    {
        #region Constructors

        public User()
        {
            CreationDate = MinDate();
            LastPasswordChangedDate = MinDate();
            LastActivityDate = MinDate();
            LastLockedOutDate = MinDate();
            FailedPasswordAnswerAttemptWindowStart = MinDate();
            FailedPasswordAttemptWindowStart = MinDate();
            LastLoginDate = MinDate();
        }

        #endregion Constructors

        #region Properties

        public virtual string ApplicationName
        {
            get; set;
        }

        public virtual string Comment
        {
            get; set;
        }

        public virtual DateTime CreationDate
        {
            get; set;
        }

        public virtual string Email
        {
            get; set;
        }

        public virtual int FailedPasswordAnswerAttemptCount
        {
            get; set;
        }

        public virtual DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get; set;
        }

        public virtual int FailedPasswordAttemptCount
        {
            get; set;
        }

        public virtual DateTime FailedPasswordAttemptWindowStart
        {
            get; set;
        }

        public virtual long Id
        {
            get; set;
        }

        public virtual bool IsApproved
        {
            get; set;
        }

        public virtual bool IsLockedOut
        {
            get; set;
        }

        public virtual bool IsOnLine
        {
            get; set;
        }

        public virtual DateTime LastActivityDate
        {
            get; set;
        }

        public virtual DateTime LastLockedOutDate
        {
            get; set;
        }

        public virtual DateTime LastLoginDate
        {
            get; set;
        }

        public virtual DateTime LastPasswordChangedDate
        {
            get; set;
        }

        public virtual string Password
        {
            get; set;
        }

        public virtual string PasswordAnswer
        {
            get; set;
        }

        public virtual string PasswordQuestion
        {
            get; set;
        }

        public virtual IList<Role> Roles
        {
            get; set;
        }

        public virtual string UserName
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public virtual void AddRole(Role role)
        {
            role.UsersInRole.Add(this);
            Roles.Add(role);
        }

        public virtual void RemoveRole(Role role)
        {
            role.UsersInRole.Remove(this);
            Roles.Remove(role);
        }

        private static DateTime MinDate()
        {
            return Convert.ToDateTime("01/01/1753");
        }

        #endregion Methods
    }
}