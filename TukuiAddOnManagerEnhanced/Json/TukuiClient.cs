using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using TukuiAddOnManagerEnhanced.Json;
using TukuiAddOnManagerEnhanced.Utilities;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;

namespace TukuiAddOnManagerEnhanced.Json
{
	public class TukuiClient : SafeDependencyObject
	{
		// Dependency Property that is Thread Safe
		public GenericDependencyProperty<TukuiClient, TukuiUser> CurrentUser;
		public GenericDependencyProperty<TukuiClient, ImageSource> UserThumbnail;

		public TukuiClient( )
		{
			CurrentUser = new GenericDependencyProperty<TukuiClient, TukuiUser>( this, "CurrentUser", null );
			UserThumbnail = new GenericDependencyProperty<TukuiClient, ImageSource>( this, "UserThumbnail", null );
		}

		public void BeginLogin( String Username, String Password, TukuiActionCallback Callback, object source = null )
		{
			ThreadPool.QueueUserWorkItem(
				o => {
					//http://tukui.org/api.php?username=DrPepper&password=ItTasteSoGoodHihi

					TukuiActionCallbackEventArgs args = new TukuiActionCallbackEventArgs( );
					try
					{
						using ( WebClient client = new WebClient( ) )
						{
							Uri myUrl = new Uri( "http://tukui.org/api.php?username="
								+ System.Net.WebUtility.UrlEncode( Username )
								+ "&password="
								+ System.Net.WebUtility.UrlEncode( Password ) );

							string jsonResult = client.DownloadString( myUrl.AbsoluteUri );
							List<TukuiUser> user = JsonConvert.DeserializeObject<List<TukuiUser>>( jsonResult );
							TukuiUser tempUser = user[ 0 ];

							if ( tempUser.status == "OK" )
							{
								UpdateImage( tempUser.avatar );
								CurrentUser.SafeValue = tempUser;
							}
							else
							{
								args.Error = TukuiExceptions.WrongUsernameOrPassword;
							}
						}
					}
					catch( Exception ex )
					{
						args.Error = ex;
					}
					finally
					{
						Callback( source ?? this, args );
					}
				} );
		}

		private void UpdateImage( string imageURL )
		{
			var webClient = new WebClient( );
			var url = new Uri( imageURL, UriKind.Absolute );
			var buffer = webClient.DownloadData( url );
			var bitmapImage = new BitmapImage( );

			using ( var stream = new MemoryStream( buffer ) )
			{
				bitmapImage.BeginInit( );
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = stream;
				bitmapImage.EndInit( );
				bitmapImage.Freeze( );
			}

			UserThumbnail.SafeValue = bitmapImage;
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
