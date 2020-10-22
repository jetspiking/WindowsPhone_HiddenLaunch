using HiddenLaunch.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace HiddenLaunch
{
    public sealed partial class MainPage : Page, IDialogCallback
    {
        List<AppListEntry> applicationList = new List<AppListEntry>();

        public MainPage()
        {
            this.InitializeComponent();
            AppDesign.SetStatusBarColor((Color)this.Resources[PropertyString.SYSTEM_ACCENT_COLOR], Colors.Black);
            InitializeAppList();
            AppList.SelectionChanged += AppListViewEvent;
        }

        async void InitializeAppList()
        {
            IReadOnlyList<AppListEntry> appListEntryList;
            List<Package> packages = AppManager.GetAppPackages();

            foreach (Package package in packages)
            {
                try
                {
                    appListEntryList = await package.GetAppListEntriesAsync();

                    foreach (AppListEntry appListEntry in appListEntryList)
                    {
                        if (appListEntry == null) continue;

                        TextBlock appName = new TextBlock();
                        appName.Text = appListEntry.DisplayInfo.DisplayName;

                        TextBlock appDescription = new TextBlock();
                        appDescription.Text = appListEntry.DisplayInfo.Description;

                        Image appIcon = AppManager.GetImageFromEntry(appListEntry);

                        Size screenSize = Tool.GetScreenSize();

                        StackPanel stackPanel = new StackPanel();
                        stackPanel.Orientation = Orientation.Horizontal;
                        stackPanel.Width = screenSize.Width;

                        appIcon.Width = screenSize.Width / 5;
                        appIcon.Height = appIcon.Width;
                        stackPanel.Height = appIcon.Height;

                        stackPanel.Children.Add(appIcon);
                        stackPanel.Children.Add(appName);
                        stackPanel.Margin = new Thickness(0,0,0,5);

                        appName.Margin = new Thickness(appIcon.Width / 2, 0, 0, 0);

                        AppList.Items.Add(stackPanel);

                        this.applicationList.Add(appListEntry);
                    }
                }
                catch (System.Exception) { };
            }
        }

        private void AppListViewEvent(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView.SelectedIndex < 0 || listView.SelectedIndex > applicationList.Count) return;
            CustomDialog customDialog = new CustomDialog(this, "Launch Application", applicationList[listView.SelectedIndex].DisplayInfo.DisplayName, "Ok", "Cancel");
            customDialog.ShowDialog(applicationList[listView.SelectedIndex]);
        }


        public async void NotifyFromDialog(ContentDialogResult dialogResult, object argument)
        {
            if (dialogResult == ContentDialogResult.Primary)
            {
                AppListEntry app = argument as AppListEntry;
                await app.LaunchAsync();
            }
        }
    }

}

