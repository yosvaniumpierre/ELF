namespace Avanade.Repository.Services.Hibernate.Integration.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Domain.Security;
    using Core;

    using FluentNHibernate.Testing;

    using NHibernate;

    using NUnit.Framework;

    [TestFixture]
    public class TestNHibernateMapping
    {
        #region Fields

        private ISession session;

        #endregion Fields

        #region Methods

        [Test]
        public void CanCorrectlyMapRole()
        {
            new PersistenceSpecification<Role>(session, new RoleEqualityComparer())
                .CheckProperty(c => c.Id, (long)1)
                .CheckProperty(c => c.ApplicationName, "App")
                .CheckProperty(c => c.RoleName, "RoleName")
                .CheckList(c => c.UsersInRole, new List<User> { new User { ApplicationName = "AppName", UserName = "User"} })
                .VerifyTheMappings();
        }


        [Test]
        public void CanCorrectlyMapUser()
        {
            new PersistenceSpecification<User>(session, new RoleEqualityComparer())
                .CheckProperty(c => c.Id, (long)1)
                .CheckProperty(c => c.UserName, "Doe")
                .CheckProperty(c => c.ApplicationName, "App")
                .CheckProperty(c => c.Comment, "Comment")
                .CheckProperty(c => c.CreationDate, DateTime.Today)
                .CheckProperty(c => c.Email, "Doe@asd.com")
                .CheckProperty(c => c.FailedPasswordAnswerAttemptCount, 1)
                .CheckProperty(c => c.FailedPasswordAnswerAttemptWindowStart, DateTime.Today)
                .CheckProperty(c => c.FailedPasswordAttemptCount, 3)
                .CheckProperty(c => c.FailedPasswordAttemptWindowStart, DateTime.Today)
                .CheckProperty(c => c.IsApproved, true)
                .CheckProperty(c => c.IsLockedOut, false)
                .CheckProperty(c => c.IsOnLine, false)
                .CheckProperty(c => c.LastActivityDate, DateTime.Today)
                .CheckProperty(c => c.LastLockedOutDate, DateTime.Today)
                .CheckProperty(c => c.LastLoginDate, DateTime.Today)
                .CheckProperty(c => c.LastPasswordChangedDate, DateTime.Today)
                .CheckProperty(c => c.Password, "Password")
                .CheckProperty(c => c.PasswordAnswer, "Answer")
                .CheckProperty(c => c.PasswordQuestion, "Question")
                .CheckList(c => c.Roles, new List<Role> { new Role { ApplicationName = "AppName" } })
                .VerifyTheMappings();
        }

        [SetUp]
        public void Setup()
        {
            var persistenceManager = new NHibernatePersistenceManager();
            persistenceManager.Init("Data Source=localhost;Initial Catalog=Test;Integrated Security=True", true);
            session = persistenceManager.SessionFactory.OpenSession();
        }

        #endregion Methods
    }

    internal class RoleEqualityComparer : IEqualityComparer
    {
        #region Methods

        public new bool Equals(object x, object y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (x is Role && y is Role)
            {
                return ((Role)x).Id == ((Role)y).Id;
            }
            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}