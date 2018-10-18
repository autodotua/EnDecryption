using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CryptographyToolbox
{
    public interface ICryptographyPageBase
    {

        TextBox TxtSource { get;  }
        TextBox TxtResult { get;  }

        Task Encrypte();
        Task Decrypte();
        

        Task GenerateKey();

        void RefreshUISettings();
    }
}
