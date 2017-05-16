using System;
using Microsoft.Xna.Framework.Graphics;


namespace Nez
{
	public class VignettePostProcessor : PostProcessor
	{
		public float Power
		{
			get { return _power; }
			set
			{
				if( _power != value )
				{
					_power = value;

					if( Effect != null )
						_powerParam.SetValue( _power );
				}
			}
		}

		public float Radius
		{
			get { return _radius; }
			set
			{
				if( _radius != value )
				{
					_radius = value;

					if( Effect != null )
						_radiusParam.SetValue( _radius );
				}
			}
		}

		float _power = 1f;
		float _radius = 1.25f;
		EffectParameter _powerParam;
		EffectParameter _radiusParam;


		public VignettePostProcessor( int executionOrder ) : base( executionOrder )
		{}


		public override void OnAddedToScene()
		{
			Effect = Scene.Content.LoadEffect<Effect>( "vignette", EffectResource.VignetteBytes );

			_powerParam = Effect.Parameters["_power"];
			_radiusParam = Effect.Parameters["_radius"];
			_powerParam.SetValue( _power );
			_radiusParam.SetValue( _radius );
		}

	}
}

