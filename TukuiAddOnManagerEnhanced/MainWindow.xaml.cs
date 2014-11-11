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
		private WindowChrome MyChrome;
		private TukuiClient MyClient;

		#region Constructors (Inits)

		public MainWindow( )
		{
			InitializeComponent( );

			// VMMV - The View (Window) is its own Model (making it a ViewModel)
			DataContext = this;

			// Get ready for a borderless Window
			MyChrome = new WindowChrome( );
			MyChrome.ResizeBorderThickness = new Thickness( 12 );
			WindowChrome.SetWindowChrome( this, MyChrome );

			// Register Window Events
			this.Loaded += MainWindow_Loaded;
			this.StateChanged += MainWindow_StateChanged;
			this.LocationChanged += MainWindow_LocationChanged;
			this.SizeChanged += MainWindow_SizeChanged;
			this.Activated += MainWindow_ActivatedChange;
			this.Deactivated += MainWindow_ActivatedChange;


			// Register UI Controls Events

			// Init Animations
			InitAnimations( );
		}

		private void MainWindow_ActivatedChange( object sender, EventArgs e )
		{
			ColorAnimation caAnimate = new ColorAnimation( )
			{
				Duration = TimeSpan.FromMilliseconds(60),
			};

			SolidColorBrush currentBrush = VisualLayout.Background as SolidColorBrush;

			if ( this.IsActive )
				caAnimate.To = Color.FromArgb( 0xFF, 0x2E, 0x2E, 0x2E );
			else
				caAnimate.To = Color.FromArgb( 0xFF, 0x4B, 0x4B, 0x4B );

			currentBrush.BeginAnimation( SolidColorBrush.ColorProperty, caAnimate );
		}

		#endregion Constructors (Inits)

		#region Window Event Handlers

		private void MainWindow_StateChanged( object sender, EventArgs e )
		{
			// Windowless borders do not have this code to force the window in the working area
			if ( this.WindowState == WindowState.Maximized )
			{
				RootLayout.Margin = new Thickness( 0 );

				// Check the Working Size and make sure the window does not get too big
				WPFMonitor monitor = this.CurrentMonitor( );
				MonitorInfo monitorInfo = monitor.GetMonitorInfo( );
				this.ForceWindowMove( monitorInfo.WorkingArea );

				MyChrome.ResizeBorderThickness = new Thickness( 0 );
			}
			else
			{
				RootLayout.Margin = new Thickness( );
				MyChrome.ResizeBorderThickness = new Thickness( 12 );
			}
		}

		private void MainWindow_LocationChanged( object sender, EventArgs e )
		{
			// So this logic mess is basically to hide the shadow when you use aerosnap... would love to know of a better way if you got one...
			WPFMonitor monitor = this.CurrentMonitor( );
			MonitorInfo monitorInfo = monitor.GetMonitorInfo( );
			Rect workingArea = monitorInfo.WorkingArea;

			if ( this.WindowState == WindowState.Normal )
			{
				if ( this.Left == workingArea.Width / 2 )
				{
					RootLayout.Margin = new Thickness( 8, 0, 0, 0 );
					MyChrome.ResizeBorderThickness = new Thickness( 12, 0, 0, 0 );
				}
				else if( this.Left == 0 )
				{
					RootLayout.Margin = new Thickness( 0, 0, 8, 0 );
					MyChrome.ResizeBorderThickness = new Thickness( 0, 0, 12, 0 );
				}
				else
				{
					RootLayout.Margin = new Thickness( 8, RootLayout.Margin.Top, 8, RootLayout.Margin.Bottom );
					MyChrome.ResizeBorderThickness = new Thickness( 12, MyChrome.ResizeBorderThickness.Top, 12, MyChrome.ResizeBorderThickness.Bottom );
				}
			}
		}

		private void MainWindow_SizeChanged( object sender, SizeChangedEventArgs e )
		{
			// So this logic mess is basically to hide the shadow when you use aerosnap... would love to know of a better way if you got one...
			WPFMonitor monitor = this.CurrentMonitor( );
			MonitorInfo monitorInfo = monitor.GetMonitorInfo( );
			Rect workingArea = monitorInfo.WorkingArea;

			if ( this.WindowState == WindowState.Normal )
			{
				if ( this.ActualWidth == workingArea.Width / 2 )
				{
					if ( this.ActualHeight != workingArea.Height )
					{
						if ( this.Left == workingArea.Width / 2 )
						{
							RootLayout.Margin = new Thickness( 8, 8, 8, 0 );
							MyChrome.ResizeBorderThickness = new Thickness( 12, 12, 12, 0 );
						}
						else if ( this.Left == 0 )
						{
							RootLayout.Margin = new Thickness( 0, 8, 8, 8 );
							MyChrome.ResizeBorderThickness = new Thickness( 0, 12, 12, 12 );
						}
						else
						{
							RootLayout.Margin = new Thickness( 8 );
							MyChrome.ResizeBorderThickness = new Thickness( 12 );
						}
					}
					else if ( this.Left == workingArea.Width / 2 )
					{
						RootLayout.Margin = new Thickness( 8, 0, 0, 0 );
						MyChrome.ResizeBorderThickness = new Thickness( 12, 0, 0, 0 );
					}
					else
					{
						RootLayout.Margin = new Thickness( 0, 0, 8, 0 );
						MyChrome.ResizeBorderThickness = new Thickness( 0, 0, 12, 0 );
					}
				}
				else
				{
					RootLayout.Margin = new Thickness( 8 );
					MyChrome.ResizeBorderThickness = new Thickness( 12 );
				}
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
							Clicked_Refresh( );
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

		bool updateVisible = false;
		private void Clicked_Refresh( )
		{
			if ( updateVisible )
			{
				this.Title = "Tukui: 5 updates.";
				Animate_UpdateText_Show( );
			}
			else
			{
				this.Title = "Tukui";
				Animate_UpdateText_Hide( );
			}
			updateVisible = !updateVisible;
		}

		#endregion Sub Menu Buttons - Logic

		#region Animations

		public void InitAnimations( )
		{
			LoginRibbonAnimate.RenderTransform = new TranslateTransform( );
			UpdateText.RenderTransform = new TranslateTransform( );
		}

		public void Animate_LoginRibbon_Show( )
		{
			TranslateTransform currentTransform = LoginRibbonAnimate.RenderTransform as TranslateTransform;

			DoubleAnimation daMove = new DoubleAnimation( )
			{
				From = -60,
				To = -50,
				Duration = TimeSpan.FromMilliseconds( 300 ),
				DecelerationRatio = 1,
			};
			
			DoubleAnimation daFade = new DoubleAnimation( )
			{
				From = 0,
				To = 1,
				Duration = TimeSpan.FromMilliseconds( 100 ),
				BeginTime = TimeSpan.FromMilliseconds( 10 ),
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
				From = -50,
				To = -60,
				Duration = TimeSpan.FromMilliseconds( 300 ),
				AccelerationRatio = 1,
			};

			DoubleAnimation daFade = new DoubleAnimation( )
			{
				From = 1,
				To = 0,
				Duration = TimeSpan.FromMilliseconds( 100 ),
				BeginTime = TimeSpan.FromMilliseconds( 200 ),
				AccelerationRatio = 1,
			};

			// When the animation is complete, hide the login ribbon's container
			daFade.Completed += ( object _sender, EventArgs _e ) => LoginRibbon.Visibility = Visibility.Collapsed;

			currentTransform.BeginAnimation( TranslateTransform.YProperty, daMove );
			LoginRibbonAnimate.BeginAnimation( Grid.OpacityProperty, daFade );

			
		}

		public void Animate_UpdateText_Show( )
		{
			TranslateTransform currentTransform = UpdateText.RenderTransform as TranslateTransform;

			currentTransform.BeginAnimation( TranslateTransform.XProperty, null );
			currentTransform.X = 0;

			DoubleAnimation daMove = new DoubleAnimation( )
			{
				From = 8,
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

			currentTransform.BeginAnimation( TranslateTransform.YProperty, daMove );
			UpdateText.BeginAnimation( Grid.OpacityProperty, daFade );

			// Force Show the Container for the Ribbon
			UpdateText.Visibility = Visibility.Visible;
		}

		public void Animate_UpdateText_Hide( )
		{
			TranslateTransform currentTransform = UpdateText.RenderTransform as TranslateTransform;

			currentTransform.BeginAnimation( TranslateTransform.YProperty, null );
			currentTransform.Y = 0;

			DoubleAnimation daMove = new DoubleAnimation( )
			{
				From = 0,
				To = -200,
				Duration = TimeSpan.FromMilliseconds( 300 ),
				AccelerationRatio = 1,
			};

			DoubleAnimation daFade = new DoubleAnimation( )
			{
				From = 1,
				To = 0,
				BeginTime = TimeSpan.FromMilliseconds( 100 ),
				Duration = TimeSpan.FromMilliseconds( 200 ),
			};


			// When the animation is complete, hide the login ribbon's container
			daFade.Completed += ( object _sender, EventArgs _e ) => UpdateText.Visibility = Visibility.Collapsed;

			currentTransform.BeginAnimation( TranslateTransform.XProperty, daMove );
			UpdateText.BeginAnimation( Grid.OpacityProperty, daFade );
		}


		#endregion Animations

		private void buttonCancelLogin_Click( object sender, RoutedEventArgs e )
		{
			Clicked_Logout( );
		}
	}
}
