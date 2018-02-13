using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static EnDecryption.Tools;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace EnDecryption
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            mainWindow = this;
        }

        public bool DoingWork
        {
            set
            {
                grdLoding.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                prgLoading.IsActive = value;
            }
        }

        private async void MakeComplicatedWork(Func<Task> action)
        {
            DoingWork = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
          await  action();
            stopwatch.Stop();
            ShowToast("用时" + (int)stopwatch.Elapsed.TotalSeconds + "." + stopwatch.Elapsed.Milliseconds + "秒");
            DoingWork = false;
        }


        private void BtnEncrypteClickEventHandler(object sender, RoutedEventArgs e)
        {
            MakeComplicatedWork(() => currentInterface.Encrypte());
        }

        private void BtnDecrypteClickEventHandler(object sender, RoutedEventArgs e)
        {
            MakeComplicatedWork(() => currentInterface.Decrypte());
        }

        private async void BtnOpenFileClickEventHandler(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                //if((await file.GetBasicPropertiesAsync()).Size>1024*1024*1024)
                //{
                //    ShowError("请选择小于1GB的文件!");
                //    return;
                //}
                MessageDialog dialog = new MessageDialog("请选择文件的打开方式：", "打开文件");
                dialog.Commands.Add(new UICommand("文本", async (p1) =>
                {
                    //  txtSource.Text=  await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                    var buffer = await FileIO.ReadBufferAsync(file);
                    byte[] bytes = new byte[buffer.Length];
                    using (var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                    {
                        reader.ReadBytes(bytes);
                    }
                    currentInterface.TxtSource.Text = CurrentEncoding.GetString(bytes);
                }));
                dialog.Commands.Add(new UICommand("Base64", async (p1) =>
                {
                    var buffer = await FileIO.ReadBufferAsync(file);
                    byte[] bytes = new byte[buffer.Length];
                    using (var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                    {
                        reader.ReadBytes(bytes);
                    }
                    currentInterface.TxtSource.Text = Convert.ToBase64String(bytes);
                }));

                await dialog.ShowAsync();
            }
        }

        private void TxtSourcePreviewKeyDownEventHandler(object sender, KeyRoutedEventArgs e)
        {

        }

        private void BtnCopyClickEventHandler(object sender, RoutedEventArgs e)
        {
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PivotSelectionChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    currentInterface = ucAes;
                    break;
                case 1:
                    currentInterface = ucRsa;
                    break;
            }
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            MakeComplicatedWork( () => currentInterface.GenerateKey());
        }
    }



}
