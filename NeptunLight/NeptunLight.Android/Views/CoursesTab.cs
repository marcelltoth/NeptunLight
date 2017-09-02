using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class CoursesTab : ReactiveFragment<CoursesTabViewModel>
    {
        public ExpandableListView ExpandableList { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.CoursesTab, container, false);

            this.WireUpControls(layout);

            ExpandableList.SetAdapter(new ExpandableAdapter(inflater, ViewModel.Subjects));

            return layout;
        }

        private class ExpandableAdapter : BaseExpandableListAdapter
        {
            private readonly LayoutInflater _inflater;
            private readonly IReadOnlyList<SubjectViewModel> _items;

            public ExpandableAdapter(LayoutInflater inflater, IEnumerable<SubjectViewModel> items)
            {
                _inflater = inflater;
                _items = items.ToList();
            }

            public override Object GetChild(int groupPosition, int childPosition)
            {
                return null;
            }

            public override long GetChildId(int groupPosition, int childPosition)
            {
                return childPosition;
            }

            public override int GetChildrenCount(int groupPosition)
            {
                return _items[groupPosition].Courses.Count;
            }

            public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
            {
                View layout = convertView ?? _inflater.Inflate(Resource.Layout.CoursesListItem, parent, false);
                CourseViewModel item = _items[groupPosition].Courses[childPosition];
                layout.FindViewById<TextView>(Resource.Id.expandedListItem).Text = item.Code;

                return layout;
            }

            public override Object GetGroup(int groupPosition)
            {
                return null;
            }

            public override long GetGroupId(int groupPosition)
            {
                return groupPosition;
            }

            public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
            {
                View layout = convertView ?? _inflater.Inflate(Resource.Layout.CoursesListGroup, parent, false);
                SubjectViewModel item = _items[groupPosition];
                layout.FindViewById<TextView>(Resource.Id.listTitle).Text = item.Name;
                return layout;
            }

            public override bool IsChildSelectable(int groupPosition, int childPosition)
            {
                return false;
            }

            public override int GroupCount => _items.Count;
            public override bool HasStableIds { get; } = false;
        }
    }
}