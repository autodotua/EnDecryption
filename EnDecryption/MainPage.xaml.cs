using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
            mainPage = this;
            
            InitializeSettings();

            InitializeEvents();
            App.Current.EnteredBackground += Current_EnteredBackground;
        }

        private void Current_EnteredBackground(object sender, Windows.ApplicationModel.EnteredBackgroundEventArgs e)
        {
            SetSettings("TextWrapping", switchWrapping.IsOn);
        }

        private void InitializeSettings()
        {
           
            resource.Add("TextWrapping", (switchWrapping.IsOn = GetSettings("TextWrapping", true))?TextWrapping.Wrap:TextWrapping.NoWrap);

            RefreshUISettings();
        }
        private void InitializeEvents()
        {
            ucAes.TxtSource.AllowDrop = true;
            ucAes.TxtResult.AllowDrop = true;

            ucRsa.TxtSource.AllowDrop = true;
            ucRsa.TxtResult.AllowDrop = true;

            ucMD.TxtSource.AllowDrop = true;

            ucBTT.TxtSource.AllowDrop = true;
            ucBTT.TxtResult.AllowDrop = true;

            ucAes.TxtSource.DragEnter += TxtSourceAndResultDragEnterEventHandler;
            ucAes.TxtSource.Drop += TxtSourceAndResultDropEventHandler;
            ucAes.TxtResult.DragEnter += TxtSourceAndResultDragEnterEventHandler;
            ucAes.TxtResult.Drop += TxtSourceAndResultDropEventHandler;

            ucRsa.TxtSource.DragEnter += TxtSourceAndResultDragEnterEventHandler;
            ucRsa.TxtSource.Drop += TxtSourceAndResultDropEventHandler;
            ucRsa.TxtResult.DragEnter += TxtSourceAndResultDragEnterEventHandler;
            ucRsa.TxtResult.Drop += TxtSourceAndResultDropEventHandler;

            ucMD.TxtSource.DragEnter += TxtSourceAndResultDragEnterEventHandler;
            ucMD.TxtSource.Drop += TxtSourceAndResultDropEventHandler;

            ucBTT.TxtSource.DragEnter += TxtSourceAndResultDragEnterEventHandler;
            ucBTT.TxtSource.Drop += TxtSourceAndResultDropEventHandler;
            ucBTT.TxtResult.DragEnter += TxtSourceAndResultDragEnterEventHandler;
            ucBTT.TxtResult.Drop += TxtSourceAndResultDropEventHandler;
        }

        private async void TxtSourceAndResultDropEventHandler(object sender, DragEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                string item =await e.DataView.GetTextAsync();
                txt.Text = item;
                return;
            }

            if(e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items=await e.DataView.GetStorageItemsAsync();
               if(items.Count==1 && items[0] is StorageFile)
                {
                    var item = items[0] as StorageFile;
                    OpenFile(item);
                    return;
                }
            }
            ShowError("拖放的内容不正确：" + Environment.NewLine + "支持文本或单个文件。", "拖放错误");
        }

        private  void TxtSourceAndResultDragEnterEventHandler(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy; ;
            return;
            //if (e.DataView.Contains(StandardDataFormats.Text))
            //{
            //    e.AcceptedOperation = DataPackageOperation.Copy;
            //}
            //else if (e.DataView.Contains(StandardDataFormats.StorageItems))
            //{
            //    var items = await e.DataView.GetStorageItemsAsync();
            //    Debug.WriteLine(items.Count+"      "+ (items[0] is StorageFile));

            //    if (items.Count == 1 && items[0] is StorageFile)
            //    {
            //        e.AcceptedOperation = DataPackageOperation.Copy;
            //        Debug.WriteLine("OK");
            //    }
            //}
            //else
            //{
            //    e.AcceptedOperation = DataPackageOperation.None;
            //}
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
            await action();
            stopwatch.Stop();
            ShowToast("用时" + (int)stopwatch.Elapsed.TotalSeconds + "." + stopwatch.Elapsed.Milliseconds + "秒");
            DoingWork = false;
        }
        private void MakeComplicatedWork(Action<byte[]> action, byte[] bytes)
        {
            DoingWork = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action(bytes);
            stopwatch.Stop();
            ShowToast("用时" + (int)stopwatch.Elapsed.TotalSeconds + "." + stopwatch.Elapsed.Milliseconds + "秒");
            DoingWork = false;
        }


        private void BtnEncrypteClickEventHandler(object sender, RoutedEventArgs e)
        {
            if (currentFileContent == null)
            {
                MakeComplicatedWork(() => currentPivot.Encrypte());
            }
            else
            {

                MenuFlyoutItem menuText = new MenuFlyoutItem() { Text = "文本" };
                menuText.Click += (p1, p2) => MakeComplicatedWork(() => currentPivot.Encrypte());
                MenuFlyoutItem menuFile = new MenuFlyoutItem() { Text = "文件" };
                menuFile.Click += (p1, p2) => MakeComplicatedWork(() => currentPivot.EncrypteFile());
                MenuFlyout menu = new MenuFlyout()
                {
                    Items =
                    {
                        menuText,
                        menuFile
                    }
                };
                menu.ShowAt(sender as Button);
            }
        }

        private void BtnDecrypteClickEventHandler(object sender, RoutedEventArgs e)
        {
            if (currentFileContent == null)
            {
                MakeComplicatedWork(() => currentPivot.Decrypte());
            }
            else
            {

                MenuFlyoutItem menuText = new MenuFlyoutItem() { Text = "文本" };
                menuText.Click += (p1, p2) => MakeComplicatedWork(() => currentPivot.Decrypte());
                MenuFlyoutItem menuFile = new MenuFlyoutItem() { Text = "文件" };
                menuFile.Click += (p1, p2) => MakeComplicatedWork(() => currentPivot.DecrypteFile());
                MenuFlyout menu = new MenuFlyout()
                {
                    Items =
                    {
                        menuText,
                        menuFile
                    }
                };
                menu.ShowAt(sender as Button);
            }
        }

        private void BtnOpenFileClickEventHandler(object sender, RoutedEventArgs e)
        {
            if (currentFileContent != null)
            {
                MenuFlyoutItem menuText = new MenuFlyoutItem() { Text = "替换当前文件" };
                menuText.Click += (p1, p2) => PickFile();
                MenuFlyoutItem menuFile = new MenuFlyoutItem() { Text = "清空后台文件" };
                menuFile.Click += (p1, p2) => currentFileContent = null;
                MenuFlyout menu = new MenuFlyout()
                {
                    Items =
                    {
                        menuText,
                        menuFile
                    }
                };
                menu.ShowAt(sender as Button);
            }
            else
            {
                PickFile();
            }

            async void PickFile()
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

                    OpenFile(file);


                }
            }
        }

        private async void OpenFile(StorageFile file)
        {
            MessageDialog dialog = new MessageDialog("请选择文件的打开方式：", "打开文件");
            dialog.Commands.Add(new UICommand("Base64", async (p1) =>
            {
                //  txtSource.Text=  await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                var buffer = await FileIO.ReadBufferAsync(file);
                byte[] bytes = new byte[buffer.Length];
                using (var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                {
                    MakeComplicatedWork(reader.ReadBytes, bytes);
                }
                if (BtnDecrypte.Visibility == Visibility.Visible)

                {
                    MessageDialog textDialog = new MessageDialog("请选择字符串放置位置：", "打开文件");
                    textDialog.Commands.Add(new UICommand("原文文本框", (p2) => currentPivot.TxtSource.Text = Convert.ToBase64String(bytes)));
                    textDialog.Commands.Add(new UICommand("密文文本框", (p2) => currentPivot.TxtResult.Text = Convert.ToBase64String(bytes)));
                    textDialog.Commands.Add(new UICommand("取消"));

                  await textDialog.ShowAsync();
                }
                else
                {
         currentPivot.TxtSource.Text = Convert.ToBase64String(bytes);
                }

            }));
            //dialog.Commands.Add(new UICommand("Base64", async (p1) =>
            //{
            //    var buffer = await FileIO.ReadBufferAsync(file);
            //    byte[] bytes = new byte[buffer.Length];
            //    using (var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            //    {
            //        MakeComplicatedWork(reader.ReadBytes, bytes);
            //    }
            //    currentPivot.TxtSource.Text = Convert.ToBase64String(bytes);
            //}));
            dialog.Commands.Add(new UICommand("暂存后台", async (p1) =>
            {
                var buffer = await FileIO.ReadBufferAsync(file);
                byte[] bytes = new byte[buffer.Length];
                using (var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                {
                    MakeComplicatedWork(reader.ReadBytes, bytes);
                }
                currentFileContent = bytes;
                currentFile = file;
            }));
             dialog.Commands.Add(new UICommand("取消"));

            await dialog.ShowAsync();
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
            BtnDecrypte.Visibility = Visibility.Visible;
            btnGenerateKey.Visibility = Visibility.Visible;

            switch (pivot.SelectedIndex)
            {
                case 0:
                    currentPivot = ucAes;
                    break;
                case 1:
                    currentPivot = ucRsa;
                    break;
                case 2:
                    currentPivot = ucMD;
                    BtnDecrypte.Visibility = Visibility.Collapsed;
                    btnGenerateKey.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    currentPivot = ucBTT;
                    btnGenerateKey.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void BtnGenerateKeyClickEventHandler(object sender, RoutedEventArgs e)
        {
            MakeComplicatedWork(() => currentPivot.GenerateKey());
        }

        private void BtnSaveAsFileClickEventHandler(object sender, RoutedEventArgs e)
        {
            if (BtnDecrypte.Visibility == Visibility.Visible)
            {
                MenuFlyoutItem menuSource = new MenuFlyoutItem() { Text = "明文" };
                menuSource.Click += (p1, p2) => currentPivot.SaveSourceAsFile();
                MenuFlyoutItem menuResult = new MenuFlyoutItem() { Text = "密文" };
                menuResult.Click += (p1, p2) => currentPivot.SaveResultAsFile();
                MenuFlyout menu = new MenuFlyout()
                {
                    Items =
                    {
                        menuSource,
                        menuResult
                    }
                };
                menu.ShowAt(sender as Button);
            }
            else
            {
                currentPivot.SaveResultAsFile();
            }
        }

        private void RefreshUISettings()
        {
            ICryptography[] items = { ucAes, ucRsa, ucMD, ucBTT };
            foreach (var i in items)
            {
                i.RefreshUISettings();
            }
        }

        private void switchWrapping_Toggled(object sender, RoutedEventArgs e)
        {
            resource["TextWrapping"] = switchWrapping.IsOn ? TextWrapping.Wrap : TextWrapping.NoWrap;
            RefreshUISettings();
        }
    }


}
