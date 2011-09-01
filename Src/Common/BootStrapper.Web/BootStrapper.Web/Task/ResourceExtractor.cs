namespace Avanade.BootStrapper.Web.Task
{
    using System.Collections.Generic;
    using System.Reflection;

    using NLog;

    public class ResourceExtractor
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Assembly assembly;

        #endregion Fields

        #region Constructors

        public ResourceExtractor(Assembly assembly)
        {
            this.assembly = assembly;
        }

        #endregion Constructors

        #region Methods

        public IList<EmbeddedViewResource> GetResources()
        {
            IList<EmbeddedViewResource> resources = new List<EmbeddedViewResource>();

            string[] names = assembly.GetManifestResourceNames();
            foreach (string name in names)
            {
                string assemblyName = assembly.GetName().Name;
                string qualifiedResourceName = name.Substring(assemblyName.Length + 1);
                string[] resourceTokens = qualifiedResourceName.Split(new[] { '.' });

                var resourceTokenCount = resourceTokens.Length;

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Fully Qualified Resource: {0}, Token Count: {1}", qualifiedResourceName, resourceTokenCount);
                }

                //Assumption that the last two tokens form up as the embedded resource file.
                var resourceFileExtension = resourceTokens[resourceTokenCount - 1];
                var resourceFileName = resourceTokens[resourceTokenCount - 2];
                var fileName = resourceFileName + "." + resourceFileExtension;

                string resourceFolder = string.Empty;
                if (resourceTokenCount - 3 >= 0)
                {
                    for (int i = 0; i < resourceTokenCount -2; i++)
                    {
                        resourceFolder += resourceTokens[i];
                        resourceFolder += ".";
                    }

                    resourceFolder = resourceFolder.Replace('.', '/');
                }

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Resource folder: {0}, Resource: {1}, Assembly Resource Path: {2}", resourceFolder, fileName, name);
                }

                resources.Add(new EmbeddedViewResource(resourceFolder, fileName, name));
            }
            return resources;
        }

        #endregion Methods
    }
}