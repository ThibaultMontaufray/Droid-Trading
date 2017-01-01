using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;

namespace Droid_trading
{
    public partial class TS_Trading : ServiceBase
    {
        #region Enum
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }
        public enum ServiceAction
        {
            DEMARRER_TRADING = 130,
            ARRETER_TRADING = 131,
            ENVOYER_RAPPORT = 132
        }
        #endregion

        #region Struct
        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };
        #endregion

        #region Attribute
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private int eventId;
        #endregion

        #region Constructor
        public TS_Trading(string[] args)
        {
            InitializeComponent();
            string eventSourceName = "TS_Tracing_source";
            string logName = "TS_Trading_logs";
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1) { logName = args[1]; }
            _eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            _eventLog.Source = eventSourceName;
            _eventLog.Log = logName;
        }
        #endregion
        
        #region Methods public
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            _eventLog.WriteEntry("TS Trading uptime : " + eventId.ToString() + " minutes", EventLogEntryType.Information, eventId++);
        }
        #endregion

        #region Methods protected
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventId = 0;
            _eventLog.WriteEntry("In OnStart");
            // Set up a timer to trigger every minute.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }
        protected override void OnStop()
        {
            _eventLog.WriteEntry("In onStop.");
        }
        protected override void OnContinue()
        {
            _eventLog.WriteEntry("In OnContinue.");
        }
        protected override void OnCustomCommand(int command)
        {
            try
            {
                _eventLog.WriteEntry("Command code : " + command, EventLogEntryType.SuccessAudit, eventId++);

                base.OnCustomCommand(command);
                switch (command)
                {
                    case (int)ServiceAction.ARRETER_TRADING:
                        _eventLog.WriteEntry("Stop trading.");
                        break;
                    case (int)ServiceAction.DEMARRER_TRADING:
                        _eventLog.WriteEntry("Start trading.");
                        break;
                    case (int)ServiceAction.ENVOYER_RAPPORT:
                        _eventLog.WriteEntry("Send report.");
                        break;
                    default:
                        _eventLog.WriteEntry("Default action : " + command);
                        break;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
        #endregion
    }
}
