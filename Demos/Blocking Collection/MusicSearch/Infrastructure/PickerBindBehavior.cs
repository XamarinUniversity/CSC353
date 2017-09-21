using System;
using Xamarin.Forms;
using System.Collections;
using System.Diagnostics;

namespace MusicSearch.Infrastructure
{
    public class PickerBindBehavior : Behavior<Picker>
    {
        Picker associatedPicker;
        bool updatingValue;

        #region ItemsProperty
        public static BindableProperty ItemsProperty =
            BindableProperty.Create("Items", typeof(IEnumerable),
                typeof(PickerBindBehavior),null,
                propertyChanged: ItemsChanged);

        public IEnumerable Items
        {
            get { return (IEnumerable) base.GetValue(ItemsProperty); }
            set { base.SetValue(ItemsProperty, value); }
        }

        static void ItemsChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            var behavior = bindableObject as PickerBindBehavior;
            if (behavior != null)
                behavior.OnItemsChanged((IEnumerable)oldValue, (IEnumerable)newValue);
        }
        #endregion

        #region SelectedItemProperty
        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem",
                typeof(object), typeof(PickerBindBehavior),
                null,
                BindingMode.TwoWay,
                propertyChanged: SelectedItemChanged);

        public object SelectedItem
        {
            get { return base.GetValue(SelectedItemProperty); }
            set { base.SetValue(SelectedItemProperty, value); }
        }

        static void SelectedItemChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            var behavior = bindableObject as PickerBindBehavior;
            if (behavior != null)
                behavior.OnSelectedItemChanged(oldValue, newValue);
        }
        #endregion

        protected override void OnAttachedTo(Picker bindable)
        {
            if (associatedPicker != null)
            {
                associatedPicker.SelectedIndexChanged -= OnSelectedIndexChanged;
            }

            associatedPicker = bindable;
            associatedPicker.SelectedIndexChanged += OnSelectedIndexChanged;

            OnItemsChanged(null, Items);
            OnSelectedItemChanged(null, SelectedItem);
        }

        protected override void OnDetachingFrom(Picker bindable)
        {
            Debug.Assert(associatedPicker == bindable);
            bindable.SelectedIndexChanged -= OnSelectedIndexChanged;
        }

        void OnItemsChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            INotifyCollectionChanged ncc = oldValue as INotifyCollectionChanged;
            if (ncc != null)
            {
                ncc.CollectionChanged -= OnCollectionChanged;
            }

            if (associatedPicker == null)
                return;

            associatedPicker.Items.Clear ();

            if (newValue == null)
                return;

            foreach (var item in newValue) {
                associatedPicker.Items.Add ((item ?? "").ToString());
            }

            ncc = newValue as INotifyCollectionChanged;
            if (ncc != null)
            {
                ncc.CollectionChanged += OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.Assert(ReferenceEquals(sender, Items));

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    associatedPicker.Items.Add((item ?? "").ToString());
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    string value = (item ?? "").ToString();
                    associatedPicker.Items.Remove(value);
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                associatedPicker.Items.Clear();
                foreach (var item in Items)
                {
                    associatedPicker.Items.Add((item ?? "").ToString());
                }
            }

            if (SelectedItem != null && associatedPicker.SelectedIndex == -1) {
                OnSelectedItemChanged (null, SelectedItem);
            }
        }

        void OnSelectedItemChanged(object oldValue, object newValue)
        {
            if (associatedPicker == null || updatingValue)
                return;

            if (Object.Equals(oldValue, newValue))
                return;

            updatingValue = true;
            try
            {
                if (newValue == null)
                {
                    associatedPicker.SelectedIndex = -1;
                }

                var items = Items;
                if (items == null)
                    return;

                int index = -1;
                IList itemList = items as IList;
                if (itemList != null)
                {
                    index = itemList.IndexOf(newValue);
                }
                else
                {
                    foreach (object testValue in items)
                    {
                        index++;
                        if (Equals(testValue, newValue))
                            break;
                    }
                }
                associatedPicker.SelectedIndex = index;
            }
            finally
            {
                updatingValue = false;
            }
        }

        void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (associatedPicker == null || updatingValue)
                return;

            var items = Items;
            if (items == null)
                return;

            int selectedIndex = associatedPicker.SelectedIndex;
            if (selectedIndex == -1)
            {
                SelectedItem = null;
                return;
            }

            object value = null;

            IList itemList = items as IList;
            if (itemList != null)
            {
                value = itemList[selectedIndex];
            }
            else
            {
                int index = 0;
                foreach (object testValue in items)
                {
                    if (index == selectedIndex)
                    {
                        value = testValue;
                        break;
                    }
                    index++;
                }
            }

            SelectedItem = value;
        }
    }

