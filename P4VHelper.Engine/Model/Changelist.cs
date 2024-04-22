// jdyun 24/04/10(수)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base;
using P4VHelper.Engine.Search;

namespace P4VHelper.Engine.Model
{
    public class Changelist : Bindable, ISearchable, IComparable<Changelist>
    {
        public const int REVISION_KEY       = 0;
        public const int DATE_KEY           = 1;
        public const int USERNAME_KEY       = 2;
        public const int DESCRIPTION_KEY    = 3;
        public const int MAX_KEY            = 4;        // END

        public static readonly IComparer<Changelist> DefaultComparer = new DescendingComparer<Changelist>();

        public SearchableType SearchableType => SearchableType.Changelist;

        public Changelist()
        {
            _revision = new FieldHolder<int>(0);
            _date = new FieldHolder<DateTime>(DateTime.MinValue);
            _userName = new FieldHolder<string>(string.Empty);
            _description = new FieldHolder<string>(string.Empty);
        }

        private FieldHolder<int> _revision;
        public FieldHolder<int> RevisionHolder => _revision;
        public int Revision
        {
            get => _revision.Value;
            set
            {
                _revision.Value = value;
                OnPropertyChanged();
            }
        }

        private FieldHolder<DateTime> _date;
        public FieldHolder<DateTime> DateHolder => _date;
        public DateTime Date
        {
            get => _date.Value;
            set
            {
                _date.Value = value;
                OnPropertyChanged();
            }
        }

        private FieldHolder<string> _userName;
        public FieldHolder<string> UserNameHolder => _userName;
        public string UserName
        {
            get => _userName.Value;
            set
            {
                _userName.Value = value; 
                OnPropertyChanged();
            }
        }

        private FieldHolder<string> _description;
        public FieldHolder<string> DescriptionHolder => _description;
        public string Description
        {
            get => _description.Value;
            set
            {
                _description.Value = value;
                OnPropertyChanged();
            }
        }

        public int CompareTo(Changelist? other)
        {
            if (other is null)
                throw new ArgumentException($"other must be of type {nameof(Changelist)}");

            return _revision.Value.CompareTo(other._revision.Value);
        }

        public class _Reflection : Reflection<Changelist>
        {
            static _Reflection()
            {
                Holders = new ThreadLocal<IFieldHolder>[MAX_KEY];
                // Holders[REVISION_KEY] = new ThreadLocal<FieldHolder<int>>();
                // Holders[DATE_KEY] = new ThreadLocal<FieldHolder<DateTime>>();
                // Holders[USERNAME_KEY] = new ThreadLocal<FieldHolder<string>>();
                // Holders[DESCRIPTION_KEY] = new ThreadLocal<FieldHolder<string>>();

                Setters = new Action<Changelist, IFieldHolder>[MAX_KEY];
                Setters[REVISION_KEY] = ((changelist, holder) => holder.Set(changelist.Revision));
                Setters[DATE_KEY] = ((changelist, holder) => holder.Set(changelist.Date));
                Setters[USERNAME_KEY] = ((changelist, holder) => holder.Set(changelist.UserName));
                Setters[DESCRIPTION_KEY] = ((changelist, holder) => holder.Set(changelist.Description));

                Getters = new Func<Changelist, IFieldHolder>[MAX_KEY];
                Getters[REVISION_KEY] = ((changelist) => changelist.RevisionHolder);
                Getters[DATE_KEY] = ((changelist) => changelist.DateHolder);
                Getters[USERNAME_KEY] = ((changelist) => changelist.UserNameHolder);
                Getters[DESCRIPTION_KEY] = ((changelist) => changelist.DescriptionHolder);
            }

            public _Reflection() : base(MAX_KEY)
            {

            }
        }
    }
}
