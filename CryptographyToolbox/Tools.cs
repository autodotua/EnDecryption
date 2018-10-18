using CryptographyToolbox.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptographyToolbox
{
    public static class Tools
    {

        public static async Task ShowError(string text)
        {
            await MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog dialog = new MessageDialog(text);
                await dialog.ShowAsync();
            });
        }
        public static async Task ShowError(string text, string title)
        {
            await MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog dialog = new MessageDialog(Environment.NewLine + text, title);
                await dialog.ShowAsync();
            });
        }
        public static async Task ShowError(string text, string title, Exception ex)
        {
            await MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
             {
                 MessageDialog dialog = new MessageDialog(Environment.NewLine + text + Environment.NewLine + Environment.NewLine + ex.Message, title);
                 await dialog.ShowAsync();
             });
        }
        public static async Task ShowError(string text,  Exception ex)
        {
            await MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog dialog = new MessageDialog(Environment.NewLine + text + Environment.NewLine + Environment.NewLine + ex.Message, Package.Current.DisplayName);
                await dialog.ShowAsync();
            });
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

        public static ICryptographyPageBase currentPivot;
        
      

    }
}
