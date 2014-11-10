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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TukuiAddOnManagerEnhanced.DEV;
using TukuiAddOnManagerEnhanced.Json;
using TukuiAddOnManagerEnhanced.Utilities;

namespace TukuiAddOnManagerEnhanced
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private TukuiClient MyClient;

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

			// Init Animations
			InitAnimations( );
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


		#region Sub Menu Buttons - Logic
		private void SubMenuTextButton_MouseDown( object sender, MouseButtonEventArgs e )
		{
			// Make the Sub Menu Buttons (TextBlocks) act like Buttons
			FrameworkElement currentElement = sender as FrameworkElement;
			currentElement.CaptureMouse( );
		}

		private void SubMenuTextButton_MouseUp( object sender, MouseButtonEventArgs e )
		{
			// Make the Sub Menu Buttons (TextBlocks) act like Buttons
			FrameworkElement currentElement = sender as FrameworkElement;
			
			if (currentElement.IsMouseCaptured)
			{
				currentElement.ReleaseMouseCapture( );

				if (currentElement.IsMouseOver)
				{
					// OnClicked!
					switch(currentElement.Name)
					{
						case "SubMenuTextButton_UpdateAll":
							break;

						case "SubMenuTextButton_Refresh":
							break;

						case "SubMenuTextButton_Settings":
							break;

						case "SubMenuTextButton_Logout": Clicked_Logout( ); break;
					}
				}
			}
		}



		bool loginWindowVisible = false;
		private void Clicked_Logout( )
		{
			if ( loginWindowVisible )
			{
				Animate_LoginRibbon_Hide( );
			}
			else
			{
				Animate_LoginRibbon_Show( );
			}
			loginWindowVisible = !loginWindowVisible;
		}

		#endregion Sub Menu Buttons - Logic

		#region Animations

		public void InitAnimations( )
		{
			LoginRibbonAnimate.RenderTransform = new TranslateTransform( );
		}

		public void Animate_LoginRibbon_Show( )
		{
			TranslateTransform currentTransform = LoginRibbonAnimate.RenderTransform as TranslateTransform;

			DoubleAnimation daMove = new DoubleAnimation( )
			{
				From = -30,
				To = 0,
				Duration = TimeSpan.FromMilliseconds( 300 ),
				DecelerationRatio = 1,
			};
			
			DoubleAnimation daFade = new DoubleAnimation( )
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds( 200 ),
				BeginTime = TimeSpan.FromMilliseconds( 100 ),
			};

			// Reset the Opacity to 0
			LoginRibbonAnimate.BeginAnimation( Grid.OpacityProperty, null );
			LoginRibbonAnimate.Opacity = 0;

			currentTransform.BeginAnimation( TranslateTransform.YProperty, daMove );
			LoginRibbonAnimate.BeginAnimation( Grid.OpacityProperty, daFade );

			// Force Show the Container for the Ribbon
			LoginRibbon.Visibility = Visibility.Visible;
		}

		public void Animate_LoginRibbon_Hide( )
		{
			TranslateTransform currentTransform = LoginRibbonAnimate.RenderTransform as TranslateTransform;

			DoubleAnimation daMove = new DoubleAnimation( )
			{
				From = 0,
				To = -30,
				Duration = TimeSpan.FromMilliseconds( 300 ),
				AccelerationRatio = 1,
			};

			DoubleAnimation daFade = new DoubleAnimation( )
			{
				From = 1,
				To = 0,
				Duration = TimeSpan.FromMilliseconds( 200 ),
			};

			// Reset the Opacity to 0
			LoginRibbonAnimate.BeginAnimation( Grid.OpacityProperty, null );
			LoginRibbonAnimate.Opacity = 0;

			// When the animation is complete, hide the login ribbon's container
			daFade.Completed += ( object _sender, EventArgs _e ) => LoginRibbon.Visibility = Visibility.Collapsed;

			currentTransform.BeginAnimation( TranslateTransform.YProperty, daMove );
			LoginRibbonAnimate.BeginAnimation( Grid.OpacityProperty, daFade );

			
		}

		#endregion Animations
	}
}
