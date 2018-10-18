using CryptographyToolbox.Helper;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static CryptographyToolbox.Tools;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace CryptographyToolbox
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current { get; private set; }
        public MainPage()
        {
            InitializeComponent();
            Current = this;
            InitializeSettings();

            InitializeEvents();
            Application.Current.EnteredBackground += Current_EnteredBackground;
        }

        private void Current_EnteredBackground(object sender, Windows.ApplicationModel.EnteredBackgroundEventArgs e)
        {
            Settings.Save();
        }

        private void InitializeSettings()
        {
            PivotSelectionChangedEventHandler(null, null);
            RefreshUISettings();

            currentPivot?.TxtSource.Focus(FocusState.Programmatic);
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
                string item = await e.DataView.GetTextAsync();
                txt.Text = item;
                return;
            }

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count == 1 && items[0] is StorageFile)
                {
                    var item = items[0] as StorageFile;
                    OpenFile(item);
                    return;
                }
            }
            ShowError("拖放的内容不正确：" + Environment.NewLine + "支持文本或单个文件。", "拖放错误");
        }

        private void TxtSourceAndResultDragEnterEventHandler(object sender, DragEventArgs e)
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

        private async Task MakeComplicatedWork(Func<Task> action)
        {
            DoingWork = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await action();
            stopwatch.Stop();
            NotifyPopup.ShowToast("用时" + (int)stopwatch.Elapsed.TotalSeconds + "." + stopwatch.Elapsed.Milliseconds + "秒");
            DoingWork = false;
        }
        private void MakeComplicatedWork(Action<byte[]> action, byte[] bytes)
        {
            DoingWork = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action(bytes);
            stopwatch.Stop();
            NotifyPopup.ShowToast("用时" + (int)stopwatch.Elapsed.TotalSeconds + "." + stopwatch.Elapsed.Milliseconds + "秒");
            DoingWork = false;
        }


        private async void BtnEncrypteClickEventHandler(object sender, RoutedEventArgs e)
        {
          await  MakeComplicatedWork(() => currentPivot.Encrypte());
        }

        private async void BtnDecrypteClickEventHandler(object sender, RoutedEventArgs e)
        {
           await MakeComplicatedWork(() => currentPivot.Decrypte());
        }


        private async Task OpenFile(StorageFile file)
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
                //  currentFileContent = bytes;
                //  currentFile = file;
            }));
            dialog.Commands.Add(new UICommand("取消"));

            await dialog.ShowAsync();
        }

        //private void TxtSourcePreviewKeyDownEventHandler(object sender, KeyRoutedEventArgs e)
        //{

        //}

        private void BtnCopyClickEventHandler(object sender, RoutedEventArgs e)
        {
        }

        //private void AppBarButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        private void PivotSelectionChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {
            BtnDecrypte.Visibility = Visibility.Visible;
            btnGenerateKey.Visibility = Visibility.Visible;

            cmdBar.Visibility = Visibility.Visible;
            if(pivot.SelectedItem==null)
            {
                return;
            }
            switch ((pivot.SelectedItem as PivotItem).Header as string)
            {
                case "AES":
                    currentPivot = ucAes;
                    break;
                case "RSA":
                    currentPivot = ucRsa;
                    break;
                case "消息摘要":
                    currentPivot = ucMD;
                    BtnDecrypte.Visibility = Visibility.Collapsed;
                    btnGenerateKey.Visibility = Visibility.Collapsed;
                    break;
                case "数据表示":
                    currentPivot = ucBTT;
                    btnGenerateKey.Visibility = Visibility.Collapsed;
                    break;
                case "文字编码":
                    currentPivot = ucEncoding;
                    btnGenerateKey.Visibility = Visibility.Collapsed;
                    break;
                default:
                    currentPivot = null;
                    cmdBar.Visibility = Visibility.Collapsed;
                    break;

            }
        }

        private void BtnGenerateKeyClickEventHandler(object sender, RoutedEventArgs e)
        {
            MakeComplicatedWork(() => currentPivot.GenerateKey());
        }


        public void RefreshUISettings()
        {
            ICryptographyPageBase[] items = { ucAes, ucRsa, ucMD, ucBTT };
            foreach (var i in items)
            {
                i.RefreshUISettings();
            }
        }


    }
}
