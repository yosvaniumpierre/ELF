namespace Avanade.BootStrapper.Web.Azure.Processor
{
    using System.Collections.Generic;
    using System.Reflection;

    using NLog;

    using Task;

    internal static class ResourceUtil
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        internal static IList<EmbeddedViewResource> Extract(Assembly assembly)
        {
            var resourceExtractor = new ResourceExtractor(assembly);
            var viewResources = resourceExtractor.GetResources();

            foreach (var viewResource in viewResources)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Extracting the resource bytes for item: {0}", viewResource.FileName);
                }
                using (var stream = assembly.GetManifestResourceStream(viewResource.ManifestResourceName))
                {
                    if (stream == null || stream.Length == 0)
                    {
                        continue;
                    }

                    // Fill the bytes[] array with the stream data
                    var bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);

                    viewResource.ResourceBytes = bytesInStream;

                    stream.Close();
                }
            }

            Logger.Info("Number of resources extracted: {0}", viewResources.Count);

            return viewResources;
        }

        #endregion Methods
    }
}