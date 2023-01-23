namespace MakersWrath.Saving
{
    /// <summary>
    /// Interface for any components which need their state captured for the save ability
    /// </summary>
    public interface ISaveable
    {
        object CaptureState();
        void RestoreState(object state);
    }
}