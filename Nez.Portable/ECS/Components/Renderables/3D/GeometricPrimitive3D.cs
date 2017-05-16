using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Nez
{
	public abstract class GeometricPrimitive3D : Renderable3D, IDisposable
	{
		protected List<VertexPositionColorNormal> _vertices = new List<VertexPositionColorNormal>();
		protected List<ushort> _indices = new List<ushort>();

		VertexBuffer _vertexBuffer;
		IndexBuffer _indexBuffer;
		BasicEffect _basicEffect;


		#region Initialization and configuration

		protected void AddVertex( Vector3 position, Color color, Vector3 normal )
		{
			_vertices.Add( new VertexPositionColorNormal( position, color, normal ) );
		}


		protected void AddIndex( int index )
		{
			_indices.Add( (ushort)index );
		}


		/// <summary>
		/// Once all the geometry has been specified by calling AddVertex and AddIndex, this method copies the vertex and index data into
		/// GPU format buffers, ready for efficient rendering.
		protected void InitializePrimitive()
		{
			// create a vertex buffer, and copy our vertex data into it.
			_vertexBuffer = new VertexBuffer( Core.CoreGraphicsDevice, typeof( VertexPositionColorNormal ), _vertices.Count, BufferUsage.None );
			_vertexBuffer.SetData( _vertices.ToArray() );

			// create an index buffer, and copy our index data into it.
			_indexBuffer = new IndexBuffer( Core.CoreGraphicsDevice, typeof( ushort ), _indices.Count, BufferUsage.None );
			_indexBuffer.SetData( _indices.ToArray() );
		}


		public override void OnAddedToEntity()
		{
			base.OnAddedToEntity();

			_basicEffect = Entity.Scene.Content.LoadMonoGameEffect<BasicEffect>();
			_basicEffect.VertexColorEnabled = true;
			_basicEffect.EnableDefaultLighting();
		}

		#endregion


		#region IDisposable

		~GeometricPrimitive3D()
		{
			Dispose( false );
		}


		/// <summary>
		/// frees resources used by this object.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}


		/// <summary>
		/// frees resources used by this object.
		/// </summary>
		protected virtual void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( _vertexBuffer != null )
					_vertexBuffer.Dispose();

				if( _indexBuffer != null )
					_indexBuffer.Dispose();

				if( _basicEffect != null )
					_basicEffect.Dispose();
			}
		}

		#endregion


		public override void Render( Graphics graphics, Camera camera )
		{
			// flush the 2D batch so we render appropriately depth-wise
			graphics.Batcher.FlushBatch();

			Core.CoreGraphicsDevice.BlendState = BlendState.Opaque;
			Core.CoreGraphicsDevice.DepthStencilState = DepthStencilState.Default;

			// Set BasicEffect parameters.
			_basicEffect.World = WorldMatrix;
			_basicEffect.View = camera.ViewMatrix3D;
			_basicEffect.Projection = camera.ProjectionMatrix3D;
			_basicEffect.DiffuseColor = Color.ToVector3();

			// Set our vertex declaration, vertex buffer, and index buffer.
			Core.CoreGraphicsDevice.SetVertexBuffer( _vertexBuffer );
			Core.CoreGraphicsDevice.Indices = _indexBuffer;

			_basicEffect.CurrentTechnique.Passes[0].Apply();
			var primitiveCount = _indices.Count / 3;
			Core.CoreGraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, primitiveCount );
		}

	}
}
