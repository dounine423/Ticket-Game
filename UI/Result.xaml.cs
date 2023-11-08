using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using RAFFLE.Schema;


namespace RAFFLE.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Result : UiWindow
    {
        private BitmapImage img = null;
        public Result()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            //lblAdminPrice.Text = "Admin Price: " + Math.Round(ResultSchema.AdminPrice, 2).ToString();
            ResImg.Source = ResultSchema.Img;
            if (ResultSchema.WinnerNumber == 0)
            {
                lblWinnerNumber.Text = "Winner: None";
                lblWinnerPrice.Text = "Price: None";
                return;
            }
            lblWinnerNumber.Text = "Winner: " + ResultSchema.WinnerNumber.ToString();
            lblWinnerPrice.Text = "Price: " + Math.Round(ResultSchema.WinnerPrice, 2).ToString();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Builder.RaiseEvent(EventRaiseType.AppExit);
        }

        private void UiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Builder.RaiseEvent(EventRaiseType.MainWindow);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
           // DBMgr.InsertHistory();
            Builder.RaiseEvent(EventRaiseType.Result2History);
        }
    }
}
