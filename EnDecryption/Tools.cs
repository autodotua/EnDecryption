﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EnDecryption
{
    public static class Tools
    {
        public static Encoding CurrentEncoding
        {
            get
            {
                switch (currentInterface.EncodingIndex)
                {
                    case 0:
                        return Encoding.Default;
                    case 1:
                        return Encoding.UTF8;
                    case 2:
                        return Encoding.ASCII;
                    case 3:
                        return Encoding.Unicode;
                    case 4:
                        return Encoding.UTF32;
                    case 5:
                        return Encoding.UTF7;
                    case 6:
                        return Encoding.BigEndianUnicode;
                    case 7:
                        return Encoding.GetEncoding("GB2312");
                    default:
                        return Encoding.UTF8;
                }
            }
        }
        public static async void ShowError(string text)
        {
            await mainWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog dialog = new MessageDialog(text);
                await dialog.ShowAsync();
            });
        }
        public static async void ShowError(string text, string title)
        {
            await mainWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog dialog = new MessageDialog(Environment.NewLine + text, title);
                await dialog.ShowAsync();
            });
        }
        public static async void ShowError(string text, string title, Exception ex)
        {
            await mainWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
             {
                 MessageDialog dialog = new MessageDialog(Environment.NewLine + text + Environment.NewLine + Environment.NewLine + ex.ToString(), title);
                 await dialog.ShowAsync();
             });
        }

        public static byte[] ConvertToBytes(string text, ComboBox judegBy)
        {
            try
            {
                switch ((judegBy.SelectedItem as ComboBoxItem).Content as string)
                {
                    case "字符串":
                        return CurrentEncoding.GetBytes(text);
                    case "Base64字符串":
                        return Convert.FromBase64String(text);
                    case "十进制Byte":
                        return GetByteArrayFromDec(text);
                    case "十六进制Byte":
                        return GetByteArrayFromHex(text);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }

        }


        public static byte[] GetByteArrayFromDec(string text)
        {
            try
            {
                List<byte> bytes = new List<byte>();
                foreach (var i in text.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries))
                {
                    bytes.Add(byte.Parse(i));
                }
                return bytes.ToArray();
            }
            catch
            {
                return null;
            }
        }
        public static byte[] GetByteArrayFromHex(string text)
        {
            try
            {
                List<byte> bytes = new List<byte>();
                foreach (var i in text.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries))
                {
                    bytes.Add(Convert.ToByte(text, 16));
                }
                return bytes.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static string ToDec(byte[] bytes)
        {
            StringBuilder str = new StringBuilder();
            foreach (var i in bytes)
            {
                str.Append(i + Separator);
            }
            return str.ToString().TrimEnd(Separator.ToCharArray());
        }
        public static string ToHex(byte[] bytes)
        {
            StringBuilder str = new StringBuilder();
            foreach (var i in bytes)
            {
                str.Append(Convert.ToString(i, 16) + Separator);
            }
            return str.ToString().TrimEnd(Separator.ToCharArray()).ToUpper();
        }

        public static string ToText(byte[] bytes, ComboBox judegBy)
        {
            switch (judegBy.SelectedIndex)
            {
                case 0:
                    return Convert.ToBase64String(bytes);
                case 1:
                    return ToDec(bytes);
                case 2:
                    return ToHex(bytes);
                default:
                    return "";
            }

        }

        public static ICryptography currentInterface;

        public static string Separator
        {
            get
            {
                switch (currentInterface.SeparatorIndex)
                {
                    case 0:
                        return ",";
                    case 1:
                        return ".";
                    case 2:
                        return "\t";
                    case 3:
                        return " ";
                    case 4:
                        return Environment.NewLine;
                    default:
                        return ",";
                }
            }
        }

        public static void ShowToast(string message)
        {
            NotifyPopup notify = new NotifyPopup(message);
            notify.Show();
        }

        public static Page mainWindow;
    }
}