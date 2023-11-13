using System.Windows.Media.Imaging;

namespace RAFFLE.Schema
{
    public static class SettingSchema
    {
        private static string m_Time;
        private static double m_Rate;
        private static double m_Price;
        private static BitmapImage m_Img;
        private static string m_ImgPath;
        private static string m_Location;
        private static string m_Description;
        private static int m_CurProgress;
        private static int m_CurImplse;

        public static string Time { get => m_Time; set => m_Time = value; }
        public static double Rate { get => m_Rate; set => m_Rate = value; }
        public static double Price { get => m_Price; set => m_Price = value; }
        public static BitmapImage Img { get => m_Img; set => m_Img = value; }
        public static string ImgPath { get => m_ImgPath; set => m_ImgPath = value; }
        public static string Location { get => m_Location; set => m_Location = value; }
        public static string Description { get => m_Description; set => m_Description = value; }
        public static int CurProgress { get => m_CurProgress; set => m_CurProgress = value; }
        public static int CurImplse { get => m_CurImplse; set => m_CurImplse = value; }
    }
}
