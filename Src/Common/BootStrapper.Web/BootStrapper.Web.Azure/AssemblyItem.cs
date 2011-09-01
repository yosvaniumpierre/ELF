namespace Avanade.BootStrapper.Web.Azure
{
    internal class AssemblyItem
    {
        #region Constructors

        internal AssemblyItem(string name, byte[] bytes)
        {
            Name = name;
            Bytes = bytes;
        }

        #endregion Constructors

        #region Properties

        public byte[] Bytes
        {
            get; private set;
        }

        public string Name
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }

        #endregion Methods
    }
}