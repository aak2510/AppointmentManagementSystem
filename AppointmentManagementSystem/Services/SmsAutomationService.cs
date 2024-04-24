
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Services
{
    public class SmsAutomationService : IHostedService, IDisposable
    {
        //Timer for the checking if a SMS notification needs to be sent
        private Timer? _tTimer;
        //Store the Service Scope Factory
        private readonly IServiceScopeFactory _oScopeFactory;

        public SmsAutomationService(IServiceScopeFactory oScopeFactory)
        {
            _oScopeFactory = oScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Set up timer to check every hour
            //_tTimer = new Timer(InvokeSmsSystem, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            _tTimer = new Timer(InvokeSmsSystem, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
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
    }
}
