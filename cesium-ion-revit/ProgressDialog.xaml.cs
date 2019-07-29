using Amazon.S3.Transfer;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Cesium.Ion.Revit
{
    public partial class ProgressDialog : Window
    {
        private readonly IonAssetAPI Ion;
        private readonly IonUpload Asset;
        private readonly string FBXPath;

        private CancellationTokenSource TokenSource;

        public ProgressDialog(IonAssetAPI Ion, IonUpload Asset, string FBXPath)
        {
            InitializeComponent();
            this.Ion = Ion;
            this.Asset = Asset;
            this.FBXPath = FBXPath;
        }

        private void OnHandle(object sender, UploadProgressArgs Args) => Dispatcher.Invoke(() => ProgressBar.Value = Args.PercentDone);

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            TokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(async () =>
            {
                try {  
                    var ionUrl = await Ion.Upload(
                        Asset,
                        FBXPath,
                        Handler: OnHandle,
                        CancelToken: TokenSource.Token
                    );
                    ionUrl.OpenBrowser();
                } catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                    Dispatcher.Invoke(() => TaskDialog.Show("Upload Failed!", "Model generation crashed. See log for more information."));
                }
                Dispatcher.Invoke(Close);
            });
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            TokenSource?.Cancel();
            TokenSource = null;
        }
    }
}
