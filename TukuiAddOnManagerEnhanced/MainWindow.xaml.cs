/* ****************************************************************************** *\
|* TERMS OF USE                                                                   *|
|*                                                                                *|
|* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR     *|
|* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,       *|
|* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE    *|
|* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER         *|
|* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  *|
|* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN      *|
|* THE SOFTWARE.                                                                  *|
|*                                                                                *|
|* PLEASE NOTICE:                                                                 *|
|* ALL RIGHTS ARE RESERVED. (C)2014. YOU MAY NOT REPOST THIS WITHOUT EXPLICIT     *|
|* PERMISSION FROM EITHER MYSELF (REUBEN DELEON) OR AN AMINISTRATOR FROM THE      *|
|* TUKUI COMMUNITY (OF RANK "OVERLORD"). THIS PROGRAM ONLY COMES FROM THE TUKUI   *|
|* COMMUNITY AND IS IN NO WAY (OFFICIAL OR UNOFFICIAL) ASSOCIATED WITH THE TUKUI  *|
|* WEBSITE, BRANDING, ETC. THIS IS AN INDEPENDENTLY DEVELOPED ADDON THAT MAY OR   *|
|* OR MAY NOT BE: ACCEPTED OFFICIALLY, SHUTDOWN WITH OR WITHOUT NOTICE, REMOVED,  *|
|* AND/OR BROKEN INTENTIALLY OR UNINTENTIALLY NEVER TO WORK AGAIN.                *|
|*                                                                                *|
|* AUTHORED:    REUBEN S DELEON                                                   *|
|* DATE:        NOVEMBER 7, 2014                                                  *|
|* CONTACT:     darklordfriggs (a) gmail (dot) com                                *|
|*                                                                                *|
|*                                                                                *|
\* ****************************************************************************** */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Shell;

using TukuiAddOnManagerEnhanced.DEV;
using TukuiAddOnManagerEnhanced.Utilities;

namespace TukuiAddOnManagerEnhanced
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Constructors (Inits)

		public MainWindow( )
		{
			InitializeComponent( );

			// VMMV - The View (Window) is its own Model (making it a ViewModel)
			DataContext = this;

			// Get ready for a borderless Window
			WindowChrome chrome = new WindowChrome( );
			chrome.ResizeBorderThickness = new Thickness( 6 );
			WindowChrome.SetWindowChrome( this, chrome );

			// Register Window Events
			this.Loaded += MainWindow_Loaded;
			this.StateChanged += MainWindow_StateChanged;

			// Register UI Controls Events
		}

		#endregion Constructors (Inits)

		#region Window Event Handlers

		private void MainWindow_StateChanged( object sender, EventArgs e )
		{
			// Windowless borders do not have this code to force the window in the working area
			if ( this.WindowState == WindowState.Maximized )
			{
				// Check the Working Size and make sure the window does not get too big
				WPFMonitor monitor = this.CurrentMonitor( );
				MonitorInfo monitorInfo = monitor.GetMonitorInfo( );
				this.ForceWindowMove( monitorInfo.WorkingArea );
			}
		}

		private void MainWindow_Loaded( object sender, RoutedEventArgs e )
		{
#if DEBUG
			DevLoginWindow devWindow = new DevLoginWindow( );
			devWindow.Show( );
#endif

		}

		#endregion Window Event Handlers

		#region Windows State Buttons (Title Bar)

		private void text_WindowState_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
		{
			// Make this TextBlock act like a button
			( sender as FrameworkElement ).CaptureMouse( );
		}

		private void text_WindowState_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
		{
			// Make this TextBlock act like a button
			FrameworkElement context = sender as FrameworkElement;

			if ( context.IsMouseCaptured )
			{
				context.ReleaseMouseCapture( );

				if ( context.IsMouseOver )
					switch ( context.Name )
					{
						case "WindowState_Minimize": this.WindowState = WindowState.Minimized; break;
						case "WindowState_Maximize": this.WindowState = WindowState.Maximized; break;
						case "WindowState_Normalize": this.WindowState = WindowState.Normal; break;
						case "WindowState_Close": this.Close( ); break;
					}
			}
		}

		#endregion Windows State Buttons (Title Bar)
	}
}
