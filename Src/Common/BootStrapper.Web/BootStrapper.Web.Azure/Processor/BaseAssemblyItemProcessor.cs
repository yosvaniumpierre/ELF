namespace Avanade.BootStrapper.Web.Azure.Processor
{
    using System;
    using System.Text.RegularExpressions;

    internal class BaseAssemblyItemProcessor : IAssemblyItemProcessor
    {
        #region Methods

        public virtual bool Process(AssemblyItem assemblyItem)
        {
            throw new NotImplementedException();
        }

        protected bool IsMatch(string assemblyName, string pattern)
        {
            string searchPattern;

            if (pattern.StartsWith("*"))
            {
                searchPattern = pattern.Remove(0, 1);
            }
            else
            {
                throw new ArgumentException("Pattern used for assembly name matching is expected to started with an asterisk (*)", pattern);
            }

            return Regex.IsMatch(assemblyName, searchPattern, RegexOptions.IgnoreCase);
        }

        #endregion Methods
    }
}