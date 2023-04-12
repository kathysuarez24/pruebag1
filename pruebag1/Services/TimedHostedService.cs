using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace pruebag1.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;

        public TimedHostedService()
        {
            
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(20));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Console.WriteLine("Aqui se está ejecutando ");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
