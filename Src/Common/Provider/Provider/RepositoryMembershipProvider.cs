namespace Avanade.Provider
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Security;

    using Domain.Security;

    using NLog;

    using Repository.Services.Security;

    public class RepositoryMembershipProvider : MembershipProvider
    {
        #region Fields

        // Global connection string, generated password length, generic exception message, event log info.
        private const int NewPasswordLength = 8;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string applicationName;
        private bool enablePasswordReset;
        private bool enablePasswordRetrieval;

        // Used when determining encryption key values.
        private MachineKeySection machineKey;
        private int maxInvalidPasswordAttempts;
        private int minRequiredNonAlphanumericCharacters;
        private int minRequiredPasswordLength;
        private int passwordAttemptWindow;
        private MembershipPasswordFormat passwordFormat;
        private string passwordStrengthRegularExpression;
        private bool requiresQuestionAndAnswer;
        private bool requiresUniqueEmail;

        #endregion Fields

        #region Enumerations

        private enum FailureType
        {
            Password = 1,
            PasswordAnswer = 2
        }

        #endregion Enumerations

        #region Properties

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return passwordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }

        public virtual IUserRepository Repository
        {
            get; set;
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }

        #endregion Properties

        #region Methods

        // Change password for a user
        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            if (!ValidateUser(username, oldPwd))
            {
                return false;
            }
            var args = new ValidatePasswordEventArgs(username, newPwd, false)
                           {
                               FailureInformation =
                                   new MembershipPasswordException(
                                   "Change password canceled due to new password validation failure.")
                           };
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                throw args.FailureInformation;
            }
            try
            {
                var u = Repository.GetUser(applicationName, username);
                u.Password = EncodePassword(newPwd);
                Repository.Update(u);
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            return true;
        }

        // Change Password Question And Answer for a user
        public override bool ChangePasswordQuestionAndAnswer(string username,
            string password,
            string newPwdQuestion,
            string newPwdAnswer)
        {
            int rowsAffected = 0;
            if (!ValidateUser(username, password))
                return false;

            try
            {
                var usr = Repository.GetUser(applicationName, username);
                if (usr != null)
                {
                    usr.PasswordQuestion = newPwdQuestion;
                    usr.PasswordAnswer = newPwdAnswer;
                    Repository.Update(usr);
                    rowsAffected = 1;
                }
            }
            catch (Exception e)
            {
                Logger.Error(GetExceptionMessage("ChangePasswordQuestionAndAnswer"), e);
                throw;
            }

            return rowsAffected > 0;
        }

        // Create a new Membership user
        public override MembershipUser CreateUser(string username,
            string password,
            string email,
            string passwordQuestion,
            string passwordAnswer,
            bool isApproved,
            object providerUserKey,
            out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if ((RequiresUniqueEmail && (GetUserNameByEmail(email) != String.Empty)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            var membershipUser = GetUser(username, false);
            if (membershipUser == null)
            {
                var u = new User
                             {
                                 UserName = username,
                                 ApplicationName = applicationName,
                                 Password = EncodePassword(password),
                                 Email = email,
                                 PasswordQuestion = passwordQuestion,
                                 PasswordAnswer = EncodePassword(passwordAnswer),
                                 IsApproved = isApproved,
                                 Comment = String.Empty,
                                 CreationDate = DateTime.Now
                             };
                try
                {
                    Repository.Update(u);
                    status = MembershipCreateStatus.Success;
                }
                catch (Exception ex)
                {
                    throw new MemberAccessException("Error processing membership data - " + ex.Message);
                }
                return GetUser(username, false);
            }
            status = MembershipCreateStatus.DuplicateUserName;
            return null;
        }

        // Delete a user
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            try
            {
                User u = Repository.GetUser(applicationName, username);
                if (u != null)
                {
                    Repository.Remove(u);
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            return true;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                var uList = Repository.FindUsersByEmail(emailToMatch, pageIndex, pageSize, applicationName);
                foreach (var u in uList)
                {
                    users.Add(GetUserFromObject(u));
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            totalRecords = users.Count;
            return users;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                var uList = Repository.FindUsersByName(usernameToMatch, pageIndex, pageSize, applicationName);
                foreach (User u in uList)
                {
                    users.Add(GetUserFromObject(u));
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            totalRecords = users.Count;
            return users;
        }

        // Get all users in db
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                var uList = Repository.GetAllUsers(pageIndex, pageSize, applicationName);
                foreach (var u in uList)
                {
                    users.Add(GetUserFromObject(u));
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
            totalRecords = users.Count;
            return users;
        }

        // Gets a number of online users
        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            var compareTime = DateTime.Now.Subtract(onlineSpan);
            int numOnline;

            try
            {
                numOnline = Repository.GetNumberOfUsersOnline(applicationName, compareTime);
            }
            catch (Exception e)
            {
                Logger.Error(GetExceptionMessage("GetNumberOfUsersOnline"), e);
                throw;
            }

            return numOnline;
        }

        // Get a password fo a user
        public override string GetPassword(string username, string answer)
        {
            string password;
            string passwordAnswer;

            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }

            try
            {
                var curUser = Repository.GetUser(applicationName, username);
                password = curUser.Password;
                passwordAnswer = curUser.PasswordAnswer;
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, FailureType.PasswordAnswer);
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }
            return password;
        }

        // Get a membership user by username
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            MembershipUser membershipUser = null;

            try
            {
                var u = Repository.GetUser(applicationName, username);
                if (u != null)
                {
                    if (userIsOnline)
                    {
                        u.LastActivityDate = DateTime.Now;
                        Repository.Update(u);
                    }
                    membershipUser = GetUserFromObject(u);
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Unable to retrieve user data - " + ex.Message);
            }

            return membershipUser;
        }

        //  // Get a membership user by key ( in our case key is int)
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            MembershipUser membershipUser = null;

            try
            {
                long id;
                if (Int64.TryParse(providerUserKey.ToString(), out id))
                {
                    User u = Repository.GetUserById(ApplicationName, id);
                    if (u != null)
                    {
                        if (userIsOnline)
                        {
                            u.LastActivityDate = DateTime.Now;
                            Repository.Update(u);
                        }
                        membershipUser = GetUserFromObject(u);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }

            return membershipUser;
        }

        //Gets a membehsip user by email
        public override string GetUserNameByEmail(string email)
        {
            try
            {
                return Repository.GetUserNameByEmail(applicationName, email);
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
        }

        // Initilaize the provider
        public override void Initialize(string providerName, NameValueCollection config)
        {
            config = SetConfigDefaults(config);
            providerName = SetDefaultName(providerName);

            base.Initialize(providerName, config);

            ValidatingPassword += MembershipProviderValidatingPassword;
            SetConfigurationProperties(config);
            CheckEncryptionKey();
        }

        // Reset password for a user
        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password Reset is not enabled.");
            }

            if ((answer == null) && (RequiresQuestionAndAnswer))
            {
                UpdateFailureCount(username, FailureType.PasswordAnswer);
                throw new ProviderException("Password answer required for password Reset.");
            }

            string newPassword = "#" + Guid.NewGuid().ToString().Substring(0, 8) + "$";
            var args = new ValidatePasswordEventArgs(username, newPassword, false);
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }
                throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
            }

            try
            {
                User u = Repository.GetUser(applicationName, username);
                if (u.IsLockedOut)
                {
                    throw new MembershipPasswordException("The supplied user is locked out.");
                }
                string passwordAnswer = u.PasswordAnswer;
                if (RequiresQuestionAndAnswer && (!CheckPassword(answer, passwordAnswer)))
                {
                    UpdateFailureCount(username, FailureType.PasswordAnswer);
                    throw new MembershipPasswordException("Incorrect password answer.");
                }
                u.Password = newPassword;
                Repository.Update(u);
                return newPassword;
            }
            catch (Exception ex)
            {
                throw new MembershipPasswordException(ex.Message);
            }
        }

        //Unlock a user given a username
        public override bool UnlockUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                new MemberAccessException("Use name parameter for the UnlockUser method is either null or empty!");
            }

            var unlocked = false;

            try
            {
                var usr = Repository.GetUser(applicationName, username);

                if (usr != null)
                {
                    usr.LastLockedOutDate = DateTime.Now;
                    usr.IsLockedOut = false;
                    Repository.Update(usr);
                    unlocked = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(GetExceptionMessage("UnlockUser"), e);
                throw new MemberAccessException(string.Format("Unable to unlock the user ({0})", username));
            }

            return unlocked;
        }

        // Update a user information
        public override void UpdateUser(MembershipUser user)
        {
            if (user == null)
            {
                throw new MemberAccessException("User parameter in the UpdateUser method is null");
            }
            try
            {
                var usr = Repository.GetUser(applicationName, user.UserName);
                if (usr != null)
                {
                    usr.Email = user.Email;
                    usr.Comment = user.Comment;
                    usr.IsApproved = user.IsApproved;
                    Repository.Update(usr);
                }
            }
            catch (Exception e)
            {
                Logger.Error(GetExceptionMessage("UpdateUser"), e);
                throw;
            }
        }

        // Validates as user
        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            try
            {
                var usr = Repository.GetUser(applicationName, username);
                if (usr == null)
                    return false;
                if (usr.IsLockedOut)
                    return false;

                if (CheckPassword(password, usr.Password))
                {
                    if (usr.IsApproved)
                    {
                        isValid = true;
                        usr.LastLoginDate = DateTime.Now;
                        Repository.Update(usr);
                    }
                }
                else
                    UpdateFailureCount(username, FailureType.Password);
            }
            catch (Exception e)
            {
                string msg = GetExceptionMessage("ValidateUser");
                Logger.Error(msg, e);
                throw new MemberAccessException(msg, e);
            }
            return isValid;
        }

        protected virtual MachineKeySection GetMachineKeySection()
        {
            //Made virtual for testability
            Configuration cfg = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
            return cfg.GetSection("system.web/machineKey") as MachineKeySection;
        }

        // A Function to retrieve config values from the configuration file
        private static string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        // GetExceptionMessage
        // A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also
        // thrown by the caller.
        private static string GetExceptionMessage(string action)
        {
            var message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            return message;
        }

        //   Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.
        private static byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private void CheckEncryptionKey()
        {
            if (machineKey.ValidationKey.Contains("AutoGenerate"))
            {
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                {
                    throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");
                }
            }
        }

        //CheckPassword: Compares password values based on the MembershipPasswordFormat.
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }

        //EncodePassword:Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        private string EncodePassword(string password)
        {
            var encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                        Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    var hash = new HMACSHA1 {Key = HexToByte(machineKey.ValidationKey)};
                    encodedPassword =
                        Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new Exception("Unsupported password format.");
            }
            return encodedPassword;
        }

        /// <summary>
        /// Create a MembershipUser object from a user object.
        /// </summary>
        /// <param name="u">User Object.</param>
        /// <returns>MembershipUser object.</returns>
        private MembershipUser GetUserFromObject(User u)
        {
            var creationDate = u.CreationDate;
            var lastLoginDate = u.LastLoginDate;
            var lastActivityDate = u.LastActivityDate;
            var lastPasswordChangedDate = u.LastPasswordChangedDate;
            var lastLockedOutDate = u.LastLockedOutDate;

            var membershipUser = new MembershipUser(
                Name,
                u.UserName,
                u.Id,
                u.Email,
                u.PasswordQuestion,
                u.Comment,
                u.IsApproved,
                u.IsLockedOut,
                creationDate,
                lastLoginDate,
                lastActivityDate,
                lastPasswordChangedDate,
                lastLockedOutDate
                );
            return membershipUser;
        }

        private void MembershipProviderValidatingPassword(object sender, ValidatePasswordEventArgs e)
        {
            //Enforce our criteria
            var errorMessage = "";
            var pwChar = e.Password.ToCharArray();
            //Check Length
            if (e.Password.Length < minRequiredPasswordLength)
            {
                errorMessage += "[Minimum length: " + minRequiredPasswordLength + "]";
                e.Cancel = true;
            }
            //Check Strength
            if (passwordStrengthRegularExpression != string.Empty)
            {
                var r = new Regex(passwordStrengthRegularExpression);
                if (!r.IsMatch(e.Password))
                {
                    errorMessage += "[Insufficient Password Strength]";
                    e.Cancel = true;
                }
            }
            //Check Non-alpha characters
            var iNumNonAlpha = pwChar.Count(c => !char.IsLetterOrDigit(c));
            if (iNumNonAlpha < minRequiredNonAlphanumericCharacters)
            {
                errorMessage += "[Insufficient Non-Alpha Characters]";
                e.Cancel = true;
            }
            e.FailureInformation = new MembershipPasswordException(errorMessage);
        }

        private NameValueCollection SetConfigDefaults(NameValueCollection config)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Repository-Based Membership Provider");
            }
            return config;
        }

        private void SetConfigurationProperties(NameValueCollection config)
        {
            applicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
            maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            minRequiredNonAlphanumericCharacters =
                Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], NewPasswordLength.ToString()));
            passwordStrengthRegularExpression =
                Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));

            SetPasswordFormat(config["passwordFormat"]);
            machineKey = GetMachineKeySection();
        }

        private string SetDefaultName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = "RepositoryMembershipProvider";
            }
            return name;
        }

        private void SetPasswordFormat(string format)
        {
            if (format == null)
            {
                format = "Clear";
            }

            switch (format)
            {
                case "Hashed":
                    passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }
        }

        // UnEncodePassword :Decrypts or leaves the password clear based on the PasswordFormat.
        private string UnEncodePassword(string encodedPassword)
        {
            var password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                        Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new Exception("Cannot unencode a hashed password.");
                default:
                    throw new Exception("Unsupported password format.");
            }

            return password;
        }

        /// <summary>
        /// Update password and answer failure information.
        /// </summary>
        /// <param name="username">User name.</param>
        /// <param name="failureType">Type of failure</param>
        /// <remarks></remarks>
        private void UpdateFailureCount(string username, FailureType failureType)
        {
            try
            {
                User u = Repository.GetUser(applicationName, username);
                if (u != null)
                {
                    if (failureType == FailureType.Password) u.FailedPasswordAttemptCount++;
                    else u.FailedPasswordAnswerAttemptCount++;
                    Repository.Update(u);
                }
            }
            catch (Exception ex)
            {
                throw new MemberAccessException("Error processing membership data - " + ex.Message);
            }
        }

        #endregion Methods
    }
}