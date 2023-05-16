//Made by: Amber Kortier
using System;

/// <summary>
/// The PreferComponent attribute adds components automatically but doesn't depend on them existing.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class PreferComponent : Attribute
{
    /// <summary>
    /// The main required preferred Type.
    /// </summary>
    public Type m_Type0;
    /// <summary>
    /// Extra preferred types. These are not required.
    /// </summary>
    public Type[] m_Types1;

    /// <summary>
    /// Specify the Type that is preferred.
    /// </summary>
    /// <param name="preferredComponent">The Type that is preferred.</param>
    public PreferComponent(Type preferredComponent)
    {
        m_Type0 = preferredComponent;
    }

    /// <summary>
    /// Specify the Type that is preferred.
    /// </summary>
    /// <param name="preferredComponent">The Type that is preferred.</param>
    /// <param name="extraPrefferedTypes">The extra Types that are preferred.</param>
    public PreferComponent(Type preferredComponent, params Type[] extraPrefferedTypes)
    {
        m_Type0 = preferredComponent;
        m_Types1 = extraPrefferedTypes;
    }
}
