namespace FtdAdapter
{
	/// <summary>
	/// Provides information about an attached FTDI USB device.
	/// </summary>
	public struct FtdDeviceInfo
	{
		private readonly uint _flags;
		private readonly uint _id;
		private readonly string _serialNumber;
		private readonly uint _type;
		private readonly uint _locationId;
		private readonly string _description;

		internal FtdDeviceInfo(uint flags, uint type, uint id, uint locationId, string serialNumber, string description)
		{
			_flags = flags;
			_type = type;
			_id = id;
			_locationId = locationId;
			_serialNumber = serialNumber;
			_description = description;
		}
		
		/// <summary>
		/// Complete device ID, comprising Vendor ID and Product ID.
		/// </summary>
		public uint Id
		{
			get { return _id; }
		}

		/// <summary>
		/// Vendor ID.
		/// </summary>
		public uint VendorId
		{
			get { return (_id >> 16) & 0xFFFF; }
		}

		/// <summary>
		/// Product ID
		/// </summary>
		public uint ProductId
		{
			get { return _id & 0xFFFF; }
		}	

		/// <summary>
		/// Serial number of device.
		/// </summary>
		public string SerialNumber
		{
			get { return _serialNumber; }
		}

		/// <summary>
		/// Device flags.
		/// </summary>
		public uint Flags
		{
			get { return _flags; }
		}

		/// <summary>
		/// Device type.
		/// </summary>
		public uint Type
		{
			get { return _type; }
		}

		/// <summary>
		/// LocID
		/// </summary>
		public uint LocationId
		{
			get { return _locationId; }
		}

		/// <summary>
		/// Description of device.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		/// <summary>
		/// Gets a value indicating if the device is already open.
		/// </summary>
		public bool IsOpen
		{
			get { return (_flags & 0x01) != 0; }
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(FtdDeviceInfo left, FtdDeviceInfo right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(FtdDeviceInfo left, FtdDeviceInfo right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{			
			if (!(obj is FtdDeviceInfo))
				return false;

			FtdDeviceInfo other = (FtdDeviceInfo) obj;

			return Id == other.Id &&
				Flags == other.Flags &&
				SerialNumber == other.SerialNumber &&
				Type == other.Type &&
				LocationId == other.LocationId &&
				Description == other.Description;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			return Id.GetHashCode() ^
				Flags.GetHashCode() ^
				SerialNumber.GetHashCode() ^
				Type.GetHashCode() ^
				LocationId.GetHashCode() ^
				Description.GetHashCode();
		}		
	}
}
