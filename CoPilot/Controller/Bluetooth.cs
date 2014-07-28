using CoPilot.Utils.Enums;
using OdbCommunicator;
using OdbCommunicator.OdbEventArg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.Networking.Proximity;
using Windows.System;

namespace CoPilot.CoPilot.Controller
{

    public class Bluetooth : Base
    {
        #region EVENTS

        public event EventHandler<OdbEventArgs> DataReceive;
        public event EventHandler SelectedDeviceChange;
        public event EventHandler ConnectedToDevice;
        public event EventHandler Disconnected;

        #endregion

        #region PRIVATE

        private PeerInformation device = null;

        private bool isScanMode = false;
        private DateTime scanTime = DateTime.Now.Subtract(TimeSpan.FromHours(1));
        private OdbClient odbClient;

        private Dictionary<string, OdbClient> socketsPool = new Dictionary<string, OdbClient>();

        #endregion

        #region PROPERTY

        /// <summary>
        /// Is More Device
        /// </summary>
        private bool isMoreDevice = false;
        public bool IsMoreDevice
        {
            get
            {
                return isMoreDevice;
            }
            set
            {
                isMoreDevice = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is Finding
        /// </summary>
        private bool isFinding = false;
        public bool IsFinding
        {
            get
            {
                return isFinding;
            }
            set
            {
                isFinding = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is Success
        /// </summary>
        private bool isSuccess = false;
        public bool IsSuccess
        {
            get
            {
                return isSuccess;
            }
            set
            {
                IsError = !value;
                isSuccess = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is Error
        /// </summary>
        private bool isError = false;
        public bool IsError
        {
            get
            {
                return isError;
            }
            set
            {
                if (isError == value)
                {
                    return;
                }
                isError = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Error type
        /// </summary>
        private BluetoothErrorType errorType = BluetoothErrorType.Unknown;
        public BluetoothErrorType ErrorType
        {
            get
            {
                return errorType;
            }
            set
            {
                errorType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// All obd devices
        /// </summary>
        private ObservableCollection<PeerInformation> obdDevices = new ObservableCollection<PeerInformation>();
        public ObservableCollection<PeerInformation> ObdDevices
        {
            get
            {
                return obdDevices;
            }
        }

        /// <summary>
        /// Preselected device
        /// </summary>
        private String preselectedDevice;
        public String PreselectedDevice
        {
            get
            {
                return preselectedDevice;
            }
            set
            {
                preselectedDevice = value;
                RaisePropertyChanged();

                if (SelectedDeviceChange != null)
                {
                    SelectedDeviceChange.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Is obd allowed
        /// </summary>
        private bool isObdAllowed = false;
        public bool IsObdAllowed
        {
            get
            {
                return isObdAllowed;
            }
            set
            {
                isObdAllowed = value;
                RaisePropertyChanged();

                if (value)
                {
                    ErrorType = BluetoothErrorType.Unknown;
                    this.Scan();
                }
                else
                {
                    ErrorType = BluetoothErrorType.NotAllowed;
                    IsFinding = false;
                    this.Disconnect();
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialize BT class
        /// </summary>
        public Bluetooth()
        {
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
        }

        /// <summary>
        /// Scan again for pears
        /// </summary>
        public void Scan()
        {
            if (!this.isScanMode)
            {
                this.tryFind();
            }
        }

        /// <summary>
        /// Select
        /// </summary>
        public void Select(PeerInformation peer)
        {
            this.selectBluetoothDevice(peer);
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public void Disconnect()
        {
            if (this.isSuccess)
            {
                IsSuccess = false;
                this.device = null;
                if (this.odbClient.IsConnected)
                {
                    this.odbClient.Disconnect();
                }

                if (Disconnected != null)
                {
                    Disconnected.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #region PRIVATE

        /// <summary>
        /// Find all peers and connet to device if found
        /// </summary>
        private async void tryFind()
        {
            if (IsSuccess && this.device != null)
            {
                return;
            }

            if (!IsObdAllowed)
            {
                IsError = true;
                IsFinding = false;
                ErrorType = BluetoothErrorType.NotAllowed;
                return;
            }

            IsError = false;
            IsFinding = true;
            ErrorType = BluetoothErrorType.Unknown;

            try
            {
                await findBluedtoothDevices();

                //check if is allowed after search done
                if (IsObdAllowed)
                {
                    //for one device
                    if (obdDevices.Count == 1)
                    {
                        this.selectBluetoothDevice(obdDevices[0]);
                    }
                    //for more devices
                    else if (obdDevices.Count > 1)
                    {
                        ErrorType = BluetoothErrorType.NotSelected;
                        IsMoreDevice = true;
                        IsError = false;
                    }
                    //for zero devices
                    else
                    {
                        ErrorType = BluetoothErrorType.NotFound;
                        IsError = true;
                    }
                }
                else
                {
                    this.clearCachedConnections();
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    ErrorType = BluetoothErrorType.NotEnabled;
                }
                else
                {
                    ErrorType = BluetoothErrorType.FatalError;
                }
                IsError = true;
                this.clearCachedConnections();
            }
            finally
            {
                this.isScanMode = false;
                IsFinding = false;
            }
        }

        /// <summary>
        /// Find bluetooth devices
        /// </summary>
        /// <returns></returns>
        private async Task findBluedtoothDevices()
        {
            this.isScanMode = true;
            obdDevices.Clear();
            var peers = await PeerFinder.FindAllPeersAsync();
            await findObdDevices(peers);
            this.isScanMode = false;
        }

        /// <summary>
        /// Find obd devices
        /// </summary>
        /// <param name="peers"></param>
        /// <returns></returns>
        private async Task findObdDevices(IReadOnlyList<PeerInformation> peers)
        {

            foreach (var peer in peers)
            {
                if (await this.isOdbDevice(peer))
                {
                    if (peer.HostName.RawName != PreselectedDevice)
                    {
                        obdDevices.Add(peer);
                    }
                    else
                    {
                        obdDevices.Clear();
                        obdDevices.Add(peer);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Check if is odb
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private async Task<bool> isOdbDevice(PeerInformation i)
        {
            OdbClient odbClient = new OdbClient();
            await odbClient.Connect(i.HostName);

            var isOdb = await odbClient.IsOdb();
            if (isOdb)
            {
                socketsPool.Add(i.HostName.RawName, odbClient);
            }
            else
            {
                odbClient.Disconnect();
            }

            return isOdb;
        }

        /// <summary>
        /// Select device
        /// </summary>
        /// <param name="param"></param>
        private void selectBluetoothDevice(PeerInformation peer)
        {
            ErrorType = BluetoothErrorType.None;
            IsMoreDevice = false;
            this.device = peer;
            this.createReadSocket();
        }

        /// <summary>
        /// Create read socket or device
        /// </summary>
        private void createReadSocket()
        {
            if (this.device == null)
            {
                IsError = true;
                ErrorType = BluetoothErrorType.NoDevice;
                return;
            }

            //load data
            String rawName = this.device.HostName.RawName;
            OdbClient client = this.socketsPool[rawName];

            //clear and disconnect others
            this.clearCachedConnections(rawName);

            odbClient = client;
            if (odbClient.IsConnected)
            {
                IsSuccess = true;
                IsError = false;
                IsFinding = false;

                //save preselected device
                this.PreselectedDevice = rawName;

                //connected to device
                triggerConnectedToDevice();

                //data receive handler
                odbClient.DataReceive += odbClientDataReceive;

                //start data receiving
                odbClient.Start();
            }
            else
            {
                IsError = true;
                IsFinding = false;
                ErrorType = BluetoothErrorType.OutOfRange;
            }
        }

        /// <summary>
        /// Trigger connected to device
        /// </summary>
        private void triggerConnectedToDevice()
        {
            if (ConnectedToDevice != null)
            {
                ConnectedToDevice.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// On data receive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void odbClientDataReceive(object sender, OdbEventArgs e)
        {
            if (this.DataReceive != null)
            {
                this.DataReceive.Invoke(this, e);
            }
        }

        /// <summary>
        /// Clear and disconnected cached connections
        /// </summary>
        /// <param name="rawName"></param>
        private void clearCachedConnections(String rawName = null)
        {
            if (rawName != null)
            {
                this.socketsPool.Remove(rawName);
            }
            foreach (KeyValuePair<string, OdbClient> record in this.socketsPool)
            {
                record.Value.Disconnect();
            }
            this.socketsPool.Clear();
        }

        #endregion
    }
}
