using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;

namespace NeptunLight.Droid.Utils
{
    public delegate void ViewBinder<in T>(View viewToBind, T dataModel);

    public class ListAdapter<T> : BaseAdapter<T>
    {
        private readonly IReadOnlyList<T> _items;
        private readonly LayoutInflater _inflater;
        private readonly int _layoutId;
        private readonly ViewBinder<T> _bindView;

        public ListAdapter(LayoutInflater inflater, IEnumerable<T> items, int layoutId, ViewBinder<T> bindView)
        {
            _items = items?.ToList() ?? new List<T>();
            _layoutId = layoutId;
            _bindView = bindView;
            _inflater = inflater;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View itemLayout = convertView ?? _inflater.Inflate(_layoutId, parent, false);
            _bindView(itemLayout, _items[position]);
            return itemLayout;
        }

        public override int Count => _items.Count;

        public override T this[int position] => _items[position];
    }
}