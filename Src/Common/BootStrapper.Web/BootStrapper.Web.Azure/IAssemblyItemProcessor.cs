namespace Avanade.BootStrapper.Web.Azure
{
    internal interface IAssemblyItemProcessor
    {
        #region Methods

        bool Process(AssemblyItem assemblyItem);

        #endregion Methods
    }
}