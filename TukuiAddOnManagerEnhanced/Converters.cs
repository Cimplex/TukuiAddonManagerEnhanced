using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TukuiAddOnManagerEnhanced.Converters
{
	public class WindowsState_NormalMaximize_ToVisibility : IValueConverter
	{
		public WindowsState_NormalMaximize_ToVisibility( ) { }

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return (WindowState) value == WindowState.Normal ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return (Visibility) value == Visibility.Visible ? WindowState.Normal : WindowState.Maximized;
		}
	}

	public class WindowsState_NormalMaximize_ToVisibilityInverse : IValueConverter
	{
		public WindowsState_NormalMaximize_ToVisibilityInverse( ) { }

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return (WindowState) value == WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return (Visibility) value == Visibility.Visible ? WindowState.Maximized : WindowState.Normal;
		}
	}

	public class Boolean_ToVisibility : IValueConverter
	{
		public Boolean_ToVisibility( ) { }

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return (Boolean) value ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return (Visibility) value == Visibility.Visible;
		}
	}
}
