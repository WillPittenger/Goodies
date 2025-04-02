// Ignore Spelling: evt jso

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

/// <summary>
/// Provides a means to access some aspects of <see cref="Obj{TypeOfObj}"/> even if you don't know the type parameters.
/// </summary>
[System.Text.Json.Serialization.JsonSourceGenerationOptions(GenerationMode = System.Text.Json.Serialization
	.JsonSourceGenerationMode.Metadata | System.Text.Json.Serialization.JsonSourceGenerationMode
	.Serialization, IgnoreReadOnlyFields = true, IgnoreReadOnlyProperties = true, WriteIndented = true)]
public abstract class ObjBase : System.ComponentModel.INotifyPropertyChanged
{
	/// <summary>
	/// Constructs a new <see cref="ObjBase"/> instance.
	/// </summary>
	protected ObjBase(System.Guid guid = default)
	{
		this.guid = guid;
		mapAllInstances[guid] = this;
	}

	/// <summary>
	/// Removes this instance from the list of all instances.
	/// </summary>
	~ObjBase()
		=> mapAllInstances.Remove(guid);

	/// <summary>
	/// The <see cref="System.Guid"/> that uniquely identifies this <see cref="ObjBase"/> instance.
	/// </summary>
	public readonly System.Guid guid;

	public System.Guid GUID
		=> guid;

	public const string strAllFileTypesExt = @".*";

	/// <summary>
	/// Stores a list of all <see cref="ObjBase"/> instances indexed by their <see cref="System.Guid"/>.
	/// </summary>
	private static readonly System.Collections.Generic.SortedDictionary<System.Guid, ObjBase> mapAllInstances
		= [];

	/// <summary>
	/// Returns a readonly dictionary of all <see cref="ObjBase"/> instances indexed by their <see cref="System.Guid"/>.  Derived classes should provide their
	/// own version that typecasts the entries to the derived class.
	/// </summary>
	public static System.Collections.Generic.IReadOnlyDictionary<System.Guid, ObjBase> AllInstancesByGUID
		=> mapAllInstances;

	/// <summary>
	/// Provides a way to implicitly convert from a <see cref="ObjBase"/> of any derivation to its <see cref="System.Guid"/>.
	/// </summary>
	/// <param name="obj">The <see cref="ObjBase"/> to convert</param>
	public static implicit operator System.Guid(in ObjBase obj)
		=> obj.guid;

	/// <summary>
	/// Declares an explicit type cast operator that can attempt to convert any <see cref="System.Guid"/> to a <see cref="ObjBase"/> instance.  If no <see
	/// cref="ObjBase"/> exists for the value specified by <paramref name="guidToLookup"/>, <see langword="null"/> will be returned.
	/// </summary>
	/// <param name="guidToLookup">Which <see cref="System.Guid"/> to lookup</param>
	public static explicit operator ObjBase?(in System.Guid guidToLookup)
		=> mapAllInstances.TryGetValue(guidToLookup, out ObjBase? value)
			? value
			: null;

	/// <summary>
	/// Causes the hash code associated by <see cref="object"/> to be the hash code for <see cref="guid"/>.
	/// </summary>
	/// <returns>The required hash code</returns>
	public override int GetHashCode()
		=> guid.GetHashCode();

	public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

	protected void FirePropChanged(in string strWhichPropChanged)
		=> PropertyChanged?.Invoke(this, new(strWhichPropChanged));
}

