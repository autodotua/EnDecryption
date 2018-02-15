using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EnDecryption
{
    public interface ICryptography
    {
        string Separator { get; }
        string Encoding { get; }
        string DisplayMode { get; }

        TextBox TxtSource { get;  }
        TextBox TxtResult { get;  }

        Task Encrypte();
        Task Decrypte();

        Task EncrypteFile();
        Task DecrypteFile();

        Task GenerateKey();
        void SaveSourceAsFile();
        void SaveResultAsFile();

        void RefreshUISettings();
    }
}
