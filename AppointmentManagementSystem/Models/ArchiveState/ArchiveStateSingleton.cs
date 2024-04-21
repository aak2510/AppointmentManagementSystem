namespace AppointmentManagementSystem.Models.ArchiveState;

public class ArchiveStateSingleton
{
    private static ArchiveStateSingleton _instance;
    private bool _isViewingArchivedAppointments;

    private ArchiveStateSingleton()
    {
        // Default value for the archive state
        _isViewingArchivedAppointments = false;
    }

    public static ArchiveStateSingleton Instance
    {
        get
        {
            // Lazy initialization of the singleton instance
            if (_instance == null)
            {
                _instance = new ArchiveStateSingleton();
            }
            return _instance;
        }
    }

    public bool IsViewingArchivedAppointments
    {
        get { return _isViewingArchivedAppointments; }
        set { _isViewingArchivedAppointments = value; }
    }
}