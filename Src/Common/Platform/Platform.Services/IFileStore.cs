namespace Avanade.Platform.Services
{
    public interface IFileStore
    {
        #region Methods

        byte[] Read(string filePath);

        void Save(string filePath, byte[] contents);

        #endregion Methods
    }
}