using System;

namespace CoreAudio.Enums
{
	/// <summary>
	/// Values that are used in activation calls to indicate the execution contexts in which an object is to be run.
	/// </summary>
	/// <remarks>
	/// MSDN Reference: http://msdn.microsoft.com/en-us/library/ms693716.aspx
	/// Note: This item is external to CoreAudio API, and is defined in the Windows COM API.
	/// </remarks>
	[Flags]
	public enum ClsCtx
	{
		/// <summary>
		/// The code that creates and manages objects of this class is a DLL that runs in the same process as the caller of the function specifying the class context.
		/// </summary>
		INPROC_SERVER = 0x1,
		/// <summary>
		/// The code that manages objects of this class is an in-process handler.
		/// </summary>
		INPROC_HANDLER = 0x2,
		/// <summary>
		/// The EXE code that creates and manages objects of this class runs on same machine but is loaded in a separate process space.
		/// </summary>
		LOCAL_SERVER = 0x4,
		/// <summary>
		/// Obsolete.
		/// </summary>
		INPROC_SERVER16 = 0x8,
		/// <summary>
		/// A remote context.
		/// </summary>
		REMOTE_SERVER = 0x10,
		/// <summary>
		/// Obsolete.
		/// </summary>
		INPROC_HANDLER16 = 0x20,
		/// <summary>
		/// Reserved.
		/// </summary>
		RESERVED1 = 0x40,
		/// <summary>
		/// Reserved.
		/// </summary>
		RESERVED2 = 0x80,
		/// <summary>
		/// Reserved.
		/// </summary>
		RESERVED3 = 0x100,
		/// <summary>
		/// Reserved.
		/// </summary>
		RESERVED4 = 0x200,
		/// <summary>
		/// Disaables the downloading of code from the directory service or the Internet.
		/// </summary>
		NO_CODE_DOWNLOAD = 0x400,
		/// <summary>
		/// Reserved.
		/// </summary>
		RESERVED5 = 0x800,
		/// <summary>
		/// Specify if you want the activation to fail if it uses custom marshalling.
		/// </summary>
		NO_CUSTOM_MARSHAL = 0x1000,
		/// <summary>
		/// Enables the downloading of code from the directory service or the Internet.
		/// </summary>
		ENABLE_CODE_DOWNLOAD = 0x2000,
		/// <summary>
		/// Can be used to override the logging of failures
		/// </summary>
		NO_FAILURE_LOG = 0x4000,
		/// <summary>
		/// Disables activate-as-activator (AAA) activations for this activation only.
		/// </summary>
		DISABLE_AAA = 0x8000,
		/// <summary>
		/// Enables activate-as-activator (AAA) activations for this activation only.
		/// </summary>
		ENABLE_AAA = 0x10000,
		/// <summary>
		/// Begin this activation from the default context of the current apartment.
		/// </summary>
		FROM_DEFAULT_CONTEXT = 0x20000,
		/// <summary>
		/// Activate or connect to a 32-bit version of the server; fail if one is not registered.
		/// </summary>
		ACTIVATE_32_BIT_SERVER = 0x40000,
		/// <summary>
		/// Activate or connect to a 64 bit version of the server; fail if one is not registered. 
		/// </summary>
		ACTIVATE_64_BIT_SERVER = 0x80000,
		/// <summary>
		/// Obsolete.
		/// </summary>
		ENABLE_CLOAKING = 0x100000,
		/// <summary>
		/// Reserved.
		/// </summary>
		PS_DLL = unchecked((int)0x80000000),

		INPROC = INPROC_SERVER | INPROC_HANDLER,
		SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
		ALL = SERVER | INPROC_HANDLER
	}
}
