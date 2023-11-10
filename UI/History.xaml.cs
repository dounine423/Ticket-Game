using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using RAFFLE.Schema;
using RAFFLE.Manager;
using RAFFLE.Utils;

namespace RAFFLE.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class History : UiWindow
    {
        List<HistoryEntity> lstHistory = new List<HistoryEntity>();

        public History()
        {
            InitializeComponent();
            Initialize();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MsgHelper.ShowMessage(MsgType.AppExit, "");
        }

        private void UiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Builder.RaiseEvent(EventRaiseType.MainWindow);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Builder.RaiseEvent(EventRaiseType.History2Main);
        }

        private void Initialize()
        {
            lstHistory = DBMgr.LoadHistoryData();
            InitializeHistoryDataGrid();
        }

        private void InitializeHistoryDataGrid()
        {
            dgHistory.ItemsSource = lstHistory;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            DBMgr.ClearHistoryData();
            dgHistory.ItemsSource = null;
            lstHistory.Clear();
            InitializeHistoryDataGrid();
        }
    }
}
