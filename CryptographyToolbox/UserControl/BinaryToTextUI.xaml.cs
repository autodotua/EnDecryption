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
using static CryptographyToolbox.Tools;
using static CryptographyToolbox.Helper.Settings;
using static CryptographyToolbox.Helper.ConvertHelper;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace CryptographyToolbox
{
    public sealed partial class BinaryToTextUI : UserControl, ICryptographyPageBase
    {
        public BinaryToTextUI()
        {
            this.InitializeComponent();
        }
        
  

        public async Task Encrypte()
        {
            if (txtSource.Text == "")
            {
               await ShowError("源文本为空！");
                return;
            }
            string result = null;
            string source = txtSource.Text;
            result =await GetString(Encoding.GetBytes(source), BinaryOutputMode);

            txtResult.Text = result;

        }
        public async Task Decrypte()
        {

            if (txtResult.Text == "")
            {
              await  ShowError("源文本为空！");
                return;
            }
            
            string result = null;
            string source = TxtResult.Text;
          result = Encoding.GetString(await GetBytes(source,BinaryOutputMode));

            txtSource.Text = result;

        }


 
        public void RefreshUISettings()
        {
            txtSource.TextWrapping = txtResult.TextWrapping = TextBoxWrapping;
        }

        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => txtResult;
        

        public Task GenerateKey() => throw new NotImplementedException();
    }
}
