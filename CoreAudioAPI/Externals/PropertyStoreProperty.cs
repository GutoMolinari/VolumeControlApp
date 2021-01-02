namespace CoreAudioAPI.Externals
{
    /// <summary>
    /// Property Store Property
    /// </summary>
    public class PropertyStoreProperty
    {
        private PropVariant propertyValue;

        internal PropertyStoreProperty(PropertyKey key, PropVariant value)
        {
            Key = key;
            propertyValue = value;
        }

        /// <summary>
        /// Property Key
        /// </summary>
        public PropertyKey Key { get; }

        /// <summary>
        /// Property Value
        /// </summary>
        public object Value => propertyValue.Value;
    }
}