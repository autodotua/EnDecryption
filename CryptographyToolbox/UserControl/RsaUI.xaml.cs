using CryptographyToolbox.Helper;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static CryptographyToolbox.Helper.ConvertHelper;
using static CryptographyToolbox.Helper.Settings;
using static CryptographyToolbox.Tools;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace CryptographyToolbox
{
    public sealed partial class RsaUI : UserControl, ICryptographyPageBase
    {
        public RsaUI()
        {
            this.InitializeComponent();
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
        public async Task Encrypte()
        {
            if (txtPublicKey.Text == "")
            {
                await GenerateKey();
            }
            byte[] value = await CheckEncrypte(txtSource.Text);
            if (value != null)
            {
                byte[] result = null;
                string key = txtPublicKey.Text;
                bool oaep = cbbPadding.SelectedIndex == 1;
                int keyLength = KeyLength;
                await Task.Run(() =>
              result = RsaHelper.Encrypte(
                    value,
                    key,
                    keyLength,
                    oaep));
                txtResult.Text = await GetString(result, BinaryOutputMode);
            }
        }



        public async Task Decrypte()
        {

            if (txtPrivateKey.Text == "")
            {
                await ShowError("请输入私钥！");
                return;
            }
            byte[] value = await CheckDecrypte(txtResult.Text);
            if (value != null)
            {
                byte[] result = null;
                string key = txtPrivateKey.Text;
                bool oaep = cbbPadding.SelectedIndex == 1;
                int keyLength = KeyLength;
                await Task.Run(() =>
                result = RsaHelper.Decrypte(
                    value,
                    key,
                    keyLength,
                    oaep));
                txtSource.Text = await GetString(result, BinaryInputMode);
            }
        }

        private int KeyLength => 1024 * (int)Math.Pow(2, cbbKeyLength.SelectedIndex);

        private async Task<byte[]> CheckDecrypte(string text)
        {
            byte[] value = null;
            if (text == "")
            {
                await ShowError("待加密内容不合法。");
                return null;
            }

            value = await GetBytes(text, BinaryOutputMode);

            if (value == null)
            {
                await ShowError("密文解析错误，请检查密文格式");
                return null;
            }

            return value;
        }
        private async Task<byte[]> CheckEncrypte(string text)
        {
            byte[] value = null;
            if (text == "")
            {
                await ShowError("待加密内容不合法。");
                return null;
            }

            value = await GetBytes(txtSource.Text, BinaryInputMode);

            return value;
        }

        public void RefreshUISettings()
        {
            txtSource.TextWrapping = txtResult.TextWrapping = txtPrivateKey.TextWrapping = txtPublicKey.TextWrapping = TextBoxWrapping;
        }


        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => txtResult;

    }
}
