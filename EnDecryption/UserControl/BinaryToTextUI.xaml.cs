using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static EnDecryption.Tools;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EnDecryption
{
    public sealed partial class BinaryToTextUI : UserControl, ICryptography
    {
        public BinaryToTextUI()
        {
            this.InitializeComponent();
            cbbDisplayMode.SelectionChanged += (p1, p2) => DisplayOrInputModeChanged();
        }

        private void DisplayOrInputModeChanged()
        {
            if (cbbDisplayMode.SelectedIndex == 1 || cbbDisplayMode.SelectedIndex == 2)
            {
                cbbSeparator.Visibility = Visibility.Visible;
            }
            else
            {
                cbbSeparator.Visibility = Visibility.Collapsed;
            }
        }

        private void Initialize()
        {
            encoding =GetComboBoxSelectedItemString(cbbEncoding);
            displayMode = GetComboBoxSelectedItemString(cbbDisplayMode);
            separator = GetComboBoxSelectedItemString(cbbSeparator);
        }

        public async Task Encrypte()
        {
            if (txtSource.Text == "")
            {
                ShowError("源文本为空！");
                return;
            }
            Initialize();
            string result = null;
            string type = (cbbDisplayMode.SelectedItem as ComboBoxItem).Content as string;
            string source = txtSource.Text;
            await Task.Run(() =>
            result = GetBinaryString(CurrentEncoding.GetBytes(source), type));

            txtResult.Text = result;

        }
        public async Task Decrypte()
        {

            if (txtSource.Text == "")
            {
                ShowError("源文本为空！");
                return;
            }

            Initialize();
            string result = null;
            string type = (cbbDisplayMode.SelectedItem as ComboBoxItem).Content as string;
            string source = TxtResult.Text;
            await Task.Run(() =>
          result = CurrentEncoding.GetString(GetBytesFromBinaryString(source, type)));

            txtSource.Text = result;

        }

        public async Task EncrypteFile()
        {
            if (currentFileContent == null)
            {
                throw new ArgumentNullException("文件为Null");
            }

            Initialize();


            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Base64文件", new List<string>() { ".base64" });
            picker.FileTypeChoices.Add("文本", new List<string>() { ".txt" });
            picker.FileTypeChoices.Add("所有文件", new List<string>() { "." });
            picker.SuggestedFileName = currentFile.Name.Replace(currentFile.FileType, "");

            var file = await picker.PickSaveFileAsync();

            byte[] result = null;
            string type = (cbbDisplayMode.SelectedItem as ComboBoxItem).Content as string;

            await Task.Run(() =>
          result = CurrentEncoding.GetBytes(GetBinaryString(currentFileContent, type)));

            if (result.Length > 0)
            {
                await Task.Run(() => FileIO.WriteBufferAsync(file, CryptographicBuffer.CreateFromByteArray(result)));
                currentFile = null;
                currentFileContent = null;
            }

        }
        public async Task DecrypteFile()
        {
            if (currentFileContent == null)
            {
                throw new ArgumentNullException("文件为Null");
            }
            Initialize();

            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("请指定文件类型！", new List<string>() { "." });
            picker.SuggestedFileName = currentFile.Name.Replace(currentFile.FileType, "");

            var file = await picker.PickSaveFileAsync();


            byte[] result = null;
            string type = (cbbDisplayMode.SelectedItem as ComboBoxItem).Content as string;

            await Task.Run(() =>
            result = GetBytesFromBinaryString(CurrentEncoding.GetString(currentFileContent), type));
            if (result.Length > 0)
            {
                await Task.Run(() => FileIO.WriteBufferAsync(file, CryptographicBuffer.CreateFromByteArray(result)));
                currentFile = null;
                currentFileContent = null;
            }

        }

        public async void SaveSourceAsFile()
        {
            await PickAndSaveFile(CurrentEncoding.GetBytes(TxtSource.Text), new Dictionary<string, string>() { { "文本文件", ".txt" } }, true);
        }

        public async void SaveResultAsFile()
        {
            await PickAndSaveFile(CurrentEncoding.GetBytes(TxtResult.Text), new Dictionary<string, string>() { { "文本文件", ".txt" } }, true);

        }
        public void RefreshUISettings()
        {
            txtSource.TextWrapping = txtResult.TextWrapping  = (TextWrapping)resource["TextWrapping"];
        }
        public string Separator => separator;
        private string separator;
        public string DisplayMode => displayMode;
        private string displayMode;

        public string Encoding => encoding;
        private string encoding ;

        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => txtResult;

        string ICryptography.Separator { get; }

        public Task GenerateKey() => throw new NotImplementedException();
    }
}
