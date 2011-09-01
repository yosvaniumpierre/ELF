namespace Avanade.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Linq;
    using System.Web.Profile;

    using Domain.Security;

    using Repository.Services.Security;

    public sealed class RepositoryProfileProvider : ProfileProvider
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

        public IProfileRepository ProfileRepository
        {
            get; set;
        }

        public IUserRepository UserRepository
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            string userIds = "";
            bool anaon = false;
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    anaon = true;
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    break;
            }

            IList<Profile> profs = ProfileRepository.GetProfiles(ApplicationName, userInactiveSinceDate, anaon);

            if (profs != null)
            {
                userIds = profs.Aggregate(userIds, (current, p) => current + (p.Id.ToString() + ","));
            }

            if (userIds.Length > 0)
                userIds = userIds.Substring(0, userIds.Length - 1);

            return DeleteProfilesbyId(userIds.Split(','));
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            int deleteCount = 0;
            try
            {
                deleteCount += profiles.Cast<ProfileInfo>().Count(p => DeleteProfile(p.UserName));
            }
            catch (Exception e)
            {
                throw new ProviderException("DeleteProfiles(ProfileInfoCollection)", e);
            }
            return deleteCount;
        }

        public override int DeleteProfiles(string[] usernames)
        {
            int deleteCount = 0;
            try
            {
                deleteCount += usernames.Count(DeleteProfile);
            }
            catch (Exception e)
            {
                throw new ProviderException("DeleteProfiles(String())", e);
            }
            return deleteCount;
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch,
            DateTime userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            var profiles = new ProfileInfoCollection();
            totalRecords = 0;

            try
            {
                var profileList = ProfileRepository.GetProfiles(ApplicationName, IsAnonymous(authenticationOption), userInactiveSinceDate, pageIndex, pageSize);
                var users = UserRepository.FindUsersByName(usernameToMatch, pageIndex, pageSize, ApplicationName);
                foreach (var user in users)
                {
                    foreach (var profile in profileList)
                    {
                        if (profile.UserId == user.Id)
                        {
                            profiles.Add(GetProfileInfoFromProfile(profile));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            totalRecords = profiles.Count;
            return profiles;
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            var profiles = new ProfileInfoCollection();
            totalRecords = 0;

            try
            {
                var profileList = ProfileRepository.GetProfiles(ApplicationName, IsAnonymous(authenticationOption),
                                                                pageIndex, pageSize);
                var users = UserRepository.FindUsersByName(usernameToMatch, pageIndex, pageSize, ApplicationName);
                foreach (var user in users)
                {
                    foreach (var profile in profileList)
                    {
                        if (profile.UserId == user.Id)
                        {
                            profiles.Add(GetProfileInfoFromProfile(profile));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            totalRecords = profiles.Count;
            return profiles;
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            var profiles = new ProfileInfoCollection();
            totalRecords = 0;

            try
            {
                var profileList = ProfileRepository.GetProfiles(applicationName, IsAnonymous(authenticationOption), userInactiveSinceDate, pageIndex, pageSize);
                foreach (var profile in profileList)
                {
                    profiles.Add(GetProfileInfoFromProfile(profile));
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            totalRecords = profiles.Count;
            return profiles;
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            var profiles = new ProfileInfoCollection();
            totalRecords = 0;

            try
            {
                var profileList = ProfileRepository.GetProfiles(applicationName, IsAnonymous(authenticationOption), pageIndex, pageSize);
                foreach (var profile in profileList)
                {
                    profiles.Add(GetProfileInfoFromProfile(profile));
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            totalRecords = profiles.Count;
            return profiles;
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            return ProfileRepository.GetProfiles(ApplicationName, userInactiveSinceDate, IsAnonymous(authenticationOption)).Count;
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection ppc)
        {
            var username = (string)context["UserName"];
            var isAuthenticated = (bool)context["IsAuthenticated"];

            Profile profile = GetProfile(username, isAuthenticated);
            // The serializeAs attribute is ignored in this provider implementation.
            var svc = new SettingsPropertyValueCollection();

            if (profile == null)
            {
                if (IsMembershipUser(username))
                    profile = CreateProfile(username, false);
                else
                    throw new ProviderException("Profile cannot be created. There is no membership user");
            }

            foreach (SettingsProperty prop in ppc)
            {
                var pv = new SettingsPropertyValue(prop);
                switch (prop.Name)
                {
                    case "IsAnonymous":
                        pv.PropertyValue = profile.IsAnonymous;
                        break;
                    case "LastActivityDate":
                        pv.PropertyValue = profile.LastActivityDate;
                        break;
                    case "LastUpdatedDate":
                        pv.PropertyValue = profile.LastUpdatedDate;
                        break;
                    case "Subscription":
                        pv.PropertyValue = profile.Subscription;
                        break;
                    case "Language":
                        pv.PropertyValue = profile.Language;
                        break;
                    case "FirstName":
                        pv.PropertyValue = profile.FirstName;
                        break;
                    case "LastName":
                        pv.PropertyValue = profile.LastName;
                        break;
                    case "Gender":
                        pv.PropertyValue = profile.Gender;
                        break;
                    case "BirthDate":
                        pv.PropertyValue = profile.BirthDate;
                        break;
                    case "Occupation":
                        pv.PropertyValue = profile.Occupation;
                        break;
                    case "Website":
                        pv.PropertyValue = profile.Website;
                        break;
                    case "Street":
                        pv.PropertyValue = profile.Street;
                        break;
                    case "City":
                        pv.PropertyValue = profile.City;
                        break;
                    case "State":
                        pv.PropertyValue = profile.State;
                        break;
                    case "Zip":
                        pv.PropertyValue = profile.Zip;
                        break;
                    case "Country":
                        pv.PropertyValue = profile.Country;
                        break;

                    default:
                        throw new ProviderException("Unsupported property.");
                }

                svc.Add(pv);
            }

            UpdateActivityDates(username, isAuthenticated, true);
            return svc;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "RepositoryProfileProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Fluent Nhibernate Profile provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection ppvc)
        {
            // The serializeAs attribute is ignored in this provider implementation.
            var username = (string)context["UserName"];
            var isAuthenticated = (bool)context["IsAuthenticated"];

            Profile profile = GetProfile(username, isAuthenticated) ?? CreateProfile(username, !isAuthenticated);

            foreach (SettingsPropertyValue pv in ppvc)
            {
                switch (pv.Property.Name)
                {
                    case "IsAnonymous":
                        profile.IsAnonymous = (bool)pv.PropertyValue;
                        break;
                    case "LastActivityDate":
                        profile.LastActivityDate = (DateTime)pv.PropertyValue;
                        break;
                    case "LastUpdatedDate":
                        profile.LastUpdatedDate = (DateTime)pv.PropertyValue;
                        break;
                    case "Subscription":
                        profile.Subscription = pv.PropertyValue.ToString();
                        break;
                    case "Language":
                        profile.Language = pv.PropertyValue.ToString();
                        break;
                    case "FirstName":
                        profile.FirstName = pv.PropertyValue.ToString();
                        break;
                    case "LastName":
                        profile.LastName = pv.PropertyValue.ToString();
                        break;
                    case "Gender":
                        profile.Gender = pv.PropertyValue.ToString();
                        break;
                    case "BirthDate":
                        profile.BirthDate = (DateTime)pv.PropertyValue;
                        break;
                    case "Occupation":
                        profile.Occupation = pv.PropertyValue.ToString();
                        break;
                    case "Website":
                        profile.Website = pv.PropertyValue.ToString();
                        break;
                    case "Street":
                        profile.Street = pv.PropertyValue.ToString();
                        break;
                    case "City":
                        profile.City = pv.PropertyValue.ToString();
                        break;
                    case "State":
                        profile.State = pv.PropertyValue.ToString();
                        break;
                    case "Zip":
                        profile.Zip = pv.PropertyValue.ToString();
                        break;
                    case "Country":
                        profile.Country = pv.PropertyValue.ToString();
                        break;
                    default:
                        throw new ProviderException("Unsupported property.");
                }
            }

            ProfileRepository.Update(profile);

            UpdateActivityDates(username, isAuthenticated, false);
        }

        private static void CheckParameters(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
                throw new ArgumentException("Page index must 0 or greater.");
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0.");
        }

        private static bool IsAnonymous(ProfileAuthenticationOption authenticationOption)
        {
            bool isAnon = false;
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    isAnon = true;
                    break;
            }

            return isAnon;
        }

        private Profile CreateProfile(string username, bool isAnonymous)
        {
            var p = new Profile();
            User usr = UserRepository.GetUser(ApplicationName, username);

            if (usr != null) //membership user exits so create a profile
            {
                p.UserId = usr.Id;
                p.IsAnonymous = isAnonymous;
                p.LastUpdatedDate = DateTime.Now;
                p.LastActivityDate = DateTime.Now;
                p.ApplicationName = ApplicationName;
                ProfileRepository.Add(p);
            }
            else
                throw new ProviderException("Membership User does not exist.Profile cannot be created.");

            return p;
        }

        private bool DeleteProfile(string username)
        {
            // Check for valid user name.
            if (username == null)
                throw new ArgumentNullException("username", "User name cannot be null.");
            if (username.Contains(","))
                throw new ArgumentException("User name cannot contain a comma (,).");

            Profile profile = GetProfile(username);
            if (profile == null)
                return false;

            ProfileRepository.Remove(profile);

            return true;
        }

        private int DeleteProfilesbyId(IEnumerable<string> ids)
        {
            int deleteCount = 0;
            try
            {
                deleteCount += ids.Count(DeleteProfile);
            }
            catch (Exception e)
            {
                throw new ProviderException(e.Message, e);
            }
            return deleteCount;
        }

        // A helper function to retrieve config values from the configuration file
        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        //get a role by name
        private Profile GetProfile(string username, bool isAuthenticated)
        {
            Profile profile = null;
            //Is authenticated and IsAnonmous are opposites,so flip sign,IsAuthenticated = true -> notAnonymous
            bool isAnonymous = !isAuthenticated;
            User usr = UserRepository.GetUser(ApplicationName, username);

            if (usr != null)
            {
                profile = ProfileRepository.GetProfile(usr.Id, isAnonymous);
            }

            return profile;
        }

        private Profile GetProfile(string username)
        {
            Profile profile = null;
            User usr = UserRepository.GetUser(ApplicationName, username);

            if (usr != null)
            {
                profile = ProfileRepository.GetProfile(usr.Id);
            }

            return profile;
        }

        private ProfileInfo GetProfileInfoFromProfile(Profile p)
        {
            User usr = UserRepository.GetUserById(ApplicationName, p.UserId);

            if (usr == null)
                throw new ProviderException("The userid not found in memebership tables.GetProfileInfoFromProfile(p)");

            // ProfileInfo.Size not currently implemented.
            var pi = new ProfileInfo(usr.UserName,
                p.IsAnonymous, p.LastActivityDate, p.LastUpdatedDate, 0);

            return pi;
        }

        private bool IsMembershipUser(string username)
        {
            bool hasMembership = false;
            User usr = UserRepository.GetUser(ApplicationName, username);

            if (usr != null) //membership user exits so create a profile
                hasMembership = true;

            return hasMembership;
        }

        // Updates the LastActivityDate and LastUpdatedDate values  when profile properties are accessed by the
        // GetPropertyValues and SetPropertyValues methods. Passing true as the activityOnly parameter will update only the LastActivityDate.
        private void UpdateActivityDates(string username, bool isAuthenticated, bool activityOnly)
        {
            //Is authenticated and IsAnonmous are opposites,so flip sign,IsAuthenticated = true -> notAnonymous
            bool isAnonymous = !isAuthenticated;
            DateTime activityDate = DateTime.Now;

            Profile pr = GetProfile(username, isAuthenticated);
            if (pr == null)
                throw new ProviderException("User Profile not found");
            try
            {
                if (activityOnly)
                {
                    pr.LastActivityDate = activityDate;
                    pr.IsAnonymous = isAnonymous;
                }
                else
                {
                    pr.LastActivityDate = activityDate;
                    pr.LastUpdatedDate = activityDate;
                    pr.IsAnonymous = isAnonymous;
                }

                ProfileRepository.Update(pr);
            }
            catch (Exception e)
            {
                throw new ProviderException(e.Message, e);
            }
        }

        #endregion Methods
    }
}