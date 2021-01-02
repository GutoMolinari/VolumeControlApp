using System.Runtime.InteropServices;

namespace CoreAudio.Externals
{
    /// <summary>
    /// Property Store class, only supports reading properties at the moment.
    /// </summary>
    public class PropertyStore
    {
        private readonly IPropertyStore storeInterface;

        /// <summary>
        /// Property Count
        /// </summary>
        public int Count
        {
            get
            {
                Marshal.ThrowExceptionForHR(storeInterface.GetCount(out var result));
                return (int)result;
            }
        }

        /// <summary>
        /// Gets property by index
        /// </summary>
        /// <param name="index">Property index</param>
        /// <returns>The property</returns>
        public PropertyStoreProperty this[int index]
        {
            get
            {
                PropertyKey key = Get(index);
                Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out var result));
                return new PropertyStoreProperty(key, result);
            }
        }

        /// <summary>
        /// Contains property guid
        /// </summary>
        /// <param name="key">Looks for a specific key</param>
        /// <returns>True if found</returns>
        public bool Contains(PropertyKey key)
        {
            var count = Count;

            for (int i = 0; i < count; i++)
            {
                PropertyKey ikey = Get(i);
                if ((ikey.formatId == key.formatId) && (ikey.propertyId == key.propertyId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Indexer by guid
        /// </summary>
        /// <param name="key">Property Key</param>
        /// <returns>Property or null if not found</returns>
        public PropertyStoreProperty this[PropertyKey key]
        {
            get
            {
                var count = Count;

                for (int i = 0; i < count; i++)
                {
                    PropertyKey ikey = Get(i);
                    if ((ikey.formatId == key.formatId) && (ikey.propertyId == key.propertyId))
                    {
                        Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref ikey, out var result));
                        return new PropertyStoreProperty(ikey, result);
                    }
                }

                return null;
            }
        }

        public string GetPropertyValue(PropertyKey key)
        {
            var count = Count;

            for (int i = 0; i < count; i++)
            {
                PropertyKey ikey = Get(i);

                if ((ikey.formatId == key.formatId) && (ikey.propertyId == key.propertyId))
                {
                    Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref ikey, out var result));
                    return (string)result.Value;
                }
            }

            return "Unknown";
        }

        /// <summary>
        /// Gets property key at sepecified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Property key</returns>
        public PropertyKey Get(int index)
        {
            Marshal.ThrowExceptionForHR(storeInterface.GetAt((uint)index, out var key));
            return key;
        }

        /// <summary>
        /// Gets property value at specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Property value</returns>
        public PropVariant GetValue(int index)
        {
            PropertyKey key = Get(index);
            Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out var result));
            return result;
        }

        /// <summary>
        /// Sets property value at specified key.
        /// </summary>
        /// <param name="key">Key of property to set.</param>
        /// <param name="value">Value to write.</param>
        public void SetValue(PropertyKey key, PropVariant value)
        {
            Marshal.ThrowExceptionForHR(storeInterface.SetValue(ref key, ref value));
        }

        /// <summary>
        /// Saves a property change.
        /// </summary>
        public void Commit()
        {
            Marshal.ThrowExceptionForHR(storeInterface.Commit());
        }

        /// <summary>
        /// Creates a new property store
        /// </summary>
        /// <param name="store">IPropertyStore COM interface</param>
        internal PropertyStore(IPropertyStore store)
        {
            storeInterface = store;
        }
    }
}