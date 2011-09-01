using System.Web.Hosting;

namespace Avanade.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Linq;
    using System.Text;
    using System.Web.Security;

    using Domain.Security;

    using Repository.Services.Security;

    public sealed class RepositoryRoleProvider : RoleProvider
    {
        #region Fields

        private string applicationName;

        #endregion Fields

        #region Properties

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public IRoleRepository RoleRepository
        {
            get; set;
        }

        public IUserRepository UserRepository
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        //adds a user collection toa roles collection
        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                    throw new ProviderException(String.Format("Role name {0} not found.", rolename));
            }

            foreach (string username in usernames)
            {
                if (username.Contains(","))
                    throw new ArgumentException(String.Format("User names {0} cannot contain commas.", username));
                //is user not exiting //throw exception

                foreach (string rolename in rolenames)
                {
                    if (IsUserInRole(username, rolename))
                        throw new ProviderException(String.Format("User {0} is already in role {1}.", username, rolename));
                }
            }

            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    //get the user
                    var usr = UserRepository.GetUser(ApplicationName, username);

                    if (usr != null)
                    {
                        //get the role first from db
                        Role role = RoleRepository.GetRole(ApplicationName, rolename);

                        //Role role = GetRole(rolename);
                        usr.AddRole(role);
                    }
                    UserRepository.Update(usr);
                }
            }
        }

        //create  a new role with a given name
        public override void CreateRole(string rolename)
        {
            if (rolename.Contains(","))
                throw new ArgumentException("Role names cannot contain commas.");

            if (RoleExists(rolename))
                throw new ProviderException("Role name already exists.");

            var role = new Role { ApplicationName = ApplicationName, RoleName = rolename };
            RoleRepository.Add(role);
        }

        //delete a role with given name
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            const bool deleted = false;
            if (!RoleExists(rolename))
                throw new ProviderException("Role does not exist.");

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                throw new ProviderException("Cannot delete a populated role.");

            Role role = GetRole(rolename);
            RoleRepository.Remove(role);

            return deleted;
        }

        //find users that beloeng to a particular role , given a username, Note : does not do a LIke search
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            var sb = new StringBuilder();
            Role role = RoleRepository.GetRole(ApplicationName, rolename);

            IList<User> users = role.UsersInRole;
            if (users != null)
            {
                foreach (User u in users)
                {
                    if (String.Compare(u.UserName, usernameToMatch, true) == 0)
                        sb.Append(u.UserName + ",");
                }
            }
            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }
            return new string[0];
        }

        //get an array of all the roles
        public override string[] GetAllRoles()
        {
            var sb = new StringBuilder();
            IList<Role> allroles = RoleRepository.GetRoles(ApplicationName);

            foreach (Role r in allroles)
            {
                sb.Append(r.RoleName + ",");
            }

            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }

        //Get roles for a user by username
        public override string[] GetRolesForUser(string username)
        {
            var sb = new StringBuilder();
            var usr = UserRepository.GetUser(ApplicationName, username);

            if (usr != null)
            {
                IList<Role> usrroles = usr.Roles;
                foreach (Role r in usrroles)
                {
                    sb.Append(r.RoleName + ",");
                }
            }

            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }

        //Get users in a givenrolename
        public override string[] GetUsersInRole(string rolename)
        {
            var sb = new StringBuilder();
            Role role = RoleRepository.GetRole(ApplicationName, rolename);

            IList<User> usrs = role.UsersInRole;

            foreach (User u in usrs)
            {
                sb.Append(u.UserName + ",");
            }

            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }

        //initializes the role provider
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name) || name.Length == 0)
                name = "RepositoryRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Repository-Based Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
        }

        //determine is a user has a given role
        public override bool IsUserInRole(string username, string rolename)
        {
            bool userIsInRole = false;
            User usr = UserRepository.GetUser(ApplicationName, username);

            if (usr != null)
            {
                IList<Role> usrroles = usr.Roles;
                if (usrroles.Any(r => r.RoleName.Equals(rolename)))
                {
                    userIsInRole = true;
                }
            }

            return userIsInRole;
        }

        //remeove users from roles
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            User usr = null;
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                    throw new ProviderException(String.Format("Role name {0} not found.", rolename));
            }

            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    if (!IsUserInRole(username, rolename))
                        throw new ProviderException(String.Format("User {0} is not in role {1}.", username, rolename));
                }
            }

            //get user , get his roles , the remove the role and save
            foreach (string username in usernames)
            {
                usr = UserRepository.GetUser(ApplicationName, username);

                var rolestodelete = new List<Role>();
                foreach (string rolename in rolenames)
                {
                    IList<Role> roles = usr.Roles;
                    foreach (Role r in roles)
                    {
                        if (r.RoleName.Equals(rolename))
                            rolestodelete.Add(r);

                    }
                }
                foreach (Role rd in rolestodelete)
                    usr.RemoveRole(rd);

                UserRepository.Update(usr);
            }
        }

        //boolen to check if a role exists given a role name
        public override bool RoleExists(string rolename)
        {
            return RoleRepository.RoleExists(ApplicationName, rolename);
        }

        // A helper function to retrieve config values from the configuration file
        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        //get a role by name
        private Role GetRole(string rolename)
        {
            return RoleRepository.GetRole(ApplicationName, rolename);
        }

        #endregion Methods
    }
}