using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Externals
{
    /// <summary>
    /// Exposes methods for enumerating, getting, and setting property values.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/bb761474.aspx
    /// Note: This item is external to CoreAudio API, and is defined in the Windows Property System API.
    /// </remarks>
    [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyStore
    {
        /// <summary>
        /// Gets the number of properties attached to the file.
        /// </summary>
        /// <param name="propertyCount">Receives the property count.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetCount([Out][MarshalAs(UnmanagedType.U4)] out uint propertyCount);

        /// <summary>
        /// Gets a property key from an item's array of properties.
        /// </summary>
        /// <param name="propertyIndex">The index of the property key in the array of <see cref="PropertyKey"/> structures.</param>
        /// <param name="propertyKey">The unique identifier for a property.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetAt(
            [In][MarshalAs(UnmanagedType.U4)] uint propertyIndex,
            [Out] out PropertyKey propertyKey
            );

        /// <summary>
        /// Gets data for a specific property.
        /// </summary>
        /// <param name="propertyKey">A <see cref="PropertyKey"/> structure containing a unique identifier for the property in question.</param>
        /// <param name="value">Receives a <see cref="PropVariant"/> structure that contains the property data.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetValue(
            [In] ref PropertyKey propertyKey,
            [Out] out PropVariant value
            );

        /// <summary>
        /// Sets a new property value, or replaces or removes an existing value.
        /// </summary>
        /// <param name="propertyKey">A <see cref="PropertyKey"/> structure containing a unique identifier for the property in question.</param>
        /// <param name="value">A <see cref="PropVariant"/> structure that contains the new property data.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetValue(
            [In] ref PropertyKey propertyKey,
            [In] ref PropVariant value
            );

        /// <summary>
        /// Saves a property change.
        /// </summary>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int Commit();
    }
}
