﻿using ServerMonitoringAgent.BashExecutor;
using ServerMonitoringAgent.Executors;
using ServerMonitoringAgent.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoringAgent.Monitor.SystemTime
{
    public class SystemTimeMonitor : IMonitor
    {
        readonly ILogger _logger;
        readonly ILinuxExecutor _executor;

        public SystemTimeMonitor(ILogger logger, ILinuxExecutor executor)
        {
            _logger = logger;
            _executor = executor;
        }

        public async Task MonitorAsync()
        {
            try
            {
                string dateCommand = "date -u +'%d.%m.%Y %H:%M:%S'";

                string dateOutput = (await _executor.ExecuteLinuxCommandAsync(dateCommand)).Trim();

                DateTime dateOutputDT = DateTime.ParseExact(dateOutput, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                dateOutputDT = dateOutputDT.AddHours(3);

                DateTime systemTimeDT = DateTime.Now;

                TimeSpan difference = systemTimeDT - dateOutputDT;
                double secondsDifference = Math.Abs(difference.TotalSeconds);

                if (secondsDifference >= 15) 
                {
                    _logger.Warn("[SYSTIME] 0");
                }
                else
                {
                    _logger.Info("[SYSTIME] 1");
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.Error($"[SYSTIME] Monitoring failed: {ex.Message}");
            }
        }
    }
}
