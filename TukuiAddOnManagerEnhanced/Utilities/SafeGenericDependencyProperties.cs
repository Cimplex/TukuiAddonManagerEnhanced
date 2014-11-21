using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TukuiAddOnManagerEnhanced.Utilities
{
	/// <summary>
	/// This is a special args set that combines the important parts of RoutedEventArgs (Handled) with DependencyPropertyChangedEventArgs
	/// </summary>
	public class RoutedDependencyPropertyChangedEventArgs : EventArgs
	{
		public Boolean Handled { get; set; }
		public Object OldValue { get; set; }
		public Object NewValue { get; set; }
		public DependencyProperty Property { get; set; }
	}

	public delegate void RoutedPropertyChangedCallback( DependencyObject sender, RoutedDependencyPropertyChangedEventArgs e );

	public class SafeDependencyObject : DependencyObject
	{
		internal Dictionary<String, Boolean> IsUpdatingSafeValue = new Dictionary<string, Boolean>( );
		private Dictionary<String, Object> SafeDependencyValues = new Dictionary<string, object>( );
		internal Dictionary<DependencyProperty, List<EventHandler<RoutedDependencyPropertyChangedEventArgs>>> EventList = new Dictionary<DependencyProperty, List<EventHandler<RoutedDependencyPropertyChangedEventArgs>>>( );


		internal void SetSafeValue( string Name, object Value )
		{
			lock ( SafeDependencyValues )
			{
				if ( SafeDependencyValues.ContainsKey( Name ) )
				{
					SafeDependencyValues[ Name ] = Value;
				}
				else
				{
					SafeDependencyValues.Add( Name, Value );
				}
			}
		}

		internal object GetSafeValue( string Name )
		{
			return SafeDependencyValues[ Name ];
		}

		internal void AddEventHandle( DependencyProperty property, EventHandler<RoutedDependencyPropertyChangedEventArgs> handle )
		{
			lock ( EventList )
			{
				if ( EventList.ContainsKey( property ) )
				{
					EventList[ property ].Add( handle );
				}
				else
				{
					EventList.Add( property, new List<EventHandler<RoutedDependencyPropertyChangedEventArgs>>( ) );
					EventList[ property ].Add( handle );
				}
			}
		}

		internal void RemoveEventHandle( DependencyProperty property, EventHandler<RoutedDependencyPropertyChangedEventArgs> handle )
		{
			lock ( EventList )
			{
				if ( EventList.ContainsKey( property ) )
				{
					EventList[ property ].Remove( handle );
				}
			}
		}
	}

	public sealed class GenericDependencyProperty<TParent, TValue>
		where TParent : SafeDependencyObject
	{
		private static Dictionary<Type, Dictionary<String, DependencyProperty>> RegisteredTypes = new Dictionary<Type, Dictionary<string, DependencyProperty>>( );
		private TParent MyParent;

		/// <summary>
		/// See's if you are allowed to write this dependency property.
		/// </summary>
		//public Boolean IsReadOnly { get; private set; }

		// The type that declared it as being readonly
		private Type ReadOnlyDeclaringType { get; set; }

		/// <summary>
		/// The dependency property which is guaranteed to be the same for similar types.
		/// </summary>
		public DependencyProperty Property { get; private set; }

		/// <summary>
		/// The non-thread-safe value of the dependency property.
		/// </summary>
		public TValue DefaultValue { get; private set; }

		/// <summary>
		/// The name of this dependency property.
		/// </summary>
		public String PropertyName { get; private set; }

		public GenericDependencyProperty( TParent MyParent, String PropertyName, TValue DefaultValue )
		{
			this.MyParent = MyParent;
			this.PropertyName = PropertyName;
			this.DefaultValue = DefaultValue;
			this.cacheValue = DefaultValue;

			MyParent.SetSafeValue( PropertyName, DefaultValue );

			if ( !RegisteredTypes.ContainsKey( typeof( TParent ) ) )
			{
				RegisteredTypes[ typeof( TParent ) ] = new Dictionary<string, DependencyProperty>( );
			}

			if ( !RegisteredTypes[ typeof( TParent ) ].ContainsKey( PropertyName ) )
			{
				Property = DependencyProperty.Register( PropertyName, typeof( TValue ), typeof( TParent ), new PropertyMetadata( DefaultValue, OnValuePropertyChanged ) );
				RegisteredTypes[ typeof( TParent ) ][ PropertyName ] = Property;
			}
			else
			{
				Property = RegisteredTypes[ typeof( TParent ) ][ PropertyName ];
			}
		}

		// Read only constructor
		public GenericDependencyProperty( TParent MyParent, String PropertyName, TValue DefaultValue, Boolean IsReadOnly )
			: this( MyParent, PropertyName, DefaultValue )
		{
			//this.IsReadOnly = IsReadOnly;

			// Only allow this Type to change the value now
			//StackTrace stackTrace = new StackTrace( );
			//this.ReadOnlyDeclaringType = stackTrace.GetFrame( 1 ).GetMethod( ).DeclaringType;

		}

		// a IDisposable object that needs to be disposed when the Dispatcher sets the dependency property
		private TValue cacheValue;

		/// <summary>
		/// Allows you to infrequently set the Value safely from another thread.
		/// </summary>
		public TValue SafeValue
		{
			get { return (TValue) MyParent.GetSafeValue( PropertyName ); }
			set
			{
				IDisposable oldValue = null;

#if DEBUG
				// Should be able to catch errors when debuging
				/*if ( this.IsReadOnly )
				{
					StackTrace stackTrace = new StackTrace( );
					if ( this.ReadOnlyDeclaringType != stackTrace.GetFrame( 1 ).GetMethod( ).DeclaringType )
						throw new TypeAccessException( "This property ('" + this.PropertyName + "') was declared as ReadOnly by type: '" + ReadOnlyDeclaringType.ToString( ) + "' Only this type can write to it." );
				}*/
#endif

				// Check to see if we are dealing with a IDisposable class
				if ( typeof( IDisposable ).IsAssignableFrom( typeof( TValue ) ) )
				{
					if ( ( DefaultValue == null && cacheValue == null ) || cacheValue.Equals( DefaultValue ) )
						// Cache the old value until the dispatcher can change it
						cacheValue = SafeValue;
					else
						// We need to dispose of this value
						oldValue = SafeValue as IDisposable;
				}

				MyParent.SetSafeValue( PropertyName, value );

				if ( oldValue is IDisposable ) oldValue.Dispose( );

				lock ( MyParent.IsUpdatingSafeValue )
				{
					if ( !MyParent.IsUpdatingSafeValue.ContainsKey( PropertyName ) )
						MyParent.IsUpdatingSafeValue.Add( PropertyName, false );

					if ( !MyParent.IsUpdatingSafeValue[ PropertyName ] && Application.Current != null )
					{
						MyParent.IsUpdatingSafeValue[ PropertyName ] = true;
						Application.Current.Dispatcher.BeginInvoke( new ThreadStart( ( ) =>
						{

							MyParent.SetValue( Property, SafeValue );
							MyParent.IsUpdatingSafeValue[ PropertyName ] = false;

							if ( cacheValue is IDisposable )
							{
								( cacheValue as IDisposable ).Dispose( );
								cacheValue = DefaultValue;
							}
						} ) );
					}
				}
			}
		}

		public TValue Value
		{
			get { return (TValue) MyParent.GetValue( Property ); }
			set { MyParent.SetValue( Property, value ); }
		}

		public Boolean HasValue
		{
			get
			{
				object value = MyParent.GetValue( Property );
				
				if ( value == null )
					return DefaultValue != null;

				return !value.Equals( DefaultValue );
			}
		}

		public Boolean HasSafeValue
		{
			get { return !MyParent.GetSafeValue( PropertyName ).Equals( DefaultValue ); }
		}

		private static void OnValuePropertyChanged( DependencyObject DependencyObject, DependencyPropertyChangedEventArgs e )
		{
			TParent currentObject = (TParent) DependencyObject;
			TValue currentValue = (TValue) e.NewValue;

			// Update our "Thread Safe" property
			currentObject.SetSafeValue( e.Property.Name, currentValue );
			currentObject.IsUpdatingSafeValue[ e.Property.Name ] = false;

			// Create a useful set of args
			RoutedDependencyPropertyChangedEventArgs args = new RoutedDependencyPropertyChangedEventArgs( )
			{
				Handled = false,
				NewValue = e.NewValue,
				OldValue = e.OldValue,
				Property = e.Property,
			};

			// Call this properties changed event
			if ( currentObject.EventList.ContainsKey( e.Property ) )
			{
				// Create a temporary list of event handlers incase one of them unregisters themselves on the dispatcher thread (which will bypass the locks).
				List<EventHandler<RoutedDependencyPropertyChangedEventArgs>> temp = new List<EventHandler<RoutedDependencyPropertyChangedEventArgs>>( currentObject.EventList[ e.Property ] );

				IEnumerator<EventHandler<RoutedDependencyPropertyChangedEventArgs>> iter = temp.GetEnumerator( );

				while ( iter.MoveNext( ) && args.Handled == false )
				{
					iter.Current.DynamicInvoke( currentObject, args );
				}
			}
		}

		public void RegisterEventHandle( EventHandler<RoutedDependencyPropertyChangedEventArgs> handle )
		{
			this.MyParent.AddEventHandle( this.Property, handle );
		}

		public void UnregisterEventHandle( EventHandler<RoutedDependencyPropertyChangedEventArgs> handle )
		{
			this.MyParent.RemoveEventHandle( this.Property, handle );
		}

		private string ValueToString( object value )
		{
			if ( value == null )
				return "Null";
			else
				return value.ToString( );
		}

		public override string ToString( )
		{
			return "GenericDependencyProperty: Parent='" + typeof( TParent ).ToString( ) + "', Type='" + typeof( TValue ).ToString( ) + "', Value='" + ValueToString( Value ) + "', SafeValue='" + ValueToString( SafeValue ) + "'";
		}
	}
}
