namespace MakersWrath.Saving
{
    /// <summary>
    /// Interface for entities able to load and save
    /// </summary>
    public interface ISaveManager
    {
        void Load();
        void Save();
    }
}