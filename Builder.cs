﻿using RAFFLE.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAFFLE
{
    public static class Builder
    {
        public static MainWindow uiMainWindow;
        public static Setting uiSetting;
        public static History uiHistory;
        public static Result uiResult;
        public static Login uiLogin;
        public static UserBoard uiUserBoard;
        public static AccountSetting uiAccountSetting;

        public static void RaiseEvent(EventRaiseType type)
        {
            
            switch (type)
            {
                case EventRaiseType.Init:

                    break;
                case EventRaiseType.Login:
                    uiLogin = new Login();
                    uiLogin.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EventRaiseType.LoginSuccess:
                    uiLogin.Visibility = System.Windows.Visibility.Hidden;
                    uiLogin = null;
                    uiMainWindow = new MainWindow();
                    uiMainWindow.Show();
                    uiMainWindow.Visibility = System.Windows.Visibility.Visible;

                    break;
                case EventRaiseType.AccountSetting:
                    uiLogin.Visibility = System.Windows.Visibility.Hidden;
                    if (uiLogin != null)
                    {
                        uiLogin = null;
                    }
                    if (uiAccountSetting == null)
                    {
                        uiAccountSetting = new AccountSetting();
                    }
                    uiAccountSetting.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EventRaiseType.AccountSetting_Closed:
                    uiAccountSetting.Visibility = System.Windows.Visibility.Hidden;
                    if (uiAccountSetting != null)
                    {
                        uiAccountSetting = null;
                    }
                    if (uiLogin == null)
                    {
                        uiLogin = new Login();
                    }
                    uiLogin.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EventRaiseType.MainWindow:
                    uiSetting.Visibility = System.Windows.Visibility.Hidden;
                    uiSetting = null;
                    uiMainWindow = new MainWindow();
                    uiMainWindow.Show();
                    uiMainWindow.Visibility = System.Windows.Visibility.Visible;

                    //if (uiUserBoard != null)
                    //{
                    //    uiUserBoard.Visibility = System.Windows.Visibility.Hidden;
                    //    uiUserBoard = null;
                    //}
                    //uiUserBoard = new UserBoard();
                    //uiUserBoard.Show();
                    break;
                case EventRaiseType.UserBoard:
                    if (uiUserBoard == null)
                    {
                        uiUserBoard = new UserBoard();
                    }
                    uiUserBoard.Show();
                    uiUserBoard.WindowState = System.Windows.WindowState.Maximized;
                    uiUserBoard.Visibility = System.Windows.Visibility.Visible;

                    break;
                case EventRaiseType.UserBoard_Closed:
                    uiUserBoard.Visibility = System.Windows.Visibility.Hidden;
                    if (uiUserBoard != null)
                    {
                        uiUserBoard = null;
                    }
                    break;
                case EventRaiseType.Setting:
                    uiMainWindow.Visibility = System.Windows.Visibility.Hidden;
                    uiMainWindow = null;
                    uiSetting = new Setting();
                    uiSetting.Show();
                    uiSetting.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EventRaiseType.History:
                    uiMainWindow.Visibility = System.Windows.Visibility.Hidden;
                    uiMainWindow = null;
                    uiHistory = new History();
                    uiHistory.Show();
                    uiHistory.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EventRaiseType.Result:
                    if (uiMainWindow == null)
                    {
                        uiMainWindow = new MainWindow ();
                    }
                    uiMainWindow.Visibility = System.Windows.Visibility.Hidden;
                    uiMainWindow = null;
                    uiResult = new Result();
                    uiResult.Show();
                    uiResult.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EventRaiseType.History2Main:
                    uiHistory.Visibility = System.Windows.Visibility.Hidden;
                    uiHistory = null;
                    uiMainWindow = new MainWindow();
                    uiMainWindow.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EventRaiseType.Result2History:
                    uiResult.Visibility = System.Windows.Visibility.Hidden;
                    uiResult = null;
                    uiHistory = new History();
                    uiHistory.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EventRaiseType.AppExit:
                    //Application.Current.Shutdown();
                    Process.GetCurrentProcess().Kill();
                    break;
            }
        }

        
    }
    public enum EventRaiseType
    {
        Init = 0x0001,
        Login = 0x0002,
        AccountSetting = 0x0010,
        AccountSetting_Closed = 0x0011,
        LoginSuccess = 0x0003,
        MainWindow = 0x0101,
        UserBoard = 0x0111,
        UserBoard_Closed = 0x0112,
        Setting = 0x0102,
        History = 0x0103,
        Result = 0x0104,
        History2Main = 0x0201,
        Result2History = 0x0202,
        AppExit = 0x0301
    }
}
