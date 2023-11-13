using RAFFLE.Schema;
using RAFFLE.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RAFFLE.Manager
{
    public static class DBMgr
    {
        public static SQLiteConnection sqliteConn = new SQLiteConnection("Data Source=db_raffle.db;Version=3;New=True;Compress=True");
        /// <summary>
		/// true: login, false: logout
		/// </summary>
		/// <param name="bLogInOut"></param>
		/// <returns></returns>
		public static bool LogInOutDB(bool bLogInOut)
        {
            bool bResult = false;
            if (bLogInOut)
            {
                try
                {
                    sqliteConn.Open();
                    bResult = true;
                }
                catch (Exception ex)
                {
                    bResult = false;
                    MessageBox.Show(ex.Message);
                }
                return bResult;
            }
            else
            {
                try
                {
                    sqliteConn.Close();
                    bResult = true;
                }
                catch (Exception)
                {
                    bResult = false;
                }

                return bResult;
            }

        }

        public static string ReadPIN(string username)
        {
            string pin = "";
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_reader;
            sqlite_cmd = sqliteConn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("select password from tbl_user where username = '{0}'", username);
            sqlite_reader = sqlite_cmd.ExecuteReader();
            while (sqlite_reader.Read())
            {
                pin = sqlite_reader.GetString(0);
            }
            if (pin == "")
            {
                pin = null;
            }
            return pin;
        }

        public static void UpdatePIN(string username, string new_pin, string new_username)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqliteConn.CreateCommand();
                sqlite_cmd.CommandText = String.Format("update tbl_user set password = '{0}', username = '{1}' where username = '{2}'", new_pin, new_username, username);
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MsgHelper.ShowMessage(MsgType.Other, "Faild update username and password");
            }
        }

        public static int validateUsername(string username)
        {
            int result = 0;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_reader;
            sqlite_cmd = sqliteConn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("select count(username) from tbl_user where username = '{0}'", username);
            sqlite_reader = sqlite_cmd.ExecuteReader();
            while (sqlite_reader.Read())
            {
                result = sqlite_reader.GetInt16(0);
            }
            if (result == 0)
            {
                result = -1;
            }

            return result;
        }

        public static void InsertHistory()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = String.Format("Insert into tbl_history (time, rate, price, winner_number, winner_price, admin_price, location, description, image) values ('{0}', {1}, {2}, {3}, {4}, {5}, '{6}', '{7}', @0);", SettingSchema.Time, SettingSchema.Rate, SettingSchema.Price, ResultSchema.WinnerNumber, ResultSchema.WinnerPrice, ResultSchema.AdminPrice, SettingSchema.Location, SettingSchema.Description);
            cmd.CommandText = query;
            byte[] bytesImage;

            System.Drawing.Image img = new Bitmap(SettingSchema.ImgPath);
            bytesImage = ImageToByte(img, System.Drawing.Imaging.ImageFormat.Jpeg);
            SQLiteParameter param = new SQLiteParameter("@0", System.Data.DbType.Binary);
            param.Value = bytesImage;
            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();
        }

        public static List<HistoryEntity> LoadHistoryData()
        {
            List<HistoryEntity> lstHistory = new List<HistoryEntity>();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = "Select * FROM tbl_history";
            cmd.CommandText = query;
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                HistoryEntity historyEntity = new HistoryEntity();
                historyEntity.Time = reader.GetString(1);
                historyEntity.Rate = reader.GetFloat(2);
                historyEntity.Price = reader.GetFloat(3);
                historyEntity.WinnerNumber = reader.GetInt32(5);
                historyEntity.WinnerPrice = reader.GetFloat(6);
                historyEntity.AdminPrice = reader.GetFloat(7);
                historyEntity.Location = reader.GetString(8);
                historyEntity.Description = reader.GetString(9);
                lstHistory.Add(historyEntity);
            }

            return lstHistory;
        }

        public static void ClearHistoryData()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = "DELETE FROM tbl_history";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        public static byte[] ImageToByte(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                return imageBytes;
            }
        }
        //public Image Base64ToImage(string base64String)
        public static Image ByteToImage(byte[] imageBytes)
        {
            // Convert byte[] to Image
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(ms);
            return image;
        }

        public static void CreateTmpCache()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = "CREATE TABLE tmp_cache  (\r\n  `username` varchar(255)  Primary key,\r\n  `rate` double,\r\n  `price` double,\r\n `implse` int,\r\n  `process` int,\r\n  `imgpath` varchar(255),\r\n  `endtime` varchar(255) ,\r\n  `location` varchar(255) ,\r\n  `description` varchar(255) \r\n) ";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        public static void DropTmpCache()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = "DROP TABLE IF EXISTS tmp_cache";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        public static Boolean CheckTmpCache()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = "Select count(username) from  tmp_cache";
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            int rowCount = 0;
            rowCount = Convert.ToInt32(cmd.ExecuteScalar());
            if (rowCount == 0)
                return false;
            else
                return true;
        }

        public static void InsertTmpCache()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = String.Format("Insert into tmp_cache (username, rate, price, implse, process, imgpath , location, description, endtime) values ( 'admin', {0}, {1}, {2}, {3}, '{4}', '{5}', '{6}', '{7}');", SettingSchema.Rate, SettingSchema.Price, SettingSchema.CurImplse, SettingSchema.CurProgress, SettingSchema.ImgPath, SettingSchema.Location, SettingSchema.Description, SettingSchema.Time); ;
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        public static void DeleteTmpCache()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = "Delete from tmp_cache";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        public static void ReadTmpCache()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query = "Select username, rate, price, implse, process, imgpath, location, description, endtime from tmp_cache";
            cmd.CommandText = query;
            SQLiteDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                SettingSchema.Rate = reader.GetDouble(1);
                SettingSchema.Price = reader.GetDouble(2);
                SettingSchema.CurImplse = reader.GetInt32(3);
                SettingSchema.CurProgress = reader.GetInt32(4);
                SettingSchema.ImgPath = reader.GetString(5);
                var url = new Uri(SettingSchema.ImgPath);
                SettingSchema.Img=new BitmapImage(url);
                SettingSchema.Location = reader.GetString(6);
                SettingSchema.Description = reader.GetString(7);
                SettingSchema.Time = reader.GetString(8);
            }
        }

        public static void UpdateTmpCache()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd = sqliteConn.CreateCommand();
            string query =String.Format("Update tmp_cache Set implse = {0}, process = {1}, endtime = '{2}'  Where username = 'admin'" ,SettingSchema.CurImplse, SettingSchema.CurProgress, SettingSchema.Time);
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

    }
}
