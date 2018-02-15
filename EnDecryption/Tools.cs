using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Pickers;
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
                switch (currentPivot.Encoding)
                {
                    case "默认":
                        return Encoding.Default;
                    case "UTF8":
                        return new UTF8Encoding(false);
                    case "ASCII":
                        return Encoding.ASCII;
                    case "Unicode":
                        return Encoding.Unicode;
                    case "UTF32":
                        return Encoding.UTF32;
                    case "UTF7":
                        return Encoding.UTF7;
                    case "BigEndianUnicode":
                        return Encoding.BigEndianUnicode;
                    case "GB2312":
                        return Encoding.GetEncoding("GB2312");
                    default:
                        return Encoding.UTF8;
                }
}
        }
        public static async void ShowError(string text)
        {
            await mainPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog dialog = new MessageDialog(text);
                await dialog.ShowAsync();
            });
        }
        public static async void ShowError(string text, string title)
        {
            await mainPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog dialog = new MessageDialog(Environment.NewLine + text, title);
                await dialog.ShowAsync();
            });
        }
        public static async void ShowError(string text, string title, Exception ex)
        {
            await mainPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
             {
                 MessageDialog dialog = new MessageDialog(Environment.NewLine + text + Environment.NewLine + Environment.NewLine + ex.ToString(), title);
                 await dialog.ShowAsync();
             });
        }

        public static byte[] GetBytesFromBinaryString(string text, ComboBox judgeBy)
        {
            return GetBytesFromBinaryString(text, GetComboBoxSelectedItemString(judgeBy));
        }

        public static byte[] GetBytesFromBinaryString(string text,string judgeBy)
        {
            try
            { 
            switch (judgeBy)
            {
                case "字符串":
                    return CurrentEncoding.GetBytes(text);
                case "Base64字符串":
                    return Convert.FromBase64String(text);
                case "十进制Byte":
                    return GetBytesFromDec(text);
                case "十六进制Byte":
                    return GetBytesFromHex(text);
                default:
                    return null;
            }
        }
            catch
            {
                return null;
            }
        }

        public static byte[] GetBytes(string text)
        {
            try
            {
                return  Convert.FromBase64String(text);
            }
            catch
            {
                return  CurrentEncoding.GetBytes(text);
            }
        }


        public static byte[] GetBytesFromDec(string text)
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
        public static byte[] GetBytesFromHex(string text)
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

        public static string GetBinaryString(byte[] bytes, ComboBox judgeBy)
        {
           return GetBinaryString(bytes, GetComboBoxSelectedItemString(judgeBy));

        }
        public static string GetBinaryString(byte[] bytes, string judegBy)
        {
            switch (judegBy)
            {
                case "Base64字符串":
                    return Convert.ToBase64String(bytes);
                case "十进制Byte":
                    return ToDec(bytes);
                case "十六进制Byte":
                    return ToHex(bytes);
                default:
                    return "";
            }

        }

        public static string Separator
        {
            get
            {
                switch (currentPivot.Separator)
                {
                    case ",":
                        return ",";
                    case ".":
                        return ".";
                    case "（制表符）":
                        return "\t";
                    case "（空格）":
                        return " ";
                    case "（换行）":
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

        public static async Task PickAndSaveFile(byte[] data,IDictionary<string,string> filter,bool AllFileType,string sugguestedName=null)
        {
            if(data==null || (filter==null && !AllFileType))
            {
                throw new ArgumentNullException();
            }
            if(filter.Count==0 && !AllFileType)
            {
                throw new Exception("筛选器内容为空");
            }
            FileSavePicker picker = new FileSavePicker();
            if (filter != null && filter.Count > 0)
            {
                foreach (var i in filter)
                {
                    picker.FileTypeChoices.Add(i.Key, new List<string>() { i.Value });
                }
            }
            if(AllFileType)
            {
                picker.FileTypeChoices.Add("所有文件", new List<string>() { "."});
            }

            if (sugguestedName != null)
            {
                picker.SuggestedFileName = sugguestedName;
            }

            var file = await picker.PickSaveFileAsync();
            await Task.Run(() => FileIO.WriteBufferAsync(file, CryptographicBuffer.CreateFromByteArray(data)));

        }

        public static ICryptography currentPivot;
        public static Page mainPage;
        public static byte[] currentFileContent = null;
        public static StorageFile currentFile = null;

        public static string GetComboBoxSelectedItemString(ComboBox cbb)
        {
           return (cbb.SelectedItem as ComboBoxItem).Content as string;
        }

        public static ApplicationDataContainer settings;

   
        public static T GetSettings<T>(string name, T defautValue)
        {

            if (!settings.Values.ContainsKey(name))
            {
                return defautValue;
            }
            else
            {
                return (T)settings.Values[name];
            }
        }
        public static void EnsureSettings<T>(string name, T defautValue)
        {

            if (!settings.Values.ContainsKey(name))
            {
                SetSettings(name, defautValue);
            }
        }
        public static T GetSettings<T>(string name)
        {
                return (T)settings.Values[name];
        }

        public static void SetSettings<T>(string name, T value)
        {
            settings.Values[name] = value;
        }
        public static ResourceDictionary resource=new ResourceDictionary();


    }
}
