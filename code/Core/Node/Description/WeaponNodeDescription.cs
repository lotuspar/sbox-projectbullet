﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace ProjectBullet.Core.Node.Description;

public class WeaponNodeDescription : IStaticDescription
{
	private static readonly List<WeaponNodeDescription> Instances = new();

	private string _name;

	public NodeAttribute NodeAttribute { get; private set; }
	public EnergyAttribute EnergyAttribute { get; private set; }
	public IEnumerable<ConnectorAttribute> ConnectorAttributes { get; private set; }

	public TypeDescription TypeDescription { get; private set; }
	public Type TargetType => TypeDescription.TargetType;

	public WeaponNodeDescription( Type type )
	{
		// Find TypeDescription for provided type
		var typeDescription = TypeLibrary.GetDescription( type );
		if ( typeDescription == null )
		{
			// todo: write custom exception
			throw new Exception( $"No type description found for {type.Name}, {type}" );
		}

		UseNewTypeDescription( typeDescription );

		Event.Register( this );

		Instances.Add( this );
	}

	~WeaponNodeDescription() => Event.Unregister( this );

	private void OnHotload() => UseNewTypeDescription
		( TypeLibrary.GetDescription( _name ) );

	private void UseNewTypeDescription( TypeDescription typeDescription )
	{
		TypeDescription = typeDescription;
		_name = TypeDescription.TargetType.FullName;

		NodeAttribute = TypeLibrary.GetAttribute<NodeAttribute>( TargetType );
		EnergyAttribute = TypeLibrary.GetAttribute<EnergyAttribute>( TargetType );
		ConnectorAttributes = TypeLibrary.GetAttributes<ConnectorAttribute>( TargetType );
	}

	public static WeaponNodeDescription Get( Type type ) => Instances.SingleOrDefault( v => v.TargetType == type ) ??
	                                                        new WeaponNodeDescription( type );
}
