using System;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using RAFFLE.Schema;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace RAFFLE.Utils
{
    public static class MsgHelper
    {
        public static DateTime getDateTimeFromString(string dateString)
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
        public static bool ShowMessage(MsgType msgType, string msgTxt)
        {
            bool bRes = false;
            Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox();

            switch (msgType)
            {
                case MsgType.AppExit:
                    messageBox.Title = "Warnig";
                    messageBox.Content = new TextBlock
                    {
                        Margin = new Thickness(0, 8, 0, 0),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        TextAlignment = TextAlignment.Center,
                        FontSize = 18,
                        Text = "Are you sure exit?"
                    };
                    messageBox.Height = 200;
                    messageBox.Topmost = false;
                    messageBox.ButtonRightName = "Yes";
                    messageBox.ButtonLeftName = "No";
                    messageBox.ShowTitle = true;
                    messageBox.ButtonLeftClick += (_, _) => { messageBox.Hide(); bRes = true; };
                    messageBox.ButtonRightClick += (_, _) =>
                    {
                   
                        Builder.RaiseEvent(EventRaiseType.AppExit);
                    }; 
                    break;
                case MsgType.Other:
                    messageBox.Title = "Error";
                    messageBox.Content = new TextBlock
                    {
                        Margin = new Thickness(0, 8, 0, 0),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        TextAlignment = TextAlignment.Center,
                        FontSize = 14,
                        Text = msgTxt,
                        Width = 400,
                        Height = 200
                    };
                    messageBox.Height = 200;
                    messageBox.Topmost = false;
                    messageBox.ButtonRightName = "Exit";
                    messageBox.ButtonLeftName = "Retry";
                    messageBox.ShowTitle = true;
                    messageBox.ButtonLeftClick += (_, _) => { messageBox.Hide(); bRes = true; };
                    messageBox.ButtonRightClick += (_, _) => Builder.RaiseEvent(EventRaiseType.AppExit);

                    break;
                case MsgType.DocSelRemove:
                    messageBox.Title = "Warnig";
                    messageBox.Content = new TextBlock
                    {
                        Margin = new Thickness(0, 8, 0, 0),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        TextAlignment = TextAlignment.Center,
                        FontSize = 18,
                        Text = msgTxt
                    };
                    messageBox.Height = 200;
                    messageBox.Topmost = false;
                    messageBox.ButtonRightName = "Yes";
                    messageBox.ButtonLeftName = "No";
                    messageBox.ShowTitle = true;
                    messageBox.ButtonLeftClick += (_, _) => { messageBox.Hide(); bRes = true; };
                    messageBox.ButtonRightClick += (_, _) => { messageBox.Hide(); bRes = false; };

                    break;
                case MsgType.PhotoRemove:
                    messageBox.Title = "Warnig";
                    messageBox.Content = new TextBlock
                    {
                        Margin = new Thickness(0, 8, 0, 0),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        TextAlignment = TextAlignment.Center,
                        FontSize = 18,
                        Text = msgTxt
                    };
                    messageBox.Height = 200;
                    messageBox.Topmost = false;
                    messageBox.ButtonRightName = "Yes";
                    messageBox.ButtonLeftName = "No";
                    messageBox.ShowTitle = true;
                    messageBox.ButtonLeftClick += (_, _) => { messageBox.Hide(); bRes = true; };
                    messageBox.ButtonRightClick += (_, _) => { messageBox.Hide(); bRes = false; };

                    break;
                case MsgType.FileCheck:
                    messageBox.Title = "Inform";
                    messageBox.Content = new TextBlock
                    {
                        Margin = new Thickness(0, 8, 0, 0),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        TextAlignment = TextAlignment.Center,
                        FontSize = 18,
                        Text = "Do you want to load saved setting data"
                    };
                    messageBox.Height = 200;
                    messageBox.Topmost = false;
                    messageBox.ButtonRightName = "Yes";
                    messageBox.ButtonLeftName = "No";
                    messageBox.ShowTitle = true;
                    messageBox.ButtonLeftClick += (_, _) =>
                    {
                        messageBox.Hide();
                        bRes = false;
                        Builder.RaiseEvent(EventRaiseType.Setting);
                    };
                    messageBox.ButtonRightClick += (_, _) => {
                        FileStream fs = new FileStream("./log.txt", FileMode.Open, FileAccess.Read);
                        StreamReader sr = new StreamReader(fs);
                        SettingSchema.ImgPath = sr.ReadLine();
                        var uri = new Uri(SettingSchema.ImgPath);
                        SettingSchema.Img = new BitmapImage(uri);
                        SettingSchema.Time = sr.ReadLine();
                        DateTime curTime = DateTime.Now;
                        DateTime endTime = getDateTimeFromString(SettingSchema.Time);
                        if (curTime > endTime)
                        {
                            SettingSchema.Time = curTime.AddSeconds(300).ToString("M/d/yyyyHH:mm:ss");
                        }
                        SettingSchema.Rate = Convert.ToDouble(sr.ReadLine());
                        SettingSchema.Price = Convert.ToDouble(sr.ReadLine());
                        SettingSchema.Location = sr.ReadLine();
                        SettingSchema.Description = sr.ReadLine();
                        SettingSchema.CurProgress = Convert.ToInt32(sr.ReadLine());
                        Builder.uiMainWindow.UpdateState();
                        sr.Close();
                        fs.Close();
                        messageBox.Hide(); 
                        bRes = false;
                    };
                    break;
                default:
                    break;
            }

            messageBox.ShowDialog();

            return bRes;
        }


    }
}
