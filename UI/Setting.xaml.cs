using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using RAFFLE.Schema;
using RAFFLE.Manager;
using Microsoft.Win32;
using RAFFLE.Utils;
using System.Reflection;
using System.Globalization;

namespace RAFFLE.UI
{
    public partial class Setting : UiWindow
    {
        private BitmapImage img;
        private string imgPath;
        private DateTime selectedDate;
        private int selectedHour;
        private int selectedMin;
        public Setting()
        {
            InitializeComponent();
            selectedDate = DateTime.Now;
            selectedHour = DateTime.Now.Hour;
            selectedMin = DateTime.Now.Minute;
            txtTime.SelectedDate = selectedDate;
            txtTimePicker.Text = selectedHour + ":"+ selectedMin;
            txtPrice.Text = "1";
            txtRate.Text = "25";
            imgPath = "";
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string curDir = Path.GetDirectoryName(executablePath);
            var uri = new Uri(curDir + "\\Invalid.png");
            img = new BitmapImage(uri);
            SetImg.Source = img;
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
            else
            {

            }
            return Convert.ToDateTime(formattedDateTime);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string selectedTime = "";
            if (selectedHour < 10)
                selectedTime += "0" + selectedHour + ":";
            else
                selectedTime += selectedHour + ":";
            if (selectedMin < 10)
                selectedTime += "0" + selectedMin;
            else
                selectedTime += selectedMin;
            selectedTime += ":00";
            SettingSchema.Time = selectedDate.ToShortDateString() + selectedTime;
            SettingSchema.Rate = Convert.ToDouble(txtRate.Text);
            SettingSchema.Price = Convert.ToInt32(txtPrice.Text);
            SettingSchema.Location = txtLocation.Text;
            SettingSchema.Description = txtDescription.Text;
            SettingSchema.CurProgress = 1;
            SettingSchema.CurImplse = 0;
            if (getDateTimeFromString(SettingSchema.Time) <= DateTime.Now)
            {
                MsgHelper.ShowMessage(MsgType.Other, "Invalid time");
                return;
            }
            if (imgPath=="")
            {
                MsgHelper.ShowMessage(MsgType.Other, "Invalid image");
                return;
            }
            if (SettingSchema.Price <= 0 || SettingSchema.Price > 5)
            {
                MsgHelper.ShowMessage(MsgType.Other, "Invalid price(1-5)");
                return;
            }
            if (SettingSchema.Rate <= 0 || SettingSchema.Rate > 75)
            {
                MsgHelper.ShowMessage(MsgType.Other, "Invalid rate(1-75)");
                return;
            }
            if (SettingSchema.Location == null || SettingSchema.Location == "")
            {
                MsgHelper.ShowMessage(MsgType.Other, "Invalid location");
                return;
            }
            if (SettingSchema.Description == null || SettingSchema.Description == "")
            {
                MsgHelper.ShowMessage(MsgType.Other, "Invalid description");
                return;
            }
            SettingSchema.Img = img;
            SettingSchema.ImgPath = imgPath;
            DBMgr.DeleteTmpCache();
            DBMgr.InsertTmpCache();
            Builder.RaiseEvent(EventRaiseType.MainWindow);
            Builder.uiMainWindow.UpdateState();
            Builder.uiMainWindow.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Builder.RaiseEvent(EventRaiseType.MainWindow);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MsgHelper.ShowMessage(MsgType.AppExit, "");
        }

        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP; *.JPG; *JPEG; *.GIF; *PNG; *.png)| *.BMP; *.JPG; *.JPEG; *.GIF; *.PNG; *.png | All files(*.*) | *.*";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string fileName = openFileDialog.FileName;
                var uri = new Uri(fileName);
                img = new BitmapImage(uri);
                imgPath = fileName;
                SetImg.Source = img;
            }
        }

        private void UiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Builder.RaiseEvent(EventRaiseType.MainWindow);
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // Update the selected time value when the OK button is clicked
            TimeSpan selectedTime = new TimeSpan((int)hourSlider.Value, (int)minuteSlider.Value, 0);
            txtTimePicker.Text = selectedTime.ToString(@"hh\:mm");
            timePickerPopup.IsOpen = false;
        }

        private void hourSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedHour = Convert.ToInt16(hourSlider.Value);
            lblHour.Text = selectedHour + "H:";
           
        }

        private void minuteSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedMin = Convert.ToInt16(minuteSlider.Value);
            lblMinute.Text =selectedMin + "M:";
        }

        private void txtTimePicker_MouseEnter(object sender, MouseEventArgs e)
        {
            // Open the popup window when the text box is clicked
            timePickerPopup.IsOpen = true;
            timePickerPopup.StaysOpen = true;

            // Set the initial slider values to the current time
            DateTime currentTime = DateTime.Now;
            if (txtTimePicker.Text != "")
            {
                hourSlider.Value = Convert.ToInt16(txtTimePicker.Text.Split(':')[0]);
                minuteSlider.Value = Convert.ToInt16(txtTimePicker.Text.Split(':')[1]);
            }
            else
            {
                hourSlider.Value = currentTime.Hour;
                minuteSlider.Value = currentTime.Minute;
            }
        }

        private void txtTimePicker_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void btnTimeCancel_Click(object sender, RoutedEventArgs e)
        {
            timePickerPopup.IsOpen = false;
        }
    }
}