/// <summary>
/// This is meant to be the base class of all data types in this system.  It provides the ability to be marked dirty as well as some other functions.
///  (Technically, this is derived from <see cref="ObjBase"/> and <see cref="object"/>.)
/// </summary>
/// <typeparam name="TypeOfObj">Any type derived from <see cref="Obj{TypeOfObj}"/></typeparam>
public abstract class Obj<TypeOfObj> : ObjBase
	where TypeOfObj : Obj<TypeOfObj>
{
	#region Constructors & Deconstructors
		/// <summary>
		/// Constructs a new <see cref="Obj{TypeOfObj}"/> by adding this to the list of all instances.
		/// </summary>
		protected Obj(System.Guid guid = default) :
			base(guid)
			=> mapAllInstances[guid] = this;

		/// <summary>
		/// Removes this instance of <see cref="Obj{TypeOfObj}"/> from the list of all instances
		/// </summary>
		~Obj()
			=> mapAllInstances.Remove(guid);
	#endregion

	#region Delegates
		/// <summary>
		/// Provides a way to notify interested callers that an instance of <see cref="Obj{TypeOfObj}"/> either became dirty or was dirty but no longer is
		/// </summary>
		/// <param name="objSender">The object that sent the notification</param>
		/// <param name="bIsNowDirty">The new state</param>
		public delegate void DDirtyChanged(in TypeOfObj objSender, in bool bIsNowDirty);

		/// <summary>
		/// Provides a way to notify interested callers that a field has changed values
		/// </summary>
		/// <typeparam name="FieldType">The type of the field</typeparam>
		/// <param name="objSender">The sender of this notification</param>
		/// <param name="oldVal">The old value of the field</param>
		/// <param name="newVal">The new value of the field</param>
		public delegate void DFieldChanged<FieldType>(in TypeOfObj objSender, in FieldType oldVal, in FieldType newVal);

		/// <summary>
		/// Like <see cref="DFieldChanged{FieldType}"/>, but for <see cref="bool"/> fields.  This omits the old value as it's always <c>!<paramref
		/// name="bNewVal"/></c>.
		/// </summary>
		/// <param name="objSender">The sender of this notification</param>
		/// <param name="bNewVal">The new value</param>
		public delegate void DBoolFieldChanged(in TypeOfObj objSender, in bool bNewVal);

		/// <summary>
		/// Provides a way to notify interested callers that a collection has changed in some way.  Details of how these
		/// parameters are used are up to the code that implements the event.  If the collection is a map (or dictionary),
		/// use DMapFieldChanged instead.
		/// </summary>
		/// <typeparam name="CollectionType">The type of the collection.  This can be any collection type, but must be a
		/// collection of <typeparamref name="ItemType"/>.</typeparam>
		/// <typeparam name="ItemType">The type of element in the collection.</typeparam>
		/// <param name="objSender">The sender of this notification</param>
		/// <param name="collectionThatChanged">Which collection changed</param>
		/// <param name="eNewEntries">A collection of entries that were added to the collection or <see landword="null"/>
		/// if there are no new entries.</param>
		/// <param name="eRemovedEntries">A collection of entries that were removed from the collection or <see
		/// langword="null"/> if no entries were removed.</param>
		/// <param name="eRelocatedItems">A collection of entries that were moved in the collection or <see
		/// langword="null"/> if no items were moved.  This would typically mean the items received a new index or position
		/// in a list.</param>
		public delegate void DCollectionFieldChanged<CollectionType, ItemType>(in TypeOfObj objSender, in CollectionType
				collectionThatChanged, in System.Collections.Generic.IEnumerable<ItemType>? eNewEntries = null, in System
				.Collections.Generic.IEnumerable<ItemType>? eRemovedEntries = null, in System.Collections.Generic
				.IEnumerable<ItemType>? eRelocatedItems = null)
			where CollectionType : System.Collections.Generic.IReadOnlyCollection<ItemType>;

		/// <summary>
		/// THis is like <see cref="DCollectionFieldChanged{CollectionType,ItemType}"/> but specific to maps AKA
		/// dictionaries.  Unlike <see cref="DCollectionFieldChanged{CollectionType,ItemType}"/>, <see
		/// cref="DMapFieldChanged{MapType,KeyType,ElementType}"/> provides a way to list changes in the key for an entry in
		/// the map.
		/// </summary>
		/// <typeparam name="MapType">The type of map; Must be a
		/// <c>System.Collections.Generic.IReadOnlyDictionary&lt;KeyType, ElementType&lt;</c></typeparam>
		/// <typeparam name="KeyType">The key for the map</typeparam>
		/// <typeparam name="ElementType">The element type for the map</typeparam>
		/// <param name="objSender">The object firing the event.</param>
		/// <param name="mapThatChanged">The affected map</param>
		/// <param name="eNewKeys">A list of new keys that were just added or <see langword="null"/> if no new keys were
		/// added.</param>
		/// <param name="eRemovedKeys">A list of keys that were just removed from the map or <see langword="null"/> if none
		/// were removed.</param>
		/// <param name="eChangedKeys">A list of tuples containing keys that just changed.  The first two elements in the
		/// tuple should be, in order, the old key and the new key.  The final element in the tuple should be the element
		/// that was moved.</param>
		public delegate void DMapFieldChanged<MapType, KeyType, ElementType>(in TypeOfObj objSender, in MapType
				mapThatChanged, in System.Collections.Generic.IEnumerable<KeyType>? eNewKeys = null, in System.Collections
				.Generic.IEnumerable<System.Tuple<KeyType, ElementType>>? eRemovedKeys = null, in System.Collections.Generic
				.IEnumerable<System.Tuple<KeyType,KeyType, ElementType>>? eChangedKeys = null)
			where MapType : System.Collections.Generic.IReadOnlyDictionary<KeyType, ElementType>;

		/// <summary>
		/// Provides a way to notify interested callers that you have a new instance.  This will normally be an event on the owner of the <typeparamref
		/// name="TypeOfObj"/> instance.
		/// </summary>
		/// <param name="objNewInstance">The new instance of <typeparamref name="TypeOfObj"/></param>
		public delegate void DNewInstanceCreated(in TypeOfObj objNewInstance);
	#endregion

	#region Events
		/// <summary>
		/// Fired when the dirty flag changes.  It either wasn't dirty, but now is, or was dirty, but now isn't.
		/// </summary>
		public event DDirtyChanged? evtDirtyChanged;
	#endregion

	#region Constants
	#endregion

	#region Helper Types
	#endregion

	#region Members
		/// <summary>
		/// Stores if we are dirty or not
		/// </summary>
		private bool bIsDirty;

		/// <summary>
		/// Stores our lock object.  Pass this to <see langword="lock"/> to lock the instance.
		/// </summary>
		private readonly object objIsDirtyLock = new();

		/// <summary>
		/// Stores a dictionary of all instances of <see cref="Obj{TypeOfObj}"/> indexed by their <see cref="System.Guid"/>
		/// </summary>
		private static readonly System.Collections.Generic.SortedDictionary<System.Guid, Obj<TypeOfObj>>
			mapAllInstances = [];
	#endregion

	#region Properties
		/// <summary>
		/// Returns <see langword="true"/> if this <see cref="Obj{TypeOfObj}"/> is dirty and <see langword="false"/> if it isn't.
		/// </summary>
		public virtual bool IsDirty
		{
			get
			{
				lock(objIsDirtyLock)
					return bIsDirty;
			}
		}

		/// <summary>
		/// Returns a readonly dictionary of all <see cref="Obj{TypeOfObj}"/> instances indexed by their <see cref="System.Guid"/>
		/// </summary>
		public static new System.Collections.Generic.IReadOnlyDictionary<System.Guid, Obj<TypeOfObj>>
			AllInstancesByGUID
			=> mapAllInstances;
	#endregion

	#region Methods
		/// <summary>
		/// Marks this instance as dirty.  <see cref="evtDirtyChanged"/> will be fired.
		/// </summary>
		protected virtual void MakeDirty()
		{
			lock(objIsDirtyLock)
				if(!bIsDirty)
				{
					bIsDirty = true;

					evtDirtyChanged?.Invoke((TypeOfObj)this, bIsDirty);
				}
		}

		/// <summary>
		/// Clears the <see cref="bIsDirty"/> flag.  <see cref="IsDirty"/> will now return <see langword="false"/>.  <see
		/// cref="evtDirtyChanged"/> will be fired.
		/// </summary>
		protected void ClearIsDirty()
		{
			lock(objIsDirtyLock)
			{
				bIsDirty = false;

				evtDirtyChanged?.Invoke((TypeOfObj)this, bIsDirty);
			}
		}

		/// <summary>
		/// Fires the <see cref="evtDirtyChanged"/> even though the flag hasn't changed
		/// </summary>
		protected void ActAsThoughDirty()
		{
			lock(objIsDirtyLock)
				evtDirtyChanged?.Invoke((TypeOfObj)this, bIsDirty);
		}
		#endregion

	#region Operators
		/// <summary>
		/// Explicit type cast operator to convert from a <see cref="System.Guid"/> to <see cref="Obj{TypeOfObj}"/>.  Returns <see langword="null"/> if the value
		/// in <paramref name="guidToLookup"/> can't be found.
		/// </summary>
		/// <param name="guidToLookup">Which value to look up</param>

		public static explicit operator Obj<TypeOfObj>?(in System.Guid guidToLookup)
			=> mapAllInstances.TryGetValue(guidToLookup, out Obj<TypeOfObj>? value)
				? value
				: null;
	#endregion

	#region Event Handlers
	#endregion
}