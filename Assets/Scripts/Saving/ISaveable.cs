namespace GameDevTV.Saving
{
    /// <summary>
    /// Implement in any component that has state to save/restore.
    /// </summary>
    public interface ISaveable
    {

        object CaptureState();

        void RestoreState(object state);
    }
}