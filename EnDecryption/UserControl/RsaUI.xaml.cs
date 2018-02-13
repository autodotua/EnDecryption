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
    public sealed partial class RsaUI : UserControl, ICryptography
    {
        public RsaUI()
        {
            this.InitializeComponent();
            cbbDisplayMode.SelectionChanged += (p1, p2) => DisplayOrInputModeChanged();
        }

        private void DisplayOrInputModeChanged()
        {
            if (cbbDisplayMode.SelectedIndex == 1 || cbbDisplayMode.SelectedIndex == 2 )
            {
                cbbSeparator.Visibility = Visibility.Visible;
            }
            else
            {
                cbbSeparator.Visibility = Visibility.Collapsed;
            }
        }

        public async Task Encrypte()
        {
            if (txtPublicKey.Text == "" )
            {
              await  GenerateKey();
            }
            if (CheckEncrypte(txtSource.Text, out byte[] text))
            {
                byte[] result = null;
                string key = txtPublicKey.Text;
                bool oaep = cbbPadding.SelectedIndex == 1;
                int keyLength = KeyLength;
                await Task.Run(() =>
              result =RsaHelper.Encrypte(
                    text,
                    key,
                    keyLength,
                    oaep));
                txtResult.Text = ToText(result,cbbDisplayMode);
            }
        }

        public async Task GenerateKey()
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(KeyLength))
            {
                await Task.Run(() =>
                {
                    var p = RSA.ExportParameters(true);
                });
              
                txtPublicKey.Text = RSA.ToPublicXmlString();
              txtPrivateKey.Text = RSA.ToPriavateXmlString(); 
                
            }
        }

        public async Task Decrypte()
        {
            if (txtPublicKey.Text == "")
            {
               await GenerateKey();
            }
            if (CheckDecrypte(txtResult.Text, out byte[] text))
            {
                byte[] result = null;
                string key = txtPrivateKey.Text;
                bool oaep = cbbPadding.SelectedIndex == 1;
                int keyLength = KeyLength;
                await Task.Run(() =>
                result= RsaHelper.Decrypte(
                    text,
                    key,
                    keyLength,
                    oaep));
                txtSource.Text = CurrentEncoding.GetString( result);
            }
        }

        private int KeyLength => 1024 * (int)Math.Pow(2, cbbKeyLength.SelectedIndex);

        private bool CheckDecrypte(string text, out byte[] value)
        {
            value = null;
            if (text == "")
            {
                ShowError("待加密内容不合法。");
                return false;
            }
            
                value = ConvertToBytes(text,cbbDisplayMode);
        
            if(value==null)
            {
                ShowError("密文解析错误，请检查密文格式");
                return false;
            }
            
            return true;
        }
        private bool CheckEncrypte(string text, out byte[] value)
        {
            value = null;
            if (text == "")
            {
                ShowError("待加密内容不合法。");
                return false;
            }

            try
            {
                value = Convert.FromBase64String(text);
            }
            catch
            {
                value = CurrentEncoding.GetBytes(text);
            }

            return true;
        }



        public int SeparatorIndex => cbbSeparator.SelectedIndex;

        public int DisplayModeIndex => cbbDisplayMode.SelectedIndex;
        public int InputModeIndex => -1;

        public int EncodingIndex => cbbEncoding.SelectedIndex;

        public TextBox TxtSource => TxtSource;
        public TextBox TxtResult => TxtResult;
        

        }
}
