using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothWindowsLocker.Helpers
{
    public static class IconHelper
    {

        public static Icon m_icoIdle;

        /// <summary>
        /// icon used when the service is running and device is disconnected
        /// </summary>
        public static Icon m_icoPending;

        /// <summary>
        /// icon used when the service is not running
        /// </summary>
        public static Icon m_icoUnloaded;

        /// <summary>
        /// icon used when the service is running and device is connected
        /// </summary>
        public static Icon m_icoIdleLarge;

        /// <summary>
        /// icon used when the service is running and device is disconnected
        /// </summary>
        public static Icon m_icoPendingLarge;

        /// <summary>
        /// icon used when the service is not running
        /// </summary>
        public static Icon m_icoUnloadedLarge;

        /// <summary>
        /// load all the used icons from embedded resources
        /// </summary>
        public static void LoadIcons()
        {
            /* assembly currently executing */
            var asm = Assembly.GetExecutingAssembly();

            /* names of resources placed in current assembly */
            var arrRes = new List<String>(asm.GetManifestResourceNames());

            /* steam used to read icons */
            Stream strm;

            if (arrRes.Contains("BluetoothWindowsLocker.Icons.orange.ico"))
            {
                strm = asm.GetManifestResourceStream("BluetoothWindowsLocker.Icons.orange.ico");
                try
                {
                    if (strm != null)
                    {
                        m_icoIdle = new Icon(strm);
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch { }
                // ReSharper restore EmptyGeneralCatchClause
            }

            if (arrRes.Contains("BluetoothWindowsLocker.Icons.pink.ico"))
            {
                strm = asm.GetManifestResourceStream("BluetoothWindowsLocker.Icons.pink.ico");
                try
                {
                    if (strm != null)
                    {
                        m_icoPending = new Icon(strm);
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch { }
                // ReSharper restore EmptyGeneralCatchClause
            }

            if (arrRes.Contains("BluetoothWindowsLocker.Icons.silver.ico"))
            {
                strm = asm.GetManifestResourceStream("BluetoothWindowsLocker.Icons.silver.ico");
                try
                {
                    if (strm != null)
                    {
                        m_icoUnloaded = new Icon(strm);
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch { }
                // ReSharper restore EmptyGeneralCatchClause
            }

            if (arrRes.Contains("BluetoothWindowsLocker.Icons.orange_large.ico"))
            {
                strm = asm.GetManifestResourceStream(
                    "BluetoothWindowsLocker.Icons.orange_large.ico");
                try
                {
                    if (strm != null)
                    {
                        m_icoIdleLarge = new Icon(strm);
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch { }
                // ReSharper restore EmptyGeneralCatchClause
            }

            if (arrRes.Contains("BluetoothWindowsLocker.Icons.pink_large.ico"))
            {
                strm = asm.GetManifestResourceStream("BluetoothWindowsLocker.Icons.pink_large.ico");
                try
                {
                    if (strm != null)
                    {
                        m_icoPendingLarge = new Icon(strm);
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch { }
                // ReSharper restore EmptyGeneralCatchClause
            }

            if (arrRes.Contains("BluetoothWindowsLocker.Icons.orange.ico"))
            {
                strm = asm.GetManifestResourceStream(
                    "BluetoothWindowsLocker.Icons.silver_large.ico");
                try
                {
                    if (strm != null)
                    {
                        m_icoUnloadedLarge = new Icon(strm);
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch { }
                // ReSharper restore EmptyGeneralCatchClause
            }
        }
    }
}
