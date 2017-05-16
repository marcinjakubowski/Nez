using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;


namespace Nez.Tiled
{
	public class TiledTileset
	{
		public Texture2D Texture;
		public readonly int FirstId;
		public readonly int TileWidth;
		public readonly int TileHeight;
		public int Spacing;
		public int Margin;
		public Dictionary<string,string> Properties = new Dictionary<string,string>();
		public List<TiledTilesetTile> Tiles = new List<TiledTilesetTile>();

		protected readonly Dictionary<int,Subtexture> Regions;


		public TiledTileset( Texture2D texture, int firstId )
		{
			this.Texture = texture;
			this.FirstId = firstId;
			Regions = new Dictionary<int,Subtexture>();
		}


		public TiledTileset( Texture2D texture, int firstId, int tileWidth, int tileHeight, int spacing = 2, int margin = 2 )
		{
			this.Texture = texture;
			this.FirstId = firstId;
			this.TileWidth = tileWidth;
			this.TileHeight = tileHeight;
			this.Spacing = spacing;
			this.Margin = margin;

			var id = firstId;
			Regions = new Dictionary<int,Subtexture>();
			for( var y = margin; y < texture.Height - margin; y += tileHeight + spacing )
			{
				for( var x = margin; x < texture.Width - margin; x += tileWidth + spacing )
				{
					Regions.Add( id, new Subtexture( texture, x, y, tileWidth, tileHeight ) );
					id++;
				}
			}
		}
			

		/// <summary>
		/// gets the Subtexture for the tile with id
		/// </summary>
		/// <returns>The tile texture region.</returns>
		/// <param name="id">Identifier.</param>
		public virtual Subtexture GetTileTextureRegion( int id )
		{
			return Regions[id];
		}
	}
}