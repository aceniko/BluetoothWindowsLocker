using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BluetoothWindowsLocker.Bluetooth
{
    public class BluetoothScanner
    {

        #region private

        /// <summary>
        /// request scanner shutdown
        /// </summary>
        private bool ShutDownRequested;

        /// <summary>
        /// handle used to wake up the thread in case of shutdown request
        /// </summary>
        private readonly object ShutDownWaitHandle;

        /// <summary>
        /// Worker thread. Glaven thread koj sto ke se koristi za prebaruvanje uredi
        /// </summary>
        private Thread WorkerThread;

        /// <summary>
        /// address of the device used by this scanner
        /// </summary>
        private readonly BluetoothAddress BTDeviceAddress;


        /// <summary>
        /// client used to work with the device
        /// </summary>
        private readonly BluetoothClient BTClient;


        /// <summary>
        /// is the device currently in range?
        /// </summary>
        public bool DeviceInRange { get; private set; }

        #endregion

        /// <summary>
        /// default constructor of the scanner
        /// </summary>
        ///
        /// <param name="deviceAddress">
        /// address of the required device
        /// </param>
        public BluetoothScanner(string deviceAddress)
        {
            WorkerThread = null;

            var isAddrParsed = BluetoothAddress.TryParse(
                deviceAddress, out BTDeviceAddress);
            //dokolku neuspesno e parsirana bluetooth adresata
            if (!isAddrParsed)
            {
                BTDeviceAddress = null;
            }

            BTClient = new BluetoothClient();
            ShutDownRequested = false;
            ShutDownWaitHandle = new object();

        }
        /// <summary>
        /// worker wrapper used to catch exceptions
        /// </summary>
        private void Run()
        {
            try
            {
                ScannerWorker();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    string.Format("{0}: {1}", e.GetType(), e.Message),
                    @"Critical scanner error");
                throw;
            }
        }
        /// <summary>
        /// start the scanner
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                /* if the thread is already running, do nothing */
                if (WorkerThread != null)
                {
                    return;
                }

                WorkerThread = new Thread(Run);
                ShutDownRequested = false;
                WorkerThread.Start();
            }
        }

        /// <summary>
        /// stop the scanner
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                /* if the thread is not running already, do nothing */
                ShutDownRequested = true;
                WorkerThread = null;

                /* wake up waiting thread */
                lock (ShutDownWaitHandle)
                {
                    Monitor.Pulse(ShutDownWaitHandle);
                }
            }
        }

        /// <summary>
        /// actual worker of the scanner
        /// </summary>
        private void ScannerWorker()
        {
            /* device information used */
            BluetoothDeviceInfo di = null;

            while (!ShutDownRequested)
            {
                di = RefreshDeviceInfo(di);
                DeviceInRange = di != null && IsInRange(di);
                lock (ShutDownWaitHandle)
                {
                    Monitor.Wait(ShutDownWaitHandle,
                        TimeSpan.FromSeconds(10));
                }
            }
        }

        /// <summary>
        /// refresh device information or find the device requred
        /// </summary>
        ///
        /// <param name="myDevInfo">
        /// initial device information to be used
        /// </param>
        ///
        /// <returns>
        /// updated device information
        /// </returns>
        private BluetoothDeviceInfo RefreshDeviceInfo(BluetoothDeviceInfo myDevInfo)
        {
            /* devices discovered by single scan */
            BluetoothDeviceInfo[] infos;

            /* if the device info is already set, just refresh it */
            if (myDevInfo != null)
            {
                myDevInfo.Refresh();
                return myDevInfo;
            }

            infos = BTClient.DiscoverDevices();

            
            
            return infos.FirstOrDefault(inf => inf.DeviceAddress.Equals(BTDeviceAddress));
        }

        /// <summary>
        /// test if the device specified by "deviceAddress" is connected
        /// </summary>
        ///
        /// <param name="info">
        /// device being monitored
        /// </param>
        ///
        /// <returns>
        /// true if the device is connected */
        /// </returns>
        private bool IsInRange(BluetoothDeviceInfo info)
        {
            /* ensure that the settings are updated */
            //m_settings.Reload();

            /* no device -> no connection */
            if (info == null)
            {
                return false;
            }

            info.Refresh();

            /* if this device is already connected -> we are good */
            if (info.Connected)
            {
                return true;
            }

            for (var retries = 0; ; retries++)
            {
                try
                {
                    var r = info.GetServiceRecords(BluetoothService.Headset);

                    break;
                }
                catch (SocketException)
                {
                    if (retries >= 3)
                    {
                        return false;
                    }
                    retries++;
                    continue;
                }
            }

            return true;
        }

    }
}
