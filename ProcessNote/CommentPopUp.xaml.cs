using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProcessNote
{
    /// <summary>
    /// Interaction logic for CommentPopUp.xaml
    /// </summary>
    public partial class CommentPopUp : Window
    {
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        public CommentPopUp()
        {
            InitializeComponent();
            if (Application.Current != null && Owner == null)
                this.Owner = mainWindow;
            if (this.IsEnabled)
            {
                yesButton.Click += delegate
                {
                    YesButtonClick(mainWindow.saveButtons);
                    mainWindow.CommentGrid.Children.Clear();
                    mainWindow.popUpIsOpen = false;
                    this.Close();
                };
                noButton.Click += delegate
                {
                    NoButtonClick(mainWindow.cancelButtons);
                    mainWindow.CommentGrid.Children.Clear();
                    this.Close();
                };
            }

        }

        private void YesButtonClick(List<Button> buttons)
        {
            
            foreach(Button button in buttons)
            {
                ButtonAutomationPeer peer = new ButtonAutomationPeer(button);

                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

                invokeProv.Invoke();
            }
            
        }

        private void NoButtonClick(List<Button> buttons)
        {
            foreach(Button button in buttons)
            {
                ButtonAutomationPeer peer = new ButtonAutomationPeer(button);

                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

                invokeProv.Invoke();
            }
            
        }


    }
}
