using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TukuiAddOnManagerEnhanced.Utilities
{
	public class TukuiActionCallbackEventArgs : EventArgs
	{
		public Exception Error
		{
			get;
			set;
		}

		public TukuiActionCallbackEventArgs( ) { }

		public TukuiActionCallbackEventArgs( Exception Error )
		{
			this.Error = Error;
		}
	}

	public delegate void TukuiActionCallback( object sender, TukuiActionCallbackEventArgs e );
}
