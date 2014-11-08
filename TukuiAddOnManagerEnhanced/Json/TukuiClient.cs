using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TukuiAddOnManagerEnhanced.Json;
using TukuiAddOnManagerEnhanced.Utilities;

namespace TukuiAddOnManagerEnhanced.Json
{
	public class TukuiClient : SafeDependencyObject
	{
		// Dependency Property that is Thread Safe
		public GenericDependencyProperty<TukuiClient, TukuiUser> CurrentUser;

		public TukuiClient( )
		{
			CurrentUser = new GenericDependencyProperty<TukuiClient, TukuiUser>( this, "CurrentUser", null );
		}

		public void BeginLogin( String Username, String Password, TukuiActionCallback Callback )
		{
			Thread thread = new Thread( new ThreadStart( ( ) =>
			{
				//http://tukui.org/api.php?username=DrPepper&password=ItTasteSoGoodHihi

				using( WebClient client = new WebClient() )
				{
					Uri myUrl = new Uri( "http://tukui.org/api.php?username="
						+ System.Net.WebUtility.UrlEncode( Username )
						+ "&password="
						+ System.Net.WebUtility.UrlEncode( Password ) );

					string jsonResult = client.DownloadString( myUrl.AbsoluteUri );
					
				}

				
			} ) );
			thread.Start( );
		}

		public void Logout( )
		{

		}

		#region SafeDependencyObject for a safe init
		
		public static void DispatcherInit( )
		{
			TukuiClient client = new TukuiClient( );
			client.Noop( );
		}

		// This is to prevent the compiler optimizing out my Init
		private void Noop( ) { }

		#endregion SafeDependencyObject for a safe init

	}
}
