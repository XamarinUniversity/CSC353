using System;
using System.Collections.Generic;
using MusicSearch.Models;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinUniversity.Infrastructure;

namespace MusicSearch.ViewModels
{
    public enum ComparisonOperation
    {
        Equals = 0,
        GreaterEqual = 1,
        LessEqual = 2,
    }

    public class SearchCriteriaViewModel : SimpleViewModel
    {
        private string field;
        public string Field {
            get { return field; }
            set { SetPropertyValue(ref field, value); }
        }

        private string compareValue;
        public string Value { 
            get { return compareValue; }
            set { SetPropertyValue(ref compareValue, value); }
        }

        private ComparisonOperation op = ComparisonOperation.Equals;
        public ComparisonOperation Operator { 
            get { return op; }
            set { SetPropertyValue(ref op, value); }
        }

        public string OperatorText {
            get { return AvailableOperators[(int)this.op]; }
            set {
                op = (ComparisonOperation) AvailableOperators.IndexOf(value);
                RaisePropertyChanged();
            }
        }

        private List<string> availableFields;
        public IList<string> AvailableFields
        {
            get
            {
                return availableFields ?? 
                    (availableFields = typeof (Track).GetRuntimeProperties()
                        .Select(prop => prop.Name).ToList());
            }
        }

        readonly string[] operatorText = { "==", ">=", "<=" };
        public IList<string> AvailableOperators
        {
            // Must be in same order as operators.
            get { return operatorText; }
        }

        public ICommand Delete { get; private set; }

        public SearchCriteriaViewModel ()
        {
            Delete = new Command(OnDelete);
            Field = AvailableFields.First();
        }

        void OnDelete()
        {
            MessagingCenter.Send(this, "Delete");
        }

        public bool Test(Track track)
        {
            PropertyInfo pi = track.GetType().GetRuntimeProperty(field);
            Debug.Assert(pi != null);

            string value = (pi.GetValue(track) ?? "").ToString();
            int result = string.Compare(value, compareValue, StringComparison.CurrentCultureIgnoreCase);

            switch (op)
            {
                case ComparisonOperation.GreaterEqual:
                    return result >= 0;
                case ComparisonOperation.LessEqual:
                    return result <= 0;
                default:
                    return result == 0;
            }
        }
    }
}