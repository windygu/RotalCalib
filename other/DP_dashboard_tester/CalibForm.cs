﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using multiplexing_dll;
using DeltaPlcCommunication;
using DpCommunication;
using System.Diagnostics;
using TempController_dll;
using System.Threading;

namespace DP_dashboard
{

    public partial class CalibForm : Form
    {
        public static CalibForm currentForm;

        //cosntants
        private const int MAX_PRESSURE_POINT = 0x0f;

        // mult plexing protocol instance
        private MultiplexingIncomingInformation MultiplexingInfo;
        public classMultiplexing classMultiplexing;
        // end

        // plc protocol instance
        public classDeltaProtocol ClassDeltaProtocol;
        private DeltaIncomingInformation PLCinfo;
        private DeltaReturnedData IncumingParametersFromPLC;
        // end   

        // DP protocol instance  
        public ClassDpCommunication classDpCommunication;
        private DpIncomingInformation DPinfo;
        // end  

        private classLog log = new classLog();
        public ClassCalibrationInfo classCalibrationInfo;
        private string CurrentSnDeviceIsFocus = "";

        // temp controller protocol instance
        public TempControllerProtocol tempControllerInstanse;

        ConfigForm ConfigFormInstanse;
        DateTime UpdateTempTime = new DateTime();




        public CalibForm()
        {
            InitializeComponent();
            currentForm = this;


            //// plc protocol init          
            //PLCinfo = new DeltaIncomingInformation();
            //ClassDeltaProtocol = new classDeltaProtocol(Properties.Settings.Default.plcComPort, 9600, PLCinfo);

            // multplexing protocol init 
            MultiplexingInfo = new MultiplexingIncomingInformation();
            classMultiplexing = new classMultiplexing(Properties.Settings.Default.multiplexingComPort, 115200, MultiplexingInfo);


            // DP protocol init

            DPinfo = new DpIncomingInformation();
            classDpCommunication = new ClassDpCommunication(Properties.Settings.Default.dpComPort, 115200, DPinfo);


            //// Temp controller protocol init
            //tempControllerInstanse = new TempControllerProtocol(Properties.Settings.Default.TempControllerComPort, 9600);


            // Calibration class init           
            classCalibrationInfo = new ClassCalibrationInfo(tempControllerInstanse, classDpCommunication, classMultiplexing, ClassDeltaProtocol);



            UpdateTempTime = DateTime.Now;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            LoadDefoultCalibPointToList();

            tb_logsPath.Text = Properties.Settings.Default.LogPath;

            //ClassMail.SendCalibrationDone();
        }

        private void bt_writePressureTableToPlc_Click(object sender, EventArgs e)
        {

            //List<Int16> PressureTable = new List<Int16>();
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest1 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest2 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest3 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest4 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest5 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest6 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest7 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest8 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest9 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest10 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest11 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest12 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest13 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest14 * 10));
            //PressureTable.Add((Int16)(Properties.Settings.Default.PressureUnderTest15 * 10));

            //rtb_info.Text += "PLC ->  " +  classCalibrationInfo.classDeltaProtocolInstanse.WritePressureTableToPLC(PressureTable) +  "\r\n";


            // SHLOM integration
            //List<Int16> SetPointPressure = new List<Int16>();

