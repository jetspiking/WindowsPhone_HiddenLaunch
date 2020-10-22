using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace HiddenLaunch.Classes
{
    /**
     * IMPORTANT! READ CAREFULLY!     
     * 
     * To use PackageManager you first need to make some changes in your appxmanifest. (Right click on Package.appxmanifest in the Solution Explorer and select View Code)
     * These changes allow you to use restricted capabilities ("rescap" is the tag for this).
     * 
     * In package add "xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"" and under IgnorableNamespaces add "rescap".
     * 
     * Example (All adjusted properties should be inside the package opening tag!):
     * 
     * <Package  
     * xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10" 
     * xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest" 
     * xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10" 
     * xmlns:uap3="http://schemas.microsoft.com/appx/manifest/uap/windows10/3" 
     * xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
     * IgnorableNamespaces="uap mp uap3 rescap">
     *
     * NOTE: "rescap" capabilities should be listed above all other capabilities! 
     * 
     * Now under the capabilities in your manifest add "rescap:Capability Name="packageManagement"" and "rescap:Capability Name="packageQuerys"", don't forget the opening and closing tag "<>".
     *
     * Example:
     * 
     * <Capabilities>
     * <rescap:Capability Name="packageManagement"/>
     * <rescap:Capability Name="packageQuerys" />
     * </Capabilities>
     * 
     * NOTE: If your application uses restricted capabilities, you won't be able to upload to the Microsoft Store (so you must sideload your appx).
     */

    public static class AppManager
    {
        public static async void StartApp(AppListEntry appListEntry)
        {
            await appListEntry.LaunchAsync();
        }

        public static Image GetImageFromPackage(Package package)
        {
            List<AppListEntry> appListEntries = new List<AppListEntry>();
            Task<IReadOnlyList<AppListEntry>> getAppEntriesTask = package.GetAppListEntriesAsync().AsTask();
            getAppEntriesTask.Wait();
            appListEntries = getAppEntriesTask.Result.ToList();

            return GetImageFromEntry(appListEntries[0]);
        }

        public static Image GetImageFromEntry(AppListEntry appListEntry)
        {
            BitmapImage logo = new BitmapImage();
            var logoStream = appListEntry.DisplayInfo.GetLogo(new Size(50, 50));
            Task<IRandomAccessStreamWithContentType> logoStreamTask = logoStream.OpenReadAsync().AsTask();
            logoStreamTask.Wait();
            IRandomAccessStreamWithContentType logoStreamResult = logoStreamTask.Result;
            logo.SetSource(logoStreamResult);

            Image image = new Image();
            image.Source = logo;

            return image;
        }

            public static List<Package> GetAppPackages()
        {
            PackageManager packageManager = new PackageManager();
            IEnumerable<Package> packageEnumerable = packageManager.FindPackagesForUser(String.Empty);          // UWP apps can't run without administrator permissions, this is why you need to call "FindPackagesForUser("")" instead of "FindPackages()" Administrator permissions are mandatory when going outside the current user.
            return packageEnumerable.ToList();
        }

        public static async Task<List<AppListEntry>> GetAppEntries()
        {
            PackageManager packageManager = new PackageManager();
            IEnumerable<Package> packageEnumerable = packageManager.FindPackagesForUser(String.Empty);          // UWP apps can't run without administrator permissions, this is why you need to call "FindPackagesForUser("")" instead of "FindPackages()" Administrator permissions are mandatory when going outside the current user.
            List<AppListEntry> appListEntries = new List<AppListEntry>();

            foreach (Package package in packageEnumerable)
            {
                try
                {
                    IReadOnlyList<AppListEntry> apps = await package.GetAppListEntriesAsync();
                    appListEntries.AddRange(apps);
                }
                catch (System.Exception) {};                                                                   // On Windows 10 Mobile devices this try-catch is needed to prevent an error, I don't know why it is thrown.
            }

            return appListEntries;
        }
    }
}
