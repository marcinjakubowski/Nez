using System;
using System.Collections.Generic;
using System.Linq;


namespace Nez.UI
{
	public class ArraySelection<T> : Selection<T> where T : class
	{
		List<T> _array;
		bool _rangeSelect = true;
		int _rangeStart;


		public ArraySelection( List<T> array )
		{
			this._array = array;
		}


		public override void Choose( T item )
		{
			Assert.IsNotNull( item, "item cannot be null" );
			if( _isDisabled )
				return;
			
			var index = _array.IndexOf( item );
			if( Selected.Count > 0 && _rangeSelect && Multiple && InputUtils.IsShiftDown() )
			{
				int oldRangeState = _rangeStart;
				Snapshot();
				// Select new range.
				int start = _rangeStart, end = index;
				if( start > end )
				{
					var temp = end;
					end = start;
					start = temp;
				}

				if( !InputUtils.IsControlDown() )
					Selected.Clear();
				for( int i = start; i <= end; i++ )
					Selected.Add( _array[i] );

				if( FireChangeEvent() )
				{
					_rangeStart = oldRangeState;
					Revert();
				}
				Cleanup();
				return;
			}
			else
			{
				_rangeStart = index;
			}

			base.Choose( item );
		}


		public bool GetRangeSelect()
		{
			return _rangeSelect;
		}


		public void SetRangeSelect( bool rangeSelect )
		{
			this._rangeSelect = rangeSelect;
		}


		/// <summary>
		/// Removes objects from the selection that are no longer in the items array. If getRequired() is true and there is
		/// no selected item, the first item is selected.
		/// </summary>
		public void Validate()
		{
			if( _array.Count == 0 )
			{
				Clear();
				return;
			}

			for( var i = Selected.Count - 1; i >= 0; i-- )
			{
				var item = Selected[i];
				if( !_array.Contains( item ) )
					Selected.Remove( item );
			}

			if( Required && Selected.Count == 0 )
				Set( _array.First() );
		}
	}
}

