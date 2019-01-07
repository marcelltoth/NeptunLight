using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.App;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Utils
{
    public class TabListAdapter<TVm, TView> : FragmentStatePagerAdapter
        where TVm : ViewModelBase
        where TView : Fragment, IViewFor
    {
        public IReadOnlyList<TVm> Items { get; }

        public TabListAdapter(FragmentManager fm, IEnumerable<TVm> items) : base(fm)
        {
            Items = items.ToList();
        }

        public override int Count => Items.Count;
        public override Fragment GetItem(int position)
        {
            Fragment fragment = Activator.CreateInstance<TView>();
            ((IViewFor<TVm>)fragment).ViewModel = Items[position];
            return fragment;
        }
    }
}