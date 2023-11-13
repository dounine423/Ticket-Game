using System;
using System.Windows.Input;
using Wpf.Ui.Controls;
using RAFFLE.Schema;
using RAFFLE.Manager;
using System.Globalization;
using System.Windows.Threading;
using RAFFLE.Utils;

namespace RAFFLE.UI
{
    public partial class UserBoard : UiWindow
    {
        private string sImpluse;
        private int clockInterval;
        private bool raffleFlag;
        private DateTime raffleStopTime;
        DispatcherTimer timer_raffle = new DispatcherTimer();
        DispatcherTimer timer_clock = new DispatcherTimer();
        private int ClockCount;
        private int delay = 300;
        public UserBoard()
        {
            InitializeComponent();
            Initialze();
        }
        public void Update()
        {
            timer_raffle.Start();
            raffleFlag = true;
            lblEndTime.Text = "End Time: " + getDateTimeFromString(SettingSchema.Time).ToString();
            lblPrice.Text = "Price: " + SettingSchema.Price;
            Img.Source = SettingSchema.Img;
            lblLocation.Text = "" + SettingSchema.Location;
            lblDescription.Text = "" + SettingSchema.Description;
            prgThread.IsIndeterminate = true;
        }

        public void EndState()
        {
            timer_raffle.Stop();
            prgThread.IsIndeterminate = false;
            if (ResultSchema.WinnerPrice == 0)
            {
                prgThread.Progress = 100;
                return;
            }
            prgThread.Progress = 100;   
        }

        private DateTime getDateTimeFromString(string dateString)
        {
            string inputFormat = "M/d/yyyyHH:mm:ss";
            string outputFormat = "M/d/yyyy HH:mm:ss";

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

        public void Initialze()
        {
            prgThread.IsIndeterminate = false;
            raffleFlag = true;
            lblEndTime.Text = "End Time: " + getDateTimeFromString(SettingSchema.Time).ToString();
            lblPrice.Text = "Price: " + SettingSchema.Price;
            lblLocation.Text = "Location: " + SettingSchema.Location;
            lblDescription.Text = "Description: " + SettingSchema.Description;
            ResultSchema.WinnerPrice =(SettingSchema.CurProgress-1)  * SettingSchema.Price * (1 - SettingSchema.Rate / 100);
            lblWinnerPrice.Text = "Winner Prize: " + ResultSchema.WinnerPrice;
            for (int i = 0; i < SettingSchema.CurImplse; i++)
                sImpluse += "+";
            txtImpluse.Text = sImpluse;
            Img.Source = SettingSchema.Img;
            timer_raffle.Interval = TimeSpan.FromSeconds(ThreadMgr.timerSpc); // Set the interval to 1 second
            timer_raffle.Tick += Timer_Tick; // Set the event handler
            ClockCount = 3;
            lblTitle.Text = ClockCount.ToString();
            DateTime curTime = DateTime.Now;
            DateTime endTime = getDateTimeFromString(SettingSchema.Time);
            clockInterval =(int) endTime.Subtract(curTime).TotalSeconds;
            timer_clock.Interval = TimeSpan.FromSeconds(1);
            timer_clock.Tick += TimerClock_Tick;
            timer_clock.Start();
        }
        private void TimerClock_Tick(object sender, EventArgs e)
        {
            DateTime curTime = DateTime.Now;
            if (!raffleFlag && curTime.Subtract(raffleStopTime).TotalSeconds>=delay)
            {
                raffleFlag = true;
                timer_raffle.Start();
            }
            lblCurTime.Text = "Current Time: " + DateTime.Now.ToString();

            if (ClockCount > 0)
            {
                ClockCount--;
                lblTitle.Text = ClockCount.ToString();
            } else if (ClockCount == 0)
            {
                ClockCount--;
                lblTitle.Text = "Started!";
            } else
            {
                lblTitle.Text = "";
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            txtImpluse.Focus();
            DateTime curTime = DateTime.Now;
            DateTime endTime = getDateTimeFromString(SettingSchema.Time);
            if (sImpluse != "" && sImpluse != null && sImpluse.Length > 0)
            {
               // ThreadMgr.PrintText("Ticket " +SettingSchema.CurProgress  + "\n" + endTime.ToShortDateString()+" "+endTime.ToShortTimeString() + "\n" + SettingSchema.Location + "\n" + SettingSchema.Description, 14);
                SettingSchema.CurProgress++;
                ResultSchema.WinnerPrice =(SettingSchema.CurProgress-1)  * SettingSchema.Price * (1 - SettingSchema.Rate / 100);
                
                txtImpluse.Text = sImpluse;
                int cnt = (int)SettingSchema.Price;
                if (sImpluse.Length < cnt)
                    cnt = sImpluse.Length;
                sImpluse = sImpluse.Substring(cnt);
                txtImpluse.Text = sImpluse;
                SettingSchema.CurImplse = sImpluse.Length;
                lblWinnerPrice.Text = "Winner Prize: " + ResultSchema.WinnerPrice;
            }
            if(curTime > endTime)
            {
                raffleStopTime = curTime;
                timer_raffle.Stop();
                raffleFlag = false;
                DateTime nextEndTime = curTime.AddSeconds(clockInterval+delay);
                string formattedDateTime = nextEndTime.ToString("M/d/yyyyHH:mm:ss");
                SettingSchema.Time = formattedDateTime;
                lblEndTime.Text = "End Time: " + nextEndTime.ToShortDateString()+" "+ nextEndTime.ToLongTimeString();
                Random random = new Random();
                if (SettingSchema.CurProgress == 1)
                {
                    ResultSchema.WinnerNumber = 0;
                }
                else
                {
                    ResultSchema.WinnerNumber = random.Next(1, SettingSchema.CurProgress);
                }
                ResultSchema.AdminPrice =(SettingSchema.CurProgress-1)  * SettingSchema.Price * (SettingSchema.Rate / 100);
                ResultSchema.Img = SettingSchema.Img;
                Builder.RaiseEvent(EventRaiseType.Result);
                //EndState();
               // ThreadMgr.PrintText("Winner\n" + ResultSchema.WinnerNumber+" / "+ ResultSchema.WinnerPrice +" $ " + "\n" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\n" + SettingSchema.Location + "\n" + SettingSchema.Description, 14);
                DBMgr.InsertHistory();
                ResultSchema.WinnerPrice = 0;
                SettingSchema.CurProgress = 1;
                lblWinnerPrice.Text = "Winner Prize: " + ResultSchema.WinnerPrice;
            }
            DBMgr.UpdateTmpCache();
            //Update();
 
        }

        private void txtImpluse_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            char typedChar = e.Text[0];
            if (typedChar == '9')
            {
                txtImpluse.Text += "+";
                sImpluse = txtImpluse.Text;
                SettingSchema.CurImplse = sImpluse.Length;
                DBMgr.UpdateTmpCache();
            }
        }

        private void UiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer_raffle.Stop();
            timer_clock.Stop();
        }

    }
}