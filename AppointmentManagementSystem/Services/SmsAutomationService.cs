namespace AppointmentManagementSystem.Services
{
    public class SmsAutomationService : IHostedService, IDisposable
    {
        #region Global Variables
        //The timespan period for the Timer
        private TimeSpan _tsTimerPeriod = TimeSpan.FromSeconds(30);
        //Timer for the checking if a SMS notification needs to be sent
        private Timer? _tTimer;
        //Store the Service Scope Factory
        private readonly IServiceScopeFactory _oScopeFactory;
        #endregion

        #region Constructor
        public SmsAutomationService(IServiceScopeFactory oScopeFactory)
        {
            _oScopeFactory = oScopeFactory;
        }
        #endregion

        #region Methods
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Set up timer to check every hour
            _tTimer = new Timer(InvokeSmsSystem, null, TimeSpan.Zero, _tsTimerPeriod);
            return Task.CompletedTask;
        }

        private void InvokeSmsSystem(object oState)
        {
            using (var vScope = _oScopeFactory.CreateScope())
            {
                var vSmsService = vScope.ServiceProvider.GetRequiredService<ISmsService>();
                vSmsService.StoreAppointmentsDue();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Stop the timer
            _tTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //Dispose the time
            _tTimer?.Dispose();
        }
        #endregion
    }
}
