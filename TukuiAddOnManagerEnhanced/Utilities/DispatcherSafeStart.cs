using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TukuiAddOnManagerEnhanced.Utilities
{
	public static class SafeThread
	{
		public static void Start( Action Action )
		{
			if ( Application.Current != null )
				Application.Current.Dispatcher.BeginInvoke( new ThreadStart( Action ) );
		}

	}
}
