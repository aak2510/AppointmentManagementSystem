namespace AppointmentManagementSystem.Models.ArchiveState;

// Singleton class to centralise storing whether we are viewing the archived appointments
public class ArchiveStateSingleton
{
    // The single instance of this class
    private static ArchiveStateSingleton _instance;

    // Boolean to indicate if we are viewing the archived appointments
    private bool _isViewingArchivedAppointments;

    // Private constructor to prevent external instantiation
    private ArchiveStateSingleton()
    {
        // Default value for the archive state
        // Set of first instantion
        _isViewingArchivedAppointments = false;
    }

    // Property to access the singleton instance
    public static ArchiveStateSingleton Instance
    {
        // Initialization of the singleton instance
        get
        {
            // If an instance doesnt exist we create one
            if (_instance == null)
            {
                _instance = new ArchiveStateSingleton();
            }
            return _instance;
        }
    }

    // Property to get or set the viewing state of archived appointments
    public bool IsViewingArchivedAppointments
    {
        get { return _isViewingArchivedAppointments; }
        set { _isViewingArchivedAppointments = value; }
    }
}