using RAFFLE.Schema;
using System;
using System.IO;
using System.Windows;
using Wpf.Ui.Controls;
using System.Printing;
using RAFFLE.Utils;
using System.Globalization;

namespace RAFFLE.UI
{

    public partial class MainWindow : UiWindow
    {
        private bool bThreadStatus = false;

        //private string sImpluse;
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            lblEndTime.Text = "End Time: " + SettingSchema.Time == null ? getDateTimeFromString(SettingSchema.Time).ToString() : null;
            lblPrice.Text = "Price: " + SettingSchema.Price.ToString();
            Img.Source = SettingSchema.Img;
            bThreadStatus = false;
            prgThread.IsIndeterminate = false;
            Update();
        }

        public void UpdateState()
        {
            lblEndTime.Text = "End Time: " + getDateTimeFromString(SettingSchema.Time).ToString();
            lblPrice.Text = "Price: " + SettingSchema.Price.ToString();
            Img.Source = SettingSchema.Img;
        }

        private void Update()
        {
            if (bThreadStatus)
            {
                btnStart.Content = "Stop";
            }
            else
            {
                btnStart.Content = "Start";
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("./log.txt"))
            {
                MsgHelper.ShowMessage(MsgType.FileCheck, "exites");
                return;
            }
            else
            {
                Builder.RaiseEvent(EventRaiseType.Setting);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            //Builder.RaiseEvent(EventRaiseType.AppExit);
            MsgHelper.ShowMessage(MsgType.AppExit, "");
        }

        private void UiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MsgHelper.ShowMessage(MsgType.AppExit, ""))
                e.Cancel = true;
        }

        private void GetPrinters()
        {
            LocalPrintServer printServer = new LocalPrintServer();
            PrintQueueCollection printQueues = printServer.GetPrintQueues();

            foreach (PrintQueue printQueue in printQueues)
            {
                string printerName = printQueue.Name;
                // Add code here to store or display the printer names
                // You can use emoji to express your mood or attitude about the printers 🖨️📠🎨
            }
        }

        private DateTime getDateTimeFromString(string dateString)
        {
            string inputFormat = "M/d/yyyyHH:mm:ss";
            string outputFormat = "M/d/yyyy h:mm:ss tt";

            DateTime dateTime;
            string formattedDateTime = "";

            if (DateTime.TryParseExact(dateString, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                formattedDateTime = dateTime.ToString(outputFormat);
            }
            return Convert.ToDateTime(formattedDateTime);
        }


        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (SettingSchema.Location == null)
            {
                MsgHelper.ShowMessage(MsgType.Other,"Invalid Setting");
                return;
            }
           
            if (!bThreadStatus)
            {
                Builder.RaiseEvent(EventRaiseType.UserBoard);
                Builder.uiUserBoard.Update();
            }
            else
            {
                Builder.RaiseEvent(EventRaiseType.UserBoard_Closed);
            }
            bThreadStatus = !bThreadStatus;
            Update();
        }

       

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            Builder.RaiseEvent(EventRaiseType.History);
        }

    }
}
