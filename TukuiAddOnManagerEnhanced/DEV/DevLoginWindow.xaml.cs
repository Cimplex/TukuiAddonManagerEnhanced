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
using System.Windows.Shapes;

using Newtonsoft.Json;

using TukuiAddOnManagerEnhanced.Json;
using TukuiAddOnManagerEnhanced.Utilities;

namespace TukuiAddOnManagerEnhanced.DEV
{
	/// <summary>
	/// Interaction logic for DevLoginWindow.xaml
	/// </summary>
	public partial class DevLoginWindow : Window
	{
		TukuiClient client;

		public DevLoginWindow( )
		{
			InitializeComponent( );

			client = new TukuiClient( );
			this.DataContext = client;
		}

		private void Button_Click( object sender, RoutedEventArgs e )
		{
			client.BeginLogin( UserName.Text, Password.Text, TukuiActionCallback_Handler );
		}

		private void TukuiActionCallback_Handler( object sender, TukuiActionCallbackEventArgs e ) { }
	}
}
