using BluetoothWindowsLocker.Bluetooth;
using BluetoothWindowsLocker.Command;
using BluetoothWindowsLocker.Helpers;
using BluetoothWindowsLocker.Properties;
using BluetoothWindowsLocker.Workflow;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BluetoothWindowsLocker
{
    public partial class BluetoothLock : Form
    {
        [DllImport("user32.dll")]
        static extern void LockWorkStation();

        private Settings m_settings;

        private BluetoothAddress m_deviceAddr;
        /// <summary>
        /// listening thread
        /// </summary>
        private Thread m_trd;

        /// <summary>
        /// request to the listening thread to shut down
        /// </summary>
        private bool m_shutdownRequest;

        /// <summary>
        /// event used to abort lock during countdown
        /// </summary>
        private readonly AutoResetEvent m_evAbortLock = new AutoResetEvent(false);

        /// <summary>
        /// request to shutdown the worker thread
        /// </summary>
        private readonly AutoResetEvent m_shutdownRequestEvent = new AutoResetEvent(false);


        /// <summary>
        /// is this application hidden on startup?
        /// </summary>
        private bool m_isHidden;

        /// <summary>
        /// get state of application window
        /// </summary>
        public bool IsHidden
        {
            get { return m_isHidden; }
        }

        /// <summary>
        /// logger used by this locker
        /// </summary>
        private Logger m_logger;

        /// <summary>
        /// player of ticks sound
        /// </summary>
        private SoundPlayer m_player;


        /// <summary>
        /// resource manager used to pull data
        /// </summary>
        private static readonly ResourceManager MRmg = new ResourceManager(
            "BluetoothWindowsLocker.LanguageResources.Resources",
           Assembly.GetExecutingAssembly());

        public BluetoothLock()
        {
            m_settings = new Settings();

            /* set the culture used by this program */
            Thread.CurrentThread.CurrentUICulture =
                new CultureInfo(new Settings().locale);

            InitializeComponent();
        }

        public void Initialize(IEnumerable<string> args)
        {
            InitializeControls();
            IconHelper.LoadIcons();

            timeoutBox.Value = m_settings.Timeout;
            //lockCommandBox.Text = m_settings.LockCommand;
            //lockCommandArguments.Text = m_settings.LockArguments;
            //unlockCommandBox.Text = m_settings.UnlockCommand;
            //unlockCommandArguments.Text = m_settings.UnlockArguments;
            //m_checkBoxSkipLock.Checked = m_settings.SkipLock;

            /* find last used device */
            if (!String.IsNullOrEmpty(m_settings.DeviceAddress))
            {
                /* is the address parsed successfully? */
                var isAddrParsed = BluetoothAddress.TryParse(
                    m_settings.DeviceAddress, out m_deviceAddr);
                if (isAddrParsed)
                {
                    m_deviceTextbox.Text = m_settings.DeviceName;
                }
            }

            m_isHidden = false;

            /* see if we are started in quiet mode */
            var argParser = new Arguments(args);
            if (!String.IsNullOrEmpty(argParser["q"]) ||
                !String.IsNullOrEmpty(argParser["quiet"]))
            {
                WindowState = FormWindowState.Minimized;
                Hide();
                StartClickHandler(null, null);
                m_isHidden = true;
            }

            /* create new logger */

            /* should window log be used */
            var uwl = m_settings.useWindowsLog;
            m_logger = new Logger(argParser["log"], uwl);

            /* load ticks sound */
            m_player = new SoundPlayer(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "btprox.tick.wav"));

            /* log program startup */
            m_logger.Event("started", "BtProx");
        }
        /// <summary>
        /// initializes all the controls with their translations
        /// </summary>
        private void InitializeControls()
        {
            m_buttonStart.Text = GetMessage("startButton");
            m_buttonStop.Text = GetMessage("stopButton");
            m_buttonHide.Text = GetMessage("hideButton");
            m_buttonExit.Text = GetMessage("exitButton");
            m_settingsToolStripMenuItem.Text = GetMessage("menuOptions");
            m_labelUsedDevices.Text = GetMessage("labelUsedDevice");
            m_deviceTextbox.Text = GetMessage("labelUnknown");
            m_labelTimeout.Text = GetMessage("labelTimeout");
            //m_labelLockCommand.Text = GetMessage("labelLockCommand");
            //m_labelLockArguments.Text = GetMessage("labelArguments");
            //m_labelReleaseCommand.Text = GetMessage("labelReleaseCommand");
            //m_labelReleaseArguments.Text = GetMessage("labelArguments");
            //m_checkBoxSkipLock.Text = GetMessage("dontLock");
            m_labelMinutes.Text = GetMessage("labelMinutes");

            Text = @"Bluetooth Lock - " + Text;
            m_restoreToolStripMenuItem.Text = GetMessage("menuItemRestore");
            m_abortPendingLockToolStripMenuItem.Text =
                GetMessage("menuItemAbortLock");
            m_aboutToolStripMenuItem.Text = GetMessage("menuItemAbout");
            m_exitToolStripMenuItem.Text = GetMessage("menuItemExit");
            toolStripStatusLabel.Text = GetMessage("messageStopped");
        }

        private void ServiceThread()
        {
            /* state machine used by this thread */
            StateMachine sm;

            /* has the timeout expired? */
            bool isTimeout;

            /* timer used for this */
            System.Threading.Timer tmr = null;

            /* action that has to be taken */
            StateMachine.Action act;

            /* countdown timer counter */
            int countDown;

            /* scanner used to detect the device */
            BluetoothScanner scanner;

            /* array if wait handles to use during the loop */
            var wo = new WaitHandle[] {
                m_evAbortLock,
                m_shutdownRequestEvent
            };

            sm = new StateMachine();
            scanner = new BluetoothScanner(m_settings.DeviceAddress);
            scanner.Start();
            isTimeout = false;

            while (!m_shutdownRequest)
            {
                /* did the user request to abort locking? */
                var abortRequested = false;
                var idx = WaitHandle.WaitAny(wo, TimeSpan.FromSeconds(1));
                abortRequested = (idx == 0);
                if (abortRequested || m_shutdownRequest)
                {
                    if (tmr != null)
                    {
                        tmr.Dispose();
                        tmr = null;
                    }
                    isTimeout = false;
                }

                act = sm.Iterate(scanner.DeviceInRange, isTimeout);
                switch (act)
                {
                    case StateMachine.Action.None:
                        break;
                    case StateMachine.Action.Lock:
                        if (tmr != null)
                        {
                            tmr.Dispose();
                            tmr = null;
                        }
                        LockActions();
                        break;
                    case StateMachine.Action.Unlock:
                        if (tmr != null)
                        {
                            tmr.Dispose();
                            tmr = null;
                        }
                        
                        ShowDeviceAppearance();
                        break;
                    case StateMachine.Action.StartTimer:
                        tmr = new System.Threading.Timer(
                            o => isTimeout = true,
                            null,
                            new TimeSpan(0, (int)m_settings.Timeout, 0),
                            new TimeSpan(0, 0, 0, 0, -1));
                        ShowDeviceDisappearance((int)m_settings.Timeout);
                        break;
                    case StateMachine.Action.AbortTimer:
                        if (tmr != null)
                        {
                            tmr.Dispose();
                            tmr = null;
                        }
                        ShowDeviceAppearance();
                        break;
                    case StateMachine.Action.ShowDeviceAppearance:
                        if (tmr != null)
                        {
                            tmr.Dispose();
                            tmr = null;
                        }
                        ShowDeviceAppearance();
                        break;
                    case StateMachine.Action.Countdown:
                        countDown = (int)m_settings.Countdown;
                        isTimeout = false;
                        m_evAbortLock.Reset();
                        tmr = new System.Threading.Timer(
                            o => { if (countDown-- <= 0) isTimeout = true; else ShowLockingBaloon(countDown); },
                            null,
                            new TimeSpan(0, 0, 1),
                            new TimeSpan(0, 0, 1));
                        break;
                    default:
                        break;
                }
            }

            /* stop the device scanner */
            scanner.Stop();
        }


        /// <summary>
        /// handler of form being closed
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void FormClosedHandler(object sender,
            FormClosedEventArgs e)
        {
            ExitClickHandler(sender, e);
        }


        /// <summary>
        /// resize event handler: in case of minimization hides the window
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void BtProxSizeChanged(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }
        /// <summary>
        /// handler of "Restore" item in context menu
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void ContextMenuRestore(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// handler of tray icon double click: restores the window
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void TrayIconDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// update notify icon in GUI thread
        /// </summary>
        ///
        /// <param name="text">
        /// text to set for notification icon
        /// </param>
        ///
        /// <param name="icon">
        /// updated icon image
        /// </param>
        private void UpdateNotifyIcon(String text, Icon icon)
        {
            /* don't process GUI updates when the handle was not created yet */
            if (!IsHandleCreated || IsDisposed)
            {
                return;
            }

            /* ensure that the update is performed on GUI thread */
            if (InvokeRequired)
            {
                BeginInvoke((Action)
                    (() => UpdateNotifyIcon(text, icon)));
                return;
            }

            /* something strange is going on here, sometimes there is       */
            /* NullReferenceException on notification icon update, just     */
            /* a workaround for this bug                                    */
            try
            {
                notifyIcon.Icon = icon;
                notifyIcon.Text = text;
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch { }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// update icon of this form in GUI thread
        /// </summary>
        ///
        /// <param name="icon">
        /// icon to set for the form
        /// </param>
        private void UpdateMyIcon(Icon icon)
        {
            /* don't process GUI updates when the handle was not created yet */
            if (!IsHandleCreated || IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => UpdateMyIcon(icon)));
                return;
            }
            Icon = icon;
        }

        /// <summary>
        /// update status bar in GUI thread
        /// </summary>
        ///
        /// <param name="status">
        /// text to set for the status bar
        /// </param>
        private void UpdateStatusBar(String status)
        {
            /* don't process GUI updates when the handle was not created yet */
            if (!IsHandleCreated || IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => UpdateStatusBar(status)));
                return;
            }
            toolStripStatusLabel.Text = status;
        }

        /// <summary>
        /// wrapper for service thread allowing to catch exceptions thrown by
        /// the main functionality block
        /// </summary>
        private void ServiceThreadWrapper()
        {
            try
            {
                ServiceThread();
            }
            catch (Exception e)
            {
                ExceptionHandler(e);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// handles exception thrown by the service thread
        /// </summary>
        ///
        /// <param name="e">
        /// excepton that should be shown
        /// </param>
        private void ExceptionHandler(Exception e)
        {
            try
            {
                var strMsg = String.Format(
                    "Exception {0}: {1}{2}At{3}",
                    e.GetType(),
                    e.Message,
                    Environment.NewLine,
                    e.StackTrace);

                m_logger.Event(strMsg, String.Empty);

                Invoke(new MethodInvoker(() =>
                {
                    //var ef = new ErrorMessageForm(strMsg);
                    //ef.ShowDialog();
                }));
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// update the GUI on device disappearance
        /// </summary>
        ///
        /// <param name="minutesToLock">
        /// number of seconds until the device is locked
        /// </param>
        private void ShowDeviceDisappearance(int minutesToLock)
        {
            UpdateNotifyIcon(GetMessage("messageDeviceDisappeared"),
                IconHelper.m_icoPending);
            UpdateMyIcon(IconHelper.m_icoPendingLarge);
            UpdateStatusBar(GetMessage("messageDeviceDisappeared"));

            m_logger.Event("disappeared", m_settings.DeviceName);

            /* if no baloons have to be shown, nothing to do here anymore */
            if (!m_settings.ShowBaloons)
            {
                return;
            }

            /* if the station has to locked now, show no baloons here */
            if (minutesToLock <= 0)
            {
                return;
            }

            var strMsg = String.Format(GetMessage("messageLockingInMin"),
                minutesToLock);
            notifyIcon.ShowBalloonTip(10000,
                GetMessage("messageDeviceDisappeared"),
                strMsg, ToolTipIcon.Info);
        }

        /// <summary>
        /// update the GUI on device appearance
        /// </summary>
        private void ShowDeviceAppearance()
        {
            UpdateNotifyIcon(GetMessage("messageDeviceInRange"), IconHelper.m_icoIdle);
            UpdateMyIcon(IconHelper.m_icoIdleLarge);
            UpdateStatusBar(GetMessage("messageDeviceAppeared"));

            m_logger.Event("appeared", m_settings.DeviceName);

            /* if no baloons have to be shown, nothing else to do... */
            if (!m_settings.ShowBaloons)
            {
                return;
            }
            var strMsg = String.Format(GetMessage("messageDeviceInRangeName"),
                m_settings.DeviceName);
            notifyIcon.ShowBalloonTip(10000, GetMessage("messageDeviceFound"),
                strMsg, ToolTipIcon.Info);
        }

        /// <summary>
        /// execute relevant lock operations
        /// </summary>
        private void LockActions()
        {

            /* lock the workstation only if the "skip lock" checkbox is not
             * ticked */
            if (!m_settings.SkipLock)
            {
                LockWorkStation();
            }

            m_logger.Event("Workstation locked", m_settings.DeviceName);
        }
        

        /// <summary>
        /// show locking baloon and tick if needed
        /// </summary>
        ///
        /// <param name="sec">
        /// number of seconds to lock
        /// </param>
        private void ShowLockingBaloon(int sec)
        {
            if (m_settings.ShowBaloons)
            {
                notifyIcon.ShowBalloonTip(2000, GetMessage("messageLocking"),
                    String.Format(GetMessage("messageLockingInSec"), sec),
                    ToolTipIcon.Info);
            }
            if (m_settings.soundEnabled)
            {
                m_player.Play();
            }
        }

        /// <summary>
        /// handler of "Start" button: starts listening control thread
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void StartClickHandler(object sender, EventArgs e)
        {
            /* if no device selected, do nothing */
            if (m_deviceAddr == null)
            {
                MessageBox.Show(GetMessage("messageSelectDevice"));
                return;
            }

            /* update the GUI and start listening thread */
            UpdateStatusBar(GetMessage("messageStarted"));

            m_shutdownRequest = false;
            m_buttonStart.Enabled = false;
            buttonSelect.Enabled = false;
            timeoutBox.Enabled = false;
            //lockCommandBox.Enabled = false;
            //lockCommandArguments.Enabled = false;
            //buttonSelectLockCommand.Enabled = false;
            m_buttonStop.Enabled = true;
            //buttonSelectUnlockCommand.Enabled = false;
            //unlockCommandBox.Enabled = false;
            //unlockCommandArguments.Enabled = false;
            //m_checkBoxSkipLock.Enabled = false;

            UpdateNotifyIcon(GetMessage("messageWaitingForDevice"),
                IconHelper.m_icoUnloaded);
            UpdateMyIcon(IconHelper.m_icoIdleLarge);

            m_trd = new Thread(ServiceThreadWrapper);
            //m_trd.SetApartmentState(ApartmentState.STA);
            m_trd.Start();
        }

        /// <summary>
        /// handler of "Stop" button click: stop running thread
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event argument
        /// </param>
        private void StopClickHandler(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = GetMessage("messageStopping");
            m_buttonStop.Enabled = false;

            /* try to stop the running thread. in order to leave the GUI    */
            /* responsive during thread shutdown, actual stopping is done   */
            /* asynchronously                                               */
            BeginInvoke(new MethodInvoker(delegate
            {
            /* request thread shutdown and wait until it exits */
                m_shutdownRequest = true;
                m_shutdownRequestEvent.Set();
                if (m_trd != null)
                {
                    m_trd.Interrupt();
                    m_trd.Join();
                }

            /* update GUI in its own thread */
                UpdateStatusBar(GetMessage("messageStopped"));
                UpdateNotifyIcon(GetMessage("messageStopped"),
                    IconHelper.m_icoUnloaded);
                UpdateMyIcon(IconHelper.m_icoUnloadedLarge);
                Invoke(new MethodInvoker(delegate
                {
                    m_buttonStart.Enabled = true;
                    buttonSelect.Enabled = true;
                    timeoutBox.Enabled = true;
                    //lockCommandBox.Enabled = true;
                    //lockCommandArguments.Enabled = true;
                    //buttonSelectLockCommand.Enabled = true;
                    //buttonSelectUnlockCommand.Enabled = true;
                    //unlockCommandBox.Enabled = true;
                    //unlockCommandArguments.Enabled = true;
                    //m_checkBoxSkipLock.Enabled = true;
                }));
            }));
        }

        /// <summary>
        /// handler of device click handler
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void SelectDeviceClickHandler(object sender, EventArgs e)
        {
            SelectBluetoothDeviceDialog selDia;

            selDia = new SelectBluetoothDeviceDialog();
            //selDia.AddNewDeviceWizard = true;
            selDia.ForceAuthentication = true;
            if (selDia.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            m_deviceAddr = selDia.SelectedDevice.DeviceAddress;
            m_deviceTextbox.Text = selDia.SelectedDevice.DeviceName;
            m_settings.DeviceName = selDia.SelectedDevice.DeviceName;
            m_settings.DeviceAddress =
                selDia.SelectedDevice.DeviceAddress.ToString();
            m_settings.Save();
        }

        /// <summary>
        /// handler of "Hide" button: minimized the window
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void HideClickHandler(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// handler of "Exit" button: closes the window
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void ExitClickHandler(object sender, EventArgs e)
        {
            m_shutdownRequest = true;
            m_shutdownRequestEvent.Set();
            m_logger.Event("stopped", "BtProx");
            notifyIcon.Visible = false;
            Application.ExitThread();
        }

        /// <summary>
        /// about context menu strip
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void AboutMenuItemHandler(object sender, EventArgs e)
        {
            var ab = new AboutBox(this);
            ab.ShowDialog();
        }

        /// <summary>
        /// handler of tray notification icon event
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of this event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void MouseClickHandler(object sender, MouseEventArgs e)
        {
            /* handle only right mouse key */
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
        }

        /// <summary>
        /// handler of timeout numeric box change
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void TimeoutValueChanged(object sender, EventArgs e)
        {
            m_settings.Timeout = timeoutBox.Value;
            m_settings.Save();
        }

        /// <summary>
        /// handler of lock command change: saving the value in settings
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void LockCommandChangedHandler(object sender, EventArgs e)
        {
            //m_settings.LockCommand = lockCommandBox.Text;
            m_settings.Save();
        }

        /// <summary>
        /// handler of lock command change: saving the value in settings
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void LockArgumentsChangedHandler(object sender, EventArgs e)
        {
            //m_settings.LockArguments = lockCommandArguments.Text;
            m_settings.Save();
        }

        /// <summary>
        /// handler of text change of the unlock command handler
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event: unlock command text box
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void UnlockCommandChangedHandler(object sender, EventArgs e)
        {
            //m_settings.UnlockCommand = unlockCommandBox.Text;
            m_settings.Save();
        }

        /// <summary>
        /// handler of lock command change: saving the value in settings
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void UnlockArgumentsChangedHandler(object sender, EventArgs e)
        {
            //m_settings.UnlockArguments = unlockCommandArguments.Text;
            m_settings.Save();
        }

        /// <summary>
        /// handler of "Abort pending lock" tray menu item
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void AbortPendingLockHandler(object sender, EventArgs e)
        {
            m_evAbortLock.Set();
            UpdateStatusBar(GetMessage("messageAbortedLock"));
            UpdateNotifyIcon(GetMessage("messageAbortedLock"), IconHelper.m_icoPending);
        }

        /// <summary>
        /// handler of "Select Lock Command" button
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void SelectLockCommand(object sender, EventArgs e)
        {
            //SelectCommand(lockCommandBox);
        }

        /// <summary>
        /// handler of "Select Unlock Command" button
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void SelectUnlockCommand(object sender, EventArgs e)
        {
            //SelectCommand(unlockCommandBox);
        }

        /// <summary>
        /// select a file using open dialog and add its full path to specified
        /// textbox
        /// </summary>
        ///
        /// <param name="tb">
        /// textbox that will receive full path of the file
        /// </param>
        private static void SelectCommand(Control tb)
        {
            /* dialog used to select the file */
            OpenFileDialog ofd;

            /* result of user dialog with file selection */
            DialogResult dr;

            ofd = new OpenFileDialog();
            dr = ofd.ShowDialog();
            if (dr != DialogResult.OK)
            {
                return;
            }

            tb.Text = ofd.FileName;
        }

        /// <summary>
        /// handler of "Skip lock" button change of state
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event: the check button itself
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void LockSkipStateChanged(object sender, EventArgs e)
        {
            //m_settings.SkipLock = m_checkBoxSkipLock.Checked;
            m_settings.Save();
        }

        /// <summary>
        /// handler of "Options" menu item handler
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void OptionsMenuItem(object sender, EventArgs e)
        {
            OptionsController oc;

            oc = new OptionsController(m_settings, this);
            oc.SettingsChanged += SettingsChangedHandler;
            oc.ShowForm();
        }

        /// <summary>
        /// handler of options dialog "Ok" button
        /// </summary>
        ///
        /// <param name="set">
        /// new settings
        /// </param>
        private void SettingsChangedHandler(Settings set)
        {
            m_settings = set;
            m_settings.Save();

            /* initialize windows log if needed */
            m_logger.UseWindowsLog = m_settings.useWindowsLog;
        }

        /// <summary>
        /// initializes all the controls with their translations
        /// </summary>
        private void InitializeLanguageControls()
        {
            m_buttonStart.Text = GetMessage("startButton");
            m_buttonStop.Text = GetMessage("stopButton");
            m_buttonHide.Text = GetMessage("hideButton");
            m_buttonExit.Text = GetMessage("exitButton");
            m_settingsToolStripMenuItem.Text = GetMessage("menuOptions");
            m_labelUsedDevices.Text = GetMessage("labelUsedDevice");
            m_deviceTextbox.Text = GetMessage("labelUnknown");
            m_labelTimeout.Text = GetMessage("labelTimeout");
            //m_labelLockCommand.Text = GetMessage("labelLockCommand");
            //m_labelLockArguments.Text = GetMessage("labelArguments");
            //m_labelReleaseCommand.Text = GetMessage("labelReleaseCommand");
            //m_labelReleaseArguments.Text = GetMessage("labelArguments");
            //m_checkBoxSkipLock.Text = GetMessage("dontLock");
            m_labelMinutes.Text = GetMessage("labelMinutes");

            Text = @"BtProx - " + Text;
            m_restoreToolStripMenuItem.Text = GetMessage("menuItemRestore");
            m_abortPendingLockToolStripMenuItem.Text =
                GetMessage("menuItemAbortLock");
            m_aboutToolStripMenuItem.Text = GetMessage("menuItemAbout");
            m_exitToolStripMenuItem.Text = GetMessage("menuItemExit");
            toolStripStatusLabel.Text = GetMessage("messageStopped");
        }

        /// <summary>
        /// retrieves the message to be shown (dependent on current culture)
        /// </summary>
        ///
        /// <param name="strMsgName">
        /// name of the message to retrieve
        /// </param>
        ///
        /// <returns>
        /// translated message
        /// </returns>
        public string GetMessage(string strMsgName)
        {
            /* string from resource manager */
            return MRmg.GetString(strMsgName) ?? strMsgName;
        }

        /// <summary>
        /// handler of form loading, restores previous window position from
        ///  settings
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void BtProxLockLoad(object sender, EventArgs e)
        {
            Location = m_settings.PreviousPosition;
        }

        /// <summary>
        /// handler of window position change: saves new position into settings
        /// </summary>
        ///
        /// <param name="sender">
        /// </param>
        ///
        /// <param name="e"></param>
        private void BtProxLockLocationChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                m_settings.PreviousPosition = Location;
            }
        }

        /// <summary>
        /// handler of Close button on window title, instead, minimizes the
        /// window
        /// </summary>
        ///
        /// <param name="sender">
        /// sender of the event
        /// </param>
        ///
        /// <param name="e">
        /// event arguments
        /// </param>
        private void FormClosingHandler(object sender,
            FormClosingEventArgs e)
        {
            e.Cancel = true;
            WindowState = FormWindowState.Minimized;
        }
    }

}
