namespace Bld.Libnl.Types;

/// <summary>
/// Netlink attribute types from linux/netlink.h
/// </summary>
public enum NlAttributeType
{
    NLA_UNSPEC,	/**< Unspecified type, binary data chunk */
    NLA_U8,		/**< 8 bit integer */
    NLA_U16,	/**< 16 bit integer */
    NLA_U32,	/**< 32 bit integer */
    NLA_U64,	/**< 64 bit integer */
    NLA_STRING,	/**< NUL terminated character string */
    NLA_FLAG,	/**< Flag */
    NLA_MSECS,	/**< Micro seconds (64bit) */
    NLA_NESTED,	/**< Nested attributes */
    NLA_NESTED_COMPAT,
    NLA_NUL_STRING,
    NLA_BINARY,
    NLA_S8,
    NLA_S16,
    NLA_S32,
    NLA_S64,
}
