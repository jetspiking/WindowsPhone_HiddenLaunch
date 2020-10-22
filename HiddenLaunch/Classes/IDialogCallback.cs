using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HiddenLaunch.Classes
{
    public interface IDialogCallback
    {
        void NotifyFromDialog(ContentDialogResult dialogResult, Object argument);
    }
}