            //SetPointPressure.Add(Int16.Parse(tb_setpoint.Text));
            //classCalibrationInfo.classDeltaProtocolInstanse.classDeltaWriteSetpoint(SetPointPressure);
        }

        private void bt_readPressureTableFromPlc_Click(object sender, EventArgs e)
        {
            //IncumingParametersFromPLC = DeltaProtocolInstanse.SendNewMessage(DeltaMsgType.ReadHoldingRegisters, DeltaMemType.D, 300, MAX_PRESSURE_POINT);

            //if (DeltaProtocolInstanse.newPressureTableReceive == true)
            //{
            //    DeltaProtocolInstanse.newPressureTableReceive = false;


            //    for (int i = 1; i <= MAX_PRESSURE_POINT; i++)
            //    {
            //        //dgv_prressureTable.Rows[i - 1].Cells[1].Value = IncumingParametersFromPLC.IntValue[i - 1].ToString();
            //    }

            //}

            //classCalibrationInfo.classDeltaProtocolInstanse.SendNewMessage
            //Shalom integration
            IncumingParametersFromPLC = classCalibrationInfo.classDeltaProtocolInstanse.SendNewMessage(DeltaMsgType.ReadHoldingRegisters, DeltaMemType.D, 300, 1);
            rtb_info.Text += "flag register " + IncumingParametersFromPLC.IntValue[0].ToString() + "\r\n";

            IncumingParametersFromPLC = classCalibrationInfo.classDeltaProtocolInstanse.SendNewMessage(DeltaMsgType.ReadHoldingRegisters, DeltaMemType.D, 301, 1);
            rtb_info.Text += "PV = " + IncumingParametersFromPLC.IntValue[0].ToString() + "\r\n";



        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckComPorts();
#if true
            // && !classCalibrationInfo.ConnectingToDP
            if (!classCalibrationInfo.CriticalStates && !classCalibrationInfo.ConnectingToDP)
            {
                if (CheckTimout(UpdateTempTime, 1))
                {
                    classCalibrationInfo.classDpCommunicationInstanse.DPgetDpInfo();
                    classCalibrationInfo.classDpCommunicationInstanse.NewDpInfoEvent = false;

                    tb_temperatureOnDP.Text = classCalibrationInfo.classDpCommunicationInstanse.dpInfo.CurrentTemp.ToString();

                    UpdateTempTime = DateTime.Now;
                }
            }
#endif

            UpdateGUI();
            //if (classCalibrationInfo.DoCalibration)
            //{
            //    UpdateCalibrationGUI();
            //}

            if (classCalibrationInfo.FinishCalibrationEvent)
            {
                classCalibrationInfo.FinishCalibrationEvent = false;
                UpdateColorStatus();
                UpdateDataTable(CurrentSnDeviceIsFocus);
                MessageBox.Show("Calibration done!");
            }

            if(classCalibrationInfo.TempTimoutErrorEvent)
            {
                classCalibrationInfo.TempTimoutErrorEvent = false;

                DialogResult result = MessageBox.Show("Fail to set temperature!","Error", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    classCalibrationInfo.NextAfterTempTimoutErrorEvent = true;
                }
            }

            if (classCalibrationInfo.PressureTimoutErrorEvent)
            {
                classCalibrationInfo.PressureTimoutErrorEvent = false;

                DialogResult result = MessageBox.Show("Fail to set pressure!", "Error", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    classCalibrationInfo.NextAfterPressureTimoutErrorEvent = true;
                }
            }


            if (classCalibrationInfo.classCalibrationSettings.AlertToTechnican)
            {
                classCalibrationInfo.classCalibrationSettings.AlertToTechnican = false;
                DialogResult result = MessageBox.Show("Pressure stable. the system wait to tachnicatan approve.", "Pressure info", MessageBoxButtons.OK);
                if(result ==  DialogResult.OK)
                {
                    classCalibrationInfo.classCalibrationSettings.TechnicianApproveGoNext = true;   
                }
            }

            if (classCalibrationInfo.EndDetectEvent)
            {
                classCalibrationInfo.EndDetectEvent = false;

                UpdateDeviceTable();

            }


            if (classCalibrationInfo.ErrorEvent)
            {
                classCalibrationInfo.ErrorEvent = false;
                rtb_info.Text += "Error:   " + classCalibrationInfo.ErrorMessage + DateTime.Now.ToString() +  "\r\n";
            }

            if (classCalibrationInfo.IncermentCalibPointStep)
            {
                classCalibrationInfo.IncermentCalibPointStep = false;
                string Message = string.Format("Calibration in process :  {0}  Temp index:{1}. Pressure index:{2}", DateTime.Now.ToString(), classCalibrationInfo.CurrentCalibTempIndex.ToString(), classCalibrationInfo.CurrentCalibPressureIndex.ToString());
                rtb_info.Text += Message + "\r\n";

                UpdateDataTable(CurrentSnDeviceIsFocus);
            }

            //if (classCalibrationInfo.ClassTempControllerInstanse.TempControllerConnectionEvent)
            //{
            //    classCalibrationInfo.ClassTempControllerInstanse.TempControllerConnectionEvent = false;
            //    if (classCalibrationInfo.ClassTempControllerInstanse.TempControllerConnectionStatus)
            //    {
            //        classCalibrationInfo.ClassTempControllerInstanse.TempControllerConnectionStatus = false;
            //    }
            //}

            if (classCalibrationInfo.ChengeStateEvent)
            {

            }

        }

        private void UpdateGUI()
        {
            if (classCalibrationInfo.DoCalibration)
            {
                try
                {
                    tb_timeFromSendTemp.Text = DateTime.Now.Subtract(classCalibrationInfo.TimeFromSetTempPointRequest).Minutes.ToString();

                    UpdateCalibrationGUI();
                }
                catch(Exception ex)
                {
                    rtb_info.Text += "update GUI error\r\n";
                }
            }


            UpdateProgressBar();
            CalibrationButtonHandele();
            MarkConnectedDevice();
        }

        private void UpdateProgressBar()
        {
            if (classCalibrationInfo.DoCalibration)
            {
                pb_calibProgressBar.Visible = true;

                int TotalCalibPoints = classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Count * classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Count;
                int FinishCalibPoints = classCalibrationInfo.CurrentCalibTempIndex * classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Count + classCalibrationInfo.CurrentCalibPressureIndex;

                float Precent = ((float)FinishCalibPoints / (float)TotalCalibPoints) * 100;
                pb_calibProgressBar.Value = Convert.ToInt16(Precent);
            }
            else
            {
                pb_calibProgressBar.Visible = false;
                pb_calibProgressBar.Value = 0;
            }
        }

        private void pnl_plcControl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bt_connectToDp_Click(object sender, EventArgs e)
        {
            //byte DpId = (byte)(int.Parse(cmb_dpDeviceNumber.SelectedItem.ToString()));
            //MultiplexingProtocolInstanse.ConnectDpDevice(DpId);            
        }

        private void bt_disconnect_Click(object sender, EventArgs e)
        {
            classMultiplexing.DisConnectAllDp();
        }

        private bool FlashDpDevice(string fileName)
        {
            string path = @"C:\Program Files (x86)\Texas Instruments\SmartRF Tools\Flash Programmer\bin\SmartRFProgConsole.exe";
            string args = string.Format("S() EPV F={0}", fileName);
            Process burn = new Process();
            burn.StartInfo.FileName = path;
            burn.StartInfo.Arguments = args;
            burn.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            burn.Start();
            burn.WaitForExit();
            return burn.ExitCode == 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool res = FlashDpDevice(@"C:\work\grow_me\projects\BLE-CC254x-1.4.0\Projects\ble\grow_me\CC2541DB\CC2541\Exe\SimpleBLEPeripheral.hex");

            if (!res)
            {
                MessageBox.Show("burn fail");
            }
        }

        private void bt_writePressursToDP_Click(object sender, EventArgs e)
        {
            // DpProtocolInstanse.SendDpSerialNumber(System.Text.Encoding.ASCII.GetBytes(tb_dpSerialNumber.Text));

            classDpCommunication.SendPressuresTableToDP();
        }

        private void bt_exportPressursTableToCSVfile_Click(object sender, EventArgs e)
        {
            //log.OpenFileForLogging(Application.StartupPath + @"\Logs", "1.0.0", "1.0.0","6778899");
            //log.PrintLogRecordToFile(DpProtocolInstanse);
            //log.CloseFileForLogging();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bt_getDPinfo_Click(object sender, EventArgs e)
        {
            classDpCommunication.DPgetDpInfo();
        }

        private void bt_configuration_Click(object sender, EventArgs e)
        {
            ConfigForm configForm = new ConfigForm(classDpCommunication, this);
            this.Hide();
            configForm.Show();
        }

        private void rtbLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void bt_startCalibration_Click(object sender, EventArgs e)
        {
            classCalibrationInfo.DetectFlag = true;
            classCalibrationInfo.InitDetectTread();

            //wait to finish the detect.
            while (!classCalibrationInfo.EndDetectEvent)
            {
                Application.DoEvents();
            }


            if (classCalibrationInfo.DpCountAxist > 0)
            {
                classCalibrationInfo.EndDetectEvent = false;

                UpdateDeviceTable();


                classDpCommunication.SendStartCalibration();

                classCalibrationInfo.ResetStateMachine();
                classCalibrationInfo.DoCalibration = true;
                classCalibrationInfo.CalibrationPaused = false;
                classCalibrationInfo.InitCalibTread();
                classCalibrationInfo.CreateLogFiles();
                ClearColorIndication();
            }
            else
            {
                MessageBox.Show("No exist dp devices!");
            }

        }

        private void UpdateDeviceTable()
        {
            dgv_devicesQueue.Rows.Clear();
            for (int i = 0; i < classCalibrationInfo.DpCountAxist; i++)
            {
                dgv_devicesQueue.Rows.Add(i.ToString(),
                    classCalibrationInfo.classDevices[i].DeviceMacAddress.ToString(),
                    classCalibrationInfo.classDevices[i].DeviceSerialNumber.ToString(),
                    classCalibrationInfo.classDevices[i].PositionOnBoard.ToString(),
                    classCalibrationInfo.classDevices[i].BoardNumber.ToString());
            }
        }
        private void UpdateDataTable(string serialNumber)
        {
            bool ExistDevice = false;
            int i = 0;
            for (i = 0; i < classCalibrationInfo.DpCountAxist; i++)
            {
                if (classCalibrationInfo.classDevices[i].DeviceSerialNumber == serialNumber)
                {
                    ExistDevice = true;
                    break;
                }
            }

            int deviceIndex = i;

            if (ExistDevice)
            {
                dgv_deviceData.Rows.Clear();
                ExistDevice = false;
                string[] dataRow = new string[classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Count * 2 + 1];

                for (i = 0; i < classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Count; i++)
                {
                    dataRow[0] = classCalibrationInfo.classCalibrationSettings.PressureUnderTestList[i].ToString();
                    for (int j = 0; j < classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Count; j++)
                    {
                        dataRow[j * 2 + 1] = classCalibrationInfo.classDevices[deviceIndex].CalibrationData[j, i].LeftA2DValue.ToString();
                        dataRow[j * 2 + 2] = classCalibrationInfo.classDevices[deviceIndex].CalibrationData[j, i].RightA2DValue.ToString();
                    }
                    dgv_deviceData.Rows.Add(dataRow);
                }
            }

        }

        private void dgv_devicesQueue_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_devicesQueue.Rows[e.RowIndex].Cells[2].Value != null)
            {
                CurrentSnDeviceIsFocus = dgv_devicesQueue.Rows[e.RowIndex].Cells[2].Value.ToString();
            }

            UpdateDataTable(CurrentSnDeviceIsFocus);

        }


        void UpdateCalibrationGUI()
        {
            //temp controller
            tb_currentTemperature.Text = classCalibrationInfo.CurrentTempControllerValue.ToString();
            tb_targetTemperature.Text = classCalibrationInfo.classCalibrationSettings.TempUnderTestList[classCalibrationInfo.CurrentCalibTempIndex].ToString();
            tb_currentSkipTime.Text = ((classCalibrationInfo.classCalibrationSettings.TempSkipStartTime[classCalibrationInfo.CurrentCalibTempIndex]) / 60).ToString() + " [min]";
            //tb_temperatureOnDP.Text = classCalibrationInfo.CurrentTempOnDP.ToString();

            //pressure
            tb_pressCurrentPressure.Text = classCalibrationInfo.CurrentPressure.ToString();
            tb_pressTargetPressure.Text = classCalibrationInfo.PlcBar2Adc(classCalibrationInfo.classCalibrationSettings.PressureUnderTestList[classCalibrationInfo.CurrentCalibPressureIndex]).ToString();            

            //disable dp selection
            if(classCalibrationInfo.ConnectingToDP == true)
            {
                pnl_dpSelection.Enabled = false;
            }
            else
            {
                pnl_dpSelection.Enabled = true;
            }

            if (classCalibrationInfo.PressureStableFlag)
            {
                tb_preeStable.Text = "Yes";
                tb_preeStable.BackColor = Color.Green;

            }
            else
            {
                tb_preeStable.Text = "No";
                tb_preeStable.BackColor = Color.White;
            }

        }


        private void dgv_deviceData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void bt_stopCalibration_Click(object sender, EventArgs e)
        {
            classCalibrationInfo.DoCalibration = false;
            classCalibrationInfo.StateMachineReset();
        }

        private void pnl_TempData_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void bt_clear_Click(object sender, EventArgs e)
        {
            rtb_info.Text = "";
        }


        private bool CheckTimout(DateTime startTime, int timeout)
        {
            if (DateTime.Now.Subtract(startTime).TotalSeconds > timeout)
            {
                return true;
            }
            return false;
        }

        private void bt_connectDP_Click(object sender, EventArgs e)
        {
            byte DpId = (byte)(int.Parse(cmb_dpList.SelectedItem.ToString()));
            classCalibrationInfo.classMultiplexingInstanse.ConnectDpDevice(DpId);
        }

        private void bt_disConnectDP_Click(object sender, EventArgs e)
        {
            classCalibrationInfo.classMultiplexingInstanse.DisConnectAllDp();
        }

        private void dgv_devicesQueue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pnl_calibrationPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bt_settings_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (ConfigFormInstanse == null)
            {
                ConfigFormInstanse = new ConfigForm(classDpCommunication, this);
            }
            ConfigFormInstanse.Show();
        }

        private void bt_detectDp_Click(object sender, EventArgs e)
        {
            classCalibrationInfo.DetectFlag = true;
            classCalibrationInfo.InitDetectTread();
        }

        public void LoadDefoultCalibPointToList()
        {

            classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Clear();
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Clear();


            classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Add(Properties.Settings.Default.TempUnderTest1);
            classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Add(Properties.Settings.Default.TempUnderTest2);
            classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Add(Properties.Settings.Default.TempUnderTest3);
            classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Add(Properties.Settings.Default.TempUnderTest4);
            classCalibrationInfo.classCalibrationSettings.TempUnderTestList.Add(Properties.Settings.Default.TempUnderTest5);


            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest1);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest2);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest3);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest4);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest5);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest6);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest7);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest8);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest9);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest10);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest11);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest12);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest13);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest14);
            classCalibrationInfo.classCalibrationSettings.PressureUnderTestList.Add(Properties.Settings.Default.PressureUnderTest15);
        }

        private void DGVSetCellColor(DataGridView dgv, int col, int row, Color color)
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = color;
            dgv_devicesQueue[col, row].Style = style;
        }

        private void ClearColorIndication()
        {
            for (int i = 0; i < dgv_devicesQueue.Rows.Count; i++)
            {
                DGVSetCellColor(dgv_devicesQueue, 1, i, Color.White);
            }
        }


        private void UpdateColorStatus()
        {
            for (int i = 0; i < dgv_devicesQueue.Rows.Count - 1; i++)
            {
                int j = 0;
                while (j < classCalibrationInfo.DpCountAxist)
                {
                    if (dgv_devicesQueue.Rows[i].Cells[2].Value.ToString().Equals(classCalibrationInfo.classDevices[j].DeviceSerialNumber.ToString()))
                    {
                        DGVSetCellColor(dgv_devicesQueue, 1, i, classCalibrationInfo.classDevices[j].deviceStatus == DeviceStatus.Pass ? Color.Green : Color.Red);
                        break;
                    }
                    j++;
                }
            }
        }

        private void CalibForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            classCalibrationInfo.DoCalibration = false;
            classCalibrationInfo.DetectFlag = false;

            //ClassDeltaProtocol.CloseComPort();
            classDpCommunication.CloseComPort();
            classMultiplexing.CloseComPort();
            //tempControllerInstanse.CloseComPort();
        }

        private void CalibrationButtonHandele()
        {

            if (classCalibrationInfo.DoCalibration == true)
            {
                bt_startCalibration.Enabled = false;

                if (!classCalibrationInfo.ConnectingToDP)
                {
                    if (classCalibrationInfo.CalibrationPaused)
                    {
                        bt_pauseStartCalib.Text = "Start";
                    }
                    else
                    {
                        bt_pauseStartCalib.Text = "Pause";
                    }
                    bt_pauseStartCalib.Enabled = true;

                    bt_stopCalibration.Enabled = true;
                }
                else
                {
                    if (classCalibrationInfo.CalibrationPaused)
                    {
                        bt_startCalibration.Enabled = false;
                    }

                    bt_pauseStartCalib.Text = "Pause";
                    bt_pauseStartCalib.Enabled = false;

                    bt_stopCalibration.Enabled = false;
                }

            }
            else
            {
                bt_startCalibration.Enabled = true;

                bt_pauseStartCalib.Text = "Pause";
                bt_pauseStartCalib.Enabled = false;

                bt_stopCalibration.Enabled = false;
            }

        }

        private void bt_pauseStartCalib_Click(object sender, EventArgs e)
        {
            classCalibrationInfo.CalibrationPaused = !classCalibrationInfo.CalibrationPaused;

            if (classCalibrationInfo.CalibrationPaused == false)
            {
                byte tempIndex;
                if (tb_tempIndexAfterPause.Text.ToString() != "")
                {
                    tempIndex = byte.Parse(tb_tempIndexAfterPause.Text.ToString());
                }
                else
                {
                    tempIndex = 0;
                }
                classCalibrationInfo.StateMachineResetAfterPause(tempIndex);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string CutSn = tb_tempIndexAfterPause.Text.Substring(4);

            byte[] SN = System.Text.Encoding.ASCII.GetBytes(CutSn);
            classDpCommunication.SendDpSerialNumber(SN);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //TEST
            classCalibrationInfo.classDpCommunicationInstanse.DPgetDpInfo();
        }

        private void dgv_devicesQueue_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        public void MarkConnectedDevice()
        {
            try
            {
                DataGridViewCellStyle spatialStyle = new DataGridViewCellStyle();
                DataGridViewCellStyle normalStyle = new DataGridViewCellStyle();
                spatialStyle.ForeColor = Color.Blue;
                normalStyle.ForeColor = Color.Black;


                for (int RowCount = 0; RowCount < dgv_devicesQueue.Rows.Count; RowCount++)
                {
                    foreach (DataGridViewCell cell in dgv_devicesQueue.Rows[RowCount].Cells)
                    {
                        //if (RowCount == Convert.ToInt32(classMultiplexing.ConnectedChanel))
                        //{
                        //    cell.Style.ForeColor = spatialStyle.ForeColor;
                        //}
                        //else
                        //{
                        //    cell.Style.ForeColor = normalStyle.ForeColor;
                        //}

                        if(classCalibrationInfo.classDevices[RowCount].PositionOnBoard == Convert.ToInt32(classMultiplexing.ConnectedChanel))
                        {
                            cell.Style.ForeColor = spatialStyle.ForeColor;
                        }
                        else
                        {
                            cell.Style.ForeColor = normalStyle.ForeColor;
                        }
                    }                  
                }
            }
            catch(Exception ex)
            { }
          }

        private void chb_pressureAutoMode_CheckedChanged(object sender, EventArgs e)
        {
            classCalibrationInfo.classCalibrationSettings.PressureAutoMode = chb_pressureAutoMode.Checked;
        }

        private void bt_writeSN_Click(object sender, EventArgs e)
        {
            try
            {
                string CutSn = tb_dpSN.Text.Substring(4);

                byte[] SN = System.Text.Encoding.ASCII.GetBytes(CutSn);
                classDpCommunication.SendDpSerialNumber(SN);

                classDpCommunication.DPgetDpInfo();
            }
            catch(Exception ex)
            {
                if (tb_dpSN.Text.Length < 1)
                {
                    MessageBox.Show("Please enter valid SN.");
                }

            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            float pressure = classCalibrationInfo.ReadPressureFromPlc();
            tb_testReadPressure.Text =   pressure.ToString();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            List<short> l = new List<short>();
            l.Clear();
            l.Add(Convert.ToInt16(tb_newsetPresssure.Text));
            classCalibrationInfo.classDeltaProtocolInstanse.classDeltaWriteSetpoint(l);
        }

        public void CheckComPorts()
        {
            //DP
            if (!classCalibrationInfo.classDpCommunicationInstanse.SerialPortInstanse.ComPortOk  && classCalibrationInfo.classDpCommunicationInstanse.SerialPortInstanse.ComPortErrorMessage != string.Empty)
            {
                string Message = string.Copy(classCalibrationInfo.classDpCommunicationInstanse.SerialPortInstanse.ComPortErrorMessage);
                classCalibrationInfo.classDpCommunicationInstanse.SerialPortInstanse.ComPortErrorMessage = "";

                MessageBox.Show(Message);

            }

            //MultiPlexer
            if (!classCalibrationInfo.classMultiplexingInstanse.SerialPortInstanse.ComPortOk && classCalibrationInfo.classMultiplexingInstanse.SerialPortInstanse.ComPortErrorMessage != string.Empty)
            {
                string Message = string.Copy(classCalibrationInfo.classMultiplexingInstanse.SerialPortInstanse.ComPortErrorMessage);
                classCalibrationInfo.classMultiplexingInstanse.SerialPortInstanse.ComPortErrorMessage = "";

                MessageBox.Show(Message);

            }

            ////PLC-Delta
            //if (!classCalibrationInfo.classDeltaProtocolInstanse.serial.ComPortOk && classCalibrationInfo.classDeltaProtocolInstanse.serial.ComPortErrorMessage != string.Empty)
            //{
            //    string Message = string.Copy(classCalibrationInfo.classDeltaProtocolInstanse.serial.ComPortErrorMessage);
            //    classCalibrationInfo.classDeltaProtocolInstanse.serial.ComPortErrorMessage = "";

            //    MessageBox.Show(Message);

            //}

            ////temp controller
            //if (!classCalibrationInfo.ClassTempControllerInstanse.ComPortOk && classCalibrationInfo.ClassTempControllerInstanse.ComPortErrorMessage != string.Empty)
            //{
            //    string Message = string.Copy(classCalibrationInfo.ClassTempControllerInstanse.ComPortErrorMessage);
            //    classCalibrationInfo.ClassTempControllerInstanse.ComPortErrorMessage = "";

            //    MessageBox.Show(Message);

            //}

        }

        private void tb_logsPath_TextChanged(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(Properties.Settings.Default.LogPath);
        }

        private void tb_logsPath_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.LogPath);
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void bt_startTest_Click(object sender, EventArgs e)
        {
            int DebugTempCount = 5;
            int DebugPressureCount = 15;
            List<float> DebugPressure = new List<float>();
            List<float> DebugTemp = new List<float>();

            for(int i = 0; i < DebugPressureCount; i++)
            {
                DebugPressure.Add(i * 2.1f);
            }

            for (int i = 0; i < DebugTempCount; i++)
            {
                DebugTemp.Add(i * 10.1f);
            }


            classCalibrationInfo.classMultiplexingInstanse.ConnectDpDevice((byte)0);
            Thread.Sleep(1000);


            for(byte tempOffset = 0; tempOffset< DebugTemp.Count; tempOffset++)
            {
                for (byte pressOffset = 0; pressOffset < DebugPressure.Count; pressOffset++)
                {
                    classCalibrationInfo.classDpCommunicationInstanse.DpWritePressurePointToDevice
                        (
                            DebugTemp[tempOffset],
                            tempOffset,
                            DebugPressure[pressOffset],
                            pressOffset
                        );
                    string message = string.Format("Temp {0} = {1}, Press {2} = {3}. \r\n", tempOffset,DebugTemp[tempOffset], pressOffset,DebugPressure[pressOffset]);
                    richTextBox1.Text += message;
                    message = "";
                    Thread.Sleep(2000);
                }
            }


        }
    }
}           
