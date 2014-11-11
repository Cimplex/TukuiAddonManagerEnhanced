using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

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


	#region MahMetro.App Converters

	public class BackgroundToForegroundConverter : IValueConverter, IMultiValueConverter
	{
		private static BackgroundToForegroundConverter _instance;
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static BackgroundToForegroundConverter( )
		{
		}
		private BackgroundToForegroundConverter( )
		{
		}
		public static BackgroundToForegroundConverter Instance
		{
			get { return _instance ?? ( _instance = new BackgroundToForegroundConverter( ) ); }
		}
		/// <summary>
		/// Determining Ideal Text Color Based on Specified Background Color
		/// http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
		/// </summary>
		/// <param name = "bg">The bg.</param>
		/// <returns></returns>
		private Color IdealTextColor( Color bg )
		{
			const int nThreshold = 105;
			var bgDelta = System.Convert.ToInt32( ( bg.R * 0.299 ) + ( bg.G * 0.587 ) + ( bg.B * 0.114 ) );
			var foreColor = ( 255 - bgDelta < nThreshold ) ? Colors.Black : Colors.White;
			return foreColor;
		}
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value is SolidColorBrush )
			{
				var idealForegroundColor = this.IdealTextColor( ( (SolidColorBrush) value ).Color );
				var foreGroundBrush = new SolidColorBrush( idealForegroundColor );
				foreGroundBrush.Freeze( );
				return foreGroundBrush;
			}
			return Brushes.White;
		}
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return DependencyProperty.UnsetValue;
		}
		public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
		{
			var bgBrush = values.Length > 0 ? values[ 0 ] as Brush : null;
			var titleBrush = values.Length > 1 ? values[ 1 ] as Brush : null;
			if ( titleBrush != null )
			{
				return titleBrush;
			}
			return Convert( bgBrush, targetType, parameter, culture );
		}
		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
		{
			return targetTypes.Select( t => DependencyProperty.UnsetValue ).ToArray( );
		}
	}

	// this converter is only used by DatePicker to convert the font size to width and height of the icon button
	public class FontSizeOffsetConverter : IValueConverter
	{
		private static FontSizeOffsetConverter _instance;

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static FontSizeOffsetConverter( )
		{
		}

		private FontSizeOffsetConverter( )
		{
		}

		public static FontSizeOffsetConverter Instance
		{
			get { return _instance ?? ( _instance = new FontSizeOffsetConverter( ) ); }
		}

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value is double && parameter is double )
			{
				var offset = (double) parameter;
				var orgValue = (double) value;
				return Math.Round( orgValue + offset );
			}
			return value;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return DependencyProperty.UnsetValue;
		}
	}

	/// <summary>
	/// Converts the value from true to false and false to true.
	/// </summary>
	public sealed class IsNullConverter : IValueConverter
	{
		private static IsNullConverter _instance;
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static IsNullConverter( )
		{
		}
		private IsNullConverter( )
		{
		}
		public static IsNullConverter Instance
		{
			get { return _instance ?? ( _instance = new IsNullConverter( ) ); }
		}
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return null == value;
		}
		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return Binding.DoNothing;
		}
	}

	[MarkupExtensionReturnType( typeof( IValueConverter ) )]
	public abstract class MarkupConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue( IServiceProvider serviceProvider )
		{
			return this;
		}
		protected abstract object Convert( object value, Type targetType, object parameter, CultureInfo culture );
		protected abstract object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture );
		object IValueConverter.Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			try
			{
				return Convert( value, targetType, parameter, culture );
			}
			catch
			{
				return DependencyProperty.UnsetValue;
			}
		}
		object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			try
			{
				return ConvertBack( value, targetType, parameter, culture );
			}
			catch
			{
				return DependencyProperty.UnsetValue;
			}
		}
	}

	public class MetroTabItemCloseButtonWidthConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return System.Convert.ToInt32( value ) * 0.5;
		}
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return DependencyProperty.UnsetValue;
		}
	}

	public sealed class ResizeModeMinMaxButtonVisibilityConverter : IMultiValueConverter
	{
		private static ResizeModeMinMaxButtonVisibilityConverter _instance;
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static ResizeModeMinMaxButtonVisibilityConverter( )
		{
		}
		private ResizeModeMinMaxButtonVisibilityConverter( )
		{
		}
		public static ResizeModeMinMaxButtonVisibilityConverter Instance
		{
			get { return _instance ?? ( _instance = new ResizeModeMinMaxButtonVisibilityConverter( ) ); }
		}
		public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
		{
			if ( values != null && values.Length == 2 && parameter is string )
			{
				var windowPropValue = (bool) values[ 0 ];
				var windowResizeMode = (ResizeMode) values[ 1 ];
				var whichButton = (string) parameter;
				switch ( windowResizeMode )
				{
					case ResizeMode.NoResize:
						return Visibility.Collapsed;
					case ResizeMode.CanMinimize:
						if ( whichButton == "MIN" )
						{
							return windowPropValue ? Visibility.Visible : Visibility.Collapsed;
						}
						return Visibility.Collapsed;
					case ResizeMode.CanResize:
					case ResizeMode.CanResizeWithGrip:
					default:
						return windowPropValue ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return Visibility.Visible;
		}
		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
		{
			return targetTypes.Select( t => DependencyProperty.UnsetValue ).ToArray( );
		}
	}

	public class ThicknessToDoubleConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var thickness = (Thickness) value;
			return thickness.Left;
		}
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return DependencyProperty.UnsetValue;
		}
	}

	public class ToUpperConverter : MarkupConverter
	{
		protected override object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var val = value as string;
			return val != null ? val.ToUpper( ) : value;
		}
		protected override object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return Binding.DoNothing;
		}
	}

	public class ToLowerConverter : MarkupConverter
	{
		protected override object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var val = value as string;
			return val != null ? val.ToLower( ) : value;
		}
		protected override object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return Binding.DoNothing;
		}
	}

	public class TreeViewMarginConverter : IValueConverter
	{
		public double Length { get; set; }
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			var item = value as TreeViewItem;
			if ( item == null )
				return new Thickness( 0 );
			return new Thickness( Length * item.GetDepth( ), 0, 0, 0 );
		}
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return DependencyProperty.UnsetValue;
		}
	}

	public static class TreeViewItemExtensions
	{
		public static int GetDepth( this TreeViewItem item )
		{
			TreeViewItem parent;
			while ( ( parent = GetParent( item ) ) != null )
			{
				return GetDepth( parent ) + 1;
			}
			return 0;
		}
		private static TreeViewItem GetParent( TreeViewItem item )
		{
			var parent = item != null ? VisualTreeHelper.GetParent( item ) : null;
			while ( parent != null && !( parent is TreeViewItem || parent is TreeView ) )
			{
				parent = VisualTreeHelper.GetParent( parent );
			}
			return parent as TreeViewItem;
		}
	}

	#endregion MahMetro.App Converters
}
