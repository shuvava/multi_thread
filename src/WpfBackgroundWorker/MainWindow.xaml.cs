using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;


namespace WpfBackgroundWorker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private MainWindowModel _viewModel;
        public MainWindow()
        {
            _dialogCoordinator = new DialogCoordinator();
            _viewModel = new MainWindowModel();
            InitializeComponent();

            DataContext = _viewModel;
        }

        private async void ThreadControl_Click(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource();
            var settings = new MetroDialogSettings
            {
                NegativeButtonText = "Pause",
                CancellationToken = tokenSource.Token

            };
            var _controller = await _dialogCoordinator.ShowProgressAsync(DataContext, "Title", GetMessage(), isCancelable:true, settings:settings);

            ThreadControl.Content = "Countinue Background Job";
            while (!_controller.IsCanceled && _viewModel.Count< _viewModel.Max)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                _controller.SetMessage(GetMessage());
                _viewModel.Count += 1;
                _controller.SetProgress((double)_viewModel.Count/_viewModel.Max);
            }

            if (!_controller.IsCanceled)
            {
                _viewModel.Count = 0;
                ThreadControl.Content = "Start Background Job";
            }
            else
            {
                ThreadControl.Content = "Continue Background Job";
            }
            await _controller.CloseAsync();
        }


        private string GetMessage()
        {
            if (_viewModel.Count == 0)
            {
                return "Message";
            }

            return $"Massage\r\nIteration {_viewModel.Count}";
        }
    }
}
